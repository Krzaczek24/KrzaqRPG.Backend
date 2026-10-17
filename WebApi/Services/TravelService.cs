using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travel;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelTime;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelTime.Base;
using KrzaqRPG.Backend.Database.SQLite.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace KrzaqRPG.Backend.WebApi.Services
{
    public interface ITravelService
    {
        ValueTask<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default);
        ValueTask<IReadOnlyDictionary<int, (TimeSpan Path, TimeSpan Area)>> ComputeTravelTimeAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
        ValueTask<IReadOnlySet<(int, int)>> GetInvalidTravelPathsAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
    }

    internal class TravelService(
        IMemoryCache cache,
        IDbTravelAccess access) : ITravelService
    {
        public static class Settings
        {
            public static TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromHours(1);
        }

        public async ValueTask<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var paths = await ComputeTravelTimeAsync(pathIds, cancellationToken);
            return await access.RegisterTravelAsync(characterId, paths, cancellationToken);
        }

        public async ValueTask<IReadOnlyDictionary<int, (TimeSpan Path, TimeSpan Area)>> ComputeTravelTimeAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var travelTimes = new Dictionary<int, (TimeSpan, TimeSpan)>();

            foreach (int pathId in pathIds)
            {
                travelTimes.Add(pathId, cache.TryGetValue(GetKey(pathId), out (TimeSpan, TimeSpan) times) ? times : default);
            }

            var missingTimeIds = travelTimes
                .Where(times => times.Value == default)
                .Select(t => t.Key)
                .ToList();

            if (missingTimeIds.Count == 0)
                return travelTimes.AsReadOnly();

            var missingPaths = await access.GetPathsAsync(missingTimeIds, cancellationToken);

            var factors = await GetTravelTimeFactorsAsync();
            TimeSpan distanceTime = factors.PathDistanceToTravelTimeConversionFactor.Time / factors.PathDistanceToTravelTimeConversionFactor.Distance;

            foreach (var path in missingPaths)
            {
                TimeSpan pathTime = path.Distance * distanceTime * factors.PathTypeFactors[path.Type] * factors.PathEnvironmentFactors[path.Environment];
                TimeSpan areaTime = factors.AreaBaseTravelTime * factors.AreaTypeFactors[path.EndArea.Type];
                cache.Set(GetKey(path.Id), travelTimes[path.Id] = (pathTime, areaTime), Settings.SlidingExpiration);
            }

            return travelTimes.AsReadOnly();

            static string GetKey(int pathId) => $"PATH_{pathId}_TRAVEL_TIME";

            async ValueTask<TravelTimeFactors> GetTravelTimeFactorsAsync()
            {
                const string AreaBaseTravelTimeKey = "AREA_BASE_TRAVEL_TIME";
                const string DistConvFactorKey = "DISTANCE_TO_TRAVEL_TIME";
                const string TTFAreaTypeKey = "TRAVEL_TIME_FACTOR_AREA_TYPE";
                const string TTFPathTypeKey = "TRAVEL_TIME_FACTOR_PATH_TYPE";
                const string TTFPathEnvironmentKey = "TRAVEL_TIME_FACTOR_PATH_ENVIRONMENT";

                if (cache.TryGetValue(AreaBaseTravelTimeKey, out TimeSpan areaBaseTravelTime)
                && cache.TryGetValue(DistConvFactorKey, out (TimeSpan Time, double Distance) distanceTimeConversionFactor)
                && cache.TryGetValue(TTFAreaTypeKey, out IReadOnlyDictionary<DbAreaType, double>? areaTypeFactors)
                && cache.TryGetValue(TTFPathTypeKey, out IReadOnlyDictionary<DbPathType, double>? pathTypeFactors)
                && cache.TryGetValue(TTFPathEnvironmentKey, out IReadOnlyDictionary<DbPathEnvironment, double>? pathEnvironmentFactors))
                {
                    return new(areaBaseTravelTime!, distanceTimeConversionFactor!, areaTypeFactors!, pathTypeFactors!, pathEnvironmentFactors!);
                }

                areaBaseTravelTime = await access.GetAreaBaseTravelTimeAsync(cancellationToken);
                distanceTimeConversionFactor = await access.GetDistanceToTravelTimeConversionFactorAsync(cancellationToken);
                var allFactors = await access.GetTravelTimeFactorsAsync(cancellationToken);
                Split(allFactors, out areaTypeFactors, out pathTypeFactors, out pathEnvironmentFactors);

                cache.Set(AreaBaseTravelTimeKey, areaBaseTravelTime, Settings.SlidingExpiration);
                cache.Set(DistConvFactorKey, distanceTimeConversionFactor, Settings.SlidingExpiration);
                cache.Set(TTFAreaTypeKey, areaTypeFactors, Settings.SlidingExpiration);
                cache.Set(TTFPathTypeKey, pathTypeFactors, Settings.SlidingExpiration);
                cache.Set(TTFPathEnvironmentKey, pathEnvironmentFactors, Settings.SlidingExpiration);

                return new(areaBaseTravelTime!, distanceTimeConversionFactor!, areaTypeFactors!, pathTypeFactors!, pathEnvironmentFactors!);

                static void Split(
                    IEnumerable<DbTravelTimeFactorTable> tables,
                    out IReadOnlyDictionary<DbAreaType, double> areaTypeMultipliers,
                    out IReadOnlyDictionary<DbPathType, double> pathTypeMultipliers,
                    out IReadOnlyDictionary<DbPathEnvironment, double> pathEnvironmentMultipliers)
                {
                    areaTypeMultipliers = Convert(WhereTypeOf<DbTravelTimeFactorAreaType>(tables));
                    pathTypeMultipliers = Convert(WhereTypeOf<DbTravelTimeFactorPathType>(tables));
                    pathEnvironmentMultipliers = Convert(WhereTypeOf<DbTravelTimeFactorPathEnvironment>(tables));

                    static IEnumerable<T> WhereTypeOf<T>(IEnumerable<DbTravelTimeFactorTable> tables)
                        => tables.Where(t => t is T).Cast<T>();

                    static IReadOnlyDictionary<T, double> Convert<T>(IEnumerable<DbTravelTimeFactorTable<T>> factors) where T : Enum
                        => factors.ToReadOnlyDictionary(f => f.Name, f => f.Multiplier);
                }
            }
        }

        public async ValueTask<IReadOnlySet<(int, int)>> GetInvalidTravelPathsAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var pairs = pathIds.SlidingWindow(2).Select(x =>
            {
                var validation = new PathsPairValidation(x[0], x[1]);
                cache.TryGetValue(GetKey(validation), out bool? isValid);
                return validation with { IsValid = isValid };
            });

            Split(pairs, out var cached, out var nonCached);

            if (nonCached.Count == 0)
                return GetInvalid(cached);

            var result = (await access.AreTravelPathsValidAsync(pathIds, cancellationToken))
                .Select(x => new PathsPairValidation(x.Key.Item1, x.Key.Item2) { IsValid = x.Value });

            foreach (var pair in result)
                cache.Set(GetKey(pair), pair.IsValid!.Value, new MemoryCacheEntryOptions { SlidingExpiration = Settings.SlidingExpiration });

            return GetInvalid(result);

            static IReadOnlySet<(int, int)> GetInvalid(IEnumerable<PathsPairValidation> paths)
                => paths.Where(x => x.IsValid is not true).Select(x => x.PathIds).ToHashSet().AsReadOnly();

            static void Split(
                IEnumerable<PathsPairValidation> pairs,
                out IReadOnlySet<PathsPairValidation> cached,
                out IReadOnlySet<PathsPairValidation> nonCached)
            {
                var groupedPairs = pairs.GroupBy(x => x.IsValid is null).ToDictionary(x => x.Key, x => x.ToHashSet().AsReadOnly());
                nonCached = groupedPairs.GetValueOrDefault(true, []);
                cached = groupedPairs.GetValueOrDefault(false, []);
            }

            static string GetKey(PathsPairValidation pathsPair) => $"TRAVEL_VALIDATION_PATHS_{pathsPair.PathIds.First}_&_{pathsPair.PathIds.Second}";
        }

        private readonly struct PathsPairValidation(int firstPathId, int secondPathId)
        {
            public (int First, int Second) PathIds { get; } = (firstPathId, secondPathId);
            public bool? IsValid { get; init; }
        }

        private readonly record struct TravelTimeFactors(
            TimeSpan AreaBaseTravelTime,
            (TimeSpan Time, double Distance) PathDistanceToTravelTimeConversionFactor,
            IReadOnlyDictionary<DbAreaType, double> AreaTypeFactors,
            IReadOnlyDictionary<DbPathType, double> PathTypeFactors,
            IReadOnlyDictionary<DbPathEnvironment, double> PathEnvironmentFactors);
    }
}

using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;
using Microsoft.Extensions.Caching.Memory;

namespace KrzaqRPG.Backend.WebApi.Services;

public interface ITravelService
{
    ValueTask<DbTravel?> GetTravelAsync(int travelId, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyList<DbPath>> GetTravelPathsAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
    ValueTask<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyDictionary<int, TimeSpan>> ComputeTravelTimeAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
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

    public async ValueTask<DbTravel?> GetTravelAsync(int travelId, CancellationToken cancellationToken = default)
    {
        return await access.GetTravelAsync(travelId, cancellationToken);
    }

    public async ValueTask<IReadOnlyList<DbPath>> GetTravelPathsAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
    {
        return await access.GetPathsAsync(pathIds, cancellationToken);
    }

    [Obsolete("apply character modifiers paths to travel times")]
    public async ValueTask<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default)
    {
        var paths = await ComputeTravelTimeAsync(pathIds, cancellationToken);
        // To Do: apply character modifiers paths to travel times here
        return await access.RegisterTravelAsync(characterId, paths, cancellationToken);
    }

    public async ValueTask<IReadOnlyDictionary<int, TimeSpan>> ComputeTravelTimeAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
    {
        var travelTimes = new Dictionary<int, TimeSpan>();

        foreach (int pathId in pathIds)
        {
            travelTimes.Add(pathId, cache.TryGetValue(GetKey(pathId), out TimeSpan times) ? times : default);
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
            TimeSpan travelTime = path.Distance * distanceTime * factors.PathTypeFactors[path.Type] * factors.PathEnvironmentFactors[path.Environment];
            cache.Set(GetKey(path.Id), travelTimes[path.Id] = travelTime, Settings.SlidingExpiration);
        }

        return travelTimes.AsReadOnly();

        static string GetKey(int pathId) => $"PATH_{pathId}_TRAVEL_TIME";

        async Task<TravelTimeFactors> GetTravelTimeFactorsAsync()
        {
            var pathTypeFactors = GetFromCacheOrDb("PATH_TYPE_TRAVEL_TIME_FACTOR", access.GetPathTypeTravelTimeFactorsAsync);
            var pathEnvironmentFactors = GetFromCacheOrDb("PATH_ENV_TRAVEL_TIME_FACTOR", access.GetPathEnvironmentTravelTimeFactorsAsync);
            var distanceTimeConversionFactor = GetFromCacheOrDb("DISTANCE_TO_TRAVEL_TIME", access.GetDistanceToTravelTimeConversionFactorAsync);

            return new(
                (await pathTypeFactors)!,
                (await pathEnvironmentFactors)!,
                await distanceTimeConversionFactor);

            Task<T> GetFromCacheOrDb<T>(string key, Func<CancellationToken, Task<T>> x) => cache.GetOrCreateAsync<T>(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Settings.SlidingExpiration;
                return x.Invoke(cancellationToken);
            })!;
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
        IReadOnlyDictionary<PathType, double> PathTypeFactors,
        IReadOnlyDictionary<PathEnvironment, double> PathEnvironmentFactors,
        (TimeSpan Time, double Distance) PathDistanceToTravelTimeConversionFactor);
}

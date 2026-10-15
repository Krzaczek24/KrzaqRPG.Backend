using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travel;
using Microsoft.Extensions.Caching.Memory;

namespace KrzaqRPG.Backend.WebApi.Services
{
    public interface ITravelService
    {
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
        }

        private static void Split(
            IEnumerable<PathsPairValidation> pairs,
            out IReadOnlySet<PathsPairValidation> cached,
            out IReadOnlySet<PathsPairValidation> nonCached)
        {
            var groupedPairs = pairs.GroupBy(x => x.IsValid is null).ToDictionary(x => x.Key, x => x.ToHashSet().AsReadOnly());
            nonCached = groupedPairs.GetValueOrDefault(true, []);
            cached = groupedPairs.GetValueOrDefault(false, []);
        }

        private readonly struct PathsPairValidation(int firstPathId, int secondPathId)
        {
            public (int First, int Second) PathIds { get; } = (firstPathId, secondPathId);
            public bool? IsValid { get; init; }
        }

        private static string GetKey(PathsPairValidation pathsPair) => GetKey(pathsPair.PathIds);
        private static string GetKey((int FirstPathId, int SecondPathId) pathIds)
            => $"TRAVEL_VALIDATION_PATHS_{pathIds.FirstPathId}_&_{pathIds.SecondPathId}";
    }
}

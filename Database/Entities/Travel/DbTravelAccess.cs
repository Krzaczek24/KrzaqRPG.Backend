using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelPath;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travel
{
    public interface IDbTravelAccess
    {
        Task<bool> PathExistsAsync(int pathId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
        Task<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default);
    }

    internal class DbTravelAccess(Db db) : IDbTravelAccess
    {
        public Task<bool> PathExistsAsync(int pathId, CancellationToken cancellationToken = default)
            => db.Paths.AnyAsync(p => p.Id == pathId, cancellationToken);

        public async Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var query = from p1 in db.Paths
                        join p2 in db.Paths on p1.EndAreaId equals p2.StartAreaId
                        where pathIds.Contains(p1.Id)
                        where pathIds.Contains(p2.Id)
                        select ValueTuple.Create(p1.Id, p2.Id);

            var validPairs = await query.ToHashSetAsync(cancellationToken);

            return pathIds
                .SlidingWindow(2)
                .Select(x => (x[0], x[1]))
                .ToReadOnlyDictionary(x => x, validPairs.Contains);
        }

        public async Task<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var travel = new DbTravel
            {
                CharacterId = characterId,
                StartTimestamp = DateTime.Now,
            };

            foreach ((int index, int pathId) in pathIds.Index())
            {
                var travelPath = new DbTravelPath
                {
                    PathId = pathId,
                    Sequence = index + 1,
                    Travel = travel,
                };
                travel.TravelPaths.Add(travelPath);
            }

            await db.Travels.AddAsync(travel, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return travel.Id;
        }
    }
}

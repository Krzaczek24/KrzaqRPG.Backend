using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Database.SQLite.Entities.Area;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.Database.SQLite.Entities.Path;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelPath;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travel
{
    public interface IDbTravelAccess
    {
        Task<DbTravel?> GetTravelAsync(int travelId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<DbTravel>> GetAllOngoingTravelsAsync(CancellationToken cancellationToken = default);
        Task<bool> PathExistsAsync(int pathId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
        Task<int> RegisterTravelAsync(int characterId, IList<int> pathIds, CancellationToken cancellationToken = default);
    }

    internal class DbTravelAccess(Db db) : IDbTravelAccess
    {
        public Task<DbTravel?> GetTravelAsync(int travelId, CancellationToken cancellationToken = default)
        {
            return db.Travels
                .Select(t => new DbTravel
                {
                    Id = t.Id,
                    Character = new DbCharacter
                    {
                        Id = t.Character.Id,
                        Name = t.Character.Name,
                        CurrentArea = t.Character.CurrentArea == null ? null : new DbArea
                        {
                            Id = t.Character.CurrentArea.Id,
                            Name = t.Character.CurrentArea.Name,
                        },
                        CurrentPath = t.Character.CurrentPath == null ? null : new DbPath
                        {
                            Id = t.Character.CurrentPath.Id,
                            Name = t.Character.CurrentPath.Name,
                        },
                    },
                    TravelPaths = t.TravelPaths.Select(tp => new DbTravelPath
                    {
                        Id = tp.Id,
                        Path = new DbPath
                        {
                            Id = tp.Path.Id,
                            Name = tp.Path.Name,
                            Distance = tp.Path.Distance,
                            DangerLevel = tp.Path.DangerLevel,
                            DangerProbability = tp.Path.DangerProbability,
                        }
                    }).ToList(),
                    Completed = t.Completed,
                    StartTimestamp = t.StartTimestamp,
                })
                .FirstOrDefaultAsync(t => t.Id == travelId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<DbTravel>> GetAllOngoingTravelsAsync(CancellationToken cancellationToken = default)
        {
            var travels = await db.Travels
                .Where(t => !t.Completed)
                .ToHashSetAsync(cancellationToken);
            return travels.AsReadOnly();
        }

        public Task<bool> PathExistsAsync(int pathId, CancellationToken cancellationToken = default)
        {
            return db.Paths.AnyAsync(p => p.Id == pathId, cancellationToken);
        }

        public async Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var pathsSubset = db.Paths
                .Where(p => pathIds.Contains(p.Id))
                .Select(p => new DbPath
                {
                    Id = p.Id,
                    StartAreaId = p.StartAreaId,
                    EndAreaId = p.EndAreaId,
                });

            var result = await pathsSubset.Join(
                pathsSubset,
                p1 => p1.EndAreaId,
                p2 => p2.StartAreaId,
                (p1, p2) => ValueTuple.Create(p1.Id, p2.Id))
                .ToHashSetAsync(cancellationToken);

            return pathIds
                .SlidingWindow(2)
                .Select(x => (x[0], x[1]))
                .ToReadOnlyDictionary(x => x, result.Contains);
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
                travel.TravelPaths.Add(new DbTravelPath
                {
                    PathId = pathId,
                    Sequence = index + 1,
                    Travel = travel,
                });
            }

            await db.Travels.AddAsync(travel, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return travel.Id;
        }
    }
}

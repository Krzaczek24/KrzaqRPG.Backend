using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelTimes.Base;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travels
{
    public interface IDbTravelAccess
    {
        Task<TimeSpan> GetAreaBaseTravelTimeAsync(CancellationToken cancellationToken = default);
        Task<(TimeSpan Time, double Distance)> GetDistanceToTravelTimeConversionFactorAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlySet<DbTravelTimeFactorTable>> GetTravelTimeFactorsAsync(CancellationToken cancellationToken = default);

        Task<int> RegisterTravelAsync(int characterId, IReadOnlyDictionary<int, (TimeSpan Path, TimeSpan Area)> paths, CancellationToken cancellationToken = default);
        Task<DbTravel?> GetTravelAsync(int travelId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<DbTravel>> GetAllOngoingTravelsAsync(CancellationToken cancellationToken = default);

        Task<bool> PathExistsAsync(int pathId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<DbPath>> GetPathsAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken cancellationToken = default);
    }

    internal class DbTravelAccess(Db db) : IDbTravelAccess
    {
        public Task<TimeSpan> GetAreaBaseTravelTimeAsync(CancellationToken cancellationToken = default)
        {
            return db.Parameters
                .Where(p => p.Name == Parameter.ParamName.AreaBaseTravelTime)
                .Select(p => p.TimeSpan.Value)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<(TimeSpan Time, double Distance)> GetDistanceToTravelTimeConversionFactorAsync(CancellationToken cancellationToken = default)
        {
            return db.Parameters
                .Where(p => p.Name == Parameter.ParamName.DistanceToTravelTimeConversion)
                .Select(p => ValueTuple.Create(p.TimeSpan.Value, p.Numeric.Value))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlySet<DbTravelTimeFactorTable>> GetTravelTimeFactorsAsync(CancellationToken cancellationToken = default)
        {
            var result = await db.TravelTimeFactorAreaTypes
                .Cast<DbTravelTimeFactorTable>()
                .Concat(db.TravelTimeFactorPathTypes)
                .Concat(db.TravelTimeFactorPathEnvironments)
                .ToHashSetAsync(cancellationToken);

            return result.AsReadOnly();
        }

        public async Task<int> RegisterTravelAsync(int characterId, IReadOnlyDictionary<int, (TimeSpan Path, TimeSpan Area)> paths, CancellationToken cancellationToken = default)
        {
            var travel = new DbTravel
            {
                CharacterId = characterId,
                StartTimestamp = DateTime.Now,
            };

            foreach ((int index, (int pathId, (TimeSpan pathTravelTime, TimeSpan areaTravelTime))) in paths.Index())
            {
                travel.TravelPaths.Add(new DbTravelPath
                {
                    PathId = pathId,
                    Sequence = index + 1,
                    Travel = travel,
                    PathTravelTime = pathTravelTime,
                    AreaTravelTime = areaTravelTime,
                });
            }

            await db.Travels.AddAsync(travel, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return travel.Id;
        }

        public Task<DbTravel?> GetTravelAsync(int travelId, CancellationToken cancellationToken = default)
        {
            return db.Travels
                .Select(t => new DbTravel
                {
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
                        Path = new DbPath
                        {
                            Distance = tp.Path.Distance,
                            DangerLevel = tp.Path.DangerLevel,
                            DangerProbability = tp.Path.DangerProbability,
                        }
                    }).OrderBy(tp => tp.Sequence).ToList(),
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

        public async Task<IReadOnlyCollection<DbPath>> GetPathsAsync(IList<int> pathIds, CancellationToken cancellationToken = default)
        {
            var paths = await db.Paths
                .Where(p => pathIds.Contains(p.Id))
                .Select(p => new DbPath
                {
                    Id = p.Id,
                    Distance = p.Id,
                    StartAreaId = p.StartAreaId,
                    EndArea = new DbArea
                    {
                        Id = p.EndArea.Id,
                        Name = p.EndArea.Name,
                        Type = p.EndArea.Type,
                    },
                    DangerLevel = p.DangerLevel,
                    DangerProbability = p.DangerProbability,
                    Type = p.Type,
                    Environment = p.Environment,
                })
                .ToListAsync(cancellationToken);

            var orderedItems = pathIds
                .Join(paths, id => id, p => p.Id, (id, item) => item)
                .ToList();

            return orderedItems.AsReadOnly();
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
    }
}

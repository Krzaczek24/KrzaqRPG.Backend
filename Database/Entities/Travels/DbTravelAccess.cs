using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travels;

public interface IDbTravelAccess
{
    Task<(TimeSpan Time, double Distance)> GetDistanceToTravelTimeConversionFactorAsync(CancellationToken ct = default);
    Task<IReadOnlyDictionary<PathType, double>> GetPathTypeTravelTimeFactorsAsync(CancellationToken ct = default);
    Task<IReadOnlyDictionary<PathEnvironment, double>> GetPathEnvironmentTravelTimeFactorsAsync(CancellationToken ct = default);

    Task<int> RegisterTravelAsync(int characterId, IReadOnlyDictionary<int, TimeSpan> paths, CancellationToken ct = default);
    Task<DbTravel?> GetTravelAsync(int travelId, CancellationToken ct = default);
    Task<IReadOnlySet<DbTravel>> GetAllOngoingTravelsAsync(CancellationToken ct = default);

    Task<bool> PathExistsAsync(int pathId, CancellationToken ct = default);
    Task<IReadOnlyList<DbPath>> GetPathsAsync(IList<int> pathIds, CancellationToken ct = default);
    Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken ct = default);
}

internal class DbTravelAccess(Db db) : IDbTravelAccess
{
    public Task<(TimeSpan Time, double Distance)> GetDistanceToTravelTimeConversionFactorAsync(CancellationToken ct = default)
    {
        return db.Parameters
            .Where(p => p.Name == Parameter.ParamName.DistanceToTravelTimeConversion)
            .Select(p => ValueTuple.Create(p.TimeSpan.Value, p.Numeric.Value))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyDictionary<PathType, double>> GetPathTypeTravelTimeFactorsAsync(CancellationToken ct = default)
    {
        var result = await db.PathTypeTravelTimeFactors.AsNoTracking().ToDictionaryAsync(t => t.Name, t => t.Multiplier, ct);
        return result.AsReadOnly();
    }

    public async Task<IReadOnlyDictionary<PathEnvironment, double>> GetPathEnvironmentTravelTimeFactorsAsync(CancellationToken ct = default)
    {
        var result = await db.PathEnvironmentTravelTimeFactors.AsNoTracking().ToDictionaryAsync(t => t.Name, t => t.Multiplier, ct);
        return result.AsReadOnly();
    }

    public async Task<int> RegisterTravelAsync(int characterId, IReadOnlyDictionary<int, TimeSpan> paths, CancellationToken ct = default)
    {
        var travel = new DbTravel
        {
            CharacterId = characterId,
            StartTimestamp = DateTime.Now,
        };

        foreach ((int index, (int pathId, TimeSpan travelTime)) in paths.Index())
        {
            travel.Paths.Add(new DbTravelPath
            {
                PathId = pathId,
                Sequence = index + 1,
                Travel = travel,
                TravelTime = travelTime,
            });
        }

        await db.Travels.AddAsync(travel, ct);
        await db.SaveChangesAsync(ct);

        return travel.Id;
    }

    public Task<DbTravel?> GetTravelAsync(int travelId, CancellationToken ct = default)
    {
        return db.Travels
            .Where(t => t.Id == travelId)
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
                Paths = t.Paths.OrderBy(tp => tp.Sequence).Select(tp => new DbTravelPath
                {
                    Path = new DbPath
                    {
                        Id = tp.Path.Id,
                        Name = tp.Path.Name,
                        Distance = tp.Path.Distance,
                        DangerLevel = tp.Path.DangerLevel,
                        DangerProbability = tp.Path.DangerProbability,
                    },
                    TravelTime = tp.TravelTime,
                }).ToList(),
                Completed = t.Completed,
                StartTimestamp = t.StartTimestamp,
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlySet<DbTravel>> GetAllOngoingTravelsAsync(CancellationToken ct = default)
    {
        var travels = await db.Travels
            .Where(t => !t.Completed)
            .ToHashSetAsync(ct);

        return travels.AsReadOnly();
    }

    public Task<bool> PathExistsAsync(int pathId, CancellationToken ct = default)
    {
        return db.Paths.AnyAsync(p => p.Id == pathId, ct);
    }

    public async Task<IReadOnlyList<DbPath>> GetPathsAsync(IList<int> pathIds, CancellationToken ct = default)
    {
        var paths = await db.Paths
            .Where(p => pathIds.Contains(p.Id))
            .Select(p => new DbPath
            {
                Id = p.Id,
                Name = p.Name,
                Distance = p.Distance,
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
            .ToListAsync(ct);

        var orderedItems = pathIds
            .Join(paths, id => id, p => p.Id, (id, item) => item)
            .ToList();

        return orderedItems.AsReadOnly();
    }

    public async Task<IReadOnlyDictionary<(int, int), bool>> AreTravelPathsValidAsync(IList<int> pathIds, CancellationToken ct = default)
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
            .ToHashSetAsync(ct);

        return pathIds
            .SlidingWindow(2)
            .Select(x => (x[0], x[1]))
            .ToReadOnlyDictionary(x => x, result.Contains);
    }
}

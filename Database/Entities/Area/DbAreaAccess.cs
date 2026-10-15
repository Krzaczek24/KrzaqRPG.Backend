using KrzaqRPG.Backend.Database.SQLite.Entities.Path;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Area
{
    public interface IDbAreaAccess
    {
        Task<bool> AreaExistsAsync(int areaId, CancellationToken cancellationToken = default);
        Task<IReadOnlySet<DbArea>> ListAreasAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlySet<DbPath>> ListAreaOutgoingPathsAsync(int areaId, CancellationToken cancellationToken = default);
    }

    internal class DbAreaAccess(Db db) : IDbAreaAccess
    {
        public Task<bool> AreaExistsAsync(
            int areaId,
            CancellationToken cancellationToken = default)
        {
            return db.Areas.AnyAsync(a => a.Id == areaId, cancellationToken);
        }

        public async Task<IReadOnlySet<DbArea>> ListAreasAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await db.Areas
                .Select(a => new DbArea
                {
                    Id = a.Id,
                    Name = a.Name,
                })
                .ToHashSetAsync(cancellationToken);

            return result.AsReadOnly();
        }

        public async Task<IReadOnlySet<DbPath>> ListAreaOutgoingPathsAsync(
            int areaId,
            CancellationToken cancellationToken = default)
        {
            var result = await db.Paths
                .Where(p => p.StartAreaId == areaId)
                .Select(p => new DbPath
                {
                    Id = p.Id,
                    Name = p.Name,
                    EndArea = new DbArea
                    {
                        Id = p.EndArea.Id,
                        Name = p.EndArea.Name,
                    },
                    Distance = p.Distance,
                    DangerLevel = p.DangerLevel,
                    DangerProbability = p.DangerProbability,
                })
                .ToHashSetAsync(cancellationToken);

            return result.AsReadOnly();
        }
    }
}

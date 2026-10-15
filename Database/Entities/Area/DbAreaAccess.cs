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
        public async Task<bool> AreaExistsAsync(int areaId, CancellationToken cancellationToken = default)
            => await db.Areas.AnyAsync(a => a.Id == areaId, cancellationToken);

        public async Task<IReadOnlySet<DbArea>> ListAreasAsync(CancellationToken cancellationToken = default)
        {
            var query = from a in db.Areas
                        select new DbArea
                        {
                            Id = a.Id,
                            Name = a.Name,
                        };

            return (await query.ToHashSetAsync(cancellationToken)).AsReadOnly();
        }

        public async Task<IReadOnlySet<DbPath>> ListAreaOutgoingPathsAsync(int areaId, CancellationToken cancellationToken = default)
        {
            var query = from p in db.Paths
                        where p.StartAreaId == areaId
                        select new DbPath
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
                        };

            return (await query.ToHashSetAsync(cancellationToken)).AsReadOnly();
        }
    }
}

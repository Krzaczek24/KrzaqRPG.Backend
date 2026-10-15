using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvent;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJob
{
    public interface IDbEngineWorkerJobAccess
    {
        Task<DbEngineWorkerJob?> GetJobAsync(string name, CancellationToken cancellationToken = default);
        Task UpdateJobParamsAsync(DbEngineWorkerJob config, CancellationToken cancellationToken = default);
        Task LogJobEventAsync(int jobId, WorkerJobEventType type, string message, CancellationToken cancellationToken = default);
    }

    internal class DbEngineWorkerJobAccess(Db db) : IDbEngineWorkerJobAccess
    {
        public Task<DbEngineWorkerJob?> GetJobAsync(string name, CancellationToken cancellationToken = default)
            => db.Jobs.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        public Task UpdateJobParamsAsync(DbEngineWorkerJob config, CancellationToken cancellationToken = default)
            => db.Jobs
                .Where(x => x.Name == config.Name)
                .ExecuteUpdateAsync(update => update
                    .SetProperty(x => x.ErrorCounter, config.ErrorCounter)
                    .SetProperty(x => x.LastRunTimestamp, config.LastRunTimestamp)
                    .SetProperty(x => x.LastSuccessfullRunTimestamp, config.LastSuccessfullRunTimestamp)
                    .SetProperty(x => x.NextRunTimestamp, config.NextRunTimestamp)
                , cancellationToken);

        public async Task LogJobEventAsync(int jobId, WorkerJobEventType type, string message, CancellationToken cancellationToken = default)
        {
            await db.JobEvents.AddAsync(new()
            {
                JobId = jobId,
                Type = type,
                Timestamp = DateTime.Now,
                Message = message,
            }, cancellationToken);
        }
    }
}

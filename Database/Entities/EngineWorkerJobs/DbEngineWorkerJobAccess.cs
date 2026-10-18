using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;

public interface IDbEngineWorkerJobAccess
{
    Task<DbEngineWorkerJob?> GetJobAsync(string name, CancellationToken cancellationToken = default);
    Task UpdateJobParamsAsync(DbEngineWorkerJob config, CancellationToken cancellationToken = default);
    Task LogJobEventAsync(int jobId, WorkerJobEventType type, string message, CancellationToken cancellationToken = default);
}

internal class DbEngineWorkerJobAccess(Db db) : IDbEngineWorkerJobAccess
{
    public Task<DbEngineWorkerJob?> GetJobAsync(string name, CancellationToken cancellationToken = default)
    {
        return db.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public Task UpdateJobParamsAsync(DbEngineWorkerJob config, CancellationToken cancellationToken = default)
    {
        return db.Jobs
            .Where(j => j.Name == config.Name)
            .ExecuteUpdateAsync(update => update
                .SetProperty(j => j.ErrorCounter, config.ErrorCounter)
                .SetProperty(j => j.LastRunTimestamp, config.LastRunTimestamp)
                .SetProperty(j => j.LastSuccessfullRunTimestamp, config.LastSuccessfullRunTimestamp)
                .SetProperty(j => j.NextRunTimestamp, config.NextRunTimestamp)
            , cancellationToken);
    }

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

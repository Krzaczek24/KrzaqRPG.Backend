using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;

public interface IDbEngineWorkerJobAccess
{
    Task<DbEngineWorkerJob?> GetJobAsync(string name, CancellationToken ct = default);
    Task UpdateJobParamsAsync(DbEngineWorkerJob config, CancellationToken ct = default);
    Task LogJobEventAsync(int jobId, WorkerJobEventType type, string message, CancellationToken ct = default);
}

internal class DbEngineWorkerJobAccess(Db db) : IDbEngineWorkerJobAccess
{
    public Task<DbEngineWorkerJob?> GetJobAsync(string name, CancellationToken ct = default)
    {
        return db.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, ct);
    }

    public Task UpdateJobParamsAsync(DbEngineWorkerJob config, CancellationToken ct = default)
    {
        return db.Jobs
            .Where(j => j.Name == config.Name)
            .ExecuteUpdateAsync(update => update
                .SetProperty(j => j.ErrorCounter, config.ErrorCounter)
                .SetProperty(j => j.LastRunTimestamp, config.LastRunTimestamp)
                .SetProperty(j => j.LastSuccessfullRunTimestamp, config.LastSuccessfullRunTimestamp)
                .SetProperty(j => j.NextRunTimestamp, config.NextRunTimestamp)
            , ct);
    }

    public async Task LogJobEventAsync(int jobId, WorkerJobEventType type, string message, CancellationToken ct = default)
    {
        await db.JobEvents.AddAsync(new()
        {
            JobId = jobId,
            Type = type,
            Timestamp = DateTime.Now,
            Message = message,
        }, ct);
    }
}

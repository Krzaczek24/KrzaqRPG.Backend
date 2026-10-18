using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents;

public class DbEngineWorkerJobEvent : DbTable
{
    public virtual int JobId { get; set; }
    public virtual DbEngineWorkerJob Job { get; set; }
    public virtual WorkerJobEventType Type { get; set; }
    public virtual DateTime Timestamp { get; set; }
    public virtual string Message { get; set; }
}

public enum WorkerJobEventType
{
    Error,
    Info,
    Warn,
}

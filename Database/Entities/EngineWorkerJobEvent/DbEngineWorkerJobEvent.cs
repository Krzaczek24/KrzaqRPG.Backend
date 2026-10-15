using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJob;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvent
{
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
}

using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs
{
    public class DbEngineWorkerJob : DbTable
    {
        public virtual string Name { get; set; }
        public virtual WorkerType Type { get; set; }
        public virtual bool Active { get; set; }
        public virtual DateTime? LastRunTimestamp { get; set; }
        public virtual DateTime? LastSuccessfullRunTimestamp { get; set; }
        public virtual DateTime? NextRunTimestamp { get; set; }
        public virtual int ErrorCounter { get; set; }
        public virtual string? CronExpression { get; set; }
        public virtual TimeSpan? IdleInterval { get; set; }
        public virtual int BatchSize { get; set; }
        public virtual ICollection<DbEngineWorkerJobEvent> Events { get; set; } = [];
    }

    public enum WorkerType
    {
        Continuous,
        Schedule,
    }
}

using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJob;

namespace KrzaqRPG.Backend.Engine.Core.Job
{
    public interface IJobContext
    {
        string JobName { get; }
        string InstanceId { get; }
        bool DoNextJob { get; set; }
        WorkerType WorkerType { get; }
    }

    internal record class JobContext : IJobContext
    {
        public required string JobName { get; init; }
        public required string InstanceId { get; init; }
        public bool DoNextJob { get; set; }
        public WorkerType WorkerType { get; init; }
        public ulong Iteration { get; internal set; }
    }
}

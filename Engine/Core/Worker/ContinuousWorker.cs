using Microsoft.Extensions.Caching.Memory;
using RpgAdventureGame.Backend.Database.SQLite.Entities.EngineWorkerJob;
using RpgAdventureGame.Backend.Engine.Core.Job;
using RpgAdventureGame.Backend.Engine.Core.Waiter;
using RpgAdventureGame.Backend.Engine.Core.Worker.Base;

namespace RpgAdventureGame.Backend.Engine.Core.Worker
{
    internal sealed class ContinuousWorker<TJob>(
        ILogger<TJob> logger,
        IMemoryCache cache,
        IServiceScopeFactory scopeFactory,
        IntervalWaiter waiter)
        : EngineWorker<TJob, IntervalWaiter>(logger, cache, scopeFactory, waiter) where TJob : IJob
    {
        protected override void UpdateWaiterTick(IntervalWaiter waiter, DbEngineWorkerJob config)
        {
            waiter.Interval = config.IdleInterval
                ?? throw new InvalidOperationException($"{JobName}'s configuration is invalid, {nameof(config.IdleInterval)} parameter cannot be empty for {nameof(ContinuousWorker<>)}");
        }

        protected override JobContext CreateContext() => base.CreateContext() with { WorkerType = WorkerType.Continuous };
    }
}

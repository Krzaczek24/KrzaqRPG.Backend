using Microsoft.Extensions.Caching.Memory;
using NCrontab;
using RpgAdventureGame.Backend.Database.SQLite.Entities.EngineWorkerJob;
using RpgAdventureGame.Backend.Engine.Core.Job;
using RpgAdventureGame.Backend.Engine.Core.Waiter;
using RpgAdventureGame.Backend.Engine.Core.Worker.Base;

namespace RpgAdventureGame.Backend.Engine.Core.Worker
{
    internal sealed class ScheduleWorker<TJob>(
        ILogger<TJob> logger,
        IMemoryCache cache,
        IServiceScopeFactory scopeFactory,
        ScheduleWaiter waiter)
        : EngineWorker<TJob, ScheduleWaiter>(logger, cache, scopeFactory, waiter) where TJob : IJob
    {
        protected override void UpdateWaiterTick(ScheduleWaiter waiter, DbEngineWorkerJob config)
        {
            if (string.IsNullOrEmpty(config.CronExpression))
                throw new InvalidOperationException($"{JobName}'s configuration is invalid, {nameof(config.CronExpression)} parameter cannot be empty for {nameof(ScheduleWorker<>)}");

            waiter.Cron = CrontabSchedule.TryParse(config.CronExpression)
                ?? throw new InvalidOperationException($"{JobName}'s configuration is invalid, {nameof(config.CronExpression)} parameter has incorrect format");
        }

        protected override JobContext CreateContext() => base.CreateContext() with { WorkerType = WorkerType.Schedule };
    }
}

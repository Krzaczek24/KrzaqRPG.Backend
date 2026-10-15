using KrzaqRPG.Backend.Engine.Core.Waiter.Interface;
using NCrontab;

namespace KrzaqRPG.Backend.Engine.Core.Waiter;

public class ScheduleWaiter(CrontabSchedule cron, TimeProvider? timeProvider = null) : IWaiter
{
    private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;

    public CrontabSchedule Cron { get; set; } = cron;

    public DateTime NextTick => Cron.GetNextOccurrence(timeProvider.GetLocalNow().DateTime);
    public TimeSpan Delay => NextTick - timeProvider.GetLocalNow();

    public Task AwaitAsync(CancellationToken cancellationToken = default)
        => Task.Delay(Delay, cancellationToken);

    public override string ToString() => ((IWaiter)this).Info;
}
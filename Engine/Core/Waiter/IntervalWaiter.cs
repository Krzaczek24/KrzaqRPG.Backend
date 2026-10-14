using RpgAdventureGame.Backend.Engine.Core.Waiter.Interface;

namespace RpgAdventureGame.Backend.Engine.Core.Waiter;

public class IntervalWaiter : IWaiter
{
    private readonly TimeProvider timeProvider;

    private PeriodicTimer Timer { get; }

    public TimeSpan Interval
    {
        get => Timer.Period;
        set => Timer.Period = value;
    }

    public DateTime NextTick { get; private set; }

    public TimeSpan Delay => NextTick - timeProvider.GetLocalNow();

    public IntervalWaiter(TimeSpan interval, TimeProvider? timeProvider = null)
    {
        this.timeProvider = timeProvider ?? TimeProvider.System;
        Timer = new(interval);
        NextTick = this.timeProvider.GetLocalNow().DateTime.Add(Interval);
    }

    public async Task AwaitAsync(CancellationToken cancellationToken = default)
    {
        await Timer.WaitForNextTickAsync(cancellationToken);
        NextTick = timeProvider.GetLocalNow().DateTime.Add(Interval);
    }

    public override string ToString() => ((IWaiter)this).Info;
}
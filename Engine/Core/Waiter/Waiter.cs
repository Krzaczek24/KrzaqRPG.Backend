using KrzaqRPG.Backend.Engine.Core.Waiter.Interface;

namespace KrzaqRPG.Backend.Engine.Core.Waiter;

internal abstract class Waiter<T>(TimeProvider? timeProvider = null) : IWaiter
{
    private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;

    public abstract T Value { get; set; }

    public DateTime NextTick { get; }

    public TimeSpan Delay => NextTick - timeProvider.GetLocalNow();

    public abstract Task AwaitAsync(CancellationToken cancellationToken);

    public override string ToString() => ((IWaiter)this).Info;
}
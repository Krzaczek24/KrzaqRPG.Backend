namespace RpgAdventureGame.Backend.Engine.Core.Waiter.Interface
{
    public interface IWaiter
    {
        DateTime NextTick { get; }
        TimeSpan Delay { get; }
        Task AwaitAsync(CancellationToken cancellationToken = default);
        string Info => $"{NextTick:yyyy-MM-dd HH:mm:ss} (in {Delay:c})";
    }
}

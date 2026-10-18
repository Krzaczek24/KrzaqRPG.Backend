namespace KrzaqRPG.Backend.Engine.Core.Job;

public interface IJob
{
    Task ExecuteAsync(IJobContext context, CancellationToken stoppingToken);
}

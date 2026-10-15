using KrzaqRPG.Backend.Engine.Core.Job;

namespace KrzaqRPG.Backend.Engine.Jobs
{
    internal class MapEventSettingJob : IJob
    {
        public async Task ExecuteAsync(IJobContext context, CancellationToken stoppingToken)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {context}");
        }
    }
}

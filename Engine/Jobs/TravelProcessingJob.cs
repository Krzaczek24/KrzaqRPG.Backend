using RpgAdventureGame.Backend.Engine.Core.Job;

namespace RpgAdventureGame.Backend.Engine.Jobs
{
    internal class TravelProcessingJob : IJob
    {
        public async Task ExecuteAsync(IJobContext context, CancellationToken stoppingToken)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {context}");
        }
    }
}

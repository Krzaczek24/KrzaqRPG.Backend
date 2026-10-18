using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;
using KrzaqRPG.Backend.Engine.Core.Job;

namespace KrzaqRPG.Backend.Engine.Jobs;

internal class TravelProcessingJob(IDbTravelAccess access) : IJob
{
    public async Task ExecuteAsync(IJobContext context, CancellationToken stoppingToken)
    {
        var travels = await access.GetAllOngoingTravelsAsync(stoppingToken);

        foreach (var travel in travels)
        {
            // get character current area / path entry event timestamp
            // find character current area / path on travel

        }
    }
}

using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetEstimation;

public class GetTravelEstimationQueryHandler(
    //IDbCharacterAccess characterAccess,
    ITravelService travelService)
    : IRequestHandler<GetTravelEstimationQuery, GetTravelEstimationQueryResult>
{
    public async ValueTask<GetTravelEstimationQueryResult> Handle(GetTravelEstimationQuery request, CancellationToken cancellationToken = default)
    {
        //var character = characterAccess.GetCharacterAsync(request.CharacterId, cancellationToken);
        var travelPaths = await travelService.GetTravelPathsAsync(request.PathIds, cancellationToken);
        var travelTimes = await travelService.ComputeTravelTimeAsync(request.PathIds, cancellationToken);

        var paths = travelPaths.Join(travelTimes, p => p.Id, t => t.Key, (p, t) =>
        {
            return new GetTravelEstimationPathDto
            {
                Name = p.Name,
                Distance = p.Distance,
                DangerLevel = p.DangerLevel,
                DangerProbability = p.DangerProbability,
                TravelTime = t.Value, // apply character modifiers paths to travel times here
            };
        }).ToList();

        var response = new GetTravelEstimationQueryResult
        {
            Paths = paths.AsReadOnly(),
            TotalDistance = paths.Sum(p => p.Distance),
            TotalTravelTime = paths.Aggregate(TimeSpan.Zero, (sum, next) => sum += next.TravelTime),
        };

        return response;
    }
}

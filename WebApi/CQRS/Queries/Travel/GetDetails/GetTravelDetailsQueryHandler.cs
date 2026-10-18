using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Exceptions;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails;

public class GetTravelDetailsQueryHandler(ITravelService travelService) : IRequestHandler<GetTravelDetailsQuery, GetTravelDetailsQueryResult>
{
    public async ValueTask<GetTravelDetailsQueryResult> Handle(GetTravelDetailsQuery request, CancellationToken cancellationToken = default)
    {
        var travel = await travelService.GetTravelAsync(request.TravelId, cancellationToken)
            ?? throw new NotFoundException(ErrorCode.TravelNotFound);

        var paths = travel.TravelPaths.Select(tp => new GetTravelDetailsPathDto
        {
            Name = tp.Path.Name,
            Distance = tp.Path.Distance,
            DangerLevel = tp.Path.DangerLevel,
            DangerProbability = tp.Path.DangerProbability,
            AreaTravelTime = tp.AreaTravelTime,
            PathTravelTime = tp.TravelTime,
        }).ToList();

        var response = new GetTravelDetailsQueryResult
        {
            Character = new GetTravelDetailsCharacterDto
            {
                Id = travel.Character.Id,
                Name = travel.Character.Name,
                Location = travel.Character switch
                {
                    { CurrentArea: not null } => new()
                    {
                        Type = CharacterLocationType.Area,
                        Id = travel.Character.CurrentArea.Id,
                        Name = travel.Character.CurrentArea.Name
                    },
                    { CurrentPath: not null } => new()
                    {
                        Type = CharacterLocationType.Path,
                        Id = travel.Character.CurrentPath.Id,
                        Name = travel.Character.CurrentPath.Name
                    },
                    _ => null
                },
            },
            Paths = paths.AsReadOnly(),
            IsCompleted = travel.Completed,
            StartTimestamp = travel.StartTimestamp,
            TotalTravelTime = paths.Aggregate(TimeSpan.Zero, (sum, next) => sum += next.PathTravelTime + next.AreaTravelTime),
        };

        return response;
    }
}

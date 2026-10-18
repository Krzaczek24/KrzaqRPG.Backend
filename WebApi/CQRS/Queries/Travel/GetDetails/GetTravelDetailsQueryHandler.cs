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

        var paths = travel.Paths.Select(tp => new GetTravelDetailsPathDto
        {
            Id = tp.Path.Id,
            Name = tp.Path.Name,
            Distance = tp.Path.Distance,
            DangerLevel = tp.Path.DangerLevel,
            DangerProbability = tp.Path.DangerProbability,
            TravelTime = tp.TravelTime,
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
            TotalDistance = paths.Sum(p => p.Distance),
            TotalTravelTime = paths.Aggregate(TimeSpan.Zero, (sum, next) => sum += next.TravelTime),
        };

        return response;
    }
}

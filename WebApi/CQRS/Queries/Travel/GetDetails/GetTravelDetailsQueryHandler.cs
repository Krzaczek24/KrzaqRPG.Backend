using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Exceptions;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails
{
    public class GetTravelDetailsQueryHandler(IDbTravelAccess travelAccess) : IRequestHandler<GetTravelDetailsQuery, GetTravelDetailsQueryResult>
    {
        public async ValueTask<GetTravelDetailsQueryResult> Handle(GetTravelDetailsQuery request, CancellationToken cancellationToken = default)
        {
            var travel = await travelAccess.GetTravelAsync(request.TravelId, cancellationToken)
                ?? throw new NotFoundException(ErrorCode.TravelNotFound);

            var paths = travel.TravelPaths.Select(tp => new GetTravelDetailsPathDto
            {
                Name = tp.Path.Name,
                Distance = tp.Path.Distance,
                DangerLevel = tp.Path.DangerLevel,
                DangerProbability = tp.Path.DangerProbability,
                TravelTime = TimeSpan.FromDays(1), // TODO
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
                TotalTravelTime = TimeSpan.FromSeconds(paths.Sum(x => x.TravelTime.TotalSeconds)),
            };

            return response;
        }
    }
}

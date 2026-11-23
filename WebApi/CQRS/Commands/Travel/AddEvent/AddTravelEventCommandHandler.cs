using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.AddEvent;

public class AddTravelEventCommandHandler(IDbTravelEventAccess eventAccess) : IRequestHandler<AddTravelEventCommand, AddTravelEventCommandResult>
{
    public async ValueTask<AddTravelEventCommandResult> Handle(AddTravelEventCommand request, CancellationToken cancellationToken = default)
    {
        int id = request.LocationType switch
        {
            TravelEventLocationType.Area => await eventAccess.AddTravelAreaEventAsync(request.Timestamp, request.Type, request.TravelId, request.LocationId, cancellationToken),
            TravelEventLocationType.Path => await eventAccess.AddTravelPathEventAsync(request.Timestamp, request.Type, request.TravelId, request.LocationId, cancellationToken),
            _ => throw new InvalidProgramException($"Invalid location type [{request.LocationType}]")
        };

        return new AddTravelEventCommandResult { Id = id };
    }
}

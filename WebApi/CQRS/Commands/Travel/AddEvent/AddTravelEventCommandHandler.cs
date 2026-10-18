using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.AddEvent;

public class AddTravelEventCommandHandler(IDbTravelEventAccess eventAccess) : IRequestHandler<AddTravelEventCommand, AddTravelEventCommandResult>
{
    public async ValueTask<AddTravelEventCommandResult> Handle(AddTravelEventCommand request, CancellationToken cancellationToken = default)
    {
        int id = default;

        switch (request.LocationType)
        {
            case TravelEventLocationType.Area: id = await eventAccess.AddTravelAreaEventAsync(request.Timestamp, request.Type, request.TravelId, request.LocationId, cancellationToken); break;
            case TravelEventLocationType.Path: id = await eventAccess.AddTravelPathEventAsync(request.Timestamp, request.Type, request.TravelId, request.LocationId, cancellationToken); break;
        }

        return new AddTravelEventCommandResult { Id = id };
    }
}

using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin;

public class BeginTravelCommandHandler(
    ITravelService travelService)
    : IRequestHandler<BeginTravelCommand, BeginTravelCommandResult>
{
    public async ValueTask<BeginTravelCommandResult> Handle(BeginTravelCommand request, CancellationToken cancellationToken = default)
    {
        int id = await travelService.RegisterTravelAsync(request.CharacterId, request.PathIds, cancellationToken);
        return new() { Id = id };
    }
}

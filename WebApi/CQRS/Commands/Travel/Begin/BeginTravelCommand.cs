using Krzaq.MediatR.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin;

public class BeginTravelCommand : IRequest<BeginTravelCommandResult>
{
    public required int CharacterId { get; init; }
    public required IList<int> PathIds { get; init; }
}

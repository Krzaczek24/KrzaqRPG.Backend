using Krzaq.MediatR.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetEstimation;

public class GetTravelEstimationQuery : IRequest<GetTravelEstimationQueryResult>
{
    public required int CharacterId { get; init; }
    public required IList<int> PathIds { get; init; }
}

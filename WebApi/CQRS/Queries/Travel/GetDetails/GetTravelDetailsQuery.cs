using Krzaq.MediatR.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails;

public class GetTravelDetailsQuery : IRequest<GetTravelDetailsQueryResult>
{
    public int TravelId { get; set; }
}

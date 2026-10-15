using Krzaq.MediatR.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.GetAvailablePaths
{
    public class GetAreaAvailablePathsQuery : IRequest<GetAreaAvailablePathsQueryResult>
    {
        public required int AreaId { get; init; }
    }
}

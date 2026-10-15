using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Entities.Area;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.List
{
    public class ListAreasQueryHandler(IDbAreaAccess areaAccess)
        : IRequestHandler<ListAreasQuery, ListAreasQueryResult>
    {
        public async ValueTask<ListAreasQueryResult> Handle(ListAreasQuery request, CancellationToken cancellationToken = default)
        {
            var areas = await areaAccess.ListAreasAsync(cancellationToken);

            return new()
            {
                Areas = areas.Select(area => new AreaDto
                {
                    Id = area.Id,
                    Name = area.Name
                }).ToHashSet().AsReadOnly(),
            };
        }
    }
}

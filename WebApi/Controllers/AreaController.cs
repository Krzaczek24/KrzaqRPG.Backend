using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.GetAvailablePaths;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.List;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers
{
    public class AreaController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async ValueTask<ActionResult<ListAreasQueryResult>> GetList()
            => Ok(await mediator.Send(new ListAreasQuery()));

        [HttpGet("{id}/available-paths")]
        public async ValueTask<ActionResult<GetAreaAvailablePathsQueryResult>> GetAvailablePaths([FromRoute] int id)
            => Ok(await mediator.Send(new GetAreaAvailablePathsQuery() { AreaId = id }));
    }
}

using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.GetAvailablePaths;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.List;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers;

public class AreasController(IMediator mediator) : ApiController
{
    [HttpGet]
    public async ValueTask<ActionResult<ListAreasQueryResult>> GetAreas(
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new ListAreasQuery(), cancellationToken));
    }

    [HttpGet("{areaId}/available-paths")]
    public async ValueTask<ActionResult<GetAreaAvailablePathsQueryResult>> GetAreaAvailablePaths(
        [FromRoute] int areaId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAreaAvailablePathsQuery { AreaId = areaId }, cancellationToken));
    }
}

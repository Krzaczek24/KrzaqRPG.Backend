using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.AddEvent;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetEstimation;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers;

public class TravelsController(IMediator mediator) : ApiController
{
    [HttpPost]
    public async ValueTask<ActionResult<BeginTravelCommandResult>> Begin(
        [FromBody] BeginTravelCommand command,
        CancellationToken cancellationToken)
    {
        return Created(await mediator.Send(command, cancellationToken));
    }

    [HttpGet("{id}")]
    public async ValueTask<ActionResult<GetTravelDetailsQueryResult>> GetDetails(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetTravelDetailsQuery { TravelId = id }, cancellationToken));
    }

    [HttpGet("estimation")]
    [Obsolete("apply character modifiers paths to travel times")]
    public async ValueTask<ActionResult<GetTravelEstimationQueryResult>> GetTravelEstimation(
        [FromQuery] GetTravelEstimationQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(query, cancellationToken));
    }

    [HttpPost("{id}/events")]
    public async ValueTask<ActionResult<AddTravelEventCommandResult>> AddEvent(
        [FromRoute] int id,
        [FromBody] AddTravelEventCommand command,
        CancellationToken cancellationToken)
    {
        return Created(await mediator.Send(command with
        {
            TravelId = id,
            Timestamp = DateTime.Now,
        }, cancellationToken));
    }
}

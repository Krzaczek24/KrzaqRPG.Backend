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
    public async ValueTask<ActionResult<BeginTravelCommandResult>> BeginTravel(
        [FromBody] BeginTravelCommand command,
        CancellationToken cancellationToken)
    {
        return Created(await mediator.Send(command, cancellationToken));
    }

    [HttpGet("{travelId}")]
    public async ValueTask<ActionResult<GetTravelDetailsQueryResult>> GetTravelDetails(
        [FromRoute] int travelId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetTravelDetailsQuery { TravelId = travelId }, cancellationToken));
    }

    [HttpGet("estimation")]
    public async ValueTask<ActionResult<GetTravelEstimationQueryResult>> EstimateTravel(
        [FromQuery] GetTravelEstimationQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(query, cancellationToken));
    }

    [HttpPost("{travelId}/events")]
    public async ValueTask<ActionResult<AddTravelEventCommandResult>> AddTravelEvent(
        [FromRoute] int travelId,
        [FromBody] AddTravelEventCommand command,
        CancellationToken cancellationToken)
    {
        return Created(await mediator.Send(command with
        {
            TravelId = travelId,
            Timestamp = DateTime.Now,
        }, cancellationToken));
    }
}

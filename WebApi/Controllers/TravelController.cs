using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers
{
    public class TravelController(IMediator mediator) : ApiController
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
            return Ok(await mediator.Send(new GetTravelDetailsQuery { Id = id }, cancellationToken));
        }
    }
}

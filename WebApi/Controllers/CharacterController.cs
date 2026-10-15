using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.Create;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.SetLocation;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.Search;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers
{
    public class CharacterController(IMediator mediator) : ApiController
    {
        [HttpPost]
        public async ValueTask<ActionResult<CreateCharacterCommandResult>> Create(
            [FromBody] CreateCharacterCommand command,
            CancellationToken cancellationToken)
        {
            return Created(await mediator.Send(command, cancellationToken));
        }

        [HttpGet("{id}")]
        public async ValueTask<ActionResult<GetCharacterDetailsQueryResult>> GetDetails(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send(new GetCharacterDetailsQuery
            {
                CharacterId = id
            }, cancellationToken));
        }

        [HttpGet("search")]
        public async ValueTask<ActionResult<SearchCharactersQueryResult>> Search(
            [FromQuery] SearchCharactersQuery command,
            CancellationToken cancellationToken)
        {
            return Ok(await mediator.Send(command, cancellationToken));
        }

        [HttpPatch("{id}/current-area/{areaId}")]
        public async ValueTask<ActionResult> SetArea(
            [FromRoute] int id,
            [FromRoute] int areaId,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new SetCharacterLocationCommand
            {
                CharacterId = id,
                LocationId = areaId,
                LocationType = CharacterLocationType.Area
            }, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id}/current-path/{pathId}")]
        public async ValueTask<ActionResult> SetPath(
            [FromRoute] int id,
            [FromRoute] int pathId,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new SetCharacterLocationCommand
            {
                CharacterId = id,
                LocationId = pathId,
                LocationType = CharacterLocationType.Path
            }, cancellationToken);
            return NoContent();
        }
    }
}

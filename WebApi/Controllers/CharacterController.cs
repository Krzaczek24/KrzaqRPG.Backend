using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.Create;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.SetLocation;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.StartTravel;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.StartTravel;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.Search;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers
{
    public class CharacterController(IMediator mediator) : ApiController
    {
        [HttpPost]
        public async ValueTask<ActionResult<CreateCharacterCommandResult>> Create([FromBody] CreateCharacterCommand command)
            => Created(await mediator.Send(command));

        [HttpGet("{id}")]
        public async ValueTask<ActionResult<GetCharacterDetailsQueryResult>> GetDetails([FromRoute] int id)
            => Ok(await mediator.Send(new GetCharacterDetailsQuery() { CharacterId = id }));

        [HttpGet("search")]
        public async ValueTask<ActionResult<SearchCharactersQueryResult>> Search([FromQuery] SearchCharactersQuery command)
            => Ok(await mediator.Send(command));

        [HttpPatch("{id}/current-area/{areaId}")]
        public async ValueTask<ActionResult> SetArea([FromRoute] int id, [FromRoute] int areaId)
        {
            await mediator.Send(new SetCharacterLocationCommand { CharacterId = id, LocationId = areaId, LocationType = CharacterLocationType.Area });
            return NoContent();
        }

        [HttpPatch("{id}/current-path/{pathId}")]
        public async ValueTask<ActionResult> SetPath([FromRoute] int id, [FromRoute] int pathId)
        {
            await mediator.Send(new SetCharacterLocationCommand { CharacterId = id, LocationId = pathId, LocationType = CharacterLocationType.Path });
            return NoContent();
        }

        [HttpPost("{id}/start-travel")]
        public async ValueTask<ActionResult<CharacterStartTravelCommandResult>> Travel([FromRoute] int id, [FromQuery] int[] pathId)
            => Created(await mediator.Send(new CharacterStartTravelCommand { CharacterId = id, PathIds = pathId }));
    }
}

using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.WebApi.Core.Controllers;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.Create;
using KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.SetLocation;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails;
using KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.Search;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers;

public class CharactersController(IMediator mediator) : ApiController
{
    [HttpPost]
    public async ValueTask<ActionResult<CreateCharacterCommandResult>> CreateCharacter(
        [FromBody] CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        return Created(await mediator.Send(command, cancellationToken));
    }

    [HttpGet("{characterId}")]
    public async ValueTask<ActionResult<GetCharacterDetailsQueryResult>> GetCharacterDetails(
        [FromRoute] int characterId,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCharacterDetailsQuery
        {
            CharacterId = characterId
        }, cancellationToken));
    }

    [HttpGet("search")]
    public async ValueTask<ActionResult<SearchCharactersQueryResult>> SearchCharacter(
        [FromQuery] SearchCharactersQuery command,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }

    [HttpPatch("{characterId}/location")]
    public async ValueTask<ActionResult> SetCharacterLocation(
        [FromRoute] int characterId,
        [FromBody] SetCharacterLocationCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command with { CharacterId = characterId }, cancellationToken);
        return NoContent();
    }
}

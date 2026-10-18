using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Common.Enums;
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

    [HttpPatch("{id}/location")]
    public async ValueTask<ActionResult> SetLocation(
        [FromRoute] int id,
        [FromBody] SetCharacterLocationCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command with { CharacterId = id }, cancellationToken);
        return NoContent();
    }
}

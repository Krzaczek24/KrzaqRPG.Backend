using FluentValidation;
using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Extensions;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin;

public class BeginTravelCommandValidator : RequestValidator<BeginTravelCommand>
{
    public BeginTravelCommandValidator(
        IDbCharacterAccess characterAccess,
        ITravelService travelService)
    {
        IReadOnlySet<(int, int)> invalidPaths = new HashSet<(int, int)>();

        RuleFor(x => x.CharacterId)
            .NotEmpty()
            .MustAsync(async (id, cancellationToken) => await characterAccess.CharacterExistsAsync(id, cancellationToken))
                .WithErrorCode(ErrorCode.CharacterNotFound)
            .MustAsync(async (id, cancellationToken) => !await characterAccess.IsCharacterTravelingAsync(id, cancellationToken))
                .WithErrorCode(ErrorCode.CharacterInTravel)
            .DependentRules(() => {
                RuleFor(x => x.PathIds)
                    .NotEmpty()
                    .MustAsync(async (command, ids, cancellationToken) => await characterAccess.IsCharacterOnPathBeginningAsync(command.CharacterId, ids[0], cancellationToken))
                        .WithErrorCode(ErrorCode.CharacterNotInTravelStartingArea)
                    .MustAsync(async (ids, cancellationToken) => (invalidPaths = await travelService.GetInvalidTravelPathsAsync(ids, cancellationToken)).Count == 0)
                        .WithErrorCode(ErrorCode.InvalidTravelPaths, string.Join(", ", invalidPaths));
            });
    }
}

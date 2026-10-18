using FluentValidation;
using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Extensions;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin
{
    public class BeginTravelCommandValidator : RequestValidator<BeginTravelCommand>
    {
        public BeginTravelCommandValidator(
            IDbCharacterAccess characterAccess,
            ITravelService travelService)
        {
            IReadOnlySet<(int, int)> invalidPaths = new HashSet<(int, int)>();

            RuleFor(x => x.CharacterId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await characterAccess.CharacterExistsAsync(id, ct))
                    .WithErrorCode(ErrorCode.CharacterNotFound)
                .MustAsync(async (id, ct) => !await characterAccess.IsCharacterTravelingAsync(id, ct))
                    .WithErrorCode(ErrorCode.CharacterInTravel)
                .DependentRules(() => {
                    RuleFor(x => x.PathIds)
                        .NotEmpty()
                        .MustAsync(async (ids, ct) => (invalidPaths = await travelService.GetInvalidTravelPathsAsync(ids, ct)).Count == 0)
                            .WithErrorCode(ErrorCode.InvalidTravelPaths, string.Join(", ", invalidPaths));
                });
        }
    }
}

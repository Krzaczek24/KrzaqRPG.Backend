using FluentValidation;
using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Extensions;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.SetLocation;

public class SetCharacterLocationCommandValidator
     : RequestValidator<SetCharacterLocationCommand>
{
    public SetCharacterLocationCommandValidator(
        IDbAreaAccess areaAccess,
        IDbCharacterAccess characterAccess,
        IDbTravelAccess travelAccess)
    {
        RuleFor(x => x.CharacterId)
            .NotNull()
            .MustAsync(async (id, cancellationToken) => await characterAccess.CharacterExistsAsync(id, cancellationToken))
                .WithErrorCode(ErrorCode.CharacterNotFound);

        RuleFor(x => x.LocationId)
            .NotNull()
            .MustAsync(async (id, cancellationToken) => await areaAccess.AreaExistsAsync(id, cancellationToken))
                .WithErrorCode(ErrorCode.AreaNotFound)
                .When(x => x.LocationType is CharacterLocationType.Area, ApplyConditionTo.CurrentValidator)
            .MustAsync(async (id, cancellationToken) => await travelAccess.PathExistsAsync(id, cancellationToken))
                .WithErrorCode(ErrorCode.PathNotFound)
                .When(x => x.LocationType is CharacterLocationType.Path, ApplyConditionTo.CurrentValidator);
    }
}

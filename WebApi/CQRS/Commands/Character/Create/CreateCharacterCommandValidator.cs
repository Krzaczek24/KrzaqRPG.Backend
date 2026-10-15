using FluentValidation;
using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Extensions;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.Create
{
    public class CreateCharacterCommandValidator : RequestValidator<CreateCharacterCommand>
    {
        public CreateCharacterCommandValidator(IDbCharacterAccess characterAccess)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(async (name, ct) => !await characterAccess.IsCharacterNameUsedAsync(name, ct))
                    .WithErrorCode(ErrorCode.NameAlreadyInUse);
        }
    }
}

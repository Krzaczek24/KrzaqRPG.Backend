using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.Create;

public class CreateCharacterCommandHandler(IDbCharacterAccess characterAccess)
    : IRequestHandler<CreateCharacterCommand, CreateCharacterCommandResult>
{
    public async ValueTask<CreateCharacterCommandResult> Handle(CreateCharacterCommand request, CancellationToken cancellationToken = default)
    {
        int id = await characterAccess.CreateCharacterAsync(request.Name, cancellationToken);
        return new CreateCharacterCommandResult { Id = id };
    }
}

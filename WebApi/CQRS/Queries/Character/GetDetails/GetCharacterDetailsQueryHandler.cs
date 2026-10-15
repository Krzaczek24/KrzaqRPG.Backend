using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Exceptions;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails
{
    public class GetCharacterDetailsQueryHandler(IDbCharacterAccess characterAccess)
        : IRequestHandler<GetCharacterDetailsQuery, GetCharacterDetailsQueryResult>
    {
        public async ValueTask<GetCharacterDetailsQueryResult> Handle(GetCharacterDetailsQuery request, CancellationToken cancellationToken = default)
        {
            var character = await characterAccess.GetCharacterAsync(request.CharacterId, cancellationToken)
                ?? throw new NotFoundException(ErrorCode.CharacterNotFound);

            return new()
            {
                Id = character.Id,
                Name = character.Name,
                Level = character.Level,
                Experience = character.Experience,
                CurrentHealth = character.CurrentHealth,
                MaxHealth = character.MaxHealth,
                Location = character switch
                {
                    { CurrentArea: not null } => new()
                    {
                        Type = CharacterLocationType.Area,
                        Id = character.CurrentArea.Id,
                        Name = character.CurrentArea.Name
                    },
                    { CurrentPath: not null } => new()
                    {
                        Type = CharacterLocationType.Path,
                        Id = character.CurrentPath.Id,
                        Name = character.CurrentPath.Name
                    },
                    _ => null
                },
            };
        }
    }
}

using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Entities.Area;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.Database.SQLite.Entities.Path;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travel;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Exceptions;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.StartTravel
{
    public class CharacterStartTravelCommandHandler(
        IDbCharacterAccess characterAccess,
        IDbAreaAccess areaAccess,
        IDbTravelAccess travelAccess)
        : IRequestHandler<CharacterStartTravelCommand, CharacterStartTravelCommandResult>
    {
        public async ValueTask<CharacterStartTravelCommandResult> Handle(CharacterStartTravelCommand request, CancellationToken cancellationToken = default)
        {
            var character = await characterAccess.GetCharacterAsync(request.CharacterId, cancellationToken);
            var currentArea = character!.CurrentArea ?? throw new ConflictException(ErrorCode.CharacterOutsideArea);
            var availablePaths = await GetAvailablePaths();

            foreach ((int idx, int pathId) in request.PathIds.Index())
            {
                var selectedPath = availablePaths.FirstOrDefault(x => x.Id == pathId) ?? throw new ConflictException(ErrorCode.InvalidTravelPaths);
                currentArea = selectedPath.EndArea;
                if (idx < request.PathIds.Count - 1)
                    availablePaths = await GetAvailablePaths();
            }

            int id = await travelAccess.RegisterTravelAsync(character.Id, request.PathIds, cancellationToken);

            return new() { Id = id };

            Task<IReadOnlySet<DbPath>> GetAvailablePaths() => areaAccess.ListAreaOutgoingPathsAsync(currentArea.Id, cancellationToken);
        }
    }
}

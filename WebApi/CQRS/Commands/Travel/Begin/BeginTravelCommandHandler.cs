using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Exceptions;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.Begin
{
    public class BeginTravelCommandHandler(
        IDbCharacterAccess characterAccess,
        IDbAreaAccess areaAccess,
        ITravelService travelService)
        : IRequestHandler<BeginTravelCommand, BeginTravelCommandResult>
    {
        public async ValueTask<BeginTravelCommandResult> Handle(BeginTravelCommand request, CancellationToken cancellationToken = default)
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

            int id = await travelService.RegisterTravelAsync(character.Id, request.PathIds, cancellationToken);

            return new() { Id = id };

            Task<IReadOnlySet<DbPath>> GetAvailablePaths() => areaAccess.ListAreaOutgoingPathsAsync(currentArea.Id, cancellationToken);
        }
    }
}

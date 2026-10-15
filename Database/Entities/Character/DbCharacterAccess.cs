using KrzaqRPG.Backend.Database.SQLite.Entities.Area;
using KrzaqRPG.Backend.Database.SQLite.Entities.Path;
using KrzaqRPG.Backend.Database.SQLite.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Character
{
    public interface IDbCharacterAccess
    {
        Task<bool> CharacterExistsAsync(int characterId, CancellationToken cancellationToken = default);
        Task<bool> IsCharacterNameUsedAsync(string characterName, CancellationToken cancellationToken = default);
        Task<bool> IsCharacterTravelingAsync(int characterId, CancellationToken cancellationToken = default);
        Task<int> CreateCharacterAsync(string characterName, CancellationToken cancellationToken = default);
        Task<DbCharacter?> GetCharacterAsync(int characterId, CancellationToken cancellationToken = default);
        Task<IReadOnlySet<DbCharacter>> SearchCharactersAsync(ISearchCharacterParams searchParams, CancellationToken cancellationToken = default);
        Task<bool> SetCharacterCurrentAreaAsync(int characterId, int areaId, CancellationToken cancellationToken = default);
        Task<bool> SetCharacterCurrentPathAsync(int characterId, int pathId, CancellationToken cancellationToken = default);
    }

    internal class DbCharacterAccess(Db db) : IDbCharacterAccess
    {
        public Task<bool> CharacterExistsAsync(
            int characterId,
            CancellationToken cancellationToken = default)
        {
            return db.Characters.AnyAsync(c => c.Id == characterId, cancellationToken);
        }

        public Task<bool> IsCharacterNameUsedAsync(
            string characterName,
            CancellationToken cancellationToken = default)
        {
            return db.Characters.AnyAsync(c => EF.Functions.Like(c.Name, characterName), cancellationToken);
        }

        public Task<bool> IsCharacterTravelingAsync(
            int characterId,
            CancellationToken cancellationToken = default)
        {
            return db.Characters.AnyAsync(c => c.Id == characterId && (c.CurrentTravel != null || c.CurrentTravel.Completed), cancellationToken);
        }

        public async Task<int> CreateCharacterAsync(
            string characterName,
            CancellationToken cancellationToken = default)
        {
            var character = new DbCharacter { Name = characterName };

            await db.Characters.AddAsync(character, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return character.Id;
        }

        public async Task<DbCharacter?> GetCharacterAsync(int characterId, CancellationToken cancellationToken = default)
        {
            var result = await db.Characters
                .Select(c => new DbCharacter
                {
                    Id = c.Id,
                    Name = c.Name,
                    CurrentArea = c.CurrentArea == null ? null : new DbArea
                    {
                        Id = c.CurrentArea.Id,
                        Name = c.CurrentArea.Name,
                    },
                    CurrentPath = c.CurrentPath == null ? null : new DbPath
                    {
                        Id = c.CurrentPath.Id,
                        Name = c.CurrentPath.Name,
                    },
                })
                .FirstOrDefaultAsync(c => c.Id == characterId, cancellationToken);

            return result;
        }

        public async Task<IReadOnlySet<DbCharacter>> SearchCharactersAsync(ISearchCharacterParams searchParams, CancellationToken cancellationToken = default)
        {
            var result = await db.Characters
                .Where(c => EF.Functions.Like(c.Name, $"%{searchParams.Name}%"))
                .Where(c => searchParams.AreaIds.Count == 0 || (c.CurrentAreaId.HasValue && searchParams.AreaIds.Contains(c.CurrentAreaId.Value)))
                .Where(c => searchParams.PathIds.Count == 0 || (c.CurrentPathId.HasValue && searchParams.PathIds.Contains(c.CurrentPathId.Value)))
                .Where(c => searchParams.MinLevel == null || c.Level >= searchParams.MinLevel)
                .Where(c => searchParams.MaxLevel == null || c.Level <= searchParams.MaxLevel)
                .Select(c => new DbCharacter
                {
                    Id = c.Id,
                    Name = c.Name,
                    CurrentArea = c.CurrentArea == null ? null : new DbArea
                    {
                        Id = c.CurrentArea.Id,
                        Name = c.CurrentArea.Name,
                    },
                    CurrentPath = c.CurrentPath == null ? null : new DbPath
                    {
                        Id = c.CurrentPath.Id,
                        Name = c.CurrentPath.Name,
                    },
                })
                .ToHashSetAsync(cancellationToken);

            return result.AsReadOnly();
        }

        public Task<bool> SetCharacterCurrentAreaAsync(int characterId, int areaId, CancellationToken cancellationToken = default)
        {
            return SetCurrentLocation(characterId, areaId, null, cancellationToken);
        }

        public Task<bool> SetCharacterCurrentPathAsync(int characterId, int pathId, CancellationToken cancellationToken = default)
        {
            return SetCurrentLocation(characterId, null, pathId, cancellationToken);
        }

        private async Task<bool> SetCurrentLocation(int characterId, int? areaId, int? pathId, CancellationToken cancellationToken = default)
        {
            int affectedRows = await db.Characters
                .Where(c => c.Id == characterId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(c => c.CurrentAreaId, areaId)
                    .SetProperty(c => c.CurrentPathId, pathId)
                , cancellationToken);
            return affectedRows > 0;
        }
    }
}

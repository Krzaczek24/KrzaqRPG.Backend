using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Characters;

public interface IDbCharacterAccess
{
    Task<bool> CharacterExistsAsync(int characterId, CancellationToken ct = default);
    Task<bool> IsCharacterNameUsedAsync(string characterName, CancellationToken ct = default);
    Task<bool> IsCharacterTravelingAsync(int characterId, CancellationToken ct = default);
    Task<bool> IsCharacterOnPathBeginningAsync(int characterId, int pathId, CancellationToken ct = default);
    Task<int> CreateCharacterAsync(string characterName, CancellationToken ct = default);
    Task<DbCharacter?> GetCharacterAsync(int characterId, CancellationToken ct = default);
    Task<IReadOnlySet<DbCharacter>> SearchCharactersAsync(ISearchCharacterParams searchParams, CancellationToken ct = default);
    Task<bool> SetCharacterCurrentAreaAsync(int characterId, int areaId, CancellationToken ct = default);
    Task<bool> SetCharacterCurrentPathAsync(int characterId, int pathId, CancellationToken ct = default);
}

internal class DbCharacterAccess(Db db) : IDbCharacterAccess
{
    public Task<bool> CharacterExistsAsync(
        int characterId,
        CancellationToken ct = default)
    {
        return db.Characters.AnyAsync(c => c.Id == characterId, ct);
    }

    public Task<bool> IsCharacterNameUsedAsync(
        string characterName,
        CancellationToken ct = default)
    {
        return db.Characters.AnyAsync(c => EF.Functions.Like(c.Name, characterName), ct);
    }

    public Task<bool> IsCharacterTravelingAsync(
        int characterId,
        CancellationToken ct = default)
    {
        return db.Characters.AnyAsync(c => c.Id == characterId && (c.CurrentTravel != null || c.CurrentTravel.Completed), ct);
    }

    public Task<bool> IsCharacterOnPathBeginningAsync(int characterId, int pathId, CancellationToken ct = default)
    {
        var characterSubquery = db.Characters.Where(c => c.Id == characterId);
        var pathsSubquery = db.Paths.Where(p => p.Id == pathId);
        return characterSubquery
            .Join(pathsSubquery, c => c.CurrentAreaId, p => p.StartAreaId, (c, p) => 1)
            .AnyAsync(ct);
    }

    public async Task<int> CreateCharacterAsync(
        string characterName,
        CancellationToken ct = default)
    {
        var character = new DbCharacter { Name = characterName };

        await db.Characters.AddAsync(character, ct);
        await db.SaveChangesAsync(ct);

        return character.Id;
    }

    public async Task<DbCharacter?> GetCharacterAsync(int characterId, CancellationToken ct = default)
    {
        var result = await db.Characters
            .Select(c => new DbCharacter
            {
                Id = c.Id,
                Name = c.Name,
                Level = c.Level,
                Experience = c.Experience,
                CurrentHealth = c.CurrentHealth,
                MaxHealth = c.MaxHealth,
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
                CurrentTravelId = c.CurrentTravelId,
            })
            .FirstOrDefaultAsync(c => c.Id == characterId, ct);

        return result;
    }

    public async Task<IReadOnlySet<DbCharacter>> SearchCharactersAsync(ISearchCharacterParams searchParams, CancellationToken ct = default)
    {
        var result = await db.Characters
            .Where(c => searchParams.Name == null || searchParams.Name.Length == 0 || EF.Functions.Like(c.Name, $"%{searchParams.Name}%"))
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
            .ToHashSetAsync(ct);

        return result.AsReadOnly();
    }

    public Task<bool> SetCharacterCurrentAreaAsync(int characterId, int areaId, CancellationToken ct = default)
    {
        return SetCurrentLocation(characterId, areaId, null, ct);
    }

    public Task<bool> SetCharacterCurrentPathAsync(int characterId, int pathId, CancellationToken ct = default)
    {
        return SetCurrentLocation(characterId, null, pathId, ct);
    }

    private async Task<bool> SetCurrentLocation(int characterId, int? areaId, int? pathId, CancellationToken ct = default)
    {
        int affectedRows = await db.Characters
            .Where(c => c.Id == characterId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(c => c.CurrentAreaId, areaId)
                .SetProperty(c => c.CurrentPathId, pathId)
            , ct);

        return affectedRows > 0;
    }
}

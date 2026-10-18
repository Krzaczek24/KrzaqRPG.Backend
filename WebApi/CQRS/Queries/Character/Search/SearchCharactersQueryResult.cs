namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.Search;

public class SearchCharactersQueryResult
{
    public required IReadOnlySet<SearchCharacterDto> Characters { get; init; }
}

public class SearchCharacterDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }

    public override string ToString() => $"({Id}) {Name}";
}

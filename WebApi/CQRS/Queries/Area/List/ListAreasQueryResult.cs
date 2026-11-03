namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.List;

public class ListAreasQueryResult()
{
    public required IReadOnlySet<AreaDto> Areas { get; init; }
}

public class AreaDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required Coordinates Coordinates { get; init; }

    public override string ToString() => $"({Id}) {Name}";
}

public record struct Coordinates(int X, int Y);
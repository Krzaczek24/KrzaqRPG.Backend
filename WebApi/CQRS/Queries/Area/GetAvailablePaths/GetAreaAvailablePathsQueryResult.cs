namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Area.GetAvailablePaths
{
    public class GetAreaAvailablePathsQueryResult
    {
        public required IReadOnlySet<AvailablePathDto> AvailablePaths { get; init; }
    }

    public class AvailablePathDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required AvailablePathTargetAreaDto TargetArea { get; init; }

        public override string ToString() => $"({Id}) {Name} -> {TargetArea}";
    }

    public class AvailablePathTargetAreaDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }

        public override string ToString() => $"({Id}) {Name}";
    }
}

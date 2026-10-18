using KrzaqRPG.Backend.Common.Enums;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails;

public class GetTravelDetailsQueryResult
{
    public required GetTravelDetailsCharacterDto Character { get; set; }
    public required IReadOnlyList<GetTravelDetailsPathDto> Paths { get; set; }
    public required bool IsCompleted { get; set; }
    public required DateTime StartTimestamp { get; set; }
    public required TimeSpan TotalTravelTime { get; set; }
}

public class GetTravelDetailsCharacterDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required GetTravelDetailsCharacterLocationDto? Location { get; set; }
}

public class GetTravelDetailsCharacterLocationDto
{
    public required CharacterLocationType Type { get; init; }
    public required int Id { get; init; }
    public required string Name { get; init; }

    public override string ToString() => $"[{Type}] ({Id}) {Name}";
}

public class GetTravelDetailsPathDto
{
    public required string Name { get; set; }
    public required decimal Distance { get; set; }
    public required int DangerLevel { get; set; }
    public required decimal DangerProbability { get; set; }
    public required TimeSpan PathTravelTime { get; set; }
    public required TimeSpan AreaTravelTime { get; set; }
}

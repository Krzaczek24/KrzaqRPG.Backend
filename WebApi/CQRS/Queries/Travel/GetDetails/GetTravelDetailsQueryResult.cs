using KrzaqRPG.Backend.Common.Enums;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails;

public class GetTravelDetailsQueryResult
{
    public required GetTravelDetailsCharacterDto Character { get; init; }
    public required IReadOnlyList<GetTravelDetailsPathDto> Paths { get; init; }
    public required bool IsCompleted { get; init; }
    public required DateTime StartTimestamp { get; init; }
    public required int TotalDistance { get; init; }
    public required TimeSpan TotalTravelTime { get; init; }
}

public class GetTravelDetailsCharacterDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required GetTravelDetailsCharacterLocationDto? Location { get; init; }
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
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int Distance { get; init; }
    public required int DangerLevel { get; init; }
    public required decimal DangerProbability { get; init; }
    public required TimeSpan TravelTime { get; init; }
    public required int TargetAreaId { get; init; }
    public required bool EventsGenerated { get; init; }
    public required IReadOnlyList<GetTravelDetailsPathEventDto> Events { get; init; }
}

public class GetTravelDetailsPathEventDto
{
    public required int Id { get; init; }
    public required EventType Type { get; init; }
    public required bool Handled { get; init; }
}

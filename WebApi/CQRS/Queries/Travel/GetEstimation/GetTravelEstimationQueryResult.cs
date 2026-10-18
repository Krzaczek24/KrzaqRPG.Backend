namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetEstimation;

public class GetTravelEstimationQueryResult
{
    public required IReadOnlyList<GetTravelEstimationPathDto> Paths { get; set; }
    public required TimeSpan TotalTravelTime { get; set; }
}

public class GetTravelEstimationPathDto
{
    public required string Name { get; set; }
    public required decimal Distance { get; set; }
    public required int DangerLevel { get; set; }
    public required decimal DangerProbability { get; set; }
    public required TimeSpan TravelTime { get; set; }
}

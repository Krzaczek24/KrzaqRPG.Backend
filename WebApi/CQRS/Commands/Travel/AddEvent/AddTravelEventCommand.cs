using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using System.Text.Json.Serialization;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Travel.AddEvent;

public record class AddTravelEventCommand : IRequest<AddTravelEventCommandResult>
{
    [JsonIgnore]
    public int TravelId { get; set; }
    [JsonIgnore]
    public DateTime Timestamp { get; set; }
    public EventType Type { get; set; }
    public TravelEventLocationType LocationType { get; set; }
    public int LocationId { get; set; }
}

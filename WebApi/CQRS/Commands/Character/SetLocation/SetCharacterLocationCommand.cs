using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Common.Enums;
using System.Text.Json.Serialization;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.SetLocation;

public record class SetCharacterLocationCommand : IRequest
{
    [JsonIgnore]
    public int CharacterId { get; init; }
    public int LocationId { get; init; }
    public CharacterLocationType LocationType { get; init; }
}

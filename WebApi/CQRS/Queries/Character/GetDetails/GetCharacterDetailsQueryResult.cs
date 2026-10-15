using KrzaqRPG.Backend.Common.Enums;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails
{
    public class GetCharacterDetailsQueryResult
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required int Level { get; init; }
        public required int Experience { get; init; }
        public required int CurrentHealth { get; init; }
        public required int MaxHealth { get; init; }
        public required CharacterLocationDto? Location {  get; init; }

        public override string ToString() => $"({Id}) {Name} [{Level}] {{{Location?.Name}}}";
    }

    public class CharacterLocationDto
    {
        public required CharacterLocationType Type { get; init; }
        public required int Id { get; init; }
        public required string Name { get; init; }

        public override string ToString() => $"[{Type}] ({Id}) {Name}";
    }
}

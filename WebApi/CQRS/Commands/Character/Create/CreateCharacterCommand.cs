using Krzaq.MediatR.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Commands.Character.Create
{
    public class CreateCharacterCommand : IRequest<CreateCharacterCommandResult>
    {
        public required string Name { get; init; }
    }
}

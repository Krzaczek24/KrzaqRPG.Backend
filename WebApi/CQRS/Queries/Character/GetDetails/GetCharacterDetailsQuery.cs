using Krzaq.MediatR.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.GetDetails
{
    public class GetCharacterDetailsQuery : IRequest<GetCharacterDetailsQueryResult>
    {
        public int CharacterId { get; set; }
    }
}

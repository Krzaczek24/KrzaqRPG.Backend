using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travel;
using KrzaqRPG.Backend.WebApi.Core.Errors;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetDetails
{
    public class GetTravelDetailsQueryHandler(IDbTravelAccess travelAccess) : IRequestHandler<GetTravelDetailsQuery>
    {
        public ValueTask Handle(GetTravelDetailsQuery request, CancellationToken cancellationToken = default)
        {
            var character = await travelAccess.GetTravelAsync(request.CharacterId, cancellationToken)
                ?? throw new NotFoundException(ErrorCode.CharacterNotFound);
        }
    }
}

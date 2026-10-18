using FluentValidation;
using Krzaq.MediatR.Implementations;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Extensions;
using KrzaqRPG.Backend.WebApi.Services;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Travel.GetEstimation;

public class GetTravelEstimationQueryValidator : RequestValidator<GetTravelEstimationQuery>
{
    public GetTravelEstimationQueryValidator(
        IDbCharacterAccess characterAccess,
        ITravelService travelService)
    {
        RuleFor(x => x.CharacterId)
            .NotEmpty()
            .MustAsync(async (id, cancellationToken) => await characterAccess.CharacterExistsAsync(id, cancellationToken))
                .WithErrorCode(ErrorCode.CharacterNotFound)
            .DependentRules(() => {
                RuleFor(x => x.PathIds)
                    .NotEmpty()
                    .MustAsync(async (query, ids, context, cancellationToken) =>
                    {
                        var invalidPathIds = await travelService.GetInvalidTravelPathsAsync(ids, cancellationToken);
                        if (invalidPathIds.Count == 0)
                            return true;

                        context.MessageFormatter.AppendArgument(ErrorParams.InvalidPathIds, string.Join(", ", invalidPathIds.Select(x => $"{x.Item1}->{x.Item2}")));
                        return false;
                    }).WithErrorCode(ErrorCode.InvalidTravelPaths);
            });
    }
}

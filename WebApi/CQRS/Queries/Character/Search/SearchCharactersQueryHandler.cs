using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.Search;

public class SearchCharactersQueryHandler(IDbCharacterAccess characterAccess)
    : IRequestHandler<SearchCharactersQuery, SearchCharactersQueryResult>
{
    public async ValueTask<SearchCharactersQueryResult> Handle(SearchCharactersQuery request, CancellationToken cancellationToken = default)
    {
        var areaCharacters = await characterAccess.SearchCharactersAsync(request, cancellationToken);
        return new()
        {
            Characters = areaCharacters.Select(character => new SearchCharacterDto
            {
                Id = character.Id,
                Name = character.Name,
            }).ToHashSet().AsReadOnly(),
        };
    }
}

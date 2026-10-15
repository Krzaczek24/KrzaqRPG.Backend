using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite.Interfaces;

namespace KrzaqRPG.Backend.WebApi.CQRS.Queries.Character.Search
{
    public class SearchCharactersQuery : IRequest<SearchCharactersQueryResult>, ISearchCharacterParams
    {
        public string? Name { get; set; }
        public ICollection<int> AreaIds { get; set; } = [];
        public ICollection<int> PathIds { get; set; } = [];
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
    }
}

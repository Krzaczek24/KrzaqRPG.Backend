namespace KrzaqRPG.Backend.Database.SQLite.Interfaces;

public interface ISearchCharacterParams
{
    string? Name { get; }
    ICollection<int> AreaIds { get; }
    ICollection<int> PathIds { get; }
    int? MinLevel { get; }
    int? MaxLevel { get; }
}

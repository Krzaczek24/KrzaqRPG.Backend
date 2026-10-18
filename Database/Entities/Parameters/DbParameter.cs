using KrzaqRPG.Backend.Database.SQLite.Core;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Parameters;

public class DbParameter : DbTable
{
    public virtual string Name { get; set; }
    public virtual string? String { get; set; }
    public virtual DateTime? DateTime { get; set; }
    public virtual TimeSpan? TimeSpan { get; set; }
    public virtual double? Numeric { get; set; }
    public virtual bool? Boolean { get; set; }
}

using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Areas;

public class DbArea : DbTable
{
    public virtual string Name { get; set; }
    public virtual AreaType Type { get; set; }
    public virtual ICollection<DbCharacter> Characters { get; set; } = [];
    public virtual ICollection<DbPath> OutgoingPaths { get; set; } = [];
}

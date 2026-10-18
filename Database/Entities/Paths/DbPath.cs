using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Paths;

public class DbPath : DbTable
{
    public virtual string Name { get; set; }
    public virtual int StartAreaId { get; set; }
    public virtual DbArea StartArea { get; set; }
    public virtual int EndAreaId { get; set; }
    public virtual DbArea EndArea { get; set; }
    public virtual int Distance { get; set; }
    public virtual int DangerLevel { get; set; }
    public virtual decimal DangerProbability { get; set; }
    public virtual PathType Type { get; set; }
    public virtual PathEnvironment Environment { get; set; }
    public virtual ICollection<DbCharacter> Characters { get; set; } = [];
}

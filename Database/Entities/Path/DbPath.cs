using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.Area;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.Database.SQLite.Enums;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Path
{
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
        public virtual DbPathType Type { get; set; }
        public virtual DbPathEnvironment Environment { get; set; }
        public virtual ICollection<DbCharacter> Characters { get; set; } = [];
    }
}

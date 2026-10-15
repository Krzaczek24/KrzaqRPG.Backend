using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.Area;
using KrzaqRPG.Backend.Database.SQLite.Entities.Path;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travel;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Character
{
    public class DbCharacter : DbTable
    {
        public virtual string Name { get; set; }
        public virtual int Level { get; set; }
        public virtual int Experience { get; set; }
        public virtual int CurrentHealth { get; set; }
        public virtual int MaxHealth { get; set; }
        public virtual int? CurrentAreaId { get; set; }
        public virtual int? CurrentPathId { get; set; }
        public virtual DbArea? CurrentArea { get; set; }
        public virtual DbPath? CurrentPath { get; set; }
        public virtual DbTravel? CurrentTravel { get; set; }
    }
}

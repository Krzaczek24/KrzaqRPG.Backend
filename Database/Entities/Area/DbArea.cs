using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.Database.SQLite.Entities.Path;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Area
{
    public class DbArea : DbTable
    {
        public virtual string Name { get; set; }
        public virtual ICollection<DbCharacter> Characters { get; set; } = [];
        public virtual ICollection<DbPath> OutgoingPaths { get; set; } = [];
    }
}

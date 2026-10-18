using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travels;

public class DbTravel : DbTable
{
    public virtual int CharacterId { get; set; }
    public virtual DbCharacter Character { get; set; }
    public virtual DateTime StartTimestamp { get; set; }
    public virtual bool Completed { get; set; }
    public virtual ICollection<DbTravelPath> Paths { get; set; } = [];
    public virtual ICollection<DbTravelEvent> Events { get; set; } = [];
}

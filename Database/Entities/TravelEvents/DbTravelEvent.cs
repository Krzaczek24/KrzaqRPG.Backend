using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;

public class DbTravelEvent : DbTable
{
    public virtual DateTime Timestamp { get; set; }
    public virtual EventType Type { get; set; }
    public virtual int TravelId { get; set; }
    public virtual DbTravel Travel { get; set; }
    public virtual int? TravelPathId { get; set; }
    public virtual DbTravelPath? TravelPath { get; set; }
    public virtual int? AreaId { get; set; }
    public virtual DbArea? Area { get; set; }
}

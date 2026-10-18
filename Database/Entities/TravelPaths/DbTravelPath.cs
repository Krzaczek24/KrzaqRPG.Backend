using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths
{
    public class DbTravelPath : DbTable
    {
        public virtual int TravelId { get; set; }
        public virtual DbTravel Travel { get; set; }
        public virtual int Sequence { get; set; }
        public virtual int PathId { get; set; }
        public virtual DbPath Path { get; set; }
        public virtual TimeSpan PathTravelTime { get; set; }
        public virtual TimeSpan AreaTravelTime { get; set; }
    }
}

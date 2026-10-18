using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Core;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.PathTravelTimeFactors.Type;

public class DbPathTypeTravelTimeFactor : DbTable
{
    public virtual PathType Name { get; set; }
    public virtual double Multiplier { get; set; }
}

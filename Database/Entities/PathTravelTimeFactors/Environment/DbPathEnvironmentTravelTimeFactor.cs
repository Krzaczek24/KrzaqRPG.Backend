using KrzaqRPG.Backend.Common.Enums;
using KrzaqRPG.Backend.Database.SQLite.Core;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.PathTravelTimeFactors.Environment;

public class DbPathEnvironmentTravelTimeFactor : DbTable
{
    public virtual PathEnvironment Name { get; set; }
    public virtual double Multiplier { get; set; }
}

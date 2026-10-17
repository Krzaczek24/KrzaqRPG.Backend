using KrzaqRPG.Backend.Database.SQLite.Base;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelTime.Base
{
    public abstract class DbTravelTimeFactorTable : DbTable
    {
        public virtual double Multiplier { get; set; }
    }

    public class DbTravelTimeFactorTable<TEnum> : DbTravelTimeFactorTable
        where TEnum : Enum
    {
        public virtual TEnum Name { get; set; }
    }
}

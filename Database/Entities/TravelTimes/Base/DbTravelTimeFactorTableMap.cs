using KrzaqRPG.Backend.Database.SQLite.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelTimes.Base
{
    internal class DbTravelTimeFactorTableMap<TTable> : DbTableMap<TTable>
        where TTable : DbTravelTimeFactorTable
    {
        public override void Configure(EntityTypeBuilder<TTable> builder)
        {
            base.Configure(builder);

            builder.Property("Name")
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.Multiplier)
                .HasColumnName("Multiplier")
                .IsRequired();
        }
    }
}

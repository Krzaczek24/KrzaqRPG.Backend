using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.PathTravelTimeFactors.Type;

internal class DbPathTypeTravelTimeFactorMap : DbTableMap<DbPathTypeTravelTimeFactor>
{
    public override void Configure(EntityTypeBuilder<DbPathTypeTravelTimeFactor> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasBooleanConversion()
            .IsRequired();

        builder.Property(x => x.Multiplier)
            .HasColumnName("Multiplier")
            .IsRequired();
    }
}

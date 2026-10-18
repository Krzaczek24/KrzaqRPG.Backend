using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.PathTravelTimeFactors.Environment;

internal class DbPathEnvironmentTravelTimeFactorMap : DbTableMap<DbPathEnvironmentTravelTimeFactor>
{
    public override void Configure(EntityTypeBuilder<DbPathEnvironmentTravelTimeFactor> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasEnumConversion()
            .IsRequired();

        builder.Property(x => x.Multiplier)
            .HasColumnName("Multiplier")
            .IsRequired();
    }
}
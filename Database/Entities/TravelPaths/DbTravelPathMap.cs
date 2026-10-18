using KrzaqRPG.Backend.Database.SQLite.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths;

internal class DbTravelPathMap : DbTableMap<DbTravelPath>
{
    public override void Configure(EntityTypeBuilder<DbTravelPath> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TravelId)
            .HasColumnName("TravelId")
            .IsRequired();

        builder.HasOne(x => x.Travel)
            .WithMany(x => x.TravelPaths)
            .HasForeignKey(x => x.TravelId);

        builder.Property(x => x.Sequence)
            .HasColumnName("Sequence")
            .IsRequired();

        builder.Property(x => x.PathId)
            .HasColumnName("PathId")
            .IsRequired();

        builder.HasOne(x => x.Path)
            .WithMany()
            .HasForeignKey(x => x.PathId);

        builder.Property(x => x.TravelTime)
            .HasColumnName("PathTravelTime")
            .IsRequired();

        builder.Property(x => x.AreaTravelTime)
            .HasColumnName("AreaTravelTime")
            .IsRequired();
    }
}

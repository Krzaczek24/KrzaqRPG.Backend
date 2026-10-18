using KrzaqRPG.Backend.Database.SQLite.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelPaths;

internal class DbTravelPathMap : DbTableMap<DbTravelPath>
{
    public override void Configure(EntityTypeBuilder<DbTravelPath> builder)
    {
        base.Configure(builder);

        builder.Property(tp => tp.TravelId)
            .HasColumnName("TravelId")
            .IsRequired();

        builder.HasOne(tp => tp.Travel)
            .WithMany(t => t.Paths)
            .HasForeignKey(tp => tp.TravelId);

        builder.Property(tp => tp.Sequence)
            .HasColumnName("Sequence")
            .IsRequired();

        builder.Property(tp => tp.PathId)
            .HasColumnName("PathId")
            .IsRequired();

        builder.HasOne(tp => tp.Path)
            .WithMany()
            .HasForeignKey(tp => tp.PathId);

        builder.Property(tp => tp.TravelTime)
            .HasColumnName("TravelTime")
            .IsRequired();
    }
}

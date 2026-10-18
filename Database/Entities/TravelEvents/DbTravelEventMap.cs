using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;

internal class DbTravelEventMap : DbTableMap<DbTravelEvent>
{
    public override void Configure(EntityTypeBuilder<DbTravelEvent> builder)
    {
        base.Configure(builder);

        builder.Property(ce => ce.Timestamp)
            .HasColumnName("Timestamp")
            .IsRequired();

        builder.Property(ce => ce.Type)
            .HasColumnName("Type")
            .HasEnumConversion()
            .IsRequired();

        builder.Property(ce => ce.Timestamp)
            .HasColumnName("Timestamp")
            .IsRequired();

        builder.Property(ce => ce.TravelId)
            .HasColumnName("TravelId")
            .IsRequired();

        builder.HasOne(ce => ce.Travel)
            .WithMany(t => t.Events)
            .HasForeignKey(ce => ce.TravelId)
            .IsRequired();

        builder.Property(ce => ce.TravelPathId)
            .HasColumnName("TravelPathId");

        builder.HasOne(ce => ce.TravelPath)
            .WithMany()
            .HasForeignKey(ce => ce.TravelPathId);

        builder.Property(ce => ce.AreaId)
            .HasColumnName("AreaId");

        builder.HasOne(ce => ce.Area)
            .WithMany()
            .HasForeignKey(ce => ce.AreaId);
    }
}

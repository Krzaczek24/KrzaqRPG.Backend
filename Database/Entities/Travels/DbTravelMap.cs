using KrzaqRPG.Backend.Database.SQLite.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travels;

internal class DbTravelMap : DbTableMap<DbTravel>
{
    public override void Configure(EntityTypeBuilder<DbTravel> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.CharacterId)
            .HasColumnName("CharacterId")
            .IsRequired();

        builder.HasOne(t => t.Character)
            .WithMany()
            .HasForeignKey(t => t.CharacterId)
            .IsRequired();

        builder.Property(t => t.StartTimestamp)
            .HasColumnName("StartTimestamp")
            .IsRequired();

        builder.Property(t => t.Completed)
            .HasColumnName("Completed");

        builder.HasMany(t => t.Paths)
            .WithOne(tp => tp.Travel)
            .HasForeignKey(tp => tp.TravelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Events)
            .WithOne(e => e.Travel)
            .HasForeignKey(tp => tp.TravelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

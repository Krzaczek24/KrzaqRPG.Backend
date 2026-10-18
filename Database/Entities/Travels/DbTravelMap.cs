using KrzaqRPG.Backend.Database.SQLite.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Travels;

internal class DbTravelMap : DbTableMap<DbTravel>
{
    public override void Configure(EntityTypeBuilder<DbTravel> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.CharacterId)
            .HasColumnName("CharacterId")
            .IsRequired();

        builder.HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.CharacterId)
            .IsRequired();

        builder.Property(x => x.StartTimestamp)
            .HasColumnName("StartTimestamp")
            .IsRequired();

        builder.Property(x => x.Completed)
            .HasColumnName("Completed");

        builder.HasMany(x => x.TravelPaths)
            .WithOne(tp => tp.Travel)
            .HasForeignKey(tp => tp.TravelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

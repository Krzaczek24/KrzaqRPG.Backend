using KrzaqRPG.Backend.Database.SQLite.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Characters;

internal class DbCharacterMap : DbTableMap<DbCharacter>
{
    public override void Configure(EntityTypeBuilder<DbCharacter> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(c => c.CurrentAreaId)
            .HasColumnName("CurrentAreaId");

        builder.HasOne(c => c.CurrentArea)
            .WithMany(a => a.Characters)
            .HasForeignKey(a => a.CurrentAreaId);

        builder.Property(c => c.CurrentPathId)
            .HasColumnName("CurrentPathId");

        builder.HasOne(c => c.CurrentPath)
            .WithMany(p => p.Characters)
            .HasForeignKey(c => c.CurrentPathId);

        builder.Property(c => c.CurrentTravelId)
            .HasColumnName("CurrentTravelId");

        builder.HasOne(c => c.CurrentTravel)
            .WithMany()
            .HasForeignKey(c => c.CurrentTravelId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

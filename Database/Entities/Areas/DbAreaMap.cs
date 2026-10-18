using KrzaqRPG.Backend.Database.SQLite.Core;
using KrzaqRPG.Backend.Database.SQLite.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Areas;

internal class DbAreaMap : DbTableMap<DbArea>
{
    public override void Configure(EntityTypeBuilder<DbArea> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("Type")
            .HasEnumConversion()
            .IsRequired();

        builder.HasMany(x => x.Characters)
            .WithOne(c => c.CurrentArea)
            .HasForeignKey(c => c.CurrentAreaId);

        builder.HasMany(x => x.OutgoingPaths)
            .WithOne(p => p.StartArea)
            .HasForeignKey(p => p.StartAreaId);
    }
}

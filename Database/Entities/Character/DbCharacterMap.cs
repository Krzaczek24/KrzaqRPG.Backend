using KrzaqRPG.Backend.Database.SQLite.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.Character
{
    internal class DbCharacterMap : DbTableMap<DbCharacter>
    {
        public override void Configure(EntityTypeBuilder<DbCharacter> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.CurrentAreaId)
                .HasColumnName("CurrentAreaId");

            builder.Property(x => x.CurrentPathId)
                .HasColumnName("CurrentPathId");

            builder.HasOne(x => x.CurrentArea)
                .WithMany(a => a.Characters)
                .HasForeignKey(a => a.CurrentAreaId);

            builder.HasOne(x => x.CurrentPath)
                .WithMany(p => p.Characters)
                .HasForeignKey(p => p.CurrentPathId);

            builder.HasOne(x => x.CurrentTravel)
                .WithOne(t => t.Character);
        }
    }
}

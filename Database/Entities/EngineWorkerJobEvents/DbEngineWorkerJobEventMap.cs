using KrzaqRPG.Backend.Database.SQLite.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents
{
    internal class DbEngineWorkerJobEventMap : DbTableMap<DbEngineWorkerJobEvent>
    {
        public override void Configure(EntityTypeBuilder<DbEngineWorkerJobEvent> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.JobId)
                .HasColumnName("JobId")
                .IsRequired();

            builder.HasOne(x => x.Job)
                .WithMany(x => x.Events)
                .HasForeignKey(a => a.JobId);

            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired();

            builder.Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            builder.Property(x => x.Message)
                .HasColumnName("Message")
                .IsRequired();
        }
    }
}

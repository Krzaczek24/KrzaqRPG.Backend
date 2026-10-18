using KrzaqRPG.Backend.Database.SQLite.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs
{
    internal class DbEngineWorkerJobMap : DbTableMap<DbEngineWorkerJob>
    {
        public override void Configure(EntityTypeBuilder<DbEngineWorkerJob> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired();

            builder.Property(x => x.Active)
                .HasColumnName("Active")
                .IsRequired();

            builder.Property(x => x.LastRunTimestamp)
                .HasColumnName("LastRunTimestamp");

            builder.Property(x => x.LastSuccessfullRunTimestamp)
                .HasColumnName("LastSuccessfullRunTimestamp");

            builder.Property(x => x.NextRunTimestamp)
                .HasColumnName("NextRunTimestamp");

            builder.Property(x => x.ErrorCounter)
                .HasColumnName("ErrorCounter")
                .IsRequired();

            builder.Property(x => x.CronExpression)
                .HasColumnName("CronExpression");

            builder.Property(x => x.IdleInterval)
                .HasColumnName("IdleInterval");

            builder.Property(x => x.BatchSize)
                .HasColumnName("BatchSize")
                .IsRequired();

            builder.HasMany(x => x.Events)
                .WithOne(x => x.Job)
                .HasForeignKey(c => c.JobId);
        }
    }
}

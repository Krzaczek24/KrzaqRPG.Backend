using KrzaqRPG.Backend.Database.SQLite.Entities.Area;
using KrzaqRPG.Backend.Database.SQLite.Entities.Character;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJob;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvent;
using KrzaqRPG.Backend.Database.SQLite.Entities.Path;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travel;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite
{
    public class Db(DbContextOptions<Db> options) : DbContext(options)
    {
        internal virtual DbSet<DbArea> Areas { get; set; }
        internal virtual DbSet<DbCharacter> Characters { get; set; }
        internal virtual DbSet<DbPath> Paths { get; set; }
        internal virtual DbSet<DbTravel> Travels { get; set; }
        internal virtual DbSet<DbEngineWorkerJob> Jobs { get; set; }
        internal virtual DbSet<DbEngineWorkerJobEvent> JobEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure default SQLite file when no other options were configured by the caller.
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=E:\KrzaqRPG\KrzaqRPG.SQLite.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}

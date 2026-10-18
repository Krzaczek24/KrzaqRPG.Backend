using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;
using KrzaqRPG.Backend.Database.SQLite.Entities.Parameters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelTimes;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite
{
    public class Db(DbContextOptions<Db> options) : DbContext(options)
    {
        internal virtual DbSet<DbArea> Areas { get; set; }
        internal virtual DbSet<DbCharacter> Characters { get; set; }
        internal virtual DbSet<DbEngineWorkerJob> Jobs { get; set; }
        internal virtual DbSet<DbEngineWorkerJobEvent> JobEvents { get; set; }
        internal virtual DbSet<DbParameter> Parameters { get; set; }
        internal virtual DbSet<DbPath> Paths { get; set; }
        internal virtual DbSet<DbTravel> Travels { get; set; }
        internal virtual DbSet<DbTravelTimeFactorAreaType> TravelTimeFactorAreaTypes { get; set; }
        internal virtual DbSet<DbTravelTimeFactorPathEnvironment> TravelTimeFactorPathEnvironments { get; set; }
        internal virtual DbSet<DbTravelTimeFactorPathType> TravelTimeFactorPathTypes { get; set; }

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

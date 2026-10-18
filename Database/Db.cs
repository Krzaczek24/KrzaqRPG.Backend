using KrzaqRPG.Backend.Database.SQLite.Entities.Areas;
using KrzaqRPG.Backend.Database.SQLite.Entities.Characters;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobEvents;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;
using KrzaqRPG.Backend.Database.SQLite.Entities.Parameters;
using KrzaqRPG.Backend.Database.SQLite.Entities.Paths;
using KrzaqRPG.Backend.Database.SQLite.Entities.PathTravelTimeFactors.Environment;
using KrzaqRPG.Backend.Database.SQLite.Entities.PathTravelTimeFactors.Type;
using KrzaqRPG.Backend.Database.SQLite.Entities.TravelEvents;
using KrzaqRPG.Backend.Database.SQLite.Entities.Travels;
using Microsoft.EntityFrameworkCore;

namespace KrzaqRPG.Backend.Database.SQLite;

public class Db(DbContextOptions<Db> options) : DbContext(options)
{
    internal virtual DbSet<DbArea> Areas { get; set; }
    internal virtual DbSet<DbCharacter> Characters { get; set; }
    internal virtual DbSet<DbTravelEvent> TravelEvents { get; set; }
    internal virtual DbSet<DbEngineWorkerJob> Jobs { get; set; }
    internal virtual DbSet<DbEngineWorkerJobEvent> JobEvents { get; set; }
    internal virtual DbSet<DbParameter> Parameters { get; set; }
    internal virtual DbSet<DbPath> Paths { get; set; }
    internal virtual DbSet<DbTravel> Travels { get; set; }
    internal virtual DbSet<DbPathEnvironmentTravelTimeFactor> PathEnvironmentTravelTimeFactors { get; set; }
    internal virtual DbSet<DbPathTypeTravelTimeFactor> PathTypeTravelTimeFactors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure default SQLite file when no other options were configured by the caller.
        if (!optionsBuilder.IsConfigured)
        {
            var baseDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent;
            var dbFile = baseDir.EnumerateFiles("*.SQLite.db", SearchOption.TopDirectoryOnly).Single();
            optionsBuilder.UseSqlite(@$"Data Source={dbFile.FullName}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}

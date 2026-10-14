using NCrontab;
using NLog.Extensions.Logging;
using RpgAdventureGame.Backend.Common;
using RpgAdventureGame.Backend.Database.SQLite;
using RpgAdventureGame.Backend.Engine.Core.Waiter;
using RpgAdventureGame.Backend.Engine.Core.Worker;
using RpgAdventureGame.Backend.Engine.Jobs;

namespace RpgAdventureGame.Backend.Engine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Logging.AddNLog();

            builder.Services.AddMemoryCache();

            builder.Services.AddAppDatabase(EnvInfo.IsDebug ? new LoggerFactory([new NLogLoggerProvider()]) : null);

            builder.Services.AddTransient(sp => new IntervalWaiter(TimeSpan.FromSeconds(1)));
            builder.Services.AddTransient(sp => new ScheduleWaiter(CrontabSchedule.Parse("* * * * *")));

            builder.Services.AddHostedService<ContinuousWorker<TravelProcessingJob>>();
            builder.Services.AddHostedService<ScheduleWorker<MapEventSettingJob>>();

            var app = builder.Build();

            app.Run();
        }
    }
}

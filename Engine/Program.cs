using KrzaqRPG.Backend.Common;
using KrzaqRPG.Backend.Database.SQLite;
using KrzaqRPG.Backend.Engine.Core.Waiter;
using KrzaqRPG.Backend.Engine.Core.Worker;
using KrzaqRPG.Backend.Engine.Jobs;
using NCrontab;
using NLog.Extensions.Logging;

namespace KrzaqRPG.Backend.Engine
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

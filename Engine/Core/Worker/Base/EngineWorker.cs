using KrzaqRPG.Backend.Common;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;
using KrzaqRPG.Backend.Engine.Core.Job;
using KrzaqRPG.Backend.Engine.Core.Waiter.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace KrzaqRPG.Backend.Engine.Core.Worker.Base;

internal abstract class EngineWorker<TJob, TWaiter>(
    ILogger<TJob> logger,
    IMemoryCache cache,
    IServiceScopeFactory scopeFactory,
    TWaiter waiter) : BackgroundService
    where TJob : IJob
    where TWaiter : IWaiter
{
    private static object LockObj { get; } = new();
    private static bool IsAlreadyAssigned { get; set; }
    protected static string JobName { get; } = typeof(TJob).Name;
    protected static string WaiterTypeName { get; } = typeof(TWaiter).Name;

    protected ILogger<TJob> Logger { get; } = logger;
    protected IMemoryCache Cache { get; } = cache;

    private static DateTime ConfigExpiration { get; set; }
    private static string CacheKey { get; } = $"{JobName}_JOB_CONFIG";

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (IsAlreadyRunning())
        {
            Logger.LogWarning("'{JobName}' is already being performed by another worker", JobName);
            return;
        }

        Logger.LogInformation("Worker is preparing to start '{jobName}' ...", JobName);

        try
        {
            var config = await LoadConfigAsync(stoppingToken) ?? throw new InvalidOperationException($"'{JobName}' configuration has been not found, worker abandons job");
            UpdateWaiterTick(waiter, config);
            await UpdateJobParamsAsync(config);
            LogNextExecution();

            await waiter.AwaitAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                config = await LoadConfigAsync(stoppingToken);
                UpdateWaiterTick(waiter, config);

                if (config.Active)
                {
                    try
                    {
                        await ExecuteJobAsync(stoppingToken);
                        await UpdateJobParamsAsync(config, wasSuccessful: true);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error occured while executing '{JobName}'", JobName);
                        await UpdateJobParamsAsync(config, wasSuccessful: false);
                    }
                }
                else
                {
                    Logger.LogInformation("'{JobName}' is disabled, skipping execution, worker goes back to sleep", JobName);
                }

                LogNextExecution();
                await waiter.AwaitAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            Logger.LogError(ex, "Worker performing '{JobName}' has been stopped", JobName);
        }
    }

    protected abstract void UpdateWaiterTick(TWaiter waiter, DbEngineWorkerJob config);

    protected virtual JobContext CreateContext() => new()
    {
        JobName = JobName,
        InstanceId = Guid.NewGuid().ToString(),
    };

    protected void LogNextExecution()
    {
        Logger.LogInformation("Worker's next '{JobName}' execution scheduled for: {Info})", JobName, waiter.Info);
    }

    private async Task ExecuteJobAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Worker is starting '{JobName}' ...", JobName);

        var context = CreateContext();
        do
        {
            context.Iteration++;

            await using var scope = scopeFactory.CreateAsyncScope();
            var job = ActivatorUtilities.CreateInstance<TJob>(scope.ServiceProvider);
            await job.ExecuteAsync(context, stoppingToken);
        }
        while (context.DoNextJob);

        Logger.LogInformation("Worker finished '{JobName}'", JobName);
    }

    protected async Task UpdateJobParamsAsync(DbEngineWorkerJob config, bool? wasSuccessful = null)
    {
        config.NextRunTimestamp = waiter.NextTick;

        if (wasSuccessful.HasValue)
        {
            config.LastRunTimestamp = DateTime.Now;

            if (wasSuccessful.Value)
                config.LastSuccessfullRunTimestamp = config.LastRunTimestamp;
            else
                config.ErrorCounter++;
        }

        using (var scope = scopeFactory.CreateAsyncScope())
        {
            var access = scope.ServiceProvider.GetRequiredService<IDbEngineWorkerJobAccess>();
            await access.UpdateJobParamsAsync(config);
        }

        Cache.Set(CacheKey, config, ConfigExpiration);
    }

    protected async Task<DbEngineWorkerJob> LoadConfigAsync(CancellationToken stoppingToken)
    {
        var job = await Cache.GetOrCreateAsync(CacheKey, opts =>
        {
            opts.AbsoluteExpiration = ConfigExpiration = DateTime.Now.Add(EnvInfo.IsDebug ? TimeSpan.FromSeconds(10) : TimeSpan.FromMinutes(10));

            Logger.LogInformation("Loading '{JobName}' configuration from database ...", JobName);

            using var scope = scopeFactory.CreateAsyncScope();
            var access = scope.ServiceProvider.GetRequiredService<IDbEngineWorkerJobAccess>();
            return access.GetJobAsync(JobName, stoppingToken)!;
        });

        Logger.LogInformation("'{JobName}' configuration loaded", JobName);
        return job!;
    }

    private static bool IsAlreadyRunning()
    {
        if (IsAlreadyAssigned)
            return true;

        lock (LockObj)
        {
            if (IsAlreadyAssigned)
                return true;

            IsAlreadyAssigned = true;
        }

        return false;
    }
}

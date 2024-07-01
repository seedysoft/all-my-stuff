using Microsoft.Extensions.Hosting;

namespace Seedysoft.Libs.CronBackgroundService;

public abstract class CronBackgroundService(ScheduleConfig config, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected ScheduleConfig Config { get; init; } = config;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!await WaitForAppStartup(hostApplicationLifetime, cancellationToken))
            return;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                DateTimeOffset? NextExecutionAt = Config.GetNextOccurrence(DateTimeOffset.Now);
                if (NextExecutionAt.HasValue)
                {
                    TimeSpan DelayUntilNext = NextExecutionAt.Value.Subtract(DateTimeOffset.Now);
                    if (DelayUntilNext.TotalMilliseconds > 0d)   // prevent non-positive values from being passed into Timer
                    {
                        await Task.Delay(DelayUntilNext, cancellationToken);

                        if (!cancellationToken.IsCancellationRequested)
                            await DoWorkAsync(cancellationToken);
                    }
                }

                await Task.Delay(Config.DelayBetweenExecutionsTimeSpan, cancellationToken);
            }
        }
        catch (TaskCanceledException) { }
        finally { await Task.CompletedTask; }
    }

    private static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken cancellationToken)
    {
        TaskCompletionSource startedSource = new();
        TaskCompletionSource cancelledSource = new();

        using CancellationTokenRegistration reg1 = lifetime.ApplicationStarted.Register(startedSource.SetResult);
        using CancellationTokenRegistration reg2 = cancellationToken.Register(cancelledSource.SetResult);

        Task completedTask = await Task
            .WhenAny(startedSource.Task, cancelledSource.Task)
            .ConfigureAwait(false);

        // If the completed tasks was the "app started" task, return true, otherwise false
        return completedTask == startedSource.Task;
    }

    public abstract Task DoWorkAsync(CancellationToken cancellationToken);
}

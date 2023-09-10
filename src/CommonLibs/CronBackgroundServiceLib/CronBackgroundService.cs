using Microsoft.Extensions.Hosting;

namespace Seedysoft.CronBackgroundServiceLib;

public abstract class CronBackgroundService : BackgroundService
{
    protected ScheduleConfig Config { get; init; }
    protected Cronos.CronExpression Expression { get; init; }

    protected CronBackgroundService(ScheduleConfig config)
    {
        Config = config;
        Expression = Cronos.CronExpression.Parse(Config.CronExpression);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            DateTimeOffset? NextExecutionAt = Expression.GetNextOccurrence(DateTimeOffset.Now, Config.TimeZoneInfo);
            if (NextExecutionAt.HasValue)
            {
                TimeSpan DelayUntilNext = NextExecutionAt.Value.Subtract(DateTimeOffset.Now);
                if (DelayUntilNext.TotalMilliseconds > 0)   // prevent non-positive values from being passed into Timer
                {
                    await Task.Delay(DelayUntilNext, cancellationToken);

                    if (!cancellationToken.IsCancellationRequested)
                        await DoWorkAsync(cancellationToken);
                }
            }

            await Task.Delay(Config.DelayBetweenExecutionsTimeSpan, cancellationToken);
        }

        await Task.CompletedTask;
    }

    protected virtual async Task DoWorkAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
}

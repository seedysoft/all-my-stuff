namespace Seedysoft.Libs.CronBackgroundService;

public abstract class CronBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
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
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                DateTimeOffset? NextExecutionAt = Expression.GetNextOccurrence(DateTimeOffset.Now, Config.TimeZoneInfo);
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

    public abstract Task DoWorkAsync(CancellationToken cancellationToken);
}

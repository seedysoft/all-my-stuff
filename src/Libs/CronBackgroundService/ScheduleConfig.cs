namespace Seedysoft.Libs.CronBackgroundService;

public record ScheduleConfig
{
    public required string CronExpression { get; init; }

    public TimeZoneInfo TimeZoneInfo { get; init; } = TimeZoneInfo.Local;

    public TimeSpan DelayBetweenExecutionsTimeSpan { get; init; } = TimeSpan.FromSeconds(0.1);

    public DateTimeOffset? GetNextOccurrence(DateTimeOffset from) =>
        Cronos.CronExpression.Parse(CronExpression).GetNextOccurrence(from, TimeZoneInfo);
}

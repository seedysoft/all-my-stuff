namespace Seedysoft.CronBackgroundServiceLib;

public record ScheduleConfig
{
    public string CronExpression { get; init; } = string.Empty;

    public TimeZoneInfo TimeZoneInfo { get; init; } = TimeZoneInfo.Local;

    public TimeSpan DelayBetweenExecutionsTimeSpan { get; init; } = TimeSpan.FromSeconds(0.1);
}

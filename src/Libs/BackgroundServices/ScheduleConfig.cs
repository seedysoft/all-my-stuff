namespace Seedysoft.Libs.BackgroundServices;

public record class ScheduleConfig
{
    public required string CronExpression { get; init; }

    public TimeZoneInfo TimeZoneInfo { get; init; } = TimeZoneInfo.Local;

    public TimeSpan DelayBetweenExecutionsTimeSpan { get; init; } = TimeSpan.FromSeconds(0.1);

    public DateTimeOffset? GetNextOccurrence(DateTimeOffset from)
    {
        return string.IsNullOrEmpty(CronExpression) || !Cronos.CronExpression.TryParse(CronExpression, out Cronos.CronExpression? cronExpression)
            ? null
            : cronExpression.GetNextOccurrence(from, TimeZoneInfo);
    }
}

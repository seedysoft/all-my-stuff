namespace Seedysoft.Libs.Rpt.Settings;

public record RptSettings : BackgroundServices.ScheduleConfig
{
    public required string Url { get; init; }
}

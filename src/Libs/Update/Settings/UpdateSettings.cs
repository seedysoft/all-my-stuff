namespace Seedysoft.Libs.Update.Settings;

public record UpdateSettings : BackgroundServices.ScheduleConfig
{
    public required string ProcessStartInfoFileName { get; init; }
    public required string ProcessStartInfoArguments { get; init; }
}

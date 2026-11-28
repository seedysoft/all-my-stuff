namespace Seedysoft.Photo.Lib.Settings;

public record PhotoSettings : Libs.BackgroundServices.ScheduleConfig
{
    public required string PhotoRootPath { get; init; }
}

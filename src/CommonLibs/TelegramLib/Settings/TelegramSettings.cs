namespace Seedysoft.TelegramLib.Settings;

public record TelegramSettings : CronBackgroundServiceLib.ScheduleConfig
{
    public Dictionary<string, string> Tokens { get; init; } = default!;
}

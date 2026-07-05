namespace Seedysoft.BlazorWebApp.Server.Settings;

public record class BlazorWebAppServerSettings
{
    public bool UseOutbox { get; init; }
    public bool UsePvpc { get; init; }
    public bool UseTelegram { get; init; }
    public bool UseTuyaManager { get; init; }
    public bool UseUpdater { get; init; }
    public bool UseWebComparer { get; init; }
}

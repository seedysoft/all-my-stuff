namespace Seedysoft.Carburantes.Core.Settings;

public record SettingsRoot
{
    public GoogleMapsPlatform GoogleMapsPlatform { get; init; } = default!;

    public Minetur Minetur { get; init; } = default!;

    public ObtainDataSettings ObtainDataSettings { get; init; } = default!;
}

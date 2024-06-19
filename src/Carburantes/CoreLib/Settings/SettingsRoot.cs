namespace Seedysoft.Carburantes.CoreLib.Settings;

public record SettingsRoot
{
    public const int FirstObtainedDate = 230712;

    public required GoogleMapsPlatform GoogleMapsPlatform { get; init; }

    public required Minetur Minetur { get; init; }

    public required ObtainDataSettings ObtainDataSettings { get; init; }
}

namespace Seedysoft.Carburantes.Core.Settings;

//              TODO            Change class to record and set to init
public sealed class SettingsRoot
{
    public GoogleMapsPlatform GoogleMapsPlatform { get; set; } = default!;

    public Minetur Minetur { get; set; } = default!;

    public ObtainDataSettings ObtainDataSettings { get; set; } = default!;
}
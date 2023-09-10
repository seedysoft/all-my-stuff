namespace Seedysoft.ObtainPvpcLib.Settings;

public record ObtainPvpcSettings : CronBackgroundServiceLib.ScheduleConfig
{
    /// <summary>
    /// Uri desde la que se obtendrán los datos. Tiene un elemento de formato para establecer la fecha que buscaremos.
    /// </summary>
    public string DataUrlTemplate { get; init; } = default!;

    /// <summary>
    /// Identificador del objeto <see cref="Included"/> que contiene los datos del Pvpc.
    /// </summary>
    public string PvpcId { get; init; } = default!;
}

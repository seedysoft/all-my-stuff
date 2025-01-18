namespace Seedysoft.Pvpc.Lib.Settings;

public record class PvpcSettings : Libs.BackgroundServices.ScheduleConfig
{
    /// <summary>
    /// Url desde la que se obtendrán los datos. Tiene un elemento de formato para establecer la fecha que buscaremos.
    /// </summary>
    public required string DataUrlTemplate { get; init; }

    /// <summary>
    /// Identificador del objeto <see cref="Included"/> que contiene los datos del Pvpc.
    /// </summary>
    public required string PvpcId { get; init; }
}

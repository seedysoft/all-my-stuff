namespace Seedysoft.Pvpc.Lib.Settings;

public record class TuyaManagerSettings : Libs.BackgroundServices.ScheduleConfig
{
    /// <summary>
    /// Precio por debajo del cual se activarán los dispositivos.
    /// </summary>
    public required decimal AllowChargeWhenKWhPriceInEurosIsBelowThan { get; init; }

    /// <summary>
    /// Cantidad de horas de un día en las que se activarán los dispositivos.
    /// </summary>
    public required int ChargingHoursPerDay { get; init; }
}

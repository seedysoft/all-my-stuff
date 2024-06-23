namespace Seedysoft.Pvpc.Lib.Settings;

public record TuyaManagerSettings : Libs.CronBackgroundService.ScheduleConfig
{
    /// <summary>
    /// Precio por debajo del cual se activarán los dispositivos.
    /// </summary>
    public decimal AllowChargeWhenKWhPriceInEurosIsBelowThan { get; init; }

    /// <summary>
    /// Cantidad de horas de un día en las que se activarán los dispositivos.
    /// </summary>
    public int ChargingHoursPerDay { get; init; }
}

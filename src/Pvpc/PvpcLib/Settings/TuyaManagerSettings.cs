namespace Seedysoft.PvpcLib.Settings;

public record TuyaManagerSettings : CronBackgroundServiceLib.ScheduleConfig
{
    /// <summary>
    /// Precio por debajo del cual se activarán los dispositivos.
    /// </summary>
    public decimal AllowChargeWhenKWhPriceInEurosIsBelowThan { get; init; } = default!;

    /// <summary>
    /// Cantidad de horas de un día en las que se activarán los dispositivos.
    /// </summary>
    public int ChargingHoursPerDay { get; init; } = default!;
}

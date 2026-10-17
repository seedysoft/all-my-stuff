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

    public static TuyaManagerSettings GetDefault(
        decimal? allowChargeWhenKWhPriceInEurosIsBelowThan = null,
        int? chargingHoursPerDay = null,
        string? cronExpression = null,
        TimeSpan? delayBetweenExecutionsTimeSpan = null,
        TimeZoneInfo? timeZoneInfo = null
        )
    {
        return new TuyaManagerSettings()
        {
            AllowChargeWhenKWhPriceInEurosIsBelowThan = allowChargeWhenKWhPriceInEurosIsBelowThan ?? decimal.Zero,
            ChargingHoursPerDay = chargingHoursPerDay ?? 0,
            CronExpression = cronExpression ?? Cronos.CronExpression.Yearly.ToString(),
            DelayBetweenExecutionsTimeSpan = delayBetweenExecutionsTimeSpan ?? TimeSpan.MaxValue,
            TimeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local,
        };
    }
}

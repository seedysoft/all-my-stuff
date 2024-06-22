namespace Seedysoft.Libs.Core.Entities;

public abstract class PvpcBase
{
    public DateTimeOffset AtDateTimeOffset { get; set; }

    public DateTime UtcDateTime => AtDateTimeOffset.UtcDateTime;

    public decimal MWhPriceInEuros { get; set; }

    public decimal KWhPriceInEuros => decimal.Divide(MWhPriceInEuros, 1_000M);

    protected internal string GetDebuggerDisplay() => $"{UtcDateTime:yy.MM.dd HHzz} @ {KWhPriceInEuros:N5} €/kWh";
}

/// <summary>
/// Precio Voluntario para el Pequeño Consumidor
/// </summary>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Pvpc : PvpcBase
{
    public Pvpc(DateTimeOffset atDateTimeOffset, decimal mWhPriceInEuros)
    {
        AtDateTimeOffset = atDateTimeOffset;
        MWhPriceInEuros = mWhPriceInEuros;
    }
}

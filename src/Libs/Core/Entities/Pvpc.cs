namespace Seedysoft.Libs.Core.Entities;

public abstract class PvpcBase
{
    public DateTimeOffset AtDateTimeOffset { get; set; }

    public decimal MWhPriceInEuros { get; set; }

    public decimal KWhPriceInEuros => decimal.Divide(MWhPriceInEuros, 1_000M);

    protected internal string GetDebuggerDisplay => $"{AtDateTimeOffset:yy.MM.dd HHzz} @ {KWhPriceInEuros:N5} €/kWh";

    public override string ToString() => GetDebuggerDisplay;
}

/// <summary>
/// Precio Voluntario para el Pequeño Consumidor
/// </summary>
[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
public sealed class Pvpc : PvpcBase
{
    public Pvpc(DateTimeOffset atDateTimeOffset, decimal mWhPriceInEuros)
    {
        AtDateTimeOffset = atDateTimeOffset;
        MWhPriceInEuros = mWhPriceInEuros;
    }
}

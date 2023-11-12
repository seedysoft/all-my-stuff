namespace Seedysoft.CoreLib.Entities;

public abstract class PvpcBase
{
    public DateTimeOffset AtDateTimeOffset { get; set; }

    public DateTime FullDate => AtDateTimeOffset.DateTime;

    public decimal MWhPriceInEuros { get; set; }

    public decimal KWhPriceInEuros => decimal.Divide(MWhPriceInEuros, 1_000M);
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

    private string GetDebuggerDisplay() => $"{FullDate:yy.MM.dd HHzz} @ {KWhPriceInEuros:N5} €/kWh";
}

public sealed class PvpcView : PvpcBase
{
    public long AtDateTimeUnix { get; private set; }
}

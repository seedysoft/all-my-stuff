namespace Seedysoft.CoreLib.Entities;

/// <summary>
/// Precio Voluntario para el Pequeño Consumidor
/// </summary>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Pvpc
{
    protected Pvpc() { }
    public Pvpc(DateTimeOffset atDateTimeOffset, decimal mWhPriceInEuros)
    {
        AtDateTimeOffset = atDateTimeOffset;
        MWhPriceInEuros = mWhPriceInEuros;
    }

    public DateTimeOffset AtDateTimeOffset { get; set; }

    public DateTime FullDate => AtDateTimeOffset.DateTime;

    public decimal MWhPriceInEuros { get; set; }

    public decimal KWhPriceInEuros => decimal.Divide(MWhPriceInEuros, 1_000M);

    protected string GetDebuggerDisplay() => $"{FullDate:yy.MM.dd HHzz} @ {KWhPriceInEuros:N5} €/kWh";
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PvpcView : Pvpc
{
    public long AtDateTimeUnix { get; private set; }
}

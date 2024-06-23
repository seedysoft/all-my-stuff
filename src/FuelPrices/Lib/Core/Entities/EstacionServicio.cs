namespace Seedysoft.FuelPrices.Lib.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public sealed class EstacionServicio : Core.EntityBase
{
    public int IdEstacion { get; set; }

    public int IdMunicipio { get; set; } = default!;

    public string CodigoPostal { get; set; } = default!;

    public string Direccion { get; set; } = default!;

    public string Horario { get; set; } = default!;

    public string Latitud { get; set; } = default!;

    public string Localidad { get; set; } = default!;

    public string LongitudWgs84 { get; set; } = default!;

    public string Margen { get; set; } = default!;

    public string Rotulo { get; set; } = default!;

    public double LatNotNull => double.Parse(Latitud);
    public double LngNotNull => double.Parse(LongitudWgs84);

    private string GetDebuggerDisplay() => $"{Rotulo} @ {AtDate}";
}

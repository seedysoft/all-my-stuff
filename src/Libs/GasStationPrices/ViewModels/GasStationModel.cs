namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class GasStationModel
{
    [J("lat")] public double Lat { get; init; }
    [J("lng")] public double Lon { get; init; }

    // TODO                         Use Travel.Models.Location instead individual properties
    public Travel.Models.Location Location => new((decimal)Lat, (decimal)Lon);

    [J("localizacion")] public string? Localizacion { get; init; }

    [J("rotulo")] public required string Rotulo { get; init; }
    [J("rotulotrimed")] public string RotuloTrimed => Rotulo.Trim();

#pragma warning disable IDE1006 // Naming Styles
    public decimal? bie { get; init; }
    public decimal? bio { get; init; }
    public decimal? g95e10 { get; init; }
    public decimal? g95e5 { get; init; }
    public decimal? g95e5plus { get; init; }
    public decimal? g98e10 { get; init; }
    public decimal? g98e5 { get; init; }
    public decimal? glp { get; init; }
    public decimal? gnc { get; init; }
    public decimal? gnl { get; init; }
    public decimal? goa { get; init; }
    public decimal? goaplus { get; init; }
    public decimal? gob { get; init; }
    public decimal? h2 { get; init; }
#pragma warning restore IDE1006 // Naming Styles
}

namespace Seedysoft.Libs.GasStationPrices.Models.Minetur;

public record class Body
{
    private const string FechaFormat = "dd/MM/yyyy HH:mm:ss";

    [J("Fecha")]
    public required string Fecha { get; init; }
    /// <summary>
    /// Sample: 05/02/2025 20:43:02
    /// </summary>
    public DateTimeOffset DateTimeOffset
    {
        get
        {
            return DateTimeOffset.TryParseExact(Fecha, FechaFormat, Core.Constants.Globalization.DateTimeFormatInfoES, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTimeOffset result)
                ? result
                : DateTimeOffset.MinValue;
        }
    }

    [J("ListaEESSPrecio")]
    public required EstacionTerrestre[] EstacionesTerrestres { get; init; }

    [J("Nota")]
    public required string Nota { get; init; }

    [J("ResultadoConsulta")]
    public required string ResultadoConsulta { get; init; }
}

public record class EstacionTerrestre
{
    [J("C.P.")]
    public required string CodigoPostal { get; init; }

    [J("Dirección")]
    public required string Direccion { get; init; }

    [J("Horario")]
    public required string Horario { get; init; }

    [J("Localidad")]
    public required string Localidad { get; init; }

    [J("Latitud")]
    public required string Latitud { get; init; }
    public double Lat => double.Parse(Latitud, Core.Constants.Globalization.NumberFormatInfoES);

    [J("Longitud (WGS84)")]
    public required string Longitud { get; init; }
    public double Lng => double.Parse(Longitud, Core.Constants.Globalization.NumberFormatInfoES);

    public GoogleApis.Models.Shared.LatLngLiteral LatLng => new(Lat, Lng);

    public required string Margen { get; init; }

    public required string Municipio { get; init; }

    //[J("Remisión")]
    //public required string Remision { get; init; }

    [J("Rótulo")]
    public required string Rotulo { get; init; }

    //[J("Tipo Venta")]
    //public required string TipoVenta { get; init; }

    //[J("IDEESS"), K(typeof(Core.Extensions.ParseStringConverter))]
    //public long IdEstacionServicio { get; init; }

    //[J("IDMunicipio"), K(typeof(Core.Extensions.ParseStringConverter))]
    //public long IdMunicipio { get; init; }

    [J("IDProvincia")]
    public required string IdProvincia { get; init; }

    //[J("IDCCAA"), K(typeof(Core.Extensions.ParseStringConverter))]
    //public long IdComunidad { get; init; }

    public string DireccionParsed => $"{Direccion} - {CodigoPostal} {Localidad} {(string.Equals(Localidad, Municipio, StringComparison.OrdinalIgnoreCase) ? string.Empty : Municipio)}".Trim();

    [J("Precio Biodiesel")]
    public required string PrecioBiodiesel { get; init; }

    [J("Precio Bioetanol")]
    public required string PrecioBioetanol { get; init; }

    [J("Precio Gas Natural Comprimido")]
    public required string PrecioGasNaturalComprimido { get; init; }

    [J("Precio Gas Natural Licuado")]
    public required string PrecioGasNaturalLicuado { get; init; }

    [J("Precio Gases licuados del petróleo")]
    public required string PrecioGasesLicuadosDelPetróleo { get; init; }

    [J("Precio Gasoleo A")]
    public required string PrecioGasoleoA { get; init; }

    [J("Precio Gasoleo B")]
    public required string PrecioGasoleoB { get; init; }

    [J("Precio Gasoleo Premium")]
    public required string PrecioGasoleoPremium { get; init; }

    [J("Precio Gasolina 95 E10")]
    public required string PrecioGasolina95E10 { get; init; }

    [J("Precio Gasolina 95 E5")]
    public required string PrecioGasolina95E5 { get; init; }

    [J("Precio Gasolina 95 E5 Premium")]
    public required string PrecioGasolina95E5Premium { get; init; }

    [J("Precio Gasolina 98 E10")]
    public required string PrecioGasolina98E10 { get; init; }

    [J("Precio Gasolina 98 E5")]
    public required string PrecioGasolina98E5 { get; init; }

    //[J("Precio Hidrogeno")]
    //public required string PrecioHidrogeno { get; init; }

    //[J("% BioEtanol")]
    //public required string BioEtanol { get; init; }

    //[J("% Éster metílico")]
    //public required string EsterMetilico { get; init; }

    public bool IsInside(GoogleApis.Models.Shared.LatLngBoundsLiteral boundsLiteral)
    {
        return
            Lat < boundsLiteral.North &&
            Lat > boundsLiteral.South &&
            Lng < boundsLiteral.East &&
            Lng > boundsLiteral.West;
    }
}

namespace Seedysoft.FuelPrices.Lib.Core.Settings;

public record GoogleMapsPlatform
{
    public const string ModeDriving = "driving";

    public const string OutputFormatJson = "json";

    public required string ApiKey { get; init; }

    public required Directions Directions { get; init; }

    public required Maps Maps { get; init; }

    public required Places Places { get; init; }
}

public record Directions
{
    private const string AvoidTolls = "tolls";
    private const string LanguageEs = "es";
    private const string RegionEs = "es";
    private const string UnitsMetric = "metric";
    private const string RetrieveAlternatives = "true";

    /// <summary>
    /// <![CDATA[https://maps.googleapis.com/maps/api/directions/{OutputFormatJson}?origin={OriginValue}&destination={DestinationValue}&mode={ModeDriving}&avoid={AvoidTolls}&language={LanguageEs}&region={RegionEs}&units={UnitsMetric}&alternatives={RetrieveAlternatives}&key={GoogleApi}]]>
    /// <![CDATA[https://maps.googleapis.com/maps/api/directions/{0}?origin={1}&destination={2}c{3}&avoid={4}&language={5}&region={6}&units={7}&alternatives={8}&key={9}]]>
    /// </summary>
    public required string UriFormat { get; init; }

    public string GetUri(string originValue, string destinationValue, string apiKey)
    {
        return string.Format(
            UriFormat,
            GoogleMapsPlatform.OutputFormatJson,
            Uri.EscapeDataString(originValue),
            Uri.EscapeDataString(destinationValue),
            GoogleMapsPlatform.ModeDriving,
            AvoidTolls,
            LanguageEs,
            RegionEs,
            UnitsMetric,
            RetrieveAlternatives,
            apiKey);
    }
}

public record Maps
{
    /// <summary>
    /// <![CDATA[https://www.google.com/maps/dir/?api=1&origin={OriginValue}&destination={DestinationValue}&travelmode={ModeDriving}]]>
    /// <![CDATA[https://www.google.com/maps/dir/?api=1&origin={0}&destination={1}&travelmode={2}]]>
    /// </summary>
    public required string UriFormat { get; init; }

    public string GetUri(string originValue, string destinationValue)
    {
        return string.Format(
            UriFormat,
            Uri.EscapeDataString(originValue),
            Uri.EscapeDataString(destinationValue),
            GoogleMapsPlatform.ModeDriving);
    }
}

public record Places
{
    /// <summary>
    /// <![CDATA[https://maps.googleapis.com/maps/api/place/nearbysearch/{OutputFormatJson}?location={OutputFormatJson}&key={GoogleApi}]]>
    /// <![CDATA[https://maps.googleapis.com/maps/api/place/nearbysearch/{0}?location={1}&key={2}]]>
    /// </summary>
    public required string UriFormat { get; init; }

    public string GetUri(string latitud, string longitud, string apiKey)
    {
        return string.Format(
            UriFormat,
            GoogleMapsPlatform.OutputFormatJson,
            string.Join(",", latitud.Replace(",", "."), longitud.Replace(",", ".")),
            apiKey);
    }
}

namespace Seedysoft.Carburantes.Core.Settings;

public class GoogleMapsPlatform
{
    protected internal const string ModeDriving = "driving";

    protected internal const string OutputFormatJson = "json";

    public string ApiKey { get; set; } = default!;

    public Directions Directions { get; set; } = default!;

    public Maps Maps { get; set; } = default!;

    public Places Places { get; set; } = default!;
}

public class Directions
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
    public string UriFormat { get; set; } = default!;

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

public class Maps
{
    /// <summary>
    /// <![CDATA[https://www.google.com/maps/dir/?api=1&origin={OriginValue}&destination={DestinationValue}&travelmode={ModeDriving}]]>
    /// <![CDATA[https://www.google.com/maps/dir/?api=1&origin={0}&destination={1}&travelmode={2}]]>
    /// </summary>
    public string UriFormat { get; set; } = default!;

    public string GetUri(string originValue, string destinationValue)
    {
        return string.Format(
            UriFormat,
            Uri.EscapeDataString(originValue),
            Uri.EscapeDataString(destinationValue),
            GoogleMapsPlatform.ModeDriving);
    }
}

public class Places
{
    /// <summary>
    /// <![CDATA[https://maps.googleapis.com/maps/api/place/nearbysearch/{OutputFormatJson}?location={OutputFormatJson}&key={GoogleApi}]]>
    /// <![CDATA[https://maps.googleapis.com/maps/api/place/nearbysearch/{0}?location={1}&key={2}]]>
    /// </summary>
    public string UriFormat { get; set; } = default!;

    public string GetUri(string latitud, string longitud, string apiKey)
    {
        return string.Format(
            UriFormat,
            GoogleMapsPlatform.OutputFormatJson,
            string.Join(",", latitud.Replace(",", "."), longitud.Replace(",", ".")),
            apiKey);
    }
}

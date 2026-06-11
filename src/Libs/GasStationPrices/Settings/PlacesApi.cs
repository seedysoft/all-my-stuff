namespace Seedysoft.Libs.GasStationPrices.Settings;

public class PlacesApi : Api
{
    public string GetUrl(string textToFind) => string.Format(UrlFormat, textToFind);
}

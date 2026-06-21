namespace Seedysoft.Libs.Travel.Services.Geocoding;

internal abstract class GeocodingBase(Settings.GeocodingApi api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Settings.GeocodingApi Api { get; } = api;

    internal abstract Task<IList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken);
}

namespace Seedysoft.Libs.Travel.Services.Geocoding.Implementations;

internal abstract class GeocodingImplementationBase(Settings.GeocodingApi api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Settings.GeocodingApi Api { get; } = api;

    internal abstract Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken);
}

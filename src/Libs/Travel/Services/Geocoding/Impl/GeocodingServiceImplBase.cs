namespace Seedysoft.Libs.Travel.Services.Geocoding.Impl;

internal abstract class GeocodingServiceImplBase(Settings.GeocodingServiceApi api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Settings.GeocodingServiceApi Api { get; } = api;

    internal abstract Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken);
}

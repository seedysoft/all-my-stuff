namespace Seedysoft.Libs.Travel.Services.Geocoding.Impl;

/// <summary>
/// Base class for geocoding service implementations.
/// Provides common geocoding-specific functionality and delegates HTTP client management to the generic base.
/// </summary>
internal abstract class GeocodingServiceImplBase(IHttpClientFactory httpClientFactory, Settings.GeocodingServiceApi api)
    : HttpServiceImplBase<Settings.GeocodingServiceApi>(httpClientFactory, api)
{
    /// <summary>
    /// Finds places matching the provided search text.
    /// </summary>
    /// <param name="textToFind">The text to search for (e.g., place name, address).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of places matching the search criteria.</returns>
    internal abstract Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken);
}

namespace Seedysoft.Libs.Travel.Services.Reverse.Impl;

/// <summary>
/// Base class for reverse service implementations.
/// Provides common reverse geocoding-specific functionality and delegates HTTP client management to the generic base.
/// </summary>
internal abstract class ReverseServiceImplBase(IHttpClientFactory httpClientFactory, Settings.ReverseServiceApi api)
    : HttpServiceImplBase<Settings.ReverseServiceApi>(httpClientFactory, api)
{
    /// <summary>
    /// Finds places matching the provided search text.
    /// </summary>
    /// <param name="location">The location to search for a place name, an address ...</param>
    /// <param name="cancellationToken">A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of places matching the search criteria.</returns>
    internal abstract Task<IReadOnlyList<ViewModels.Place>> FindPlacesAsync(Core.Models.Location location, CancellationToken cancellationToken);
}

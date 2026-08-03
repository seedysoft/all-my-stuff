namespace Seedysoft.Libs.Travel.Services;

/// <summary>
/// Generic base class for HTTP-based service implementations.
/// Provides common HTTP client management and API configuration handling for routing and geocoding services.
/// </summary>
/// <typeparam name="TApi">The type of API configuration (e.g., RoutingServiceApi, GeocodingServiceApi)</typeparam>
internal abstract class HttpServiceImplBase<TApi>(IHttpClientFactory httpClientFactory, TApi api) 
    where TApi : Settings.Api
{
    /// <summary>
    /// Gets the configured HTTP client for making API requests.
    /// </summary>
    protected HttpClient HttpClient { get; } = httpClientFactory.CreateClient();

    /// <summary>
    /// Gets the API configuration for this service implementation.
    /// </summary>
    protected TApi Api { get; } = api;
}

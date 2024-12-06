namespace Seedysoft.Libs.GoogleApis.Json.Routes.Response;

/// <summary>
/// Reasons for using fallback response.
/// </summary>
public enum FallbackReason
{
    /// <summary>
    /// No fallback reason specified.
    /// </summary>
    FALLBACK_REASON_UNSPECIFIED,
    /// <summary>
    /// A server error happened while calculating routes with your preferred routing mode, but we were able to return a result calculated by an alternative mode.
    /// </summary>
    SERVER_ERROR,
    /// <summary>
    /// We were not able to finish the calculation with your preferred routing mode on time, but we were able to return a result calculated by an alternative mode.
    /// </summary>
    LATENCY_EXCEEDED,
}

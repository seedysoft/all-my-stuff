using System.Text.Json.Serialization;

namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Information related to how and why a fallback result was used. If this field is set, then it means the server used a different routing mode from your preferred mode as fallback.
/// </summary>
public class FallbackInfo
{
    /// <summary>
    /// Routing mode used for the response. If fallback was triggered, the mode may be different from routing preference set in the original client request.
    /// </summary>
    [J("routingMode")][I(Condition = C.WhenWritingDefault)][K(typeof(JsonStringEnumConverter))] public required FallbackRoutingMode RoutingMode { get; set; }
    /// <summary>
    /// The reason why fallback response was used instead of the original response. This field is only populated when the fallback mode is triggered and the fallback response is returned.
    /// </summary>
    [J("reason")][I(Condition = C.WhenWritingDefault)][K(typeof(JsonStringEnumConverter))] public required FallbackReason Reason { get; set; }
}

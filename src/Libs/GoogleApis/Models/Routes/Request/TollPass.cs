using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// List of toll passes around the world that we support.
/// <see href="https://developers.google.com/maps/documentation/routes/reference/rest/v2/RouteModifiers#tollpass"/>
/// </summary>
public enum TollPass
{
    /// <summary>
    /// Not used.
    /// If this value is used, then the request fails.
    /// </summary>
    [EnumMember(Value = "TOLL_PASS_UNSPECIFIED")]
    TollPassUnspecified,
}

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Traffic density indicator on a contiguous segment of a polyline or path. Given a path with points P_0, P_1, ... , P_N (zero-based index), the SpeedReadingInterval defines an interval and describes its traffic using the following categories.
/// </summary>
public class SpeedReadingInterval
{
    /// <summary>
    /// The starting index of this interval in the polyline.
    /// </summary>
    [J("startPolylinePointIndex"), I(Condition = C.WhenWritingNull)]
    public int? StartPolylinePointIndex { get; set; }

    /// <summary>
    /// The ending index of this interval in the polyline.
    /// </summary>
    [J("endPolylinePointIndex"), I(Condition = C.WhenWritingNull)]
    public int? EndPolylinePointIndex { get; set; }

    /// <summary>
    /// Traffic speed in this interval.
    /// </summary>
    [J("speed"), I(Condition = C.WhenWritingNull), K(typeof(Utils.Extensions.EnumMemberJsonConverter<Speed>))]
    public Speed? Speed { get; set; }
}

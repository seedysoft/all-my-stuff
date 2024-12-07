using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// A set of values that specify the unit of measure used in the display.
/// </summary>
public enum Units
{
    /// <summary>
    /// Units of measure not specified.
    /// Defaults to the unit of measure inferred from the request.
    /// </summary>
    [EnumMember(Value = "UNITS_UNSPECIFIED")]
    UnitsUnspecified,

    /// <summary>
    /// Imperial (English) units of measure.
    /// </summary>
    [EnumMember(Value = "IMPERIAL")]
    Imperial,

    /// <summary>
    /// Metric units of measure.
    /// </summary>
    [EnumMember(Value = "METRIC")]
    Metric,
}

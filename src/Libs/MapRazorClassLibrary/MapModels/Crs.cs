using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Seedysoft.Libs.MapRazorClassLibrary.MapModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
[method: SetsRequiredMembers]
public class Crs(string code, double[]? wrapLng = default, double[]? wrapLat = default, bool infinite = false)
{
    /// <summary>
    /// The most common CRS for online maps, used by almost all free and commercial tile providers.
    /// Uses Spherical Mercator projection. Set in by default in Map's crs option.
    /// </summary>
    public static readonly Crs EPSG3857 = new("EPSG:3857");

    /// <summary>
    /// Standard code name of the CRS passed into WMS services (e.g. 'EPSG:3857')
    /// </summary>
    [J("code")] public required string Code { get; init; } = code;

    /// <summary>
    /// An array of two numbers defining whether the longitude (horizontal) coordinate axis wraps around a given range and how.
    /// Defaults to [-180, 180] in most geographical CRSs.
    /// If undefined, the longitude axis does not wrap around.
    /// </summary>
    [J("wrapLng")] public double[]? WrapLng { get; init; } = wrapLng;

    /// <summary>
    /// Like wrapLng, but for the latitude (vertical) axis.
    /// </summary>
    [J("wrapLat")] public double[]? WrapLat { get; init; } = wrapLat;

    /// <summary>
    /// If true, the coordinate space will be unbounded (infinite in both axes)
    /// </summary>
    [J("infinite")] public bool Infinite { get; init; } = infinite;

    private string GetDebuggerDisplay() => Code;
}

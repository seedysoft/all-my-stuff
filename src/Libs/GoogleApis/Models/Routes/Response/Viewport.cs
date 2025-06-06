﻿namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// A latitude-longitude viewport, represented as two diagonally opposite low and high points.
/// A viewport is considered a closed region, i.e. it includes its boundary.
/// The latitude bounds must range between -90 to 90 degrees inclusive, and the longitude bounds must range between -180 to 180 degrees inclusive.
/// Various cases include:
/// If low = high, the viewport consists of that single point.
/// If low.longitude > high.longitude, the longitude range is inverted (the viewport crosses the 180 degree longitude line).
/// If low.longitude = -180 degrees and high.longitude = 180 degrees, the viewport includes all longitudes.
/// If low.longitude = 180 degrees and high.longitude = -180 degrees, the longitude range is empty.
/// If low.latitude > high.latitude, the latitude range is empty.
/// Both low and high must be populated, and the represented box cannot be empty (as specified by the definitions above).
/// An empty viewport will result in an error.
/// For example, this viewport fully encloses New York City:
/// { "low": { "latitude": 40.477398, "longitude": -74.259087 }, "high": { "latitude": 40.91618, "longitude": -73.70018 } }
/// </summary>
public record Viewport
{
    /// <summary>
    /// Required. The high point of the viewport.
    /// </summary>
    [J("high")]
    public required Models.Shared.LatitudeLongitude High { get; init; }

    /// <summary>
    /// Required. The low point of the viewport.
    /// </summary>
    [J("low")]
    public required Models.Shared.LatitudeLongitude Low { get; init; }
}

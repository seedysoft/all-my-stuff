using Seedysoft.Libs.Core.Json;

namespace Seedysoft.Libs.Core.Constants;

/// <summary>
/// To avoid use a pair of double values to represent a geographic coordinate, this record is used to represent a latitude and longitude pair.
/// </summary>
/// <param name="Lng"> Longitude in degrees. Values outside the range [-180, 180] will be wrapped so that they fall within the range. 
/// For example, a value of -190 will be converted to 170. A value of 190 will be converted to -170. 
/// This reflects the fact that longitudes wrap around the globe.
/// </param>
/// <param name="Lat"> Latitude in degrees. Values will be clamped to the range [-90, 90]. 
/// This means that if the value specified is less than -90, it will be set to -90. 
/// And if the value is greater than 90, it will be set to 90.
/// </param>
[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
[K(typeof(LngLatJsonConverter))]
public record LngLat(double Lng, double Lat)
{
    //public double Lat { get; set; } = Lat;
    //public double Lng { get; set; } = Lng;

    private string GetDebuggerDisplay => $"Lon:{Lng} - Lat:{Lat}";
}

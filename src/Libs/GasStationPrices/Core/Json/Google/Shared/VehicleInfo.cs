namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Shared;

public class VehicleInfo
{
    /// <summary>
    /// Describes the vehicle's emission type. Applies only to the <see cref="RouteTravelMode.DRIVE"/>.
    /// </summary>
    [J("emissionType")][K(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))] public VehicleEmissionType EmissionType { get; set; } = VehicleEmissionType.VEHICLE_EMISSION_TYPE_UNSPECIFIED;
}

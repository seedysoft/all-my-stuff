namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// Contains the vehicle information, such as the vehicle emission type.
/// </summary>
public class VehicleInfo
{
    /// <summary>
    /// Describes the vehicle's emission type. 
    /// Applies only to the <see cref="Shared.RouteTravelMode.Drive"></see> <see cref="Shared.RouteTravelMode"></see>.
    /// </summary>
    [J("emissionType"), K(typeof(Core.Extensions.EnumMemberJsonConverter<VehicleEmissionType>))]
    public virtual VehicleEmissionType EmissionType { get; set; } = VehicleEmissionType.Gasoline;
}

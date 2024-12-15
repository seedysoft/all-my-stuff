﻿namespace Seedysoft.Libs.GoogleApis.Models.Routes.Request;

/// <summary>
/// Vehicle Info.
/// Encapsulates the vehicle information, such as the license plate last character.
/// </summary>
public class VehicleInfo
{
    /// <summary>
    ///Emission Type.
    /// Describes the vehicle's emission type. Applies only to the DRIVE travel mode.
    /// </summary>
    public virtual VehicleEmissionType EmissionType { get; set; } = VehicleEmissionType.Gasoline;
}

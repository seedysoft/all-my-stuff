﻿using System.Runtime.Serialization;

namespace Seedysoft.Libs.GoogleApis.Json.Shared;

/// <summary>
/// Extra Computation.
/// Extra computations to perform while completing the request.
/// </summary>
public enum ExtraComputation
{
    /// <summary>
    /// Toll information for the route(s).
    /// </summary>
    [EnumMember(Value = "TOLLS")]
    Tolls
}

namespace Seedysoft.Libs.GoogleApis.Json.Shared;

public enum TransitPreferences
{
    /// <summary>
    /// No TransitRoutingPreference.
    /// </summary>
    Nothing = 0,

    /// <summary>
    /// Indicates that the calculated route should prefer limited amounts of walking.
    /// </summary>
    Less_Walking = 1,

    /// <summary>
    /// Indicates that the calculated route should prefer a limited number of transfers.
    /// </summary>
    Fewer_Transfers = 2
}

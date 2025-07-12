namespace Seedysoft.Libs.GoogleApis.Models.Routes.Response;

/// <summary>
/// Encapsulates toll information on a <see cref="Route"/> or on a <see cref="RouteLeg"/>.
/// </summary>
public class TollInfo
{
    /// <summary>
    /// The monetary amount of tolls for the corresponding <see cref="Route"/> or <see cref="RouteLeg"/>. This list contains a money amount for each currency that is expected to be charged by the toll stations. Typically this list will contain only one item for routes with tolls in one currency. For international trips, this list may contain multiple items to reflect tolls in different currencies.
    /// </summary>
    [J("estimatedPrice"), I(Condition = C.WhenWritingNull)]
    public Money? EstimatedPrice { get; init; }
}

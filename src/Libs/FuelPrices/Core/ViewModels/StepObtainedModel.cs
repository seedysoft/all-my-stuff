namespace Seedysoft.Libs.FuelPrices.Core.ViewModels;

public record StepObtainedModel
{
    //public string Instructions { get; set; }

    public JsonObjects.GoogleMaps.LocationJson[] Locations { get; set; } = default!;
}

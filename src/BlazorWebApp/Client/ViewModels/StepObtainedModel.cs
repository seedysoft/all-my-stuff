namespace Seedysoft.BlazorWebApp.Client.ViewModels;

public record StepObtainedModel
{
    //public string Instructions { get; set; }

    public FuelPrices.Lib.Core.JsonObjects.GoogleMaps.LocationJson[] Locations { get; set; } = default!;
}

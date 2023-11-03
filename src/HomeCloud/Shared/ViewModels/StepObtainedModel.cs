namespace Seedysoft.HomeCloud.Shared.ViewModels;

public record StepObtainedModel
{
    //public string Instructions { get; set; }

    public Carburantes.Core.JsonObjects.GoogleMaps.LocationJson[] Locations { get; set; } = default!;
}

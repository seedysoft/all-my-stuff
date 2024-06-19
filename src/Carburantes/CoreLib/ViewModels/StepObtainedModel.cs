using Seedysoft.Carburantes.CoreLib.JsonObjects.GoogleMaps;

namespace Seedysoft.Carburantes.CoreLib.ViewModels;

public record StepObtainedModel
{
    //public string Instructions { get; set; }

    public LocationJson[] Locations { get; set; } = default!;
}

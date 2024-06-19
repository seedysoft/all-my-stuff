using Seedysoft.Carburantes.CoreLib.JsonObjects.GoogleMaps.Directions;
using System.ComponentModel.DataAnnotations;

namespace Seedysoft.Carburantes.CoreLib.ViewModels;

public record RouteObtainedModel
{
    public string Summary { get; set; } = default!;

    public string FromPlace { get; set; } = default!;

    public string ToPlace { get; set; } = default!;

    public TextAndValueJson Distance { get; set; } = default!;

    [Display(Description = "Distance")]
    public string DistanceText => Distance.Text;

    public TextAndValueJson Duration { get; set; } = default!;

    [Display(Description = "Duration")]
    public string DurationText => Duration.Text;

    public string GoogleMapsUri { get; set; } = default!;

    public StepObtainedModel[] Steps { get; set; } = default!;
}

using System.ComponentModel.DataAnnotations;

namespace Seedysoft.Libs.FuelPrices.Core.ViewModels;

public record TravelObtainedModel
{
    public string Summary { get; set; } = default!;

    public string FromPlace { get; set; } = default!;

    public string ToPlace { get; set; } = default!;

    public JsonObjects.GoogleMaps.Directions.TextAndValueJson Distance { get; set; } = default!;

    [Display(Description = "Distance")]
    public string DistanceText => Distance.Text;

    public JsonObjects.GoogleMaps.Directions.TextAndValueJson Duration { get; set; } = default!;

    [Display(Description = "Duration")]
    public string DurationText => Duration.Text;

    public string GoogleMapsUri { get; set; } = default!;

    public StepObtainedModel[] Steps { get; set; } = default!;
}

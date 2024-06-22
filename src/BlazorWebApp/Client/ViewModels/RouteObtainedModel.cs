﻿using System.ComponentModel.DataAnnotations;

namespace Seedysoft.BlazorWebApp.Client.ViewModels;

public record RouteObtainedModel
{
    public string Summary { get; set; } = default!;

    public string FromPlace { get; set; } = default!;

    public string ToPlace { get; set; } = default!;

    public FuelPrices.Lib.Core.JsonObjects.GoogleMaps.Directions.TextAndValueJson Distance { get; set; } = default!;

    [Display(Description = "Distance")]
    public string DistanceText => Distance.Text;

    public FuelPrices.Lib.Core.JsonObjects.GoogleMaps.Directions.TextAndValueJson Duration { get; set; } = default!;

    [Display(Description = "Duration")]
    public string DurationText => Duration.Text;

    public string GoogleMapsUri { get; set; } = default!;

    public StepObtainedModel[] Steps { get; set; } = default!;
}

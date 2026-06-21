using Microsoft.AspNetCore.Components;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.BlazorWebApp.Client.Pages;

// TODO                 Add button to switch between Origin and Destination
// TODO                 Add button for obtain current location

public partial class TravelSearch
{
    [Inject] private ILogger<TravelSearch> Logger { get; set; } = default!;
    [Inject] private MudBlazor.ISnackbar Snackbar { get; set; } = default!;
    [Inject] private Libs.Travel.Services.Geocoding.GeocodingService GeocodingService { get; set; } = default!;

    private Libs.MapRazorClassLibrary.MapComponent TravelMap { get; set; } = default!;

    private readonly Libs.GasStationPrices.ViewModels.TravelQueryModel travelQueryModel = Libs.GasStationPrices.ViewModels.TravelQueryModel.
#if DEBUG
            CreateDefault()
#else
            CreateEmpty()
#endif
;
    private readonly Libs.GasStationPrices.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");
    }

    private async Task<IEnumerable<Libs.Travel.ViewModels.Place>> FindPlacesAsync(string textToFind, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(textToFind))
                return await GeocodingService.FindPlacesAsync(textToFind, cancellationToken) ?? [];
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }

        return [];
    }

    private async Task ValidateSearch()
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (validationResult.IsValid)
        {
            string? textToShow = await TravelMap.LoadRoutesAndGasStationsAsync(travelQueryModel, CancellationToken.None);
            if (!string.IsNullOrWhiteSpace(textToShow))
                _ = Snackbar.Add(new MarkupString($"<ul>{string.Join(string.Empty, textToShow)}</ul>"), MudBlazor.Severity.Info);
        }
        else
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join(string.Empty, errors)}</ul>"), MudBlazor.Severity.Error);
        }
    }
}

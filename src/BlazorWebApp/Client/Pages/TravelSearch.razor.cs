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

    private readonly Libs.Travel.ViewModels.TravelQueryModel travelQueryModel = Libs.Travel.ViewModels.TravelQueryModel.
#if DEBUG
            CreateDefault();
#else
            CreateEmpty();
#endif
    private readonly Libs.Travel.ViewModels.TravelQueryModelFluentValidator travelQueryModelFluentValidator = new();

    private readonly Libs.GasStationPrices.ViewModels.GasStationsQueryModel gasStationsQueryModel = Libs.GasStationPrices.ViewModels.GasStationsQueryModel.
#if DEBUG
            CreateDefault();
#else
            CreateEmpty();
#endif

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");
    }

    private void SetPetroleumProductsSelectedIds(System.Collections.Immutable.ImmutableSortedSet<Libs.GasStationPrices.Models.Minetur.ProductoPetrolifero>? fromWhat)
        => gasStationsQueryModel.PetroleumProductsSelectedIds = [.. fromWhat?.Select(static x => x.IdProducto) ?? []];

    private async Task<IEnumerable<Libs.Travel.ViewModels.Place>> FindPlacesAsync(
        string textToFind,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(textToFind))
                return await GeocodingService.FindPlacesAsync(textToFind, cancellationToken) ?? [];
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error"))
        {
            _ = Snackbar.Add(new MarkupString($"<span>{e}</span>"), MudBlazor.Severity.Error);
        }

        return [];
    }

    private async Task ValidateSearchAsync(/*Microsoft.AspNetCore.Components.Web.MouseEventArgs args*/)
    {
        FluentValidation.Results.ValidationResult validationResult = await travelQueryModelFluentValidator.ValidateAsync(travelQueryModel);
        if (validationResult.IsValid)
        {
            await LoadRoutesAsync();
        }
        else
        {
            IEnumerable<string> errors = validationResult.Errors.Select(static x => $"<li>{x.ErrorMessage}</li>");
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Concat(errors)}</ul>"), MudBlazor.Severity.Error);
        }
    }

    private async Task ReloadGasStationsAsync(Libs.MapRazorClassLibrary.MapModels.Basic.LatLngBounds mapBounds)
    {
        string? textToShow = await TravelMap.LoadGasStationsAsync(mapBounds, gasStationsQueryModel.PetroleumProductsSelectedIds, default);
        if (!string.IsNullOrWhiteSpace(textToShow))
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join(string.Empty, textToShow)}</ul>"), MudBlazor.Severity.Info);
    }
    private async Task LoadRoutesAsync()
    {
        string? textToShow = await TravelMap.LoadRoutesAsync(travelQueryModel, default);
        if (!string.IsNullOrWhiteSpace(textToShow))
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join(string.Empty, textToShow)}</ul>"), MudBlazor.Severity.Info);
    }

    private void OnMapCreatedAsyncEventCallback(Libs.MapRazorClassLibrary.MapComponent args) { }

    private void OnMapClickAsyncEventCallback(Libs.MapRazorClassLibrary.MapModels.Basic.LatLng args) { }

    private async Task OnMapMoveEndEventCallback(Libs.MapRazorClassLibrary.MapModels.Basic.LatLngBounds args)
        => await ReloadGasStationsAsync(args);

    private async Task OnCircleMarkerClicEventCallback(Libs.MapRazorClassLibrary.MapModels.Basic.LatLng args) { }

    // TODO                                                         Implement independient gas filters and css display classes
}

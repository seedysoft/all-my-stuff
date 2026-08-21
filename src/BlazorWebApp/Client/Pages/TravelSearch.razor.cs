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

    private Libs.Travel.ViewModels.TravelQueryModel TravelQueryModel { get; set; }
        = Libs.Travel.ViewModels.TravelQueryModel.CreateDefault();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation($"Called {nameof(OnInitializedAsync)}");
    }

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

    private async Task OnSearchMudButtonClick(/*Microsoft.AspNetCore.Components.Web.MouseEventArgs args*/)
    {
        string? textToShow = await TravelMap.LoadRoutesAsync(default);
        if (!string.IsNullOrWhiteSpace(textToShow))
            _ = Snackbar.Add(new MarkupString($"<ul>{string.Join(string.Empty, textToShow)}</ul>"), MudBlazor.Severity.Info);
    }

    //private void OnMapCreatedAsyncEventCallback(Libs.MapRazorClassLibrary.MapComponent args) { }

    //private void OnMapClickAsyncEventCallback(Libs.MapRazorClassLibrary.MapModels.Basic.LatLng args) { }

    //private async Task OnMapMoveEndEventCallback(Libs.MapRazorClassLibrary.MapModels.Basic.LatLngBounds args) { } // => await ReloadGasStationsAsync();

    //private async Task OnCircleMarkerClicEventCallback(Libs.MapRazorClassLibrary.MapModels.Basic.LatLng args) { }

    // TODO                                                         Implement independient gas filters and css display classes
}

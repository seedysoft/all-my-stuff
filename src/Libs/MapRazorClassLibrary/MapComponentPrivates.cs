using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent : IAsyncDisposable
{
    private DotNetObjectReference<MapComponent> ObjRef = default!;

    private readonly string MapId = $"map-{Guid.NewGuid()}";

    private bool IsMapReady = false;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private GasStationPrices.Services.GasStationPricesService GasStationPricesService { get; set; } = default!;

    [Inject] private Travel.Services.Routing.RoutingService RoutingService { get; set; } = default!;

    private static void ThreadSetts()
    {
        if (!(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator != "."))
            return;

        if (!Thread.CurrentThread.CurrentCulture.IsReadOnly && !Thread.CurrentThread.CurrentCulture.NumberFormat.IsReadOnly)
        {
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            return;
        }

        System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-us")
        {
            NumberFormat =
            {
                CurrencyDecimalSeparator = ".",
                NumberDecimalSeparator = "."
            }
        };

        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
    }

    private async Task CreateMap()
    {
        try
        {
            await MapModule.InvokeVoidAsync("createMap", MapId, Options);

            if (OnMapCreatedAsync.HasDelegate)
                await OnMapCreatedAsync.InvokeAsync(this);

            //if (OnMapClickAsync.HasDelegate)
            //    await LeafletService.InvokeVoidAsync("setClickHandler", MapId, ObjRef, nameof(OnMapClick));
        }
        catch (Exception ex)
        {
            await Console.Out.WriteAsync(ex.Message);
        }

        IsMapReady = true;

        await InvokeAsync(StateHasChanged);
    }

    private async Task AddConnector(
        double[,] arrayPolyline,
        int? start = 2,
        double? radianAngle = 0.5235987755982989,
        double? length = 10.0,
        MapModels.PolylineSymbol? symbol = null)
    {
        ThreadSetts();

        symbol ??= new MapModels.PolylineSymbol();

        await Task.Run(async delegate
        {
            if (arrayPolyline.GetLength(1) != 2)
                throw new ArgumentException("The arrayPolyline must be a 2D array with two columns for latitude and longitude.");

            IEnumerable<string> values =
                from i in Enumerable.Range(0, arrayPolyline.GetLength(0))
                select new double[2]
                {
                    arrayPolyline[i, 0],
                    arrayPolyline[i, 1]
                } into item
                select "[" + string.Join(",", item) + "]";

            MapModels.SimpleDirection simpleDirection = new()
            {
                Polyline = "[" + string.Join(",", values) + "]",
                Start = start,
                RadianAngle = Math.PI - radianAngle,
                Length = length,
                Symbol = symbol
            };

            await MapModule.InvokeVoidAsync("addDirection", simpleDirection);
        });
    }

    //private async Task EnableSpinnerAsync() =>
    //    await LeafletService.InvokeVoidAsync("enableSpinner", MapId);
    //private async Task DisableSpinnerAsync() =>
    //    await LeafletService.InvokeVoidAsync("disableSpinner", MapId);

    //private string GetIcon(Icon icon)
    //{
    //    string useIcon = icon switch
    //    {
    //        Icon.DRON => "drone",
    //        Icon.HOME => "home",
    //        Icon.DESTINATION => "destination",
    //        _ => "marker-icon"
    //    };
    //    return useIcon;
    //}

    //private string GetIconUrl(string iconUrl)
    //{
    //    if (iconUrl.Contains("http"))
    //        return iconUrl;
    //    else
    //        return $"./{Core.Helpers.ContentHelper.ContentPath}/css/images/{iconUrl}.png";
    //}
}

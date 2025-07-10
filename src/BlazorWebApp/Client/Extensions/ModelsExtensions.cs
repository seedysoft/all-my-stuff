namespace Seedysoft.BlazorWebApp.Client.Extensions;

public static class ModelsExtensions
{
    public static Libs.GoogleMapsRazorClassLib.GoogleMap.Marker ToMarker(
        this Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
    {
        return new()
        {
            Content = null,
            PinElement = new Libs.GoogleMapsRazorClassLib.GoogleMap.PinElement()
            {
                Background = "blue",
                BorderColor = "yellow",
                Glyph = gasStationModel.RotuloTrimed.First(),
                GlyphColor = "white",
                Scale = 1.5,
            },
            Position = new(gasStationModel.Lat, gasStationModel.Lng),
            Title = gasStationModel.RotuloTrimed,
            ZIndex = 1,
        };
    }
}

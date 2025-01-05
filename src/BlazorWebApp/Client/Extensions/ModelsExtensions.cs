namespace Seedysoft.BlazorWebApp.Client.Extensions;

public static class ModelsExtensions
{
    public static Libs.GoogleMapsRazorClassLib.GoogleMap.Marker ToMarker(this Libs.GasStationPrices.ViewModels.GasStationModel gasStationModel)
    {
        return new Libs.GoogleMapsRazorClassLib.GoogleMap.Marker()
        {
            Content = $"<div style='background-color:blue'>{gasStationModel.RotuloTrimed}</div>",
            PinElement = new()
            {
                Background = "",
                BorderColor = "",
                Glyph = null,
                GlyphColor = "azure",
                Scale = 1.0,
                UseIconFonts = true,
            },
            Position = new(gasStationModel.Lat, gasStationModel.Lng),
            Title = gasStationModel.RotuloTrimed,
        };
    }
}

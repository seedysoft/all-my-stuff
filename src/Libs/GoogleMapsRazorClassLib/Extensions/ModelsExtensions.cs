namespace Seedysoft.Libs.GoogleMapsRazorClassLib.Extensions;

public static class ModelsExtensions
{
    public static GoogleMap.Marker ToMarker(
        this GasStationPrices.ViewModels.GasStationModel gasStationModel
        , IReadOnlyCollection<GasStationPrices.Constants.ProductoPetroliferoId> selectedProductIds)
    {
        return new()
        {
            Address = gasStationModel.Localizacion,
            ProductsAndPrices = GetProductsAndPrices(gasStationModel, selectedProductIds),
            PinElement = new GoogleMap.PinElement()
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

        static GasStationPrices.ViewModels.ProductAndPrice[] GetProductsAndPrices(
            GasStationPrices.ViewModels.GasStationModel gasStationModel
            , IReadOnlyCollection<GasStationPrices.Constants.ProductoPetroliferoId> selectedProductIds)
            => [.. selectedProductIds.Select(gasStationModel.GetProductNameAndPrice).Where(x => !string.IsNullOrWhiteSpace(x.Price))];
    }
}

namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public record class ProductPrice(Json.Minetur.ProductoPetrolifero Product, decimal? Price) { }

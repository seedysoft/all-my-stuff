namespace Seedysoft.Libs.GasStationPrices.ViewModels;

//public record class ProductPrice(Models.Minetur.ProductoPetrolifero Product, decimal? Price) { }
public record class ProductPrice(long ProductId, decimal? Price) { }

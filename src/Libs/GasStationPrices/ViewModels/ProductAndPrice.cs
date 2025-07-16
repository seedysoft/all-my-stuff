namespace Seedysoft.Libs.GasStationPrices.ViewModels;

public readonly record struct ProductAndPrice
{
    public ProductAndPrice(string productName, string? price)
    {
        ProductName = productName;
        Price = price;
    }

    public string ProductName { get; }
    public string? Price { get; }
}

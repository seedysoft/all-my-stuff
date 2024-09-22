namespace Seedysoft.BlazorWebApp.Client.Constants;

public sealed class PetroleumProductsUris : BaseUris
{
    public const string Controller = $"{Api}petroleum-products";

    public sealed class Actions
    {
        public const string ForFilter = "ForFilter";

        private Actions() { }
    }

    private PetroleumProductsUris() { }
}

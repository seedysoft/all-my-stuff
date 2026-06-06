namespace Seedysoft.Libs.Geography.Services.Routers;

public abstract class RouterBase(Settings.Api api)
{
    protected RestSharp.RestClient RestClient { get; } = new(new Uri(api.UrlFormat).GetLeftPart(UriPartial.Authority));
    protected Settings.Api Api { get; } = api;

    public abstract Task<List<Models.RouteModel>> GetRoutesAsync(ViewModels.TravelQueryModel model, CancellationToken cancellationToken);
}

namespace Seedysoft.Libs.MapRazorClassLibrary;

public partial class MapComponent
{
    public async Task<string?> LoadRoutesAsync(CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validationResult = await TravelQueryModelFluentValidator.ValidateAsync(TravelQueryModel, cancellationToken);
        if (!validationResult.IsValid)
            return string.Join(Environment.NewLine, validationResult.Errors.Select(static x => $"{x.ErrorMessage}"));

        await ShowLoaderAsync();

        await RemoveRoutesAsync();

        string? returnText = null;
        IReadOnlyList<(string NombreRuta, double[,] Coordenadas)> res;
        try
        {
            res = await RoutingService.GetRoutesAsync(TravelQueryModel.Orig.Location, TravelQueryModel.Dest.Location, cancellationToken);
        }
        catch (Exception e)
        {
            res = [];
            returnText = e.ToString();
        }

        if (string.IsNullOrEmpty(returnText))
        {
            if (res.Count == 0)
            {
                returnText = "⚠️ No route to load ⚠️";
            }
            else
            {
                for (int i = 0; i < res.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        returnText = "Cancellation Requested";
                        break;
                    }

                    (string? NombreRuta, double[,]? Coordenadas) = res[i];

                    await AddPolylineAsync(arrayPolyline: Coordenadas, color: ColorsForRoutes[i]);
                }
            }
        }

        await HideLoaderAsync();

        return returnText;
    }
}

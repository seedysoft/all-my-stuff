namespace Seedysoft.Libs.Travel.Interfaces;

public interface IGeocodingService
{
    Task<IReadOnlyList<ViewModels.Place>> GeocodeAsync(
        string textToFind,
        CancellationToken cancellationToken);
}


namespace Seedysoft.Libs.Travel.Interfaces;

public interface IReverseService
{
    Task<ViewModels.Place> ReverseAsync(
        Core.Models.Location location,
        CancellationToken cancellationToken);
}

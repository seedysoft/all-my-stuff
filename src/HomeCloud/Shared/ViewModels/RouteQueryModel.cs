using System.ComponentModel.DataAnnotations;

namespace Seedysoft.HomeCloud.Shared.ViewModels;

public record RouteQueryModel
{
    [Required(AllowEmptyStrings = false)]
    public string Origin { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string Destination { get; set; } = default!;
}

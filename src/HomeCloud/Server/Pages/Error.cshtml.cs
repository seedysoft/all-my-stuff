using Microsoft.AspNetCore.Mvc;

namespace Seedysoft.HomeCloud.Server.Pages;

[IgnoreAntiforgeryToken]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger) => _logger = logger;

    public void OnGet()
    {
        RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        _logger.LogError("Obtained Error page with RequestId: '{RequestId}'", RequestId);
    }
}

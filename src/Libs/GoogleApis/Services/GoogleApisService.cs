using Microsoft.Extensions.Configuration;

namespace Seedysoft.Libs.GoogleApis.Services;

public abstract class GoogleApisService(IConfiguration configuration)
{
    protected Settings.GoogleApisSettings GoogleApisSettings => configuration
        .GetSection(nameof(Settings.GoogleApisSettings)).Get<Settings.GoogleApisSettings>()!;
}

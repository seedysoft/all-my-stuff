using Microsoft.Extensions.Configuration;

namespace Seedysoft.Libs.GoogleApis.Services;

public abstract class GoogleApisServiceBase(IConfiguration configuration)
{
    protected Settings.GoogleApisSettings GoogleApisSettings => configuration
        .GetSection(nameof(Settings.GoogleApisSettings)).Get<Settings.GoogleApisSettings>()!;
}

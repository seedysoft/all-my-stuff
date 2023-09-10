using Microsoft.Extensions.Configuration;

namespace Seedysoft.UtilsLib.Extensions;

public static class IConfigurationExtensions
{
    private const string ConnectionStringKey ="dbConnectionString";

    public static string GetDbCtx(this IConfiguration configuration, Enums.ConnectionMode connectionMode = Enums.ConnectionMode.ReadWrite) =>
        $"{configuration.GetConnectionString(ConnectionStringKey) + (connectionMode == Enums.ConnectionMode.ReadOnly ? ";Mode=ReadOnly;" : string.Empty)}";
}

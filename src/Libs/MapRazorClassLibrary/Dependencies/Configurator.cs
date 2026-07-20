using Microsoft.Extensions.DependencyInjection;

namespace Seedysoft.Libs.MapRazorClassLibrary.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No JsonFiles */ }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) =>
        _ = hostApplicationBuilder.Services.Configure<System.Text.Json.JsonSerializerOptions>(static jsonSerializerOptions
            => jsonSerializerOptions.Converters.Add(new OneOf.Serialization.SystemTextJson.OneOfJsonConverter()));
}

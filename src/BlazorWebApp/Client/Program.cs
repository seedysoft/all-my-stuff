using MudBlazor.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var webAssemblyHostBuilder = Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder.CreateDefault(args);

        _ = webAssemblyHostBuilder.Services
            .AddScoped(sp =>
            new HttpClient
            {
                BaseAddress = new Uri(webAssemblyHostBuilder.HostEnvironment.BaseAddress)
            })

            .AddMudServices()
        ;

        await webAssemblyHostBuilder
            .Build()
            .RunAsync();
    }
}

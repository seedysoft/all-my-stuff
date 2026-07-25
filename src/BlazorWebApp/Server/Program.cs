using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.BlazorWebApp.Server;

public class Program : Libs.Core.ProgramBase
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        await ObtainCommandLineAsync(args);

        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

        _ = webApplicationBuilder.AddAllMyDependencies();

        // TODO         Learn how it works
        _ = webApplicationBuilder.Configuration.AddInMemoryCollection(Libs.Core.Models.Config.RuntimeSettings.GetValues(Settings));

        // Add services to the container.
        _ = webApplicationBuilder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        _ = webApplicationBuilder.Services.ConfigureHttpJsonOptions(static configureOptions =>
        {
            configureOptions.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            configureOptions.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            configureOptions.SerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
        });

        WebApplication webApplication = webApplicationBuilder.Build();

        // Configure the HTTP request pipeline.
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseWebAssemblyDebugging();
        }
        else
        {
            _ = webApplication.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = webApplication.UseHsts();
        }

        _ = webApplication
            .UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true)
            .UseHttpsRedirection()
            .UseAntiforgery();

        _ = webApplication.MapStaticAssets();

        _ = webApplication.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies([
                typeof(Libs.MapRazorClassLibrary.MapComponent).Assembly,
                typeof(Client._Imports).Assembly,
            ]);

        await webApplication.RunAsync();
    }
}

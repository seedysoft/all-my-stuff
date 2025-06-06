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

        WebApplication webApplication = webApplicationBuilder.Build();

        // Configure the HTTP request pipeline.
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseWebAssemblyDebugging();

            // Add OpenAPI/Swagger generator and the Swagger UI
            _ = webApplication
                .UseOpenApi()
                .UseSwaggerUi();
        }
        else
        {
            _ = webApplication.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = webApplication.UseHsts();
        }

        _ = webApplication
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseAntiforgery();

        _ = webApplication.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        _ = webApplication.MapControllers();

        await webApplication.RunAsync();
    }
}

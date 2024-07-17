using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.BlazorWebApp.Server;

public class Program : Libs.Core.ProgramBase
{
    private static ILogger<Program> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<Program>();

    [STAThread]
    public static async Task Main(string[] args)
    {
        _ = await ObtainCommandLineAsync<Libs.Core.Models.Config.ConsoleOptions>(args);

        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

        _ = webApplicationBuilder.AddAllMyDependencies();

        _ = webApplicationBuilder.Configuration.AddInMemoryCollection(Libs.Core.Models.Config.RuntimeSettings.GetValues(Settings));

        WebApplication webApplication = webApplicationBuilder.Build();

        logger = webApplication.Services.GetRequiredService<ILogger<Program>>();

        string AppName = webApplication.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

        logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

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

        logger.LogInformation("End {ApplicationName}", AppName);
    }

    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.ExceptionObject.ToString());
        logger.LogError("Unhandled Exception: '{ExceptionObject}'", e.ExceptionObject.ToString());
        Environment.Exit(1);
    }
}

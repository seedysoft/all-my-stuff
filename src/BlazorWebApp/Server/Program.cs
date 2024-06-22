using MudBlazor.Services;
using Seedysoft.BlazorWebApp.Server.Extensions;
using Seedysoft.Libs.Infrastructure;

namespace Seedysoft.BlazorWebApp.Server;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            _ = webApplicationBuilder.Configuration.SetBasePath(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!);
        }

        // Add services to the container.
        _ = webApplicationBuilder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        _ = webApplicationBuilder.Services
            .AddSystemd()

            .AddMudServices()

            .AddHttpClient() // Needed for server rendering

            .AddControllers()
        ;

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        _ = webApplicationBuilder.Services
            .AddEndpointsApiExplorer()
            .AddOpenApiDocument()
        ;

        Dependencies.ConfigureDefaultDependencies(webApplicationBuilder, args);

        _ = webApplicationBuilder.AddMyDependencies();

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
            .UseAntiforgery()
        ;

        _ = webApplication.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        _ = webApplication.MapControllers();

        SQLitePCL.Batteries.Init();

        // Migrate and seed the database during startup. Must be synchronous.
        _ = webApplication.MigrateDbContexts();

        webApplication.Run();
    }
}

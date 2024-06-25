using MudBlazor.Services;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.BlazorWebApp.Server;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

        _ = webApplicationBuilder.AddAllMyDependencies();

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

        // TODO         Migrate and seed the database during startup. Must be synchronous.
        //_ = webApplication.MigrateDbContexts();

        webApplication.Run();
    }
}

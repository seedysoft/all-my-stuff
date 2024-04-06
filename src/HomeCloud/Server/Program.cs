using Seedysoft.HomeCloud.Server.Extensions;

namespace Seedysoft.HomeCloud.Server;

public sealed class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        if (System.Diagnostics.Debugger.IsAttached)
            _ = builder.Configuration.SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!);

        InfrastructureLib.Dependencies.ConfigureDefaultDependencies(builder, args);

        Microsoft.AspNetCore.Hosting.StaticWebAssets.StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        _ = builder.Logging.AddSystemdConsole();

        _ = builder.Services.AddControllersWithViews();
        _ = builder.Services.AddRazorPages();

        _ = builder.Host.ConfigureDefaults(args);
        // Systemd Service must be configured as type=notify
        _ = builder.Host.UseSystemd();

        // Extension method
        _ = builder.AddMyDependencies();

        WebApplication webApp = builder.Build();

        //ILogger<Program> logger = webApp.Services.GetRequiredService<ILogger<Program>>();

        // Configure the HTTP request pipeline.
        if (webApp.Environment.IsDevelopment())
        {
            webApp.UseWebAssemblyDebugging();
        }
        else
        {
            _ = webApp.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = webApp.UseHsts();
        }

        //webApp.UseHttpsRedirection();

        _ = webApp.UseBlazorFrameworkFiles();
        _ = webApp.UseStaticFiles();

        _ = webApp.UseRouting();

        _ = webApp.MapRazorPages();
        _ = webApp.MapControllers();
        _ = webApp.MapFallbackToFile("index.html");

        SQLitePCL.Batteries.Init();

        // Migrate and seed the database during startup. Must be synchronous.
        _ = webApp.MigrateDbContexts();

        webApp.Run();
    }
}

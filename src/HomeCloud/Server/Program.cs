using Serilog;

namespace Seedysoft.HomeCloud.Server;

internal class Program
{
    private const string ApplicationName = $"{nameof(Seedysoft)}.{nameof(HomeCloud)}.{nameof(Server)}.{nameof(Program)}";

    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        Microsoft.AspNetCore.Hosting.StaticWebAssets.StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        // Add services to the container.

#if DEBUG
        builder.Configuration.SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!);

        builder.Logging.AddConsole();
#endif

        builder.Logging.AddSystemdConsole();

        _ = builder.Host.ConfigureDefaults(args);
        // Systemd Service must be configured as type=notify
        _ = builder.Host.UseSystemd();

        string CurrentEnvironmentName = builder.Environment.EnvironmentName;

        builder.Configuration
            .AddJsonFile($"appsettings.HomeCloudServer.json", false, true)
            .AddJsonFile($"appsettings.HomeCloudServer.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.Serilog.{CurrentEnvironmentName}.json", false, true);

        InfrastructureLib.Dependencies.ConfigureServices(builder.Configuration, builder.Services);
        Carburantes.Infrastructure.Dependencies.ConfigureServices(builder.Configuration, builder.Services);

        IConfigurationSection configurationSection = builder.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");
        configurationSection.Value = Path.GetFullPath(configurationSection.Value!.Replace("{ApplicationName}", ApplicationName));

        builder.Logging.AddSerilog(new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger());

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddSystemd();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            _ = app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}

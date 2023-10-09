#if DEBUG
using Microsoft.Extensions.Logging;
#endif

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Seedysoft.InfrastructureLib;

public static class Dependencies
{
    public static void ConfigureDefaultDependencies(IHostBuilder builder, string[] args)
    {
        _ = builder.ConfigureHostConfiguration(iConfigurationBuilder =>
        {
            _ = iConfigurationBuilder
                .AddCommandLine(args)
                .AddEnvironmentVariables();
        });

        _ = builder.ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
            AddSerilogJsonFile(iConfigurationBuilder, hostBuilderContext.HostingEnvironment));

        _ = builder.ConfigureServices((hostBuilderContext, iServiceCollection) =>
            AddSerilogLogging(hostBuilderContext.Configuration, iServiceCollection, hostBuilderContext.HostingEnvironment));
    }

    public static void ConfigureDefaultDependencies(IConfigurationBuilder iConfigurationBuilder, IServiceCollection services, IHostEnvironment environment)
    {
        AddSerilogJsonFile(iConfigurationBuilder, environment);
        AddSerilogLogging((IConfiguration)iConfigurationBuilder, services, environment);
    }

    public static void AddDbContext<T>(IConfiguration configuration, IServiceCollection services) where T : DbContext
    {
        services.AddDbContext<T>(dbContextOptionsBuilder =>
        {
            string ConnectionStringName = typeof(T).Name;
            string ConnectionString = configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException($"Database file '{FullFilePath}' not found.");

            dbContextOptionsBuilder.UseSqlite(ConnectionString);
#if DEBUG
            dbContextOptionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Trace);
#endif
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient);

        SQLitePCL.Batteries.Init();
    }

    private static void AddSerilogJsonFile(IConfigurationBuilder configuration, IHostEnvironment environment) =>
        _ = configuration.AddJsonFile($"appsettings.Serilog.{environment.EnvironmentName}.json", false, true);

    private static void AddSerilogLogging(IConfiguration configuration, IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        _ = services.AddLogging(iLoggingBuilder =>
        {
            IConfigurationSection configurationSection = configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");
            configurationSection.Value = Path.GetFullPath(configurationSection.Value!.Replace("{ApplicationName}", hostEnvironment.ApplicationName));

            _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger());
        });
    }
}

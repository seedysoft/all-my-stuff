using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Seedysoft.InfrastructureLib;

public static class Dependencies
{
    public static void ConfigureDefaultDependencies(IHostBuilder builder, string[] args)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            _ = builder.ConfigureLogging((hostBuilderContext, iLoggingBuilder) => iLoggingBuilder.AddConsole());

        _ = builder.ConfigureHostConfiguration(iConfigurationBuilder =>
        {
            _ = iConfigurationBuilder
                .AddCommandLine(args)
                .AddEnvironmentVariables();
        });

        _ = builder.ConfigureAppConfiguration((hostBuilderContext, iConfigurationBuilder) =>
        {
            _ = iConfigurationBuilder
                .AddJsonFile($"appsettings.Serilog.json", false, true)
                .AddJsonFile($"appsettings.Serilog.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", false, true);
        });

        _ = builder.ConfigureServices((hostBuilderContext, iServiceCollection) =>
        {
            _ = iServiceCollection.AddLogging(iLoggingBuilder =>
            {
                IConfigurationSection configurationSection = hostBuilderContext.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");
                Console.WriteLine("Obtained '{0}' from Serilog:WriteTo:1:Args:path", configurationSection.Value);

                configurationSection.Value = Path.GetFullPath(configurationSection.Value!.Replace("{ApplicationName}", hostBuilderContext.HostingEnvironment.ApplicationName));
                Console.WriteLine("Final value of Serilog:WriteTo:1:Args:path: '{0}'", configurationSection.Value);

                _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(hostBuilderContext.Configuration)
                    .CreateLogger());
            });
        });
    }

    public static void AddDbCxtContext(IConfiguration configuration, IServiceCollection services)
    {
        _ = services.AddDbContext<DbContexts.DbCxt>(dbContextOptionsBuilder =>
        {
            string ConnectionStringName = nameof(DbContexts.DbCxt);
            string ConnectionString = configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found: '{FullFilePath}'", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite(ConnectionString);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                _ = dbContextOptionsBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Trace);
            }
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient);

        SQLitePCL.Batteries.Init();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.InfrastructureLib.Extensions;
using Serilog;

namespace Seedysoft.InfrastructureLib;

public static class Dependencies
{
    public static void ConfigureDefaultDependencies(IHostApplicationBuilder builder, string[] args)
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            _ = builder.Logging.AddConsole();
            _ = builder.Configuration.SetBasePath(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!);
        }

        _ = builder.Configuration
            .AddCommandLine(args)
            .AddEnvironmentVariables();

        _ = builder.Configuration
            .AddJsonFile($"appsettings.Serilog.json", false, true)
            .AddJsonFile($"appsettings.Serilog.{builder.Environment.EnvironmentName}.json", false, true);

        _ = builder.Services
            .AddLogging(iLoggingBuilder =>
            {
                IConfigurationSection configurationSection = builder.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");
                Console.WriteLine("Obtained '{0}' from Serilog:WriteTo:1:Args:path", configurationSection.Value);

                configurationSection.Value = Path.GetFullPath(
                    configurationSection.Value!.Replace("{ApplicationName}", builder.Environment.ApplicationName),
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!Path.Exists(Path.GetDirectoryName(configurationSection.Value)))
                    throw new FileNotFoundException("Log folder not found.", configurationSection.Value);

                _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .CreateLogger());
            });
    }

    public static void AddDbCxtContext(IHostApplicationBuilder builder)
    {
        _ = builder.Configuration.AddJsonFile($"appsettings.dbConnectionString.{builder.Environment.EnvironmentName}.json", false, true);

        _ = builder.Services.AddDbContext<DbContexts.DbCxt>(dbContextOptionsBuilder =>
        {
            string ConnectionStringName = nameof(DbContexts.DbCxt);
            string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString, System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite($"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient);

        SQLitePCL.Batteries.Init();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seedysoft.InfrastructureLib.Extensions;
using Seedysoft.UtilsLib.Constants;
using Serilog;

namespace Seedysoft.InfrastructureLib.Dependencies;

internal sealed class Configurator : UtilsLib.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.dbConnectionString.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.Serilog.json", false, true)
            .AddJsonFile($"appsettings.Serilog.{CurrentEnvironmentName}.json", false, true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services.AddDbContext<DbContexts.DbCxt>(dbContextOptionsBuilder =>
        {
            string ConnectionStringName = nameof(DbContexts.DbCxt);
            string ConnectionString = hostApplicationBuilder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString, System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = dbContextOptionsBuilder.UseSqlite($"{DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        }
        , ServiceLifetime.Transient
        , ServiceLifetime.Transient);

        SQLitePCL.Batteries.Init();
    }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services
            .AddLogging(iLoggingBuilder =>
            {
                IConfigurationSection configurationSection = hostApplicationBuilder.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");
                Console.WriteLine("Obtained '{0}' from Serilog:WriteTo:1:Args:path", configurationSection.Value);

                configurationSection.Value = Path.GetFullPath(
                    configurationSection.Value!.Replace("{ApplicationName}", hostApplicationBuilder.Environment.ApplicationName),
                    System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (!Path.Exists(Path.GetDirectoryName(configurationSection.Value)))
                    throw new FileNotFoundException("Log folder not found.", configurationSection.Value);

                _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(hostApplicationBuilder.Configuration)
                    .CreateLogger());
            });
    }
}

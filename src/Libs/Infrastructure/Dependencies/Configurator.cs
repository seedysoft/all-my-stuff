using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Infrastructure.Extensions;
using Serilog;

namespace Seedysoft.Libs.Infrastructure.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.dbConnectionString.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Serilog)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Serilog)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services.AddDbContext<DbContexts.DbCxt>(dbContextOptionsBuilder =>
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
            });
            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(AddDbContexts));

            string ConnectionStringName = nameof(DbContexts.DbCxt);
            string ConnectionString = hostApplicationBuilder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
            string FullFilePath = Path.GetFullPath(ConnectionString);
            while (!File.Exists(FullFilePath))
            {
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
#endif
                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("Database file {FullFilePath} does not exists", FullFilePath);
                FullFilePath = Path.GetFullPath(ConnectionString = ConnectionString.Insert(0, "../"));
            }

            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Using database file {FullFilePath}", FullFilePath);
            _ = dbContextOptionsBuilder.UseSqlite($"{Core.Constants.DatabaseStrings.DataSource}{FullFilePath}");
            dbContextOptionsBuilder.ConfigureDebugOptions();
        },
        ServiceLifetime.Transient,
        ServiceLifetime.Transient);

        SQLitePCL.Batteries.Init();
    }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.Services
            .AddLogging(iLoggingBuilder =>
            {
                IConfigurationSection configurationSection =
                    hostApplicationBuilder.Configuration.GetRequiredSection("Serilog:WriteTo:1:Args:path");

                configurationSection.Value =
                    Path.GetFullPath(configurationSection.Value!.Replace("{ApplicationName}", hostApplicationBuilder.Environment.ApplicationName));

                _ = iLoggingBuilder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(hostApplicationBuilder.Configuration)
                    .CreateLogger());
            });
    }
}

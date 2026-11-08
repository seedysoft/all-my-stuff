using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Photo.Lib.Dependencies;

public sealed class Configurator : Libs.Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.photoConnectionString.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.PhotoSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.PhotoSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(IHostApplicationBuilder hostApplicationBuilder)
    {
#if DEBUG
        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();
#endif

        string PhotoCtxFullFilePath = default!;
        _ = hostApplicationBuilder.Services.AddDbContext<DbContexts.PhotoCtx>(dbContextOptionsBuilder =>
        {
            if (string.IsNullOrEmpty(PhotoCtxFullFilePath))
                PhotoCtxFullFilePath = FindDatabaseFullFilePath(hostApplicationBuilder);

            _ = dbContextOptionsBuilder
                .UseSqlite($"{Libs.Core.Constants.DatabaseStrings.DataSource}{PhotoCtxFullFilePath}")
                .ConfigureDebugOptions();

            static string FindDatabaseFullFilePath(IHostApplicationBuilder hostApplicationBuilder)
            {
                using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    _ = builder
                        .SetMinimumLevel(LogLevel.Debug)
                        .AddConsole();
                });
                Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger(nameof(AddDbContexts));

                string ConnectionStringName = nameof(DbContexts.PhotoCtx);
                string ConnectionString = hostApplicationBuilder.Configuration.GetConnectionString($"{ConnectionStringName}")
                    ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                string FullFilePath = Path.GetFullPath(ConnectionString);

                while (!File.Exists(FullFilePath))
                {
                    if (logger.IsEnabled(LogLevel.Debug))
                        logger.LogDebug("Database file {FullFilePath} does not exists", FullFilePath);

                    FullFilePath = Path.GetFullPath(ConnectionString = ConnectionString.Insert(0, "../"));
                }

                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found.", FullFilePath);

                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Using database file {FullFilePath}", FullFilePath);

                return FullFilePath;
            }
        },
        ServiceLifetime.Transient,
        ServiceLifetime.Transient);

        SQLitePCL.Batteries.Init();
    }

    protected override void AddMyServices(IHostApplicationBuilder hostApplicationBuilder)
        => hostApplicationBuilder.Services.TryAddSingleton<Services.PhotoCronBackgroundService>();
}

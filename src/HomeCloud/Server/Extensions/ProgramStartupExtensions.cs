using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seedysoft.InfrastructureLib.Extensions;

namespace Seedysoft.HomeCloud.Server.Extensions;

public static class ProgramStartupExtensions
{
    public static IHostApplicationBuilder AddMyDependencies(this IHostApplicationBuilder builder)
    {
        return builder
            .AddJsonFiles()
            .AddDbContexts()
            .AddMyServices();
    }

    public static IHost MigrateDbContexts(this IHost builder)
    {
        builder.Services.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>().Database.Migrate();
        builder.Services.GetRequiredService<Carburantes.Infrastructure.Data.CarburantesDbContext>().Database.Migrate();
        builder.Services.GetRequiredService<Carburantes.Infrastructure.Data.CarburantesHistDbContext>().Database.Migrate();

        return builder;
    }

    private static IHostApplicationBuilder AddJsonFiles(this IHostApplicationBuilder builder)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            builder.Environment.EnvironmentName = Environments.Development/*.Production*/;

        string CurrentEnvironmentName = builder.Environment.EnvironmentName;
        _ = builder.Configuration
            .AddJsonFile($"appsettings.HomeCloudServer.json", false, true)
            .AddJsonFile($"appsettings.HomeCloudServer.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.CarburantesConnectionStrings.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.Carburantes.Infrastructure.json", false, true)
            .AddJsonFile($"appsettings.Carburantes.Infrastructure.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.Serilog.json", false, true)
            .AddJsonFile($"appsettings.Serilog.{CurrentEnvironmentName}.json", false, true)

            .AddJsonFile($"appsettings.PvpcSettings.json", false, true)
            .AddJsonFile($"appsettings.SmtpServiceSettings.json", false, true)
            .AddJsonFile($"appsettings.TelegramSettings.{CurrentEnvironmentName}.json", false, true)
            .AddJsonFile($"appsettings.TuyaManagerSettings.json", false, true);

        return builder;
    }

    private static IHostApplicationBuilder AddDbContexts(this IHostApplicationBuilder builder)
    {
        InfrastructureLib.Dependencies.AddDbCxtContext(builder);

        _ = builder.Services
            .AddDbContext<Carburantes.Infrastructure.Data.CarburantesDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
            {
                string ConnectionStringName = nameof(Carburantes.Infrastructure.Data.CarburantesDbContext);
                string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                if (System.Diagnostics.Debugger.IsAttached)
                    ConnectionString = ConnectionString.Replace($"{CoreLib.Constants.DatabaseStrings.DataSource}../../../", $"{CoreLib.Constants.DatabaseStrings.DataSource}");
                string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found.", FullFilePath);

                _ = dbContextOptionsBuilder.UseSqlite($"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
                dbContextOptionsBuilder.ConfigureDebugOptions();
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient)

            .AddDbContext<Carburantes.Infrastructure.Data.CarburantesHistDbContext>((iServiceProvider, dbContextOptionsBuilder) =>
            {
                string ConnectionStringName = nameof(Carburantes.Infrastructure.Data.CarburantesHistDbContext);
                string ConnectionString = builder.Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                if (System.Diagnostics.Debugger.IsAttached)
                    ConnectionString = ConnectionString.Replace($"{CoreLib.Constants.DatabaseStrings.DataSource}../../../", $"{CoreLib.Constants.DatabaseStrings.DataSource}");
                string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found.", FullFilePath);

                _ = dbContextOptionsBuilder.UseSqlite($"{CoreLib.Constants.DatabaseStrings.DataSource}{FullFilePath}");
                dbContextOptionsBuilder.ConfigureDebugOptions();
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient);

        return builder;
    }

    private static IHostApplicationBuilder AddMyServices(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton(builder.Configuration.GetSection(nameof(SmtpServiceLib.Settings.SmtpServiceSettings)).Get<SmtpServiceLib.Settings.SmtpServiceSettings>()!);
        builder.Services.TryAddTransient<SmtpServiceLib.Services.SmtpService>();

        builder.Services.TryAddSingleton(builder.Configuration.GetSection(nameof(TelegramLib.Settings.TelegramSettings)).Get<TelegramLib.Settings.TelegramSettings>()!);
        builder.Services.TryAddSingleton<TelegramLib.Services.TelegramHostedService>();
        _ = builder.Services.AddHostedService<TelegramLib.Services.TelegramHostedService>();

        _ = builder.Services.AddHttpClient(nameof(Carburantes.Core.Settings.Minetur));
        _ = builder.Services.AddHostedService<Carburantes.Services.ObtainDataCronBackgroundService>();

        builder.Services.TryAddSingleton(builder.Configuration.GetSection(nameof(PvpcLib.Settings.PvpcSettings)).Get<PvpcLib.Settings.PvpcSettings>()!);
        builder.Services.TryAddSingleton(builder.Configuration.GetSection(nameof(PvpcLib.Settings.TuyaManagerSettings)).Get<PvpcLib.Settings.TuyaManagerSettings>()!);
        builder.Services.TryAddSingleton<PvpcLib.Services.PvpcCronBackgroundService>();
        _ = builder.Services.AddHostedService<PvpcLib.Services.PvpcCronBackgroundService>();
        builder.Services.TryAddSingleton<PvpcLib.Services.TuyaManagerCronBackgroundService>();
        _ = builder.Services.AddHostedService<PvpcLib.Services.TuyaManagerCronBackgroundService>();

        _ = builder.Services.AddHostedService<OutboxLib.Services.OutboxCronBackgroundService>();

        builder.Services.TryAddSingleton<WebComparerLib.Services.WebComparerHostedService>();
        _ = builder.Services.AddHostedService<WebComparerLib.Services.WebComparerHostedService>();

        return builder;
    }
}

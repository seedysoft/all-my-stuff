using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#if DEBUG
using Microsoft.Extensions.Logging;
#endif

namespace Seedysoft.Carburantes.Infrastructure;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services
            .AddDbContext<Data.CarburantesDbContext>(dbContextOptionsBuilder =>
            {
                const string ConnectionStringName = nameof(Data.CarburantesDbContext);
                string ConnectionString = configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found: '{FullPath}'", FullFilePath);

                dbContextOptionsBuilder.UseSqlite(ConnectionString);
#if DEBUG
                dbContextOptionsBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Trace);
#endif
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient)
            .AddDbContext<Data.CarburantesHistDbContext>(dbContextOptionsBuilder =>
            {
                const string ConnectionStringName = nameof(Data.CarburantesHistDbContext);
                string ConnectionString = configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
                string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
                if (!File.Exists(FullFilePath))
                    throw new FileNotFoundException("Database file not found: '{FullPath}'", FullFilePath);

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
}

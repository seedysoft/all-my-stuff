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
                dbContextOptionsBuilder.UseSqlite(configuration.GetConnectionString($"{nameof(Data.CarburantesDbContext)}")
                    ?? throw new InvalidOperationException($"Connection string '{nameof(Data.CarburantesDbContext)}' not found."));
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
                dbContextOptionsBuilder.UseSqlite(configuration.GetConnectionString($"{nameof(Data.CarburantesHistDbContext)}")
                    ?? throw new InvalidOperationException($"Connection string '{nameof(Data.CarburantesHistDbContext)}' not found."));
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

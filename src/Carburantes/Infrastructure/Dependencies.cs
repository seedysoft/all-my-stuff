using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#if DEBUG
using Microsoft.Extensions.Logging;
using Seedysoft.Carburantes.Infrastructure.Data;
#endif

namespace Seedysoft.Carburantes.Infrastructure;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services
            .AddDbContext<CarburantesDbContext>(dbContextOptionsBuilder =>
            {
                dbContextOptionsBuilder.UseSqlite(configuration.GetConnectionString($"{nameof(CarburantesDbContext)}")
                    ?? throw new InvalidOperationException($"Connection string '{nameof(CarburantesDbContext)}' not found."));
#if DEBUG
                dbContextOptionsBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Trace);
#endif
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient)
            .AddDbContext<CarburantesHistDbContext>(dbContextOptionsBuilder =>
            {
                dbContextOptionsBuilder.UseSqlite(configuration.GetConnectionString($"{nameof(CarburantesHistDbContext)}")
                    ?? throw new InvalidOperationException($"Connection string '{nameof(CarburantesHistDbContext)}' not found."));
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure.Tests;

public abstract class TestClassBase : Libs.Tests.TUnitTestClassBase
{
    public static IServiceCollection AddDbContext(IServiceCollection services)
    {
        Microsoft.Data.Sqlite.SqliteConnection connection = new("Filename=:memory:");
        connection.Open();

        using (DbContexts.DbCtx context = new(new DbContextOptionsBuilder<DbContexts.DbCtx>().UseSqlite(connection).Options))
            _ = context.Database.EnsureCreated();

        _ = services
            .AddDbContext<DbContexts.DbCtx>((dbContextOptionsBuilder) =>
            {
                _ = dbContextOptionsBuilder
                    .UseSqlite(connection)
                    .ConfigureDebugOptions();
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient);

        return services;
    }
}

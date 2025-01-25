using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure.Tests;

public abstract class TestClassBase
{
    protected TestClassBase()
    {
        string NewEnvironment =
#if DEBUG
            "Development"
#else
            "Production"
#endif
        ;
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", NewEnvironment, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", NewEnvironment, EnvironmentVariableTarget.Process);
    }

    public static void AddDbContext(IServiceCollection services)
    {
        Microsoft.Data.Sqlite.SqliteConnection connection = new("Filename=:memory:");
        connection.Open();

        using (DbContexts.DbCxt context = new(new DbContextOptionsBuilder<DbContexts.DbCxt>().UseSqlite(connection).Options))
            _ = context.Database.EnsureCreated();

        _ = services
            .AddDbContext<DbContexts.DbCxt>((dbContextOptionsBuilder) =>
            {
                _ = dbContextOptionsBuilder.UseSqlite(connection);
                dbContextOptionsBuilder.ConfigureDebugOptions();
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure.Tests;

public abstract class TestClassBase(Xunit.Abstractions.ITestOutputHelper testOutputHelper) : Core.Tests.XUnitTestClassBase(testOutputHelper)
{
    public static void AddDbContext(IServiceCollection services)
    {
        Microsoft.Data.Sqlite.SqliteConnection connection = new("Filename=:memory:");
        connection.Open();

        using (DbContexts.DbCxt context = new(new DbContextOptionsBuilder<DbContexts.DbCxt>().UseSqlite(connection).Options))
            _ = context.Database.EnsureCreated();

        _ = services
            .AddDbContext<DbContexts.DbCxt>((dbContextOptionsBuilder) =>
            {
                _ = dbContextOptionsBuilder
                    .UseSqlite(connection)
                    .ConfigureDebugOptions();
            }
            , ServiceLifetime.Transient
            , ServiceLifetime.Transient);
    }
}

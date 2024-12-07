using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.Tests;

public abstract class TestClassBase
{
    private static Microsoft.Data.Sqlite.SqliteConnection SqliteConnection => new("Filename=:memory:");
    private static DbContextOptions<DbContexts.DbCxt> Options { get; set; } = default!;

    protected static DbContexts.DbCxt GetDbCxt() => new(Options);

    [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
    public static void BaseClassInitialize(TestContext context)
    {
        SqliteConnection.Open();

        Options = new DbContextOptionsBuilder<DbContexts.DbCxt>().UseSqlite(SqliteConnection).Options;

        if (System.Diagnostics.Debugger.IsAttached)
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        // Gives error... Maybe not necessary
        //using DbContexts.DbCxt dbCxt = GetDbCxt();
        //dbCxt.Database.Migrate();
    }

    [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass, ClassCleanupBehavior.EndOfClass)]
    public static void BaseClassCleanup() => SqliteConnection?.Dispose();
}

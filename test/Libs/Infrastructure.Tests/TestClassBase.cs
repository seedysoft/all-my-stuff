using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Seedysoft.Libs.Infrastructure.Tests;

public abstract class TestClassBase
{
    private static Microsoft.Data.Sqlite.SqliteConnection SqliteConnection { get; set; } = default!;
    private static DbContextOptions<DbContexts.DbCxt> Options { get; set; } = default!;

    protected static DbContexts.DbCxt GetDbCxt() => new(Options);

    [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
    public static void ClassInitialize(TestContext context)
    {
        SqliteConnection = new("Filename=:memory:");
        SqliteConnection.Open();

        Options = new DbContextOptionsBuilder<DbContexts.DbCxt>().UseSqlite(SqliteConnection).Options;

        using DbContexts.DbCxt dbCxt = GetDbCxt();
        dbCxt.Database.Migrate();
    }

    [ClassCleanup(InheritanceBehavior.BeforeEachDerivedClass)]
    public static void ClassCleanup() => SqliteConnection?.Dispose();
}

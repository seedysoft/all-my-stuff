using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.Test;

public abstract class BaseFixture : IDisposable
{
    private readonly Microsoft.Data.Sqlite.SqliteConnection sqliteConnection;
    private readonly DbContextOptions<DbContexts.DbCxt> options;

    protected BaseFixture()
    {
        sqliteConnection = new("Filename=:memory:");
        sqliteConnection.Open();

        options = new DbContextOptionsBuilder<DbContexts.DbCxt>().UseSqlite(sqliteConnection).Options;

        using DbContexts.DbCxt dbCxt = new(options);
        _ = dbCxt.Database.EnsureCreated();
    }

    protected DbContexts.DbCxt GetDbCxt() => new(options);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        sqliteConnection?.Dispose();
    }
}

using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.Test;

public abstract class BaseFixture : IDisposable
{
    private readonly Microsoft.Data.Sqlite.SqliteConnection sqliteConnection;
    private readonly DbContextOptions<InfrastructureLib.DbContexts.DbCxt> options;

    protected BaseFixture()
    {
        sqliteConnection = new("Filename=:memory:");
        sqliteConnection.Open();

        options = new DbContextOptionsBuilder<InfrastructureLib.DbContexts.DbCxt>().UseSqlite(sqliteConnection).Options;

        using InfrastructureLib.DbContexts.DbCxt dbCxt = new(options);
        _ = dbCxt.Database.EnsureCreated();
    }

    protected InfrastructureLib.DbContexts.DbCxt GetDbCxt() => new(options);

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        sqliteConnection?.Dispose();
    }
}

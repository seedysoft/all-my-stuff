using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.Tests;

public abstract class TestClassBase : IDisposable
{
    private bool disposedValue;

    private static Microsoft.Data.Sqlite.SqliteConnection SqliteConnection => new("Filename=:memory:");
    private DbContextOptions<DbContexts.DbCxt> Options { get; set; } = default!;

    protected TestClassBase()
    {
        SqliteConnection.Open();

        Options = new DbContextOptionsBuilder<DbContexts.DbCxt>().UseSqlite(SqliteConnection).Options;

        if (System.Diagnostics.Debugger.IsAttached)
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        // Throws error... Maybe not necessary
        //using DbContexts.DbCxt dbCxt = GetDbCxt();
        //dbCxt.Database.Migrate();
    }

    protected DbContexts.DbCxt GetDbCxt() => new(Options);

    protected abstract void Dispose(bool disposing);

    // // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~TestClassBase()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        if (!disposedValue)
        {
            // dispose managed state (managed objects)
            SqliteConnection?.Dispose();

            // free unmanaged resources (unmanaged objects) and override finalizer

            // set large fields to null

            disposedValue = true;
        }

        GC.SuppressFinalize(this);
    }
}

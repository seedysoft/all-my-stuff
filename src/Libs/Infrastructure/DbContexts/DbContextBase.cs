namespace Seedysoft.Libs.Infrastructure.DbContexts;

public abstract class DbContextBase : Microsoft.EntityFrameworkCore.DbContext
{
#if DEBUG
    public DbContextBase() : base() => ChangeTracker.LazyLoadingEnabled = false;
#endif
    public DbContextBase(Microsoft.EntityFrameworkCore.DbContextOptions dbContextOptions) : base(dbContextOptions) => ChangeTracker.LazyLoadingEnabled = false;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int Retries = 2;
        while (Retries > 0)
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.InnerException is Microsoft.Data.Sqlite.SqliteException se && se.SqliteErrorCode == 5)
            {
                Retries--;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

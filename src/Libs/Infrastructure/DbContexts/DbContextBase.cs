using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public class DbContextBase : DbContext
{
#if DEBUG
    public DbContextBase() : base() => ChangeTracker?.LazyLoadingEnabled = false;
#endif

    public DbContextBase(DbContextOptions options) : base(options) => ChangeTracker?.LazyLoadingEnabled = false;

#if DEBUG
    protected static void ConfigureDb(DbContextOptionsBuilder optionsBuilder, string databasePath)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string FullFilePath = Path.GetFullPath(databasePath);

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            while (!File.Exists(FullFilePath))
                FullFilePath = Path.GetFullPath(databasePath = databasePath.Insert(0, "../"));

            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{Core.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder
                .UseSqlite(ConnectionString)
                .ConfigureDebugOptions();
        }
    }
#endif

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int Retries = 2;

        while (Retries > 0)
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is Microsoft.Data.Sqlite.SqliteException se && se.SqliteErrorCode == 5)
            {
                Retries--;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

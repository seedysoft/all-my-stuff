using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public class DbContextBase : DbContext
{
    public DbContextBase() : base() => ChangeTracker?.LazyLoadingEnabled = false;
    public DbContextBase(DbContextOptions options) : base(options) => ChangeTracker?.LazyLoadingEnabled = false;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

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

#if DEBUG
    public static DbContextOptionsBuilder<T> ConfigureDb<T>(DbContextOptionsBuilder<T>? optionsBuilder, string databasePath) where T : DbContextBase
    {
        optionsBuilder ??= new();

        if (!optionsBuilder.IsConfigured)
        {
            string FullFilePath = Path.GetFullPath(databasePath);

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            while (!File.Exists(FullFilePath))
            {
                FullFilePath = Path.GetFullPath(databasePath = databasePath.Insert(0, "../"));
                Console.Write("{0,-200}", $"\rTry to find '{FullFilePath}'");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{Core.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder
                .UseSqlite(ConnectionString)
                .ConfigureDebugOptions();
        }

        return optionsBuilder;
    }
#endif
}

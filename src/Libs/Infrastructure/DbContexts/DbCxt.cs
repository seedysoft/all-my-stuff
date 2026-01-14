using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public sealed partial class DbCxt : DbContext
{
#if DEBUG
    public DbCxt() : base() => ChangeTracker.LazyLoadingEnabled = false;
#endif
    public DbCxt(DbContextOptions<DbCxt> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            string DatabasePath = "../databases/db.sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            while (!File.Exists(FullFilePath))
                FullFilePath = Path.GetFullPath(DatabasePath = DatabasePath.Insert(0, "../"));

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Core.Entities.Outbox> Outbox { get; set; } = default!;

    public DbSet<Core.Entities.Pvpc> Pvpcs { get; set; } = default!;

    public DbSet<Core.Entities.Subscriber> Subscribers { get; set; } = default!;
    public DbSet<Core.Entities.Subscription> Subscriptions { get; set; } = default!;
    public DbSet<Core.Entities.SubcriptionDataView> SubcriptionsDataView { get; set; } = default!;

    public DbSet<Core.Entities.TuyaDevice> TuyaDevices { get; set; } = default!;

    public DbSet<Core.Entities.WebData> WebDatas { get; set; } = default!;
}

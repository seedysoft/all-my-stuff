using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Constants;
using Seedysoft.Libs.Core.Entities;

namespace Seedysoft.InfrastructureLib.DbContexts;

public sealed partial class DbCxt : DbContext
{
#if DEBUG
    public DbCxt() : base() => ChangeTracker.LazyLoadingEnabled = false;
#endif
    public DbCxt(DbContextOptions<DbCxt> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string DatabasePath = "../../../../../../../databases/db.sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            string ConnectionString = $"{DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }

    public DbSet<Outbox> Outbox { get; set; } = default!;

    public DbSet<Pvpc> Pvpcs { get; set; } = default!;

    public DbSet<Subscriber> Subscribers { get; set; } = default!;
    public DbSet<Subscription> Subscriptions { get; set; } = default!;
    public DbSet<SubcriptionDataView> SubcriptionsDataView { get; set; } = default!;

    public DbSet<TuyaDevice> TuyaDevices { get; set; } = default!;

    public DbSet<WebData> WebDatas { get; set; } = default!;
}

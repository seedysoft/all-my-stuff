using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public sealed partial class DbCxt : DbContextBase
{
#if DEBUG
    public DbCxt() : base() { }
#endif
    public DbCxt(DbContextOptions<DbCxt> options) : base(options) { }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
#endif
            string DatabasePath = "../databases/db.sqlite3";
            string FullFilePath = Path.GetFullPath(DatabasePath);
            while (!File.Exists(FullFilePath))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                FullFilePath = Path.GetFullPath(DatabasePath = DatabasePath.Insert(0, "../"));
            }

            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException($"Database file for {nameof(DbCxt)} not found.", FullFilePath);

            string ConnectionString = $"{Core.Constants.DatabaseStrings.DataSource}{FullFilePath}";
            Console.WriteLine(ConnectionString);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly, t => t.FullName?.Contains(nameof(EntityTypeConfigurations.DbCtx)) ?? false);
    }

    public DbSet<Core.Entities.Outbox> Outbox { get; set; } = default!;

    public DbSet<Core.Entities.Pvpc> Pvpcs { get; set; } = default!;

    public DbSet<Core.Entities.Subscriber> Subscribers { get; set; } = default!;
    public DbSet<Core.Entities.Subscription> Subscriptions { get; set; } = default!;
    public DbSet<Core.Entities.SubcriptionDataView> SubcriptionsDataView { get; set; } = default!;

    public DbSet<Core.Entities.TuyaDevice> TuyaDevices { get; set; } = default!;

    public DbSet<Core.Entities.WebData> WebDatas { get; set; } = default!;
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Seedysoft.InfrastructureLib.DbContexts;

// TODO                     Use same fieldId across database tables
public sealed partial class DbCxt : DbContext
{
#if DEBUG
    public DbCxt() : base() { }
#endif
    public DbCxt(DbContextOptions<DbCxt> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string ConnectionString = $"{CoreLib.Constants.DatabaseStrings.DataSource}../../../../databases/db.sqlite3";
            Console.WriteLine(ConnectionString);
            string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
            if (!File.Exists(FullFilePath))
                throw new FileNotFoundException("Database file not found.", FullFilePath);

            _ = optionsBuilder.UseSqlite(ConnectionString);
        }
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }

    public DbSet<CoreLib.Entities.Outbox> Outbox { get; set; } = default!;
    public DbSet<CoreLib.Entities.OutboxView> OutboxView { get; set; } = default!;

    public DbSet<CoreLib.Entities.Pvpc> Pvpcs { get; set; } = default!;
    public DbSet<CoreLib.Entities.PvpcView> PvpcsView { get; set; } = default!;

    public DbSet<CoreLib.Entities.Subscriber> Subscribers { get; set; } = default!;
    public DbSet<CoreLib.Entities.Subscription> Subscriptions { get; set; } = default!;
    public DbSet<CoreLib.Entities.SubcriptionDataView> SubcriptionsDataView { get; set; } = default!;

    public DbSet<CoreLib.Entities.WebData> WebDatas { get; set; } = default!;
    public DbSet<CoreLib.Entities.WebDataView> WebDatasView { get; set; } = default!;
}

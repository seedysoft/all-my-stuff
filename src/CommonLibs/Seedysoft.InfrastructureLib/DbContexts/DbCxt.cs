using Microsoft.EntityFrameworkCore;
#if DEBUG
using Microsoft.Extensions.Configuration;
#endif

namespace Seedysoft.InfrastructureLib.DbContexts;

// TODO                     Use same fieldId across database tables
public sealed partial class DbCxt : DbContext
{
    public DbCxt(DbContextOptions<DbCxt> options) : base(options) => ChangeTracker.LazyLoadingEnabled = false;

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

#if DEBUG
/// <summary>
/// Con esta clase podemos generar las migraciones
/// </summary>
public class DbCxtFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<DbCxt>
{
    public DbCxtFactory(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public DbCxt CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<DbCxt>();

        const string ConnectionStringName = nameof(DbCxt);
        string ConnectionString = Configuration.GetConnectionString($"{ConnectionStringName}") ?? throw new KeyNotFoundException($"Connection string '{ConnectionStringName}' not found.");
        string FullFilePath = Path.GetFullPath(ConnectionString[CoreLib.Constants.DatabaseStrings.DataSource.Length..]);
        if (!File.Exists(FullFilePath))
            throw new FileNotFoundException("Database file not found: '{FullFilePath}'", FullFilePath);

        _ = builder.UseSqlite(ConnectionString);

        return new DbCxt(builder.Options);
    }
}
#endif

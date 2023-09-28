using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DbContexts;

// TODO                     Use same fieldId across database tables
public sealed partial class DbCxt : DbContext
{
    public DbCxt(DbContextOptions<DbCxt> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }

#if DEBUG
    public override void Dispose() => base.Dispose();
    public override ValueTask DisposeAsync() => base.DisposeAsync();
#endif

    public DbSet<Seedysoft.CoreLib.Entities.Outbox> Outbox { get; set; } = default!;
    public DbSet<Seedysoft.CoreLib.Entities.OutboxView> OutboxView { get; set; } = default!;

    public DbSet<Seedysoft.CoreLib.Entities.Pvpc> Pvpcs { get; set; } = default!;
    public DbSet<Seedysoft.CoreLib.Entities.PvpcView> PvpcsView { get; set; } = default!;

    public DbSet<Seedysoft.CoreLib.Entities.Subscriber> Subscribers { get; set; } = default!;
    public DbSet<Seedysoft.CoreLib.Entities.Subscription> Subscriptions { get; set; } = default!;
    public DbSet<Seedysoft.CoreLib.Entities.SubcriptionDataView> SubcriptionsDataView { get; set; } = default!;

    public DbSet<Entities.WebData> WebDatas { get; set; } = default!;
    public DbSet<Entities.WebDataView> WebDatasView { get; set; } = default!;
}

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
        string FullFilePath = Path.GetFullPath(ConnectionString["Data Source=".Length..]);
        if (!File.Exists(FullFilePath))
            throw new FileNotFoundException("Database file not found: '{FullPath}'", FullFilePath);

        _ = builder.UseSqlite(ConnectionString);

        return new DbCxt(builder.Options);
    }
}

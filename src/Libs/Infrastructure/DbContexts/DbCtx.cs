using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public partial class DbCtx : DbContextBase
{
#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        ConfigureDb(optionsBuilder, "../databases/db.sqlite3");
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

    public DbSet<Core.Entities.TuyaDevice> TuyaDevices { get; set; } = default!;

    public DbSet<Core.Entities.WebData> WebDatas { get; set; } = default!;
}

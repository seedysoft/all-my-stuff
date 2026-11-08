using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public partial class DbCtx : DbContextBase
{
    public DbCtx() : base() { }
    public DbCtx(DbContextOptions options) : base(options) { }

    public DbSet<Core.Entities.Outbox> Outbox { get; set; } = default!;

    public DbSet<Core.Entities.Pvpc> Pvpcs { get; set; } = default!;

    public DbSet<Core.Entities.Subscriber> Subscribers { get; set; } = default!;
    public DbSet<Core.Entities.Subscription> Subscriptions { get; set; } = default!;

    public DbSet<Core.Entities.TuyaDevice> TuyaDevices { get; set; } = default!;

    public DbSet<Core.Entities.WebData> WebDatas { get; set; } = default!;
}

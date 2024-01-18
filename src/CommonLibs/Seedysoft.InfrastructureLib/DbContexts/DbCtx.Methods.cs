using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.DbContexts;

public sealed partial class DbCxt
{
    public async Task<CoreLib.Entities.Subscriber?> GetSubscriberWithSubscriptionsAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        return await Subscribers
            .Include(x => x.Subscriptions)
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId, cancellationToken);
    }

    // TODO         Implementar reintento cuando SqliteException (0x80004005): SQLite Error 5: 'database is locked'.
    public override int SaveChanges()
        => base.SaveChanges();
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
        => base.SaveChanges(acceptAllChangesOnSuccess);
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}

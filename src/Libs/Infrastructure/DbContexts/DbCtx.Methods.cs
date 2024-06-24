using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Entities;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public sealed partial class DbCxt
{
    public async Task<Subscriber?> GetSubscriberWithSubscriptionsAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        return await Subscribers
            .Include(x => x.Subscriptions)
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId, cancellationToken);
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
}

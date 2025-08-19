using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.DbContexts;

public sealed partial class DbCxt
{
    public async Task<Core.Entities.Subscriber?> GetSubscriberWithSubscriptionsAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        return await Subscribers
            .Include(x => x.Subscriptions)
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId, cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;

namespace DbContexts;

public sealed partial class DbCxt
{
    public async Task<Seedysoft.CoreLib.Entities.PvpcView[]> GetPvpcBetweenDatesAsync(DateTime sinceDate, DateTime untilDate, CancellationToken cancellationToken)
    {
        return await PvpcsView.AsNoTracking()
            .Where(x => x.AtDateTimeUnix >= new DateTimeOffset(sinceDate).ToUnixTimeSeconds())
            .Where(x => x.AtDateTimeUnix < new DateTimeOffset(untilDate).ToUnixTimeSeconds())
            .ToArrayAsync(cancellationToken)
            ?? Array.Empty<Seedysoft.CoreLib.Entities.PvpcView>();
    }

    public async Task<Seedysoft.CoreLib.Entities.Subscriber?> GetSubscriberWithSubscriptionsAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        return await Subscribers
            .Include(x => x.Subscriptions)
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId, cancellationToken);
    }
}

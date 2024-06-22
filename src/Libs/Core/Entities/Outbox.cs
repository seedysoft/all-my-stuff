using Seedysoft.Libs.Core.Enums;

namespace Seedysoft.Libs.Core.Entities;

public abstract class OutboxBase
{
    public long OutboxId { get; set; }

    public string Payload { get; set; } = default!;

    public SubscriptionName SubscriptionName { get; set; }

    public long? SubscriptionId { get; set; }

    public DateTimeOffset? SentAtDateTimeOffset { get; set; }
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Outbox : OutboxBase
{
    public Outbox(SubscriptionName subscriptionName, string payload)
    {
        SubscriptionName = subscriptionName;
        Payload = payload;
    }

    private string GetDebuggerDisplay() => $"{SubscriptionName} {SubscriptionId} {(SentAtDateTimeOffset.HasValue ? "sent on " + SentAtDateTimeOffset.ToString() : "pending")}";
}

public sealed class OutboxView : OutboxBase
{
    public long? SentAtDateTimeUnix { get; set; }
}

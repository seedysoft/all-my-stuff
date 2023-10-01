namespace Seedysoft.CoreLib.Entities;

public abstract class OutboxBase
{
    public long OutboxId { get; set; }

    public string Payload { get; set; } = default!;

    public Enums.SubscriptionName SubscriptionName { get; set; }

    public long? SubscriptionId { get; set; }

    public DateTimeOffset? SentAtDateTimeOffset { get; set; }
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Outbox : OutboxBase
{
    protected Outbox() { }
    public Outbox(Enums.SubscriptionName subscriptionName, string payload)
    {
        SubscriptionName = subscriptionName;
        Payload = payload;
    }

    protected string GetDebuggerDisplay() => $"{SubscriptionName} {SubscriptionId} {(SentAtDateTimeOffset.HasValue ? "sent on " + SentAtDateTimeOffset.ToString() : "pending")}";
}

public class OutboxView : OutboxBase
{
    public long? SentAtDateTimeUnix { get; set; }
}

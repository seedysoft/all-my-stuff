namespace Seedysoft.CoreLib.Entities;

public abstract class OutboxTable
{
    public long OutboxId { get; set; }

    public string Payload { get; set; } = default!;

    public Enums.SubscriptionName SubscriptionName { get; set; }

    public long? SubscriptionId { get; set; }

    public DateTimeOffset? SentAtDateTimeOffset { get; set; }

    protected string GetDebuggerDisplay() => $"{SubscriptionName} {SubscriptionId} {(SentAtDateTimeOffset.HasValue ? "sent on " + SentAtDateTimeOffset.ToString() : "pending")}";
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Outbox : OutboxTable
{
    protected Outbox() { }
    public Outbox(Enums.SubscriptionName subscriptionName, string payload)
    {
        SubscriptionName = subscriptionName;
        Payload = payload;
    }
}

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class OutboxView : Outbox
{
    public long? SentAtDateTimeUnix { get; set; }
}

namespace Seedysoft.CoreLib.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Subscription
{
    private Subscription() { }
    public Subscription(Enums.SubscriptionName subscriptionName) => SubscriptionName = subscriptionName;

    public long SubscriptionId { get; set; }

    public Enums.SubscriptionName SubscriptionName { get; set; }

    public ICollection<Subscriber> Subscribers { get; set; } = new List<Subscriber>();

    private string GetDebuggerDisplay() => $"{SubscriptionName} {SubscriptionId}";
}

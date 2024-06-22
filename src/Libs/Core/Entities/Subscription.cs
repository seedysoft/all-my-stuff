using Seedysoft.Libs.Core.Enums;

namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Subscription
{
    private Subscription() { }
    public Subscription(SubscriptionName subscriptionName) => SubscriptionName = subscriptionName;

    public long SubscriptionId { get; set; }

    public SubscriptionName SubscriptionName { get; set; }

    public ICollection<Subscriber> Subscribers { get; set; } = new List<Subscriber>();

    private string GetDebuggerDisplay() => $"{SubscriptionName} {SubscriptionId}";
}

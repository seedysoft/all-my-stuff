namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
public sealed class SubscriberSubscription
{
    public long SubscriberId { get; set; }

    public long SubscriptionId { get; set; }

    private string GetDebuggerDisplay => $"{SubscriberId} subscrito a {SubscriptionId}";
}

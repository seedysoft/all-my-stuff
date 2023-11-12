using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal sealed class SubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<CoreLib.Entities.Subscription>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.Subscription> builder)
    {
        _ = builder
            .Property(s => s.SubscriptionId)
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(s => s.SubscriptionName)
            .HasConversion<string>();

        _ = builder
            .ToTable(nameof(CoreLib.Entities.Subscription))
            .HasKey(s => s.SubscriptionId);

        _ = builder
            .Navigation(e => e.Subscribers)
            // System.InvalidOperationException: 'Cycle detected while auto-including navigations: 'Subscription.Subscribers', 'Subscriber.Subscriptions'. To fix this issue, either don't configure at least one navigation in the cycle as auto included in `OnModelCreating` or call 'IgnoreAutoInclude' method on the query.'
            /*.AutoInclude()*/;
    }
}

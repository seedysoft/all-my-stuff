using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Subscription>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Subscription> builder)
    {
        _ = builder
            .Property(static s => s.SubscriptionId)
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(static s => s.SubscriptionName)
            .HasConversion<string>();

        _ = builder
            .ToTable(nameof(Core.Entities.Subscription))
            .HasKey(static s => s.SubscriptionId);

        _ = builder
            .Navigation(static e => e.Subscribers)
            // System.InvalidOperationException: 'Cycle detected while auto-including navigations: 'Subscription.Subscribers', 'Subscriber.Subscriptions'. To fix this issue, either don't configure at least one navigation in the cycle as auto included in `OnModelCreating` or call 'IgnoreAutoInclude' method on the query.'
            /*.AutoInclude()*/;
    }
}

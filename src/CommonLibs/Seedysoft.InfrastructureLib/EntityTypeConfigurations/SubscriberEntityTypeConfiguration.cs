using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal class SubscriberEntityTypeConfiguration : IEntityTypeConfiguration<CoreLib.Entities.Subscriber>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.Subscriber> builder)
    {
        _ = builder
            .Property(e => e.SubscriberId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(e => e.TelegramUserId);

        _ = builder
            .Property(s => s.MailAddress);

        _ = builder
            .ToTable(nameof(CoreLib.Entities.Subscriber))
            .HasKey(e => e.SubscriberId);

        _ = builder
            .HasMany(s => s.Subscriptions)
            .WithMany(s => s.Subscribers)
            .UsingEntity<CoreLib.Entities.SubscriberSubscription>(
                s => s.HasOne<CoreLib.Entities.Subscription>().WithMany().HasForeignKey(e => e.SubscriptionId).OnDelete(DeleteBehavior.Cascade),
                s => s.HasOne<CoreLib.Entities.Subscriber>().WithMany().HasForeignKey(e => e.SubscriberId).OnDelete(DeleteBehavior.Cascade),
                s => s.ToTable($"{nameof(CoreLib.Entities.Subscriber)}{nameof(CoreLib.Entities.Subscription)}").HasKey(e => new { e.SubscriberId, e.SubscriptionId }));

        _ = builder
            .Navigation(e => e.Subscriptions);
    }
}

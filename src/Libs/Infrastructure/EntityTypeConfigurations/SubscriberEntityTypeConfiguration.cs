using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubscriberEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Subscriber>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Subscriber> builder)
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
            .ToTable(nameof(Core.Entities.Subscriber))
            .HasKey(e => e.SubscriberId);

        _ = builder
            .HasMany(s => s.Subscriptions)
            .WithMany(s => s.Subscribers)
            .UsingEntity<Core.Entities.SubscriberSubscription>(
                s => s.HasOne<Core.Entities.Subscription>().WithMany().HasForeignKey(e => e.SubscriptionId).OnDelete(DeleteBehavior.Cascade),
                s => s.HasOne<Core.Entities.Subscriber>().WithMany().HasForeignKey(e => e.SubscriberId).OnDelete(DeleteBehavior.Cascade),
                s => s.ToTable($"{nameof(Core.Entities.Subscriber)}{nameof(Core.Entities.Subscription)}").HasKey(e => new { e.SubscriberId, e.SubscriptionId }));

        _ = builder
            .Navigation(e => e.Subscriptions);
    }
}

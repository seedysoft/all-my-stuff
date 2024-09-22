using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubscriberEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Subscriber>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Subscriber> builder)
    {
        _ = builder
            .Property(static e => e.SubscriberId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(static e => e.TelegramUserId);

        _ = builder
            .Property(static s => s.MailAddress);

        _ = builder
            .ToTable(nameof(Core.Entities.Subscriber))
            .HasKey(static e => e.SubscriberId);

        _ = builder
            .HasMany(static s => s.Subscriptions)
            .WithMany(static s => s.Subscribers)
            .UsingEntity<Core.Entities.SubscriberSubscription>(
                static s => s.HasOne<Core.Entities.Subscription>().WithMany().HasForeignKey(static e => e.SubscriptionId).OnDelete(DeleteBehavior.Cascade),
                static s => s.HasOne<Core.Entities.Subscriber>().WithMany().HasForeignKey(static e => e.SubscriberId).OnDelete(DeleteBehavior.Cascade),
                static s => s.ToTable($"{nameof(Core.Entities.Subscriber)}{nameof(Core.Entities.Subscription)}").HasKey(static e => new { e.SubscriberId, e.SubscriptionId }));

        _ = builder
            .Navigation(static e => e.Subscriptions);
    }
}

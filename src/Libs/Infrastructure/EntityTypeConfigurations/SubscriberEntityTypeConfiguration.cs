using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Entities;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubscriberEntityTypeConfiguration : IEntityTypeConfiguration<Subscriber>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Subscriber> builder)
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
            .ToTable(nameof(Subscriber))
            .HasKey(e => e.SubscriberId);

        _ = builder
            .HasMany(s => s.Subscriptions)
            .WithMany(s => s.Subscribers)
            .UsingEntity<SubscriberSubscription>(
                s => s.HasOne<Subscription>().WithMany().HasForeignKey(e => e.SubscriptionId).OnDelete(DeleteBehavior.Cascade),
                s => s.HasOne<Subscriber>().WithMany().HasForeignKey(e => e.SubscriberId).OnDelete(DeleteBehavior.Cascade),
                s => s.ToTable($"{nameof(Subscriber)}{nameof(Subscription)}").HasKey(e => new { e.SubscriberId, e.SubscriptionId }));

        _ = builder
            .Navigation(e => e.Subscriptions);
    }
}

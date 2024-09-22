using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubcriptionDataViewEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.SubcriptionDataView>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.SubcriptionDataView> builder)
    {
        _ = builder
            .Property(static e => e.SubscriberId);

        _ = builder
            .Property(static x => x.SubscriptionId);

        _ = builder
            .Property(static x => x.Firstname);

        _ = builder
            .Property(static x => x.TelegramUserId);

        _ = builder
            .Property(static x => x.MailAddress);

        _ = builder
            .Property(static x => x.SubscriptionName);

        _ = builder
            .Property(static x => x.Description);

        _ = builder
            .Property(static x => x.WebUrl);

        _ = builder
            .ToView(nameof(Core.Entities.SubcriptionDataView))
            .HasNoKey();
    }
}

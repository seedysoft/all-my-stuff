using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubcriptionDataViewEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.SubcriptionDataView>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.SubcriptionDataView> builder)
    {
        _ = builder
            .Property(e => e.SubscriberId);

        _ = builder
            .Property(x => x.SubscriptionId);

        _ = builder
            .Property(x => x.Firstname);

        _ = builder
            .Property(x => x.TelegramUserId);

        _ = builder
            .Property(x => x.MailAddress);

        _ = builder
            .Property(x => x.SubscriptionName);

        _ = builder
            .Property(x => x.Description);

        _ = builder
            .Property(x => x.WebUrl);

        _ = builder
            .ToView(nameof(Core.Entities.SubcriptionDataView))
            .HasNoKey();
    }
}

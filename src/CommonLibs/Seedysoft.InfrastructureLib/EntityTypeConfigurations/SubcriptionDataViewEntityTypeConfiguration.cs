using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal class SubcriptionDataViewEntityTypeConfiguration : IEntityTypeConfiguration<CoreLib.Entities.SubcriptionDataView>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.SubcriptionDataView> builder)
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
            .ToView(nameof(CoreLib.Entities.SubcriptionDataView))
            .HasNoKey();
    }
}

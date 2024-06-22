using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Entities;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class SubcriptionDataViewEntityTypeConfiguration : IEntityTypeConfiguration<SubcriptionDataView>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<SubcriptionDataView> builder)
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
            .ToView(nameof(SubcriptionDataView))
            .HasNoKey();
    }
}

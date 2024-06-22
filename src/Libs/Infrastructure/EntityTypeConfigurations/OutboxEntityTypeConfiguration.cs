using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Entities;
using Seedysoft.Libs.Infrastructure.ValueConverters;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal abstract class OutboxTableEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : OutboxBase
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.OutboxId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(x => x.Payload)
            .IsRequired();

        _ = builder
            .Property(x => x.SubscriptionName)
            .HasConversion<string>()
            .IsRequired();

        _ = builder
            .Property(x => x.SubscriptionId);

        _ = builder
            .Property(x => x.SentAtDateTimeOffset)
            .HasConversion(DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);
    }
}

internal sealed class OutboxEntityTypeConfiguration : OutboxTableEntityTypeConfigurationT<Outbox>, IEntityTypeConfiguration<Outbox>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Outbox> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Outbox))
            .HasKey(x => x.OutboxId);
    }
}

internal sealed class OutboxViewEntityTypeConfiguration : OutboxTableEntityTypeConfigurationT<OutboxView>, IEntityTypeConfiguration<OutboxView>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OutboxView> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.SentAtDateTimeUnix);

        _ = builder
            .ToView(nameof(OutboxView))
            .HasNoKey();
    }
}

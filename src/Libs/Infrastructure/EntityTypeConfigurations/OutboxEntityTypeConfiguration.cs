using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal abstract class OutboxTableEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : Core.Entities.OutboxBase
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
            .HasConversion(ValueConverters.DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);
    }
}

internal sealed class OutboxEntityTypeConfiguration : OutboxTableEntityTypeConfigurationT<Core.Entities.Outbox>, IEntityTypeConfiguration<Core.Entities.Outbox>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Outbox> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.Outbox))
            .HasKey(x => x.OutboxId);
    }
}

internal sealed class OutboxViewEntityTypeConfiguration : OutboxTableEntityTypeConfigurationT<Core.Entities.OutboxView>, IEntityTypeConfiguration<Core.Entities.OutboxView>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.OutboxView> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.SentAtDateTimeUnix);

        _ = builder
            .ToView(nameof(Core.Entities.OutboxView))
            .HasNoKey();
    }
}

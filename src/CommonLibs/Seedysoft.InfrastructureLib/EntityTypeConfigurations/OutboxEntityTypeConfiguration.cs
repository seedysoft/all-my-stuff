using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal abstract class OutboxTableEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : CoreLib.Entities.OutboxBase
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
            .HasConversion(UtilsLib.Constants.ValueConverters.NullableDateTimeOffsetStringValueConverter);
    }
}

internal sealed class OutboxEntityTypeConfiguration : OutboxTableEntityTypeConfigurationT<CoreLib.Entities.Outbox>, IEntityTypeConfiguration<CoreLib.Entities.Outbox>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.Outbox> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(CoreLib.Entities.Outbox))
            .HasKey(x => x.OutboxId);
    }
}

internal sealed class OutboxViewEntityTypeConfiguration : OutboxTableEntityTypeConfigurationT<CoreLib.Entities.OutboxView>, IEntityTypeConfiguration<CoreLib.Entities.OutboxView>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.OutboxView> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.SentAtDateTimeUnix);

        _ = builder
            .ToView(nameof(CoreLib.Entities.OutboxView))
            .HasNoKey();
    }
}

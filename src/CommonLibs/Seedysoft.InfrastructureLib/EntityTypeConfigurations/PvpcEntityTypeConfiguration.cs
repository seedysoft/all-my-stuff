using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal class PvpcEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : CoreLib.Entities.PvpcBase
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.AtDateTimeOffset)
            .HasConversion(UtilsLib.Constants.ValueConverters.DateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.MWhPriceInEuros);
    }
}

internal class PvpcEntityTypeConfiguration : PvpcEntityTypeConfigurationT<CoreLib.Entities.Pvpc>, IEntityTypeConfiguration<CoreLib.Entities.Pvpc>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.Pvpc> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(CoreLib.Entities.Pvpc))
            .HasKey(x => x.AtDateTimeOffset);
    }
}

internal class PvpcViewEntityTypeConfiguration : PvpcEntityTypeConfigurationT<CoreLib.Entities.PvpcView>, IEntityTypeConfiguration<CoreLib.Entities.PvpcView>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.PvpcView> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.AtDateTimeUnix);

        _ = builder
            .ToView(nameof(CoreLib.Entities.PvpcView))
            .HasNoKey();
    }
}

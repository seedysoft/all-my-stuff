using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class PvpcEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Pvpc>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Pvpc> builder)
    {
        _ = builder
            .Property(static x => x.AtDateTimeOffset)
            .HasConversion(ValueConverters.DateTimeOffsetString.DateTimeOffsetStringValueConverter);

        _ = builder
            .Property(static x => x.MWhPriceInEuros);

        _ = builder
            .ToTable(nameof(Core.Entities.Pvpc))
            .HasKey(static x => x.AtDateTimeOffset);
    }
}

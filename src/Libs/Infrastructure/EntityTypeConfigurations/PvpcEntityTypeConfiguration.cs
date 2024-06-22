using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Entities;
using Seedysoft.Libs.Infrastructure.ValueConverters;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal abstract class PvpcEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : PvpcBase
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.AtDateTimeOffset)
            .HasConversion(DateTimeOffsetString.DateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.MWhPriceInEuros);
    }
}

internal sealed class PvpcEntityTypeConfiguration : PvpcEntityTypeConfigurationT<Pvpc>, IEntityTypeConfiguration<Pvpc>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Pvpc> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Pvpc))
            .HasKey(x => x.AtDateTimeOffset);
    }
}

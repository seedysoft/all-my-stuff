using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class ProvinciaEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Provincia>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Provincia> builder)
    {
        _ = builder
            .Property(static x => x.ProvinciaId)
            .HasMaxLength(3)
            .IsFixedLength();

        _ = builder
            .Property(static x => x.ProvinciaDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Provincia))
            .HasKey(static x => x.ProvinciaId);
    }
}

using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class PaisEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Pais>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Pais> builder)
    {
        _ = builder
            .Property(static x => x.PaisId)
            .HasMaxLength(3)
            .IsFixedLength();

        _ = builder
            .Property(static x => x.PaisDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Pais))
            .HasKey(static x => x.PaisId);
    }
}

using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class UnidadEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Unidad>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Unidad> builder)
    {
        _ = builder
            .Property(static x => x.UnidadId);

        _ = builder
            .Property(static x => x.UnidadDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Unidad))
            .HasKey(static x => x.UnidadId);
    }
}

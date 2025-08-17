using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class LocalidadEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Localidad>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Localidad> builder)
    {
        _ = builder
            .Property(static x => x.LocalidadId)
            .HasMaxLength(3)
            .IsFixedLength();

        _ = builder
            .Property(static x => x.LocalidadDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Localidad))
            .HasKey(static x => x.LocalidadId);
    }
}

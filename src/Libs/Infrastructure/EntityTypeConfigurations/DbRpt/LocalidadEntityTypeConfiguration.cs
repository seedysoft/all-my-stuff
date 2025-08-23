using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class LocalidadEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Localidad>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Localidad> builder)
    {
        _ = builder
            .Property(static x => x.LocalidadId)
            .ValueGeneratedNever();

        _ = builder
            .Property(static x => x.LocalidadDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Localidad))
            .HasKey(static x => new { x.PaisId, x.ProvinciaId, x.LocalidadId });

        _ = builder
            .HasOne(static x => x.Pais)
            .WithMany(/*static x => x.Localidades*/)
            .HasForeignKey(static x => x.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Localidad)}_{nameof(Core.Entities.Localidad.PaisId)}");

        _ = builder
            .HasOne(static x => x.Provincia)
            .WithMany(/*static x => x.Localidades*/)
            .HasForeignKey(static x => new { x.PaisId, x.ProvinciaId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Localidad)}_{nameof(Core.Entities.Localidad.Provincia)}");
    }
}

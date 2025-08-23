using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class UnidadEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Unidad>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Unidad> builder)
    {
        _ = builder
            .Property(static x => x.UnidadId)
            .ValueGeneratedNever();

        _ = builder
            .Property(static x => x.UnidadDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Unidad))
            .HasKey(static x => x.UnidadId);

        _ = builder
            .HasOne(static x => x.CentroDirectivo)
            .WithMany(/*static x => x.Unidades*/)
            .HasForeignKey(static x => x.CentroDirectivoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.CentroDirectivoId)}");

        _ = builder
            .HasOne(static x => x.Pais)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => x.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.PaisId)}");

        _ = builder
            .HasOne(static x => x.Provincia)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => new { x.PaisId, x.ProvinciaId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Provincia)}");

        _ = builder
            .HasOne(static x => x.Localidad)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => new { x.PaisId, x.ProvinciaId, x.LocalidadId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Localidad)}");
    }
}

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
            .HasIndex(static e => e.CentroDirectivoId, $"IX_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.CentroDirectivoId)}");
        _ = builder
            .HasOne(static d => d.CentroDirectivo)
            .WithMany(/*static p => p.Unidades*/)
            .HasForeignKey(static d => d.CentroDirectivoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.CentroDirectivoId)}");

        _ = builder
            .HasIndex(static e => e.PaisId, $"IX_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.PaisId)}");
        _ = builder
            .HasOne(static d => d.Pais)
            .WithMany(/*static p => p.Unidades*/)
            .HasForeignKey(static d => d.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.PaisId)}");

        _ = builder
            .HasIndex(static e => e.ProvinciaId, $"IX_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.ProvinciaId)}");
        _ = builder
            .HasOne(static d => d.Provincia)
            .WithMany(/*static p => p.Unidades*/)
            .HasForeignKey(static d => d.ProvinciaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.ProvinciaId)}");

        _ = builder
            .HasIndex(static e => e.LocalidadId, $"IX_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.LocalidadId)}");
        _ = builder
            .HasOne(static d => d.Localidad)
            .WithMany(/*static p => p.Unidades*/)
            .HasForeignKey(static d => d.LocalidadId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Unidad)}_{nameof(Core.Entities.Unidad.LocalidadId)}");
    }
}

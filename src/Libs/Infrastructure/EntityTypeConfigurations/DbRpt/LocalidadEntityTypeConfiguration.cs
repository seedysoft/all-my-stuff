using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

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

        _ = builder
            .HasIndex(static e => e.PaisId, $"IX_{nameof(Core.Entities.Localidad)}_{nameof(Core.Entities.Localidad.PaisId)}");
        _ = builder
            .HasOne(static d => d.Pais)
            .WithMany(/*static p => p.Localidades*/)
            .HasForeignKey(static d => d.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Localidad)}_{nameof(Core.Entities.Localidad.PaisId)}");

        _ = builder
            .HasIndex(static e => e.ProvinciaId, $"IX_{nameof(Core.Entities.Localidad)}_{nameof(Core.Entities.Localidad.ProvinciaId)}");
        _ = builder
            .HasOne(static d => d.Provincia)
            .WithMany(/*static p => p.Localidades*/)
            .HasForeignKey(static d => d.ProvinciaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Localidad)}_{nameof(Core.Entities.Localidad.ProvinciaId)}");
    }
}

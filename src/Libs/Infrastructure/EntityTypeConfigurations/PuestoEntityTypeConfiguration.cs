using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class PuestoEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Puesto>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Puesto> builder)
    {
        _ = builder
            .Property(static x => x.PuestoId);

        _ = builder
            .Property(static x => x.PuestoDenominacionCorta);

        _ = builder
            .Property(static x => x.PuestoDenominacionLarga);

        _ = builder
            .Property(static x => x.Nivel);

        _ = builder
            .Property(static x => x.ComplementoEspecifico)
            .HasPrecision(2);

        _ = builder
            .Property(static x => x.TipoPuesto);

        _ = builder
            .Property(static x => x.Provision);

        _ = builder
            .Property(static x => x.Adscripcion);

        _ = builder
            .Property(static x => x.GrupoSubgrupo);

        _ = builder
            .Property(static x => x.Cuerpo);

        _ = builder
            .Property(static x => x.TitulacionAcademica);

        _ = builder
            .Property(static x => x.FormacionEspecifica);

        _ = builder
            .Property(static x => x.Observaciones);

        _ = builder
            .Property(static x => x.Estado);

        _ = builder
            .ToTable(nameof(Core.Entities.Puesto))
            .HasKey(static x => x.PuestoId);

        _ = builder
            .Navigation(static e => e.Ministerio);

        _ = builder
            .Navigation(static e => e.CentroDirectivo);

        _ = builder
            .Navigation(static e => e.Unidad);

        _ = builder
            .Navigation(static e => e.Pais);

        _ = builder
            .Navigation(static e => e.Provincia);

        _ = builder
            .Navigation(static e => e.Localidad);

        _ = builder
            .Navigation(static e => e.Pais);

        _ = builder
            .Navigation(static e => e.ProvinciaResidencia);

        _ = builder
            .Navigation(static e => e.LocalidadResidencia);
    }
}

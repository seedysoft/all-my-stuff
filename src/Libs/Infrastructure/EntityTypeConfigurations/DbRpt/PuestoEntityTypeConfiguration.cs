using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class PuestoEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Puesto>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Puesto> builder)
    {
        _ = builder
            .Property(static x => x.PuestoId)
            .ValueGeneratedNever();

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
            .Property(static x => x.TipoPuesto)
            /*.HasConversion(ValueConverters.RecordToString<Core.Entities.TiposPuesto>.NullableMasterFilesBaseStringValueConverter)*/;

        _ = builder
            .Property(static x => x.Provision)
            /*.HasConversion(ValueConverters.RecordToString<Core.Entities.FormasProvision>.NullableMasterFilesBaseStringValueConverter)*/;

        _ = builder
            .Property(static x => x.Adscripcion)
            /*.HasConversion(ValueConverters.RecordToString<Core.Entities.AdscripcionesAdministracion>.NullableMasterFilesBaseStringValueConverter)*/;

        _ = builder
            .Property(static x => x.GrupoSubgrupo);

        _ = builder
            .Property(static x => x.Cuerpo)
            /*.HasConversion(ValueConverters.RecordToString<Core.Entities.AdscripcionesCuerpos>.NullableMasterFilesBaseStringValueConverter)*/;

        _ = builder
            .Property(static x => x.TitulacionAcademica);

        _ = builder
            .Property(static x => x.FormacionEspecifica);

        _ = builder
            .Property(static x => x.Observaciones);

        _ = builder
            .Property(static x => x.Estado)
            /*.HasConversion(ValueConverters.RecordToString<Core.Entities.Estados>.NullableMasterFilesBaseStringValueConverter)*/;

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
            .Navigation(static e => e.ResidenciaPais);

        _ = builder
            .Navigation(static e => e.ResidenciaProvincia);

        _ = builder
            .HasOne<Core.Entities.Localidad>(nameof(Core.Entities.Puesto.ResidenciaLocalidad))
            .WithMany();

        //_ = builder
        //    .Navigation(static e => e.ResidenciaLocalidad);
    }
}

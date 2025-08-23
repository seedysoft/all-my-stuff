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
            .HasOne(static x => x.Unidad)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => x.UnidadId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.UnidadId)}");

        _ = builder
            .HasOne(static x => x.Pais)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => x.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.PaisId)}");

        _ = builder
            .HasOne(static x => x.Provincia)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => new { x.PaisId, x.ProvinciaId})
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Provincia)}");

        _ = builder
            .HasOne(static x => x.Localidad)
            .WithMany(/*static x => x.Puestos*/)
            .HasForeignKey(static x => new { x.PaisId, x.ProvinciaId, x.LocalidadId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Localidad)}");
    }
}

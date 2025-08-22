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
            .HasIndex(static e => e.UnidadId, $"IX_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.UnidadId)}");
        _ = builder
            .HasOne(static d => d.Unidad)
            .WithMany(/*static p => p.Puestos*/)
            .HasForeignKey(static d => d.UnidadId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.UnidadId)}");

        _ = builder
            .HasIndex(static e => e.PaisId, $"IX_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.PaisId)}");
        _ = builder
            .HasOne(static d => d.Pais)
            .WithMany(/*static p => p.Puestos*/)
            .HasForeignKey(static d => d.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.PaisId)}");

        _ = builder
            .HasIndex(static e => e.ProvinciaId, $"IX_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.ProvinciaId)}");
        _ = builder
            .HasOne(static d => d.Provincia)
            .WithMany(/*static p => p.Puestos*/)
            .HasForeignKey(static d => d.ProvinciaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.ProvinciaId)}");

        _ = builder
            .HasIndex(static e => e.LocalidadId, $"IX_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.LocalidadId)}");
        _ = builder
            .HasOne(static d => d.Localidad)
            .WithMany(/*static p => p.Puestos*/)
            .HasForeignKey(static d => d.LocalidadId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Puesto)}_{nameof(Core.Entities.Puesto.LocalidadId)}");
    }
}

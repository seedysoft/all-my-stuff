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
            .HasIndex(static e => e.MinisterioId, "IX_Puesto_MinisterioId");
        _ = builder
            .HasOne(static d => d.Ministerio)
            .WithMany(/*static p => p.Puesto*/)
            .HasForeignKey(static d => d.MinisterioId);

        _ = builder
            .HasIndex(static e => e.CentroDirectivoId, "IX_Puesto_CentroDirectivoId");
        _ = builder
            .HasOne(static d => d.CentroDirectivo)
            .WithMany(/*static p => p.Puesto*/)
            .HasForeignKey(static d => d.CentroDirectivoId);

        _ = builder
            .HasIndex(static e => e.UnidadId, "IX_Puesto_UnidadId");
        _ = builder
            .HasOne(static d => d.Unidad)
            .WithMany(/*static p => p.Puesto*/)
            .HasForeignKey(static d => d.UnidadId);

        _ = builder
            .HasIndex(static e => e.PaisId, "IX_Puesto_PaisId");
        _ = builder
            .HasOne(static d => d.Pais)
            .WithMany(/*static p => p.PuestoPais*/)
            .HasForeignKey(static d => d.PaisId);

        _ = builder
            .HasIndex(static e => e.ProvinciaId, "IX_Puesto_ProvinciaId");
        _ = builder
            .HasOne(static d => d.Provincia)
            .WithMany(/*static p => p.PuestoProvincia*/)
            .HasForeignKey(static d => d.ProvinciaId);

        _ = builder
            .HasIndex(static e => e.LocalidadId, "IX_Puesto_LocalidadId");
        _ = builder
            .HasOne(static d => d.Localidad)
            .WithMany(/*static p => p.PuestoLocalidad*/)
            .HasForeignKey(static d => d.LocalidadId);

        _ = builder
             .HasIndex(static e => e.ResidenciaPaisId, "IX_Puesto_ResidenciaPaisId");
        _ = builder
            .HasOne(static d => d.ResidenciaPais)
            .WithMany(/*static p => p.PuestoResidenciaPais*/)
            .HasForeignKey(static d => d.ResidenciaPaisId);

        _ = builder
            .HasIndex(static e => e.ResidenciaProvinciaId, "IX_Puesto_ResidenciaProvinciaId");
        _ = builder
            .HasOne(static d => d.ResidenciaProvincia)
            .WithMany(/*static p => p.PuestoResidenciaProvincia*/)
            .HasForeignKey(static d => d.ResidenciaProvinciaId);

        _ = builder
            .HasIndex(static e => e.ResidenciaLocalidadId, "IX_Puesto_ResidenciaLocalidadId");
        _ = builder
            .HasOne(static d => d.ResidenciaLocalidad)
            .WithMany(/*static p => p.PuestoResidenciaLocalidad*/)
            .HasForeignKey(static d => d.ResidenciaLocalidadId);
    }
}

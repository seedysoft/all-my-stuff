using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class EstacionServicioEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.EstacionServicio>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.EstacionServicio>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.EstacionServicio> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.IdEstacion)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.IdMunicipio)
            .IsRequired();

        _ = builder
            .Property(x => x.CodigoPostal)
            .IsRequired();

        _ = builder
            .Property(x => x.Direccion)
            .IsRequired();

        _ = builder
            .Property(x => x.Horario)
            .IsRequired();

        _ = builder
            .Property(x => x.Latitud)
            .IsRequired();

        _ = builder
            .Property(x => x.Localidad)
            .IsRequired();

        _ = builder
            .Property(x => x.LongitudWgs84)
            .IsRequired();

        _ = builder
            .Property(x => x.Margen)
            .IsRequired();

        _ = builder
            .Property(x => x.Rotulo)
            .IsRequired();

        _ = builder
            .ToTable(nameof(Carburantes.CoreLib.Entities.EstacionServicio))
            .HasKey(x => new { x.IdEstacion, x.AtDate });
    }
}

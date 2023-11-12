using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class EstacionServicioEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.EstacionServicioBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
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

        base.Configure(builder);
    }
}

internal sealed class EstacionServicioEntityTypeConfiguration : EstacionServicioEntityTypeConfigurationBase<Core.Entities.EstacionServicio>, IEntityTypeConfiguration<Core.Entities.EstacionServicio>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.EstacionServicio> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.EstacionServicio))
            .HasKey(x => new { x.IdEstacion });
    }
}

internal sealed class EstacionServicioHistEntityTypeConfiguration : EstacionServicioEntityTypeConfigurationBase<Core.Entities.EstacionServicioHist>, IEntityTypeConfiguration<Core.Entities.EstacionServicioHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.EstacionServicioHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.EstacionServicioHist))
            .HasKey(x => new { x.IdEstacion, x.AtDate });
    }
}

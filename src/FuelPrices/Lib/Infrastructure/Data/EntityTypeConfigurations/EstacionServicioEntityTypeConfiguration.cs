using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class EstacionServicioEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : EstacionServicioBase
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

internal sealed class EstacionServicioEntityTypeConfiguration : EstacionServicioEntityTypeConfigurationBase<EstacionServicio>, IEntityTypeConfiguration<EstacionServicio>
{
    public override void Configure(EntityTypeBuilder<EstacionServicio> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(EstacionServicio))
            .HasKey(x => new { x.IdEstacion });
    }
}

internal sealed class EstacionServicioHistEntityTypeConfiguration : EstacionServicioEntityTypeConfigurationBase<EstacionServicioHist>, IEntityTypeConfiguration<EstacionServicioHist>
{
    public override void Configure(EntityTypeBuilder<EstacionServicioHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(EstacionServicioHist))
            .HasKey(x => new { x.IdEstacion, x.AtDate });
    }
}

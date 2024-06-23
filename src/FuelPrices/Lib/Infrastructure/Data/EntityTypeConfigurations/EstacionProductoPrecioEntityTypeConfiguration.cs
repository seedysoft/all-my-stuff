using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class EstacionProductoPrecioEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : EstacionProductoPrecioBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.IdEstacion)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.IdProducto)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.CentimosDeEuro)
            .IsRequired();

        base.Configure(builder);
    }
}

internal sealed class EstacionProductoPrecioEntityTypeConfiguration : EstacionProductoPrecioEntityTypeConfigurationBase<EstacionProductoPrecio>, IEntityTypeConfiguration<EstacionProductoPrecio>
{
    public override void Configure(EntityTypeBuilder<EstacionProductoPrecio> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(EstacionProductoPrecio))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

internal sealed class EstacionProductoPrecioHistEntityTypeConfiguration : EstacionProductoPrecioEntityTypeConfigurationBase<EstacionProductoPrecioHist>, IEntityTypeConfiguration<EstacionProductoPrecioHist>
{
    public override void Configure(EntityTypeBuilder<EstacionProductoPrecioHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(EstacionProductoPrecioHist))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

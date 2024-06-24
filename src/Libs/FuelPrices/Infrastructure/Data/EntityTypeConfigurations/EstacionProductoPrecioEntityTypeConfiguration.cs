using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.Libs.FuelPrices.Core.Entities;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class EstacionProductoPrecioEntityTypeConfiguration : EntityTypeConfigurationBase<EstacionProductoPrecio>, IEntityTypeConfiguration<EstacionProductoPrecio>
{
    public override void Configure(EntityTypeBuilder<EstacionProductoPrecio> builder)
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

        _ = builder
            .ToTable(nameof(EstacionProductoPrecio))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

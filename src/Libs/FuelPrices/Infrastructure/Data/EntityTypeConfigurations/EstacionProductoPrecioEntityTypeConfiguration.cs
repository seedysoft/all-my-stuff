using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class EstacionProductoPrecioEntityTypeConfiguration : EntityTypeConfigurationBase<Core.Entities.EstacionProductoPrecio>, IEntityTypeConfiguration<Core.Entities.EstacionProductoPrecio>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.EstacionProductoPrecio> builder)
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
            .ToTable(nameof(Core.Entities.EstacionProductoPrecio))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

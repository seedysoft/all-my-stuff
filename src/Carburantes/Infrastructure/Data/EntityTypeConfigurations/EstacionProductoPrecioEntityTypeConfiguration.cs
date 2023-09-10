using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class EstacionProductoPrecioEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.EstacionProductoPrecioBase
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

internal class EstacionProductoPrecioEntityTypeConfiguration : EstacionProductoPrecioEntityTypeConfigurationBase<Core.Entities.EstacionProductoPrecio>, IEntityTypeConfiguration<Core.Entities.EstacionProductoPrecio>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.EstacionProductoPrecio> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.EstacionProductoPrecio))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

internal class EstacionProductoPrecioHistEntityTypeConfiguration : EstacionProductoPrecioEntityTypeConfigurationBase<Core.Entities.EstacionProductoPrecioHist>, IEntityTypeConfiguration<Core.Entities.EstacionProductoPrecioHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.EstacionProductoPrecioHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.EstacionProductoPrecioHist))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

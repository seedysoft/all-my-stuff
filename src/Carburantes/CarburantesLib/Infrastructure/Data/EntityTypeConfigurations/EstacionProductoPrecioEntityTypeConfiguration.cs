using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class EstacionProductoPrecioEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.EstacionProductoPrecio>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.EstacionProductoPrecio>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.EstacionProductoPrecio> builder)
    {
        base.Configure(builder);

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

        _ = builder
            .ToTable(nameof(Carburantes.CoreLib.Entities.EstacionProductoPrecio))
            .HasKey(x => new { x.IdEstacion, x.IdProducto, x.AtDate });
    }
}

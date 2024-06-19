using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProductoPetroliferoEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.ProductoPetrolifero>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.ProductoPetrolifero>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.ProductoPetrolifero> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.IdProducto)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.NombreProducto)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreProductoAbreviatura)
            .IsRequired();

        _ = builder
            .ToTable(nameof(Carburantes.CoreLib.Entities.ProductoPetrolifero))
            .HasKey(x => new { x.IdProducto, x.AtDate });
    }
}

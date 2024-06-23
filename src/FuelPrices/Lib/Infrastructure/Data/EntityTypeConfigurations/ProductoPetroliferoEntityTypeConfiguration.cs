using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProductoPetroliferoEntityTypeConfiguration : EntityTypeConfigurationBase<ProductoPetrolifero>, IEntityTypeConfiguration<ProductoPetrolifero>
{
    public override void Configure(EntityTypeBuilder<ProductoPetrolifero> builder)
    {
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

        base.Configure(builder);

        _ = builder
            .ToTable(nameof(ProductoPetrolifero))
            .HasKey(x => new { x.IdProducto, x.AtDate });
    }
}

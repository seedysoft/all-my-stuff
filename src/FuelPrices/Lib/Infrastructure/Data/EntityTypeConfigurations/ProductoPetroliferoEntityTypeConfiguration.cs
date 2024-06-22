using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ProductoPetroliferoEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : ProductoPetroliferoBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
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
    }
}

internal sealed class ProductoPetroliferoEntityTypeConfiguration : ProductoPetroliferoEntityTypeConfigurationBase<ProductoPetrolifero>, IEntityTypeConfiguration<ProductoPetrolifero>
{
    public override void Configure(EntityTypeBuilder<ProductoPetrolifero> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(ProductoPetrolifero))
            .HasKey(x => new { x.IdProducto });
    }
}

internal sealed class ProductoPetroliferoHistEntityTypeConfiguration : ProductoPetroliferoEntityTypeConfigurationBase<ProductoPetroliferoHist>, IEntityTypeConfiguration<ProductoPetroliferoHist>
{
    public override void Configure(EntityTypeBuilder<ProductoPetroliferoHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(ProductoPetroliferoHist))
            .HasKey(x => new { x.IdProducto, x.AtDate });
    }
}

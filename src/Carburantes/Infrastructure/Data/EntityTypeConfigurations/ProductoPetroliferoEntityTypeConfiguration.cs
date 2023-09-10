using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ProductoPetroliferoEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.ProductoPetroliferoBase
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

internal class ProductoPetroliferoEntityTypeConfiguration : ProductoPetroliferoEntityTypeConfigurationBase<Core.Entities.ProductoPetrolifero>, IEntityTypeConfiguration<Core.Entities.ProductoPetrolifero>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ProductoPetrolifero> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.ProductoPetrolifero))
            .HasKey(x => new { x.IdProducto });
    }
}

internal class ProductoPetroliferoHistEntityTypeConfiguration : ProductoPetroliferoEntityTypeConfigurationBase<Core.Entities.ProductoPetroliferoHist>, IEntityTypeConfiguration<Core.Entities.ProductoPetroliferoHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ProductoPetroliferoHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.ProductoPetroliferoHist))
            .HasKey(x => new { x.IdProducto, x.AtDate });
    }
}

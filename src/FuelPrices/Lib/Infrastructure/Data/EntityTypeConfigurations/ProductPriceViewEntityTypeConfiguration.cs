using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ProductPriceBaseEntityTypeConfiguration<T> : EntityTypeConfigurationBase<T> where T : ProductPriceBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.AtDate)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.CentimosDeEuro)
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
            .Property(x => x.NombreMunicipio)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreProducto)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreProductoAbreviatura)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreProvincia)
            .IsRequired();

        _ = builder
            .Property(x => x.Rotulo)
            .IsRequired();
    }
}

internal sealed class ProductPriceEntityTypeConfiguration : ProductPriceBaseEntityTypeConfiguration<ProductPrice>, IEntityTypeConfiguration<ProductPrice>
{
    public override void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        base.Configure(builder);

        _ = builder
            .HasNoKey()
            .ToView(nameof(ProductPrice));
    }
}

internal sealed class ProductPriceHistViewEntityTypeConfiguration : ProductPriceBaseEntityTypeConfiguration<ProductPriceHist>, IEntityTypeConfiguration<ProductPriceHist>
{
    public override void Configure(EntityTypeBuilder<ProductPriceHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .HasNoKey()
            .ToView(nameof(ProductPriceHist));
    }
}

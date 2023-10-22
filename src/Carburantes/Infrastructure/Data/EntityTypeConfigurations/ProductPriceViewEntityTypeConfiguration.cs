using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ProductPriceBaseEntityTypeConfiguration<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.ProductPriceBase
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

internal class ProductPriceEntityTypeConfiguration : ProductPriceBaseEntityTypeConfiguration<Core.Entities.ProductPrice>, IEntityTypeConfiguration<Core.Entities.ProductPrice>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ProductPrice> builder)
    {
        base.Configure(builder);

        _ = builder
            .HasNoKey()
            .ToView(nameof(Core.Entities.ProductPrice));
    }
}

internal class ProductPriceHistViewEntityTypeConfiguration : ProductPriceBaseEntityTypeConfiguration<Core.Entities.ProductPriceHist>, IEntityTypeConfiguration<Core.Entities.ProductPriceHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ProductPriceHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .HasNoKey()
            .ToView(nameof(Core.Entities.ProductPriceHist));
    }
}

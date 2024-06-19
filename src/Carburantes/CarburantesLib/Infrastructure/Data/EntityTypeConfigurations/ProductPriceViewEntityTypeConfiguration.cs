using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProductPriceEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.ProductPrice>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.ProductPrice>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.ProductPrice> builder)
    {
        base.Configure(builder);

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

        _ = builder
            .HasNoKey()
            .ToView(nameof(Carburantes.CoreLib.Entities.ProductPrice));
    }
}

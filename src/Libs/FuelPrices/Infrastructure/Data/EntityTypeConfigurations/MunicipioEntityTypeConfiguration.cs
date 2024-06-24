using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.Libs.FuelPrices.Core.Entities;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class MunicipioEntityTypeConfiguration : EntityTypeConfigurationBase<Municipio>, IEntityTypeConfiguration<Municipio>
{
    public override void Configure(EntityTypeBuilder<Municipio> builder)
    {
        _ = builder
            .Property(x => x.IdMunicipio)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.IdProvincia)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreMunicipio)
            .IsRequired();

        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Municipio))
            .HasKey(x => new { x.IdMunicipio, x.AtDate });
    }
}

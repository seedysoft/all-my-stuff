using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class MunicipioEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : MunicipioBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
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
    }
}

internal sealed class MunicipioEntityTypeConfiguration : MunicipioEntityTypeConfigurationBase<Municipio>, IEntityTypeConfiguration<Municipio>
{
    public override void Configure(EntityTypeBuilder<Municipio> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Municipio))
            .HasKey(x => new { x.IdMunicipio });
    }
}

internal sealed class MunicipioHistEntityTypeConfiguration : MunicipioEntityTypeConfigurationBase<MunicipioHist>, IEntityTypeConfiguration<MunicipioHist>
{
    public override void Configure(EntityTypeBuilder<MunicipioHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(MunicipioHist))
            .HasKey(x => new { x.IdMunicipio, x.AtDate });
    }
}

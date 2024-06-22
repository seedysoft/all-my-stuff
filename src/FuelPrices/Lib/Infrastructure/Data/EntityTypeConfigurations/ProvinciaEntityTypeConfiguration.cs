using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ProvinciaEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : ProvinciaBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.IdProvincia)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.IdComunidadAutonoma)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreProvincia)
            .IsRequired();

        base.Configure(builder);
    }
}

internal sealed class ProvinciaEntityTypeConfiguration : ProvinciaEntityTypeConfigurationBase<Provincia>, IEntityTypeConfiguration<Provincia>
{
    public override void Configure(EntityTypeBuilder<Provincia> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Provincia))
            .HasKey(x => new { x.IdProvincia });
    }
}

internal sealed class ProvinciaHistEntityTypeConfiguration : ProvinciaEntityTypeConfigurationBase<ProvinciaHist>, IEntityTypeConfiguration<ProvinciaHist>
{
    public override void Configure(EntityTypeBuilder<ProvinciaHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(ProvinciaHist))
            .HasKey(x => new { x.IdProvincia, x.AtDate });
    }
}

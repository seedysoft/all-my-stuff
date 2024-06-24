using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProvinciaEntityTypeConfiguration : EntityTypeConfigurationBase<Provincia>, IEntityTypeConfiguration<Provincia>
{
    public override void Configure(EntityTypeBuilder<Provincia> builder)
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

        _ = builder
            .ToTable(nameof(Provincia))
            .HasKey(x => new { x.IdProvincia, x.AtDate });
    }
}

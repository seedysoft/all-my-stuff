using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProvinciaEntityTypeConfiguration : EntityTypeConfigurationBase<Core.Entities.Provincia>, IEntityTypeConfiguration<Core.Entities.Provincia>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.Provincia> builder)
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
            .ToTable(nameof(Core.Entities.Provincia))
            .HasKey(x => new { x.IdProvincia, x.AtDate });
    }
}

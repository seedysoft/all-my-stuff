using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.Libs.FuelPrices.Core.Entities;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ComunidadAutonomaEntityTypeConfiguration : EntityTypeConfigurationBase<ComunidadAutonoma>, IEntityTypeConfiguration<ComunidadAutonoma>
{
    public override void Configure(EntityTypeBuilder<ComunidadAutonoma> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.IdComunidadAutonoma)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.NombreComunidadAutonoma)
            .IsRequired();

        _ = builder
            .ToTable(nameof(ComunidadAutonoma))
            .HasKey(x => new { x.IdComunidadAutonoma, x.AtDate });
    }
}

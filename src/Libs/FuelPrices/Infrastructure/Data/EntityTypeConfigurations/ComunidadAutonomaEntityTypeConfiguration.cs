using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ComunidadAutonomaEntityTypeConfiguration : EntityTypeConfigurationBase<Core.Entities.ComunidadAutonoma>, IEntityTypeConfiguration<Core.Entities.ComunidadAutonoma>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ComunidadAutonoma> builder)
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
            .ToTable(nameof(Core.Entities.ComunidadAutonoma))
            .HasKey(x => new { x.IdComunidadAutonoma, x.AtDate });
    }
}

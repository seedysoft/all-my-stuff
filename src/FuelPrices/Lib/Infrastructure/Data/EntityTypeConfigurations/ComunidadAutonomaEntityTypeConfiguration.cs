using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ComunidadAutonomaEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : ComunidadAutonomaBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.IdComunidadAutonoma)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.NombreComunidadAutonoma)
            .IsRequired();

        base.Configure(builder);
    }
}

internal sealed class ComunidadAutonomaEntityTypeConfiguration : ComunidadAutonomaEntityTypeConfigurationBase<ComunidadAutonoma>, IEntityTypeConfiguration<ComunidadAutonoma>
{
    public override void Configure(EntityTypeBuilder<ComunidadAutonoma> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(ComunidadAutonoma))
            .HasKey(x => new { x.IdComunidadAutonoma });
    }
}

internal sealed class ComunidadAutonomaHistEntityTypeConfiguration : ComunidadAutonomaEntityTypeConfigurationBase<ComunidadAutonomaHist>, IEntityTypeConfiguration<ComunidadAutonomaHist>
{
    public override void Configure(EntityTypeBuilder<ComunidadAutonomaHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(ComunidadAutonomaHist))
            .HasKey(x => new { x.IdComunidadAutonoma, x.AtDate });
    }
}

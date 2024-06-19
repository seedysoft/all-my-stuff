using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ComunidadAutonomaEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.ComunidadAutonomaBase
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

internal sealed class ComunidadAutonomaEntityTypeConfiguration : ComunidadAutonomaEntityTypeConfigurationBase<Core.Entities.ComunidadAutonoma>, IEntityTypeConfiguration<Core.Entities.ComunidadAutonoma>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ComunidadAutonoma> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.ComunidadAutonoma))
            .HasKey(x => new { x.IdComunidadAutonoma });
    }
}

internal sealed class ComunidadAutonomaHistEntityTypeConfiguration : ComunidadAutonomaEntityTypeConfigurationBase<Core.Entities.ComunidadAutonomaHist>, IEntityTypeConfiguration<Core.Entities.ComunidadAutonomaHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ComunidadAutonomaHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.ComunidadAutonomaHist))
            .HasKey(x => new { x.IdComunidadAutonoma, x.AtDate });
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ComunidadAutonomaEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.ComunidadAutonoma>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.ComunidadAutonoma>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.ComunidadAutonoma> builder)
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
            .ToTable(nameof(Carburantes.CoreLib.Entities.ComunidadAutonoma))
            .HasKey(x => new { x.IdComunidadAutonoma, x.AtDate });
    }
}

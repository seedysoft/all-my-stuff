using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProvinciaEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.Provincia>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.Provincia>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.Provincia> builder)
    {
        base.Configure(builder);

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

        _ = builder
            .ToTable(nameof(Carburantes.CoreLib.Entities.Provincia))
            .HasKey(x => new { x.IdProvincia, x.AtDate });
    }
}

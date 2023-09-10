using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class ProvinciaEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.ProvinciaBase
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

internal class ProvinciaEntityTypeConfiguration : ProvinciaEntityTypeConfigurationBase<Core.Entities.Provincia>, IEntityTypeConfiguration<Core.Entities.Provincia>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.Provincia> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.Provincia))
            .HasKey(x => new { x.IdProvincia });
    }
}

internal class ProvinciaHistEntityTypeConfiguration : ProvinciaEntityTypeConfigurationBase<Core.Entities.ProvinciaHist>, IEntityTypeConfiguration<Core.Entities.ProvinciaHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ProvinciaHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.ProvinciaHist))
            .HasKey(x => new { x.IdProvincia, x.AtDate });
    }
}

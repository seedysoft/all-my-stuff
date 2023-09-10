using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal abstract class MunicipioEntityTypeConfigurationBase<T> : EntityTypeConfigurationBase<T> where T : Core.Entities.MunicipioBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.IdMunicipio)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.IdProvincia)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreMunicipio)
            .IsRequired();

        base.Configure(builder);
    }
}

internal class MunicipioEntityTypeConfiguration : MunicipioEntityTypeConfigurationBase<Core.Entities.Municipio>, IEntityTypeConfiguration<Core.Entities.Municipio>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.Municipio> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.Municipio))
            .HasKey(x => new { x.IdMunicipio });
    }
}

internal class MunicipioHistEntityTypeConfiguration : MunicipioEntityTypeConfigurationBase<Core.Entities.MunicipioHist>, IEntityTypeConfiguration<Core.Entities.MunicipioHist>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.MunicipioHist> builder)
    {
        base.Configure(builder);

        _ = builder
            .ToTable(nameof(Core.Entities.MunicipioHist))
            .HasKey(x => new { x.IdMunicipio, x.AtDate });
    }
}

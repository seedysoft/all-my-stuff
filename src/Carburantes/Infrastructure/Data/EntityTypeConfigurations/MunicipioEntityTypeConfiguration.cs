using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class MunicipioEntityTypeConfiguration : EntityTypeConfigurationBase<Core.Entities.Municipio>, IEntityTypeConfiguration<Core.Entities.Municipio>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.Municipio> builder)
    {
        base.Configure(builder);

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

        _ = builder
            .ToTable(nameof(Core.Entities.Municipio))
            .HasKey(x => new { x.IdMunicipio, x.AtDate });
    }
}

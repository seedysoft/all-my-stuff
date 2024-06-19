using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.CarburantesLib.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class MunicipioEntityTypeConfiguration : EntityTypeConfigurationBase<Carburantes.CoreLib.Entities.Municipio>, IEntityTypeConfiguration<Carburantes.CoreLib.Entities.Municipio>
{
    public override void Configure(EntityTypeBuilder<Carburantes.CoreLib.Entities.Municipio> builder)
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
            .ToTable(nameof(Carburantes.CoreLib.Entities.Municipio))
            .HasKey(x => new { x.IdMunicipio, x.AtDate });
    }
}

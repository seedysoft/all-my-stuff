using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class ProvinciaEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Provincia>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Provincia> builder)
    {
        _ = builder
            .Property(static x => x.ProvinciaId)
            .ValueGeneratedNever();

        _ = builder
            .Property(static x => x.ProvinciaDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Provincia))
            .HasKey(static x => new { x.PaisId, x.ProvinciaId });

        _ = builder
            .HasOne(static x => x.Pais)
            .WithMany(/*static x => x.Provincias*/)
            .HasForeignKey(static x => x.PaisId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.Provincia)}_{nameof(Core.Entities.Provincia.PaisId)}");
    }
}

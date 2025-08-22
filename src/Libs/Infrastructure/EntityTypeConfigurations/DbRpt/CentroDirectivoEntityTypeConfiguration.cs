using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class CentroDirectivoEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.CentroDirectivo>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.CentroDirectivo> builder)
    {
        _ = builder
            .Property(static x => x.CentroDirectivoId)
            .ValueGeneratedNever();

        _ = builder
            .Property(static x => x.CentroDirectivoDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.CentroDirectivo))
            .HasKey(static x => x.CentroDirectivoId);

        _ = builder
            .HasIndex(static e => e.MinisterioId, $"IX_{nameof(Core.Entities.CentroDirectivo)}_{nameof(Core.Entities.CentroDirectivo.MinisterioId)}");
        _ = builder
            .HasOne(static d => d.Ministerio)
            .WithMany(/*static p => p.CentrosDirectivos*/)
            .HasForeignKey(static d => d.MinisterioId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName($"FK_{nameof(Core.Entities.CentroDirectivo)}_{nameof(Core.Entities.CentroDirectivo.MinisterioId)}");
    }
}

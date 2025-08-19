using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations.DbRpt;

internal sealed class MinisterioEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Ministerio>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Ministerio> builder)
    {
        _ = builder
            .Property(static x => x.MinisterioId)
            .ValueGeneratedNever();

        _ = builder
            .Property(static x => x.MinisterioDenominacion);

        _ = builder
            .ToTable(nameof(Core.Entities.Ministerio))
            .HasKey(static x => x.MinisterioId);
    }
}

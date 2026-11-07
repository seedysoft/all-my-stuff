using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class TagEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Tag>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Tag> builder)
    {
        _ = builder
            .Property(static x => x.TagId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(static x => x.Text)
            .IsRequired();

        _ = builder
            .ToTable(nameof(Core.Entities.Tag))
            .HasKey(static x => x.TagId);

        _ = builder
            .Navigation(static e => e.Photos);
    }
}

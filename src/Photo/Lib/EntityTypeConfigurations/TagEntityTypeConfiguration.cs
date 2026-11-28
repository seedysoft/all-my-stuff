using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Photo.Lib.EntityTypeConfigurations;

internal sealed class TagEntityTypeConfiguration : IEntityTypeConfiguration<Entities.Tag>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.Tag> builder)
    {
        _ = builder
            .Property(static x => x.TagId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(static x => x.Text)
            .IsRequired();

        _ = builder
            .Property(static x => x.TagGuid)
            .HasColumnType("BLOB")
            .HasConversion(Libs.Infrastructure.ValueConverters.GuidBytes.GuidBytesValueConverter)
            .IsFixedLength();

        _ = builder
            .ToTable(nameof(Entities.Tag))
            .HasKey(static x => x.TagId);

        _ = builder
            .Navigation(static e => e.Photos);
    }
}

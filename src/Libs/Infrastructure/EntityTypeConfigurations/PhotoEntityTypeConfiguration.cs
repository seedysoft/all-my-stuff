using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class PhotoEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.Photo>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.Photo> builder)
    {
        _ = builder
            .Property(static x => x.PhotoId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        _ = builder
            .Property(static x => x.FilePath)
            .IsRequired();

        _ = builder
            .Property(static x => x.Latitude);

        _ = builder
            .Property(static x => x.Longitude);

        _ = builder
            .Property(static x => x.UpdatedAtDateTimeOffset)
            .HasConversion(ValueConverters.DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .ToTable(nameof(Core.Entities.Photo))
            .HasKey(static x => x.PhotoId);

        _ = builder
            .HasMany(static s => s.Tags)
            .WithMany(static s => s.Photos)
            .UsingEntity<Core.Entities.PhotoTag>(
                static s => s.HasOne<Core.Entities.Tag>().WithMany().HasForeignKey(static e => e.TagId).OnDelete(DeleteBehavior.Cascade),
                static s => s.HasOne<Core.Entities.Photo>().WithMany().HasForeignKey(static e => e.PhotoId).OnDelete(DeleteBehavior.Cascade),
                static s => s.ToTable($"{nameof(Core.Entities.Photo)}{nameof(Core.Entities.Tag)}").HasKey(static e => new { e.PhotoId, e.TagId }));

        _ = builder
            .Navigation(static e => e.Tags);
    }
}

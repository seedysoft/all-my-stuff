using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Photo.Lib.EntityTypeConfigurations;

internal sealed class PhotoEntityTypeConfiguration : IEntityTypeConfiguration<Entities.Photo>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.Photo> builder)
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
            .Property(static x => x.Year);

        _ = builder
            .Property(static x => x.PhotoGuid)
            .HasColumnType("BLOB")
            .HasConversion(Libs.Infrastructure.ValueConverters.GuidBytes.GuidBytesValueConverter)
            .IsFixedLength();

        //_ = builder
        //    .Property(static x => x.UpdatedAtDateTimeOffset)
        //    .HasConversion(Libs.Infrastructure.ValueConverters.DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .ToTable(nameof(Entities.Photo))
            .HasKey(static x => x.PhotoId);

        _ = builder
            .HasMany(static s => s.Tags)
            .WithMany(static s => s.Photos)
            .UsingEntity<Entities.PhotoTag>(
                static s => s.HasOne<Entities.Tag>().WithMany().HasForeignKey(static e => e.TagId).OnDelete(DeleteBehavior.Cascade),
                static s => s.HasOne<Entities.Photo>().WithMany().HasForeignKey(static e => e.PhotoId).OnDelete(DeleteBehavior.Cascade),
                static s => s.ToTable($"{nameof(Entities.Photo)}{nameof(Entities.Tag)}").HasKey(static e => new { e.PhotoId, e.TagId }));

        _ = builder
            .Navigation(static e => e.Tags);
    }
}

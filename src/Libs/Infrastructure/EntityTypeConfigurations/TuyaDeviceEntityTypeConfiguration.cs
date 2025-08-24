using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class TuyaDeviceEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.TuyaDevice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.TuyaDevice> builder)
    {
        // TODO Encrypt ???

        _ = builder
            .Property(static s => s.Id)
            .ValueGeneratedNever();

        _ = builder
            .Property(static s => s.Address)
            .HasConversion<string>();

        _ = builder
            .Property(static s => s.Version)
            .IsRequired();

        _ = builder
            .Property(static s => s.LocalKey)
            .IsRequired();

        _ = builder
            .ToTable(nameof(Core.Entities.TuyaDevice))
            .HasKey(static s => s.Id);
    }
}

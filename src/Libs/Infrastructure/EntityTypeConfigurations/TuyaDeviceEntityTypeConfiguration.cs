using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class TuyaDeviceEntityTypeConfiguration : IEntityTypeConfiguration<Core.Entities.TuyaDevice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.TuyaDevice> builder)
    {
        // TODO                           Encrypt

        _ = builder
            .Property(s => s.Id)
            .ValueGeneratedNever();

        _ = builder
            .Property(s => s.Address)
            .HasConversion<string>();

        _ = builder
            .Property(s => s.Version)
            .IsRequired();

        _ = builder
            .Property(s => s.LocalKey)
            .IsRequired();

        _ = builder
            .ToTable(nameof(Core.Entities.TuyaDevice))
            .HasKey(s => s.Id);
    }
}

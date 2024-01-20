using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal sealed class TuyaDeviceEntityTypeConfiguration : IEntityTypeConfiguration<CoreLib.Entities.TuyaDevice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.TuyaDevice> builder)
    {
        //                          TODO Encrypt

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
            .ToTable(nameof(CoreLib.Entities.TuyaDevice))
            .HasKey(s => s.Id);
    }
}

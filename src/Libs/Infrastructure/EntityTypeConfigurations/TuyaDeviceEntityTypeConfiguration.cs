using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Core.Entities;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal sealed class TuyaDeviceEntityTypeConfiguration : IEntityTypeConfiguration<TuyaDevice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TuyaDevice> builder)
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
            .ToTable(nameof(TuyaDevice))
            .HasKey(s => s.Id);
    }
}

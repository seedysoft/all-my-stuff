﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.Carburantes.Infrastructure.Data.EntityTypeConfigurations;

internal sealed class ProductoPetroliferoEntityTypeConfiguration : EntityTypeConfigurationBase<Core.Entities.ProductoPetrolifero>, IEntityTypeConfiguration<Core.Entities.ProductoPetrolifero>
{
    public override void Configure(EntityTypeBuilder<Core.Entities.ProductoPetrolifero> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.IdProducto)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder
            .Property(x => x.NombreProducto)
            .IsRequired();

        _ = builder
            .Property(x => x.NombreProductoAbreviatura)
            .IsRequired();

        _ = builder
            .ToTable(nameof(Core.Entities.ProductoPetrolifero))
            .HasKey(x => new { x.IdProducto, x.AtDate });
    }
}

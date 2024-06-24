using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.Libs.FuelPrices.Core.Entities.Core;

namespace Seedysoft.Libs.FuelPrices.Infrastructure.Data.EntityTypeConfigurations;

public abstract class EntityTypeConfigurationBase<T> : IEntityTypeConfiguration<T> where T : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.AtDate);
    }
}

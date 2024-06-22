using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seedysoft.FuelPrices.Lib.Core.Entities.Core;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

public abstract class EntityTypeConfigurationBase<T> : IEntityTypeConfiguration<T> where T : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.AtDate);
    }
}

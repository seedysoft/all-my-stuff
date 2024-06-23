using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Seedysoft.FuelPrices.Lib.Infrastructure.Data.EntityTypeConfigurations;

public abstract class EntityTypeConfigurationBase<T> : IEntityTypeConfiguration<T> where T : Core.Entities.Core.EntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.AtDate);
    }
}

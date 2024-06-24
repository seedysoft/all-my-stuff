using Microsoft.EntityFrameworkCore;
using Seedysoft.Libs.Infrastructure.ValueConverters;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal abstract class WebDataEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : Core.Entities.WebDataBase
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(x => x.SubscriptionId);

        _ = builder
            .Property(x => x.WebUrl)
            .IsRequired();

        _ = builder
            .Property(x => x.Description)
            .IsRequired();

        _ = builder
            .Property(x => x.CurrentWebContent);

        _ = builder
            .Property(x => x.SeenAtDateTimeOffset)
            .HasConversion(DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.UpdatedAtDateTimeOffset)
            .HasConversion(DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.IgnoreChangeWhen);

        _ = builder
            .Property(x => x.CssSelector)
            .HasDefaultValue("body");

        _ = builder
            .Property(x => x.TakeAboveBelowLines)
            .HasDefaultValue(3);

        _ = builder
            .Property(x => x.UseHttpClient)
            .HasDefaultValue(false);
    }
}

internal sealed class WebDataEntityTypeConfiguration : WebDataEntityTypeConfigurationT<Core.Entities.WebData>, IEntityTypeConfiguration<Core.Entities.WebData>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.WebData> builder)
    {
        base.Configure(builder);

        _ = builder
            .Ignore(x => x.DataToSend);

        _ = builder
            .ToTable(nameof(Core.Entities.WebData))
            .HasKey(x => x.SubscriptionId);
    }
}

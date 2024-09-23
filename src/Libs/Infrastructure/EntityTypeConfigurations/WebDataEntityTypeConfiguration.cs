using Microsoft.EntityFrameworkCore;

namespace Seedysoft.Libs.Infrastructure.EntityTypeConfigurations;

internal abstract class WebDataEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : Core.Entities.WebDataBase
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> builder)
    {
        _ = builder
            .Property(static x => x.SubscriptionId);

        _ = builder
            .Property(static x => x.WebUrl)
            .IsRequired();

        _ = builder
            .Property(static x => x.Description)
            .IsRequired();

        _ = builder
            .Property(static x => x.CurrentWebContent);

        _ = builder
            .Property(static x => x.SeenAtDateTimeOffset)
            .HasConversion(ValueConverters.DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .Property(static x => x.UpdatedAtDateTimeOffset)
            .HasConversion(ValueConverters.DateTimeOffsetString.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .Property(static x => x.IgnoreChangeWhen);

        _ = builder
            .Property(static x => x.CssSelector)
            .HasDefaultValue("body");

        _ = builder
            .Property(static x => x.TakeAboveBelowLines)
            .HasDefaultValue(3);

        _ = builder
            .Property(static x => x.UseHttpClient)
            .HasDefaultValue(false);
    }
}

internal sealed class WebDataEntityTypeConfiguration
    : WebDataEntityTypeConfigurationT<Core.Entities.WebData>, IEntityTypeConfiguration<Core.Entities.WebData>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Core.Entities.WebData> builder)
    {
        base.Configure(builder);

        _ = builder
            .Ignore(static x => x.DataToSend);

        _ = builder
            .ToTable(nameof(Core.Entities.WebData))
            .HasKey(static x => x.SubscriptionId);
    }
}

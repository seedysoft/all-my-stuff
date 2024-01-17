using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal abstract class WebDataEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : CoreLib.Entities.WebDataBase
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
            .HasConversion(UtilsLib.Constants.ValueConverters.NullableDateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.UpdatedAtDateTimeOffset)
            .HasConversion(UtilsLib.Constants.ValueConverters.NullableDateTimeOffsetStringValueConverter);

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

internal sealed class WebDataEntityTypeConfiguration : WebDataEntityTypeConfigurationT<CoreLib.Entities.WebData>, IEntityTypeConfiguration<CoreLib.Entities.WebData>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.WebData> builder)
    {
        base.Configure(builder);

        _ = builder
            .Ignore(x => x.DataToSend);

        _ = builder
            .ToTable(nameof(CoreLib.Entities.WebData))
            .HasKey(x => x.SubscriptionId);
    }
}

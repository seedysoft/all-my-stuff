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
            .HasConversion(UtilsLib.Constants.ValueConverters.DateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.UpdatedAtDateTimeOffset)
            .HasConversion(UtilsLib.Constants.ValueConverters.DateTimeOffsetStringValueConverter);

        _ = builder
            .Property(x => x.IgnoreChangeWhen);

        _ = builder
            .Property(x => x.CssSelector)
            .HasDefaultValue("body");

        _ = builder
            .Property(x => x.TakeAboveBelowLines)
            .HasDefaultValue(5);
    }
}

internal class WebDataEntityTypeConfiguration : WebDataEntityTypeConfigurationT<CoreLib.Entities.WebData>, IEntityTypeConfiguration<CoreLib.Entities.WebData>
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

internal class WebDataViewEntityTypeConfiguration : WebDataEntityTypeConfigurationT<CoreLib.Entities.WebDataView>, IEntityTypeConfiguration<CoreLib.Entities.WebDataView>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CoreLib.Entities.WebDataView> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.SeenAtDateTimeUnix);

        _ = builder
            .Property(x => x.UpdatedAtDateTimeUnix);

        _ = builder
            .ToView(nameof(CoreLib.Entities.WebDataView))
            .HasNoKey();
    }
}

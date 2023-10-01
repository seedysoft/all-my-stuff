using Microsoft.EntityFrameworkCore;

namespace Seedysoft.InfrastructureLib.EntityTypeConfigurations;

internal abstract class WebDataEntityTypeConfigurationT<T> : IEntityTypeConfiguration<T> where T : Entities.WebDataBase
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

internal class WebDataEntityTypeConfiguration : WebDataEntityTypeConfigurationT<Entities.WebData>, IEntityTypeConfiguration<Entities.WebData>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.WebData> builder)
    {
        base.Configure(builder);

        _ = builder
            .Ignore(x => x.DataToSend);

        _ = builder
            .ToTable(nameof(Entities.WebData))
            .HasKey(x => x.SubscriptionId);
    }
}

internal class WebDataViewEntityTypeConfiguration : WebDataEntityTypeConfigurationT<Entities.WebDataView>, IEntityTypeConfiguration<Entities.WebDataView>
{
    public new void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.WebDataView> builder)
    {
        base.Configure(builder);

        _ = builder
            .Property(x => x.SeenAtDateTimeUnix);

        _ = builder
            .Property(x => x.UpdatedAtDateTimeUnix);

        _ = builder
            .ToView(nameof(Entities.WebDataView))
            .HasNoKey();
    }
}

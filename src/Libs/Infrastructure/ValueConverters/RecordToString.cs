namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class RecordToString<T> where T : Core.Entities.MasterFilesBase
{
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<T?, string?> NullableMasterFilesBaseStringValueConverter
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
        get
        {
            return new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<T?, string?>(
                convertToProviderExpression: static masterFile => masterFile == null ? null : masterFile.ToString(),
                convertFromProviderExpression: static value => value as T);
        }
    }
}

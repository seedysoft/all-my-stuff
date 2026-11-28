using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Seedysoft.Libs.Infrastructure.ValueConverters;

public static class GuidBytes
{
    public static ValueConverter<Guid, byte[]> GuidBytesValueConverter
    {
        get
        {
            return new ValueConverter<Guid, byte[]>(
                convertToProviderExpression: static guid => guid.ToByteArray(),
                convertFromProviderExpression: static bytes => new Guid(bytes));
        }
    }
    public static ValueConverter<Guid?, byte[]?> NullableGuidBytesValueConverter
    {
        get
        {
            return new ValueConverter<Guid?, byte[]?>(
                convertToProviderExpression: static guid => guid.HasValue ? guid.Value.ToByteArray() : default,
                convertFromProviderExpression: static bytes => bytes == null ? default : new Guid(bytes));
        }
    }
}

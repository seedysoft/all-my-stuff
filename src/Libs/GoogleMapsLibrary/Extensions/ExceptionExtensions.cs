namespace GoogleMapsLibrary.Extensions;

public static class ExceptionExtensions
{
    public static bool HasInnerExceptionsOfType<T>(this Exception ex) where T : Exception => ex.GetInnerExceptionsOfType<T>().Any();

    public static IEnumerable<T> GetInnerExceptionsOfType<T>(this Exception ex) where T : Exception
    {
        Exception?[] candidates = [ex, ex.InnerException];

        if (ex is AggregateException aggEx)
        {
            Exception[] innerEceptions = [.. aggEx.InnerExceptions];
            candidates = candidates.Concat(innerEceptions).Where(x => x is not null).ToArray();
        }

        IEnumerable<T> exceptions = candidates.Select(x => x as T).Where(x => x is not null).Cast<T>().Distinct();

        return exceptions;
    }
}

using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.Utils.Extensions;

public static class ILoggerExtensions
{
    public static bool LogAndHandle(this ILogger logger, Exception? exception, string? message, params object?[] args)
    {
        logger.LogError(exception, message, args);

        return true;
    }

    //public static bool LogThenRaise(this ILogger logger, Exception? exception, string message, params object?[] args)
    //{
    //    logger.LogError(exception, message, args);

    //    return false;
    //}
}

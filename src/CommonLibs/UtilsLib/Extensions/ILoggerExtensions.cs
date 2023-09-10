using Microsoft.Extensions.Logging;

namespace Seedysoft.UtilsLib.Extensions;

public static class ILoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> _Critical;
    private static readonly Action<ILogger, string, Exception> _Handle;
    private static readonly Action<ILogger, string, Exception?> _Information;

    private static readonly EventId _EventId = new(1, "Log information");

    static ILoggerExtensions()
    {
        _Critical = LoggerMessage.Define<string>(LogLevel.Critical, 1, "{Message}");
        _Handle = LoggerMessage.Define<string>(LogLevel.Error, 1, "{Message}");
        _Information = LoggerMessage.Define<string>(LogLevel.Information, _EventId, "{Message}");
    }

    public static void Critical(this ILogger logger, string message) => _Critical(logger, message, null);
    public static bool Handle(this ILogger logger, Exception exception, string message) { _Handle(logger, message, exception); return true; }
    public static void Information(this ILogger logger, string message) => _Information(logger, message, null);

    public static bool LogAndHandle(this ILogger logger, Exception? exception, string? message, params object?[] args)
    {
#pragma warning disable CA2254 // Template should be a static expression
        logger.LogError(exception, message, args);
#pragma warning restore CA2254 // Template should be a static expression

        return true;
    }

    public static bool LogThenRaise(this ILogger logger, Exception? exception, string? message, params object?[] args)
    {
#pragma warning disable CA2254 // Template should be a static expression
        logger.LogError(exception, message, args);
#pragma warning restore CA2254 // Template should be a static expression

        return false;
    }
}

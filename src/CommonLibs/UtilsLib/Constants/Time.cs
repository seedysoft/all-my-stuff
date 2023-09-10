namespace Seedysoft.UtilsLib.Constants;

public static class Time
{
    public static readonly TimeSpan OneTenthOfSecondTimeSpan = TimeSpan.FromSeconds(0.1);

    //public static readonly TimeSpan OneSecondTimeSpan = TimeSpan.FromSeconds(1);

#if DEBUG
    public static readonly TimeSpan TenSecondsTimeSpan = TimeSpan.FromSeconds(10);
#endif
}

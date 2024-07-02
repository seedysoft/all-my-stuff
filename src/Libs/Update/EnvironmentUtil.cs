namespace Seedysoft.Libs.Update;

public static class EnvironmentUtil
{
    public static string Version()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        return $"v{fvi.ProductVersion}";
    }

    public static string InstallationPath() => Path.GetDirectoryName(ExecutablePath())!;

    public static string ExecutablePath() => System.Reflection.Assembly.GetEntryAssembly()?.Location!;

    //    public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

    //    public static bool IsDebug =>
    //#if DEBUG
    //            true;
    //#else
    //            false;
    //#endif
}

namespace Seedysoft.Libs.Update;

public static partial class EnvironmentUtil
{
    [System.Text.RegularExpressions.GeneratedRegex(
        @"(?<major>\d{2})\.(?<minor>\d{2,4})\.(?<build>\d{2,4})",
        System.Text.RegularExpressions.RegexOptions.Compiled)]
    private static partial System.Text.RegularExpressions.Regex ReleaseVersionRegex();
    private static readonly System.Text.RegularExpressions.Regex _VersionRegex = ReleaseVersionRegex();

    public static Version? ParseVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return null;

        System.Text.RegularExpressions.Match parsed = _VersionRegex.Match(version);

        int major;
        int minor;
        int build;

        if (parsed.Success)
        {
            major = Convert.ToInt32(parsed.Groups["major"].Value);
            minor = Convert.ToInt32(parsed.Groups["minor"].Value);
            build = Convert.ToInt32(parsed.Groups["build"].Value);
        }
        else
        {
            major = 0;
            minor = 0;
            build = 0;
        }

        return new Version(major, minor, build);
    }

    public static string MyVersion()
    {
        var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);

        return $"v{fvi.ProductVersion}";
    }

    //public static string InstallationPath() => Path.GetDirectoryName(ExecutablePath())!;

    public static string ExecutablePath() => System.Reflection.Assembly.GetEntryAssembly()?.Location!;

    public static string GetUpdaterFileName() => $"{nameof(Seedysoft)}.{nameof(Update)}.ConsoleApp{(IsWindows ? ".exe" : string.Empty)}";

    public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

    //    public static bool IsDebug =>
    //#if DEBUG
    //            true;
    //#else
    //            false;
    //#endif
}

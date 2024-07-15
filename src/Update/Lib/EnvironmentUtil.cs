using System.Text.RegularExpressions;

namespace Seedysoft.Libs.Update;

public static partial class EnvironmentUtil
{
    [GeneratedRegex(@"v(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)", RegexOptions.Compiled)]
    private static partial Regex ReleaseVersionRegex();
    private static readonly Regex _VersionRegex = ReleaseVersionRegex();

    public static Version? ParseVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return null;

        Match parsed = _VersionRegex.Match(version);

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

    public static string Version()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        return $"v{fvi.ProductVersion}";
    }

    public static string InstallationPath() => Path.GetDirectoryName(ExecutablePath())!;

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

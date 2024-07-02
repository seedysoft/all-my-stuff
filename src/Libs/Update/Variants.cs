using System.Reflection;
using System.Runtime.InteropServices;

namespace Seedysoft.Libs.Update;

public class Variants
{
    public enum Variant
    {
        NotFound,
        CoreWindows,
        CoreLinuxAmdx64,
        CoreLinuxArm64,
    }

    public static Variant GetVariant()
    {
        bool isRunningOnDotNetCore;
        try
        {
            isRunningOnDotNetCore = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?
                .FrameworkName
                .IndexOf("Core", StringComparison.OrdinalIgnoreCase) >= 0;
        }
        catch
        {
            //Issue only appears to occur for small number of users on Mono
            isRunningOnDotNetCore = false;
        }

        if (isRunningOnDotNetCore)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Variant.CoreWindows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                switch (RuntimeInformation.ProcessArchitecture)
                {
                    case Architecture.Arm64:
                        return Variant.CoreLinuxArm64;
                    case Architecture.X64:
                        return Variant.CoreLinuxAmdx64;
                }
            }
        }

        return Variant.NotFound;
    }

    public static string GetArtifactFileName(Variant variant)
    {
        return variant switch
        {
            Variant.CoreWindows => "Seedysoft.Binaries.Windows.zip",
            Variant.CoreLinuxAmdx64 or Variant.CoreLinuxArm64 => "Seedysoft.Binaries.LinuxARM64.tar.gz",
            _ => string.Empty,
        };
    }

    public static bool IsNonWindowsDotNetCoreVariant(Variant variant) => variant is Variant.CoreLinuxAmdx64 or Variant.CoreLinuxArm64;
}

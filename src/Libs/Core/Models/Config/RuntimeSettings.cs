namespace Seedysoft.Libs.Core.Models.Config;

public record RuntimeSettings
{
    public short SecondsToDelayWebApplicationStart { get; set; }

    //public bool TracingEnabled { get; set; }

    //public bool LogRequests { get; set; }

    //public string? ClientOverride { get; set; }

    //public bool? IgnoreSslErrors { get; set; }

    //public string? CustomDataFolder { get; set; }

    //public string? BasePath { get; set; }

    //public bool NoRestart { get; set; }

    //public string? CustomLogFileName { get; set; }

    //public string? PIDFile { get; set; }

    //public bool NoUpdates { get; set; }

    //public string GetDataFolder()
    //{
    //    return string.IsNullOrWhiteSpace(CustomDataFolder)
    //        ? Environment.OSVersion.Platform == PlatformID.Unix
    //            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Jackett")
    //            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Jackett")
    //        : CustomDataFolder;
    //}

    public static Dictionary<string, string?> GetValues(RuntimeSettings obj)
    {
        return obj
            .GetType()
            .GetProperties()
            .ToDictionary(p => "RuntimeSettings:" + p.Name, p => p.GetValue(obj)?.ToString());
    }
}

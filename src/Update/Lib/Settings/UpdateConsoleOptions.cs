namespace Seedysoft.Update.Lib.Settings;

public record UpdateConsoleOptions : Libs.Core.Models.Config.ConsoleOptions
{
    public static new UpdateConsoleOptions Default
        => Build(delayStart: 0, /*launchDebugger: false, */installDirectory: Directory.GetCurrentDirectory());

    public static UpdateConsoleOptions Build(ushort delayStart, /*bool launchDebugger, */string installDirectory)
    {
        var updateConsoleOptions = (UpdateConsoleOptions)Build<UpdateConsoleOptions>(delayStart/*, launchDebugger*/);

        updateConsoleOptions.InstallDirectory = installDirectory;

        return updateConsoleOptions;
    }

    [CommandLine.Option(
        shortName: 'i',
        longName: $"{nameof(InstallDirectory)}",
        HelpText = "Specify the location where updated files must be copied.",
        Required = true)]
    public string InstallDirectory { get; set; } = default!;

    public override string ToString()
    {
        IEnumerable<string> options =
            from p in GetType().GetProperties()
            let o = p.GetCustomAttributes(false).OfType<CommandLine.OptionAttribute>().FirstOrDefault()
            where o != null
            select $"-{o.ShortName} \"{p.GetValue(this) ?? o.Default}\"";

        return string.Join(" ", options);
    }
}

namespace Seedysoft.Update.Lib.Settings;

public class ConsoleOptions : Libs.Core.Models.Config.ConsoleOptions
{
    [CommandLine.Option('f', $"{nameof(UpdateFilesFolder)}", HelpText = "Specify the location where files are extracted.")]
    public required string UpdateFilesFolder { get; set; }
}

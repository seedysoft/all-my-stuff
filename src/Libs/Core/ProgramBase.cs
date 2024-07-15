using CommandLine;

namespace Seedysoft.Libs.Core;

public class ProgramBase
{
    public static Models.Config.RuntimeSettings Settings { get; set; } = default!;

    public static async Task<T> ObtainCommandLineAsync<T>(string[] args) where T : Models.Config.ConsoleOptions
    {
        Parser commandLineParser = new(settings => settings.CaseSensitive = false);
        ParserResult<T> parserResult = commandLineParser.ParseArguments<T>(args);

        _ = parserResult.WithNotParsed(errors =>
        {
            var text = CommandLine.Text.HelpText.AutoBuild(parserResult);
            text.Copyright = " ";
            text.Heading = "All My Stuff " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(text);
            Environment.Exit(1);
        });

        _ = parserResult.WithParsed(parsedValues => Settings = parsedValues.ToRunTimeSettings());

        if (Settings.LaunchDebugger)
            _ = System.Diagnostics.Debugger.Launch();

        for (int i = Settings.SecondsToDelayWebApplicationStart; i > 0; --i)
        {
            Console.Write("\rWaiting for {0} seconds   ", i);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        Console.Write("\rStarting ...              ");
        Console.WriteLine(string.Empty);

        return parserResult.Value;
    }
}

using CommandLine;

namespace Seedysoft.Libs.Core;

public class ProgramBase
{
    public static Models.Config.RuntimeSettings Settings { get; set; } = default!;

    public static async Task ObtainCommandLineAsync(string[] args)
    {
        Parser commandLineParser = new(settings => settings.CaseSensitive = false);
        ParserResult<Models.Config.ConsoleOptions> parserResult = commandLineParser.ParseArguments<Models.Config.ConsoleOptions>(args);

        _ = parserResult.WithNotParsed(errors =>
        {
            var text = CommandLine.Text.HelpText.AutoBuild(parserResult);
            text.Copyright = " ";
            text.Heading = "All My Stuff " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(text);
            Environment.Exit(1);
        });

        Models.Config.ConsoleOptions consoleOptions = new();
        _ = parserResult.WithParsed(options =>
        {
            //if (string.IsNullOrEmpty(options.Client))
            //    options.Client = DotNetCoreUtil.IsRunningOnDotNetCore ? "httpclient2" : "httpclient";

            Settings = options.ToRunTimeSettings();
            consoleOptions = options;
        });

        for (int i = Settings.SecondsToDelayWebApplicationStart; i > 0; --i)
        {
            Console.Write("\rWaiting for {0} seconds   ", i);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        Console.Write("\rStarting ...              ");
        Console.WriteLine(string.Empty);
    }
}

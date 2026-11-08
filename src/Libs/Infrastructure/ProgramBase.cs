using CommandLine;
using Microsoft.Extensions.Configuration;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.Infrastructure;

public class ProgramBase
{
    public static Core.Models.Config.RuntimeSettings Settings { get; set; } = default!;

    public static void ObtainCommandLineArgs(string[] args)
    {
        Parser commandLineParser = new(settings => settings.CaseSensitive = false);
        ParserResult<Core.Models.Config.ConsoleOptions> parserResult = commandLineParser.ParseArguments<Core.Models.Config.ConsoleOptions>(args);

        _ = parserResult.WithNotParsed(errors =>
        {
            var text = CommandLine.Text.HelpText.AutoBuild(parserResult);
            text.Copyright = " ";
            text.Heading = "All My Stuff " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(text);
            Environment.Exit(1);
        });

        Core.Models.Config.ConsoleOptions consoleOptions = new();
        _ = parserResult.WithParsed(options =>
        {
            //if (string.IsNullOrEmpty(options.Client))
            //    options.Client = DotNetCoreUtil.IsRunningOnDotNetCore ? "httpclient2" : "httpclient";

            Settings = options.ToRunTimeSettings();
            consoleOptions = options;
        });

        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types#composite-formatting
        for (int i = Settings.SecondsToDelayWebApplicationStart; i > 0; --i)
        {
            Console.Write("{0,-30}", $"\rWaiting for {i} seconds");
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        Console.Write("{0,-30}", "\rStarting ...");
        Console.WriteLine(string.Empty);
    }

    public static Microsoft.Extensions.Hosting.HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        ObtainCommandLineArgs(args);

        Microsoft.Extensions.Hosting.HostApplicationBuilder hostApplicationBuilder = new(args);

        // TODO         Learn how it works
        _ = hostApplicationBuilder.Configuration.AddInMemoryCollection(Core.Models.Config.RuntimeSettings.GetValues(Settings));

        return hostApplicationBuilder;
    }
}

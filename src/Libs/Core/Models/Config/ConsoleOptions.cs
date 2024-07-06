namespace Seedysoft.Libs.Core.Models.Config;

public class ConsoleOptions
{
    [CommandLine.Option(shortName: 'd', longName: "delay", HelpText = "Time in seconds that the web application will wait until it starts")]
    public short DelayStart { get; set; }

    //[CommandLine.Option('i', "Install", HelpText = "Install Jackett windows service (Must be admin)")]
    //public bool Install { get; set; }

    //[CommandLine.Option('r', "ReserveUrls", HelpText = "(Re)Register windows port reservations (Required for listening on all interfaces).")]
    //public bool ReserveUrls { get; set; }

    //[CommandLine.Option('u', "Uninstall", HelpText = "Uninstall Jackett windows service (Must be admin).")]
    //public bool Uninstall { get; set; }

    //[CommandLine.Option('l', "Logging", HelpText = "Log all requests/responses to Jackett")]
    //public bool Logging { get; set; }

    //[CommandLine.Option('t', "Tracing", HelpText = "Enable tracing")]
    //public bool Tracing { get; set; }

    //[CommandLine.Option('c', "UseClient", HelpText = "Override web client selection. [automatic(Default)/httpclient/httpclient2]")]
    //public string Client { get; set; } = "automatic";

    //[CommandLine.Option('s', "Start", HelpText = "Start the Jacket Windows service (Must be admin)")]
    //public bool StartService { get; set; }

    //[CommandLine.Option('k', "Stop", HelpText = "Stop the Jacket Windows service (Must be admin)")]
    //public bool StopService { get; set; }

    //[CommandLine.Option('x', "ListenPublic", HelpText = "Listen publicly")]
    //public bool ListenPublic { get; set; }

    //[CommandLine.Option('z', "ListenPrivate", HelpText = "Only allow local access")]
    //public bool ListenPrivate { get; set; }

    //[CommandLine.Option('p', "Port", HelpText = "Web server port")]
    //public int Port { get; set; }

    //[CommandLine.Option('n', "IgnoreSslErrors", HelpText = "[true/false] Ignores invalid SSL certificates")]
    //public bool? IgnoreSslErrors { get; set; }

    //[CommandLine.Option('d', "DataFolder", HelpText = "Specify the location of the data folder (Must be admin on Windows) eg. --DataFolder=\"D:\\Your Data\\Jackett\\\". Don't use this on Unix (mono) systems. On Unix just adjust the HOME directory of the user to the datadir or set the XDG_CONFIG_HOME environment variable.")]
    //public string? DataFolder { get; set; }

    //[CommandLine.Option("NoRestart", HelpText = "Don't restart after update")]
    //public bool NoRestart { get; set; }

    //[CommandLine.Option("PIDFile", HelpText = "Specify the location of PID file")]
    //public string? PIDFile { get; set; }

    //[CommandLine.Option("NoUpdates", HelpText = "Disable automatic updates")]
    //public bool NoUpdates { get; set; }

    public RuntimeSettings ToRunTimeSettings()
    {
        ConsoleOptions options = this;
        RuntimeSettings runtimeSettings = new()
        {
            SecondsToDelayWebApplicationStart = options.DelayStart
        };

        //// Logging
        //if (options.Logging)
        //    runtimeSettings.LogRequests = true;

        //// Tracing
        //if (options.Tracing)
        //    runtimeSettings.TracingEnabled = true;

        //if (options.ListenPublic && options.ListenPrivate)
        //{
        //    Console.WriteLine("You can only use listen private OR listen publicly.");
        //    Environment.Exit(1);
        //}

        //// Use curl
        //if (options.Client != null)
        //    runtimeSettings.ClientOverride = options.Client.ToLowerInvariant();

        //// Ignore SSL errors on Curl
        //runtimeSettings.IgnoreSslErrors = options.IgnoreSslErrors;

        //runtimeSettings.NoRestart = options.NoRestart;
        //runtimeSettings.NoUpdates = options.NoUpdates;

        //if (!string.IsNullOrWhiteSpace(options.DataFolder))
        //    runtimeSettings.CustomDataFolder = options.DataFolder;

        //runtimeSettings.PIDFile = options.PIDFile;

        return runtimeSettings;
    }
}

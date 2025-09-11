using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Update.Services;

public sealed class UpdaterCronBackgroundService : BackgroundServices.Cron
{
    private readonly Octokit.GitHubClient gitHubClient;
    private readonly ILogger<UpdaterCronBackgroundService> Logger;
    private Settings.UpdateSettings Settings => (Settings.UpdateSettings)Config;

    public UpdaterCronBackgroundService(IServiceProvider serviceProvider, Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        gitHubClient = ServiceProvider.GetRequiredService<Octokit.GitHubClient>();
        Logger = ServiceProvider.GetRequiredService<ILogger<UpdaterCronBackgroundService>>();

        Config = ServiceProvider.GetRequiredService<IConfiguration>()
            .GetSection(nameof(Update.Settings.UpdateSettings)).Get<Settings.UpdateSettings>()!;
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();

        string? AppName = GetType().FullName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            Enums.UpdateResults UpgradeResult = await DownloadLatestReleaseAsset();

            Logger.LogInformation($"Updating result: {UpgradeResult}");
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    internal async Task<Enums.UpdateResults> DownloadLatestReleaseAsset()
    {
        Octokit.Release? release = await GetLatestReleaseFromGithubAsync();
        if (release == null)
        {
            Logger.LogError("LatestReleaseFromGithub is null.");
            return Enums.UpdateResults.LatestReleaseFromGithubIsNull;
        }

        Octokit.ReleaseAsset? releaseAsset =
            release.Assets.FirstOrDefault(x => x.Name.Contains(System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier, StringComparison.InvariantCultureIgnoreCase));
        if (releaseAsset == null)
        {
            Logger.LogError($"Asset not found for {System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier}.");
            return Enums.UpdateResults.AssetNotFound;
        }

        System.Reflection.Assembly EntryAssembly = System.Reflection.Assembly.GetEntryAssembly()!;
        Version CurrentVersion = EntryAssembly.GetName().Version ?? new Version();
        Version NewVersion = new(release.Name);

        if (NewVersion <= CurrentVersion)
        {
            Logger.LogInformation($"Current version is: {CurrentVersion}. Latest version is: {NewVersion}");
            return Enums.UpdateResults.NoNewVersionFound;
        }
        // Here, NewVersion is greather than CurrentVersion

        return ExecuteUpdateScript(Path.GetDirectoryName(EntryAssembly.Location)!, releaseAsset.Name)
            ? Enums.UpdateResults.Ok
            : Enums.UpdateResults.ErrorExecutingUpdateScript;
    }

    internal async Task<Octokit.Release?> GetLatestReleaseFromGithubAsync()
    {
        // Retrieve a List of Releases in the Repository, and get latest using [0]-subscript
        IReadOnlyList<Octokit.Release> releases =
            await gitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName);

        Logger.LogInformation("Obtained {releasesCount} releases", releases.Count);

        return releases.Any() ? releases[0] : null;
    }

    internal bool ExecuteUpdateScript(string asssemblyLocation, string assetName)
    {
        Logger.LogInformation("Asssembly location is '{asssemblyLocation}'", asssemblyLocation);

        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            CreateNoWindow = true,
            UseShellExecute = false,
        };

        string runtimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
        switch (runtimeIdentifier)
        {
            case Core.Constants.SupportedRuntimeIdentifiers.LinuxArm64:
                //case Core.Constants.SupportedRuntimeIdentifiers.LinuxX64:
                processStartInfo.FileName = "/bin/bash";
                processStartInfo.Arguments = $"-c \"sudo systemd-run --on-active=30 {Path.Combine(asssemblyLocation, "update.sh")} {assetName}\"";
                break;

            //case Core.Constants.SupportedRuntimeIdentifiers.WinX64:
            //    processStartInfo.FileName = $"update.bat {zipFileName}";
            //    break;

            default:
                Logger.LogError($"RuntimeIdentifier {runtimeIdentifier} not supported");
                return false;
        }

        return System.Diagnostics.Process.Start(processStartInfo) != null;
    }
}

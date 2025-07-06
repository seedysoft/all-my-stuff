using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Update.Services;

public sealed class UpdaterCronBackgroundService : BackgroundServices.Cron
{
    private readonly Octokit.GitHubClient GitHubClient;
    private readonly ILogger<UpdaterCronBackgroundService> Logger;

    public UpdaterCronBackgroundService(IServiceProvider serviceProvider, Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        GitHubClient = serviceProvider.GetRequiredService<Octokit.GitHubClient>();
        //string GitHubToken = "--- token goes here ---";
        //var tokenAuth = new Octokit.Credentials(GitHubToken);
        //client.Credentials = tokenAuth;

        Config = ServiceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Settings.UpdateSettings)).Get<Settings.UpdateSettings>()!;

        Logger = ServiceProvider.GetRequiredService<ILogger<UpdaterCronBackgroundService>>();
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();

        string? AppName = GetType().FullName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            Enums.UpdateResults UpgradeResult = await CheckAndUpgradeToNewVersion();

            Logger.LogInformation($"Updating result: {UpgradeResult}");
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    internal async Task<Enums.UpdateResults> CheckAndUpgradeToNewVersion()
    {
        Octokit.Release? release = await GetLatestReleaseFromGithubAsync();
        if (release == null)
        {
            Logger.LogError("LatestReleaseFromGithub is null.");
            return Enums.UpdateResults.LatestReleaseFromGithubIsNull;
        }

        var ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        Version? CurrentVersion = ExecutingAssembly.GetName().Version;
        Version? NewVersion = new(release.Name);

        if (NewVersion <= CurrentVersion)
        {
            Logger.LogInformation($"Current version is: {CurrentVersion}. Latest version is: {NewVersion}");
            return Enums.UpdateResults.NoNewVersionFound;
        }

        Octokit.ReleaseAsset? asset =
            release.Assets.FirstOrDefault(x => x.Name.Contains(System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier, StringComparison.InvariantCultureIgnoreCase));
        if (asset == null)
        {
            Logger.LogError($"Asset not found for {System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier}.");
            return Enums.UpdateResults.AssetNotFound;
        }

        if (!ExecuteUpdateScript(new Uri(asset.BrowserDownloadUrl)))
        {
            Logger.LogError("Cannot execute update script");
            return Enums.UpdateResults.ErrorExecutingUpdateScript;
        }

        Logger.LogInformation("Update in progress...");

        Environment.Exit(0);

        return Enums.UpdateResults.Ok;
    }

    internal async Task<Octokit.Release?> GetLatestReleaseFromGithubAsync()
    {
        // Retrieve a List of Releases in the Repository, and get latest using [0]-subscript
        IReadOnlyList<Octokit.Release> releases =
            await GitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName);

        Logger.LogInformation("Obtained {releasesCount} releases", releases.Count);

        return releases.Any() ? releases[0] : null;
    }

    internal bool ExecuteUpdateScript(Uri assetUri)
    {
        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            CreateNoWindow = false,
            UseShellExecute = true,
        };

        string runtimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
        switch (runtimeIdentifier)
        {
            case Core.Constants.SupportedRuntimeIdentifiers.LinuxArm64:
                //case Core.Constants.SupportedRuntimeIdentifiers.LinuxX64:
                processStartInfo.FileName = $"sudo ./update.sh {assetUri}";
                break;

            //case Core.Constants.SupportedRuntimeIdentifiers.WinX64:
            //    processStartInfo.FileName = $"update.bat {zipFileName}";
            //    break;

            default:
                // TODO: use interpolated strings in all solution
                Logger.LogError("RuntimeIdentifier {runtimeIdentifier} not supported", runtimeIdentifier);
                return false;
        }

        return System.Diagnostics.Process.Start(processStartInfo) != null;
    }
}

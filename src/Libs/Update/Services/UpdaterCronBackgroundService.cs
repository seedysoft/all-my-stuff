using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Update.Services;

public sealed class UpdaterCronBackgroundService : BackgroundServices.Cron
{
    private readonly Octokit.GitHubClient gitHubClient;
    private readonly ILogger<UpdaterCronBackgroundService> Logger;

    public UpdaterCronBackgroundService(IServiceProvider serviceProvider, Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        gitHubClient = ServiceProvider.GetRequiredService<Octokit.GitHubClient>();
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
            if (await IsNewVersionAvailableAsync())
            {
                // Execute external process
                ExecuteUpdateScript();
            }
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    private void ExecuteUpdateScript()
    {
        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            CreateNoWindow = false,
            UseShellExecute = true,
        };

        string runtimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
        switch (runtimeIdentifier)
        {
            case "linux-arm64":
            case "linux-x64":
                processStartInfo.FileName = "update.sh";
                break;

            case "win-x64":
                processStartInfo.FileName = "update.bat";
                break;

            default:
                Logger.LogError("RuntimeIdentifier {runtimeIdentifier} not supported", runtimeIdentifier);
                break;
        }

        _ = System.Diagnostics.Process.Start(processStartInfo);
    }

    private async Task<bool> IsNewVersionAvailableAsync()
    {
        Octokit.Release? latestRelease = await GetLatestReleaseFromGithubAsync();

        if (latestRelease == null)
            return false;

        // System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        // System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        // string version = fvi.FileVersion;

        Version? CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        Version? LatestVersion = new(latestRelease.Name);

        Logger.LogInformation("Current version is: {CurrentVersion}", CurrentVersion);
        Logger.LogInformation("Latest version is: {LatestVersion}", LatestVersion);

        return LatestVersion > CurrentVersion && !string.IsNullOrEmpty(await DownloadReleaseFromGithubAsync(latestRelease));
    }

    internal async Task<Octokit.Release?> GetLatestReleaseFromGithubAsync()
    {
        IReadOnlyList<Octokit.Release> releases =
            await gitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName);

        Logger.LogInformation("Obtained {releasesCount} releases", releases.Count);

        return releases.Any() ? releases[0] : null;
    }

    internal async Task<string> DownloadReleaseFromGithubAsync(Octokit.Release release)
    {
        foreach (Octokit.ReleaseAsset? asset in release.Assets)
        {
            if (!asset.Name.Contains($"{System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier}", StringComparison.InvariantCultureIgnoreCase))
                continue;

            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/octet-stream");
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Core.Constants.Github.Token);

            Logger.LogInformation("Try to download {assetBrowserDownloadUrl}", asset.BrowserDownloadUrl);

            HttpResponseMessage response = await httpClient.GetAsync(asset.BrowserDownloadUrl);

            _ = response.EnsureSuccessStatusCode();

            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                using FileStream fileStream = File.Create(asset.Name);
                await stream.CopyToAsync(fileStream);
            }

            Logger.LogInformation("Downloaded {assetName}", asset.Name);

            return asset.Name;
        }

        return string.Empty;
    }
}

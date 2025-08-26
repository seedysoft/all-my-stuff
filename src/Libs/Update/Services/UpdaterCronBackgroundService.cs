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

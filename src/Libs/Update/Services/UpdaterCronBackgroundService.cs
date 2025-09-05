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
            Enums.UpdateResults UpgradeResult = await DownloadLatestReleaseAsset(cancellationToken);

            Logger.LogInformation($"Updating result: {UpgradeResult}");
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    internal async Task<Enums.UpdateResults> DownloadLatestReleaseAsset(CancellationToken cancellationToken)
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

        string assetName = releaseAsset.Name;
        if (File.Exists(assetName))
        {
            Logger.LogInformation($"New version '{new FileInfo(assetName).FullName}' waiting for deploy");
            return Enums.UpdateResults.NewVersionAlreadyDownloaded;
        }

        var ExecutingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        Version? CurrentVersion = ExecutingAssembly.GetName().Version;
        Version NewVersion = new(release.Name);

        if (NewVersion <= (CurrentVersion ?? new Version()))
        {
            Logger.LogInformation($"Current version is: {CurrentVersion}. Latest version is: {NewVersion}");
            return Enums.UpdateResults.NoNewVersionFound;
        }

        // Here, NewVersion is greather than CurrentVersion
        using var httpClient = new HttpClient();
        //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token my-token");
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet));
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(assetName);
        using Stream streamToReadFrom = await httpClient.GetStreamAsync(releaseAsset.BrowserDownloadUrl, cancellationToken);
        using Stream streamToWriteTo = File.Open(assetName, FileMode.Create);
        await streamToReadFrom.CopyToAsync(streamToWriteTo, cancellationToken);

        Logger.LogInformation("Update asset downloaded");

        return ExecuteUpdateScript(ExecutingAssembly.Location, assetName)
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

    internal bool ExecuteUpdateScript(string executingAssemblyLocation, string assetName)
    {
        foreach (Octokit.ReleaseAsset? asset in release.Assets)
        {
            CreateNoWindow = true,
            UseShellExecute = true,
        };

        string runtimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
        switch (runtimeIdentifier)
        {
            case Core.Constants.SupportedRuntimeIdentifiers.LinuxArm64:
                //case Core.Constants.SupportedRuntimeIdentifiers.LinuxX64:
                processStartInfo.FileName = $"sudo systemd-run --on-active=60 --working-directory={Path.Combine(executingAssemblyLocation)} {Path.Combine(executingAssemblyLocation, "update.sh")} {assetName}";
                processStartInfo.WorkingDirectory = Path.Combine(executingAssemblyLocation);
                break;

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

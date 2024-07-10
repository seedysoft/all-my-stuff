using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Libs.Update.Services;

public class UpdateBackgroundServiceCron : BackgroundServices.Cron, IDisposable
{
    private readonly ILogger<UpdateBackgroundServiceCron> logger;
    private readonly Variants.Variant variant;
    private readonly HttpClient httpClient = new();
    private readonly ManualResetEvent locker = new(false);
    private readonly IServiceProvider serviceProvider;
    private bool disposedValue;

    public UpdateBackgroundServiceCron(IServiceProvider serviceProvider) : base(
        serviceProvider,
        serviceProvider.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>())
    {
        this.serviceProvider = serviceProvider;

        logger = serviceProvider.GetRequiredService<ILogger<UpdateBackgroundServiceCron>>();

        variant = Variants.GetVariant();

        // Increase the HTTP client timeout just for update download (not other requests)
        // The update is heavy and can take longer time for slow connections. Fix #12711
        httpClient.Timeout = TimeSpan.FromMinutes(5);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                base.Dispose();

                // dispose managed state (managed objects)
                httpClient?.Dispose();
                locker?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            disposedValue = true;
        }
    }

    // // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~UpdateService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public override void Dispose()
    {
        base.Dispose();

        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            Version? currentVersion = EnvironmentUtil.ParseVersion(EnvironmentUtil.Version());

            if (currentVersion == new Version(0, 0, 0))
            {
                logger.LogInformation("Skipping checking for new releases because is runing in IDE.");
                return;
            }

            GitHubClient? gitHubClient = await ConnectAsync();
            if (gitHubClient == null)
            {
                logger.LogInformation("GitHubClient is null.");
                return;
            }

            //ApiOptions apiOptions = new() { PageCount = 1, PageSize = 10, StartPage = 1 };
            Release? latestRelease =
                (await gitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName/*, apiOptions*/))
                .Where(x => x.Draft == false && x.Prerelease == false)
                .OrderByDescending(x => x.PublishedAt)
                .FirstOrDefault();

            if (latestRelease == null)
            {
                logger.LogInformation("No release obtained");
                return;
            }

            Version? latestVersion = EnvironmentUtil.ParseVersion(latestRelease.Name);
            if (latestVersion == null)
            {
                logger.LogInformation("Failed to parse latest version.");
                return;
            }

            if (latestVersion < currentVersion)
            {
                logger.LogWarning("Downgrade detected. Current version: v{currentVersion} New version: v{latestVersion}", currentVersion, latestVersion);
                return;
            }

            if (latestVersion == currentVersion)
            {
                logger.LogInformation("Jackett is already updated. Current version: v{currentVersion}", currentVersion);
                return;
            }

            logger.LogInformation("New release found. Current version: v{currentVersion} New version: v{latestVersion}", currentVersion, latestVersion);
            logger.LogInformation("Downloading release v{latestVersion} It could take a while...", latestVersion);

            IReadOnlyList<ReleaseAsset> releaseAssets = await gitHubClient.Repository.Release
                .GetAllAssets(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName, latestRelease.Id);

            try
            {
                string? tempDir = await DownloadReleaseAsset(gitHubClient, releaseAssets, latestRelease.Name);

                //// Copy updater
                //string installDir = EnvironmentUtil.InstallationPath();
                //string updaterPath = GetUpdaterPath(tempDir);

                //if (updaterPath != null)
                //    StartUpdate(updaterPath, installDir, EnvironmentUtil.IsWindows, serverConfig.RuntimeSettings.NoRestart, trayIsRunning);
            }
            catch (Exception e) when (logger.Handle(e, "Unhandled exception.")) { }
        }
        catch (TaskCanceledException e) when (logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }
    }

    protected internal async Task<GitHubClient?> ConnectAsync()
    {
        GitHubClient gitHubClient = new(new ProductHeaderValue(Core.Constants.Github.RepositoryName))
        {
            Credentials = new Credentials(serviceProvider.GetRequiredService<Settings.UpdateSettings>().GithubPat)
        };

        return (await gitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName)).Any()
            ? gitHubClient
            : null;
    }

    private async Task<string?> DownloadReleaseAsset(GitHubClient gitHubClient, IReadOnlyList<ReleaseAsset> releaseAssets, string version)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), $"Update-{version}-{DateTime.Now.Ticks}");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        _ = Directory.CreateDirectory(tempDir);

        string extension = EnvironmentUtil.IsWindows ? ".zip" : ".tar.gz";

        for (int i = 0; i < releaseAssets.Where(x => x.BrowserDownloadUrl.EndsWith(extension)).Count(); i++)
        {
            ReleaseAsset asset = releaseAssets[i];

            string localFilename = Path.Combine(tempDir, asset.Name);

            using HttpClient client = new() { Timeout = TimeSpan.FromMinutes(5) };
            {
                string token = gitHubClient.Connection.Credentials.GetToken();
                client.DefaultRequestHeaders.Accept.ParseAdd("application/octet-stream");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(Core.Constants.Github.RepositoryName);

                using HttpResponseMessage response = await client.GetAsync($"https://api.github.com/repos/{Core.Constants.Github.OwnerName}/{Core.Constants.Github.RepositoryName}/releases/assets/{asset.Id}");
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Cannot download release {asset}. Received code {StatusCode} with response {response}", asset, response.StatusCode, response);
                        continue;
                    }

                    using FileStream fileStream = File.Create(localFilename);
                    {
                        await response.Content.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        fileStream.Close();
                    }
                }
            }

            if (EnvironmentUtil.IsWindows)
            {
                new ICSharpCode.SharpZipLib.Zip.FastZip().ExtractZip(localFilename, tempDir, null);
            }
            else
            {
                ICSharpCode.SharpZipLib.GZip.GZipInputStream gzipStream = new(File.OpenRead(localFilename));

                var tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream, null);
                tarArchive.ExtractContents(tempDir);
                tarArchive.Close();
                gzipStream.Close();
                File.OpenRead(localFilename).Close();
            }
        }

        return tempDir;
    }
}

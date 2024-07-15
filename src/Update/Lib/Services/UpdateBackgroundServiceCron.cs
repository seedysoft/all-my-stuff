using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;
using Seedysoft.Libs.Update;
using Seedysoft.Libs.Utils.Extensions;
using Seedysoft.Update.Lib.Settings;

namespace Seedysoft.Update.Lib.Services;

public class UpdateBackgroundServiceCron(IServiceProvider serviceProvider) : Libs.BackgroundServices.Cron(
    serviceProvider,
    serviceProvider.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>()), IDisposable
{
    private readonly ILogger<UpdateBackgroundServiceCron> logger = serviceProvider.GetRequiredService<ILogger<UpdateBackgroundServiceCron>>();
    private readonly ManualResetEvent locker = new(false);
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                base.Dispose();

                // dispose managed state (managed objects)
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

            Release? latestRelease =
                await gitHubClient.Repository.Release.GetLatest(Libs.Core.Constants.Github.OwnerName, Libs.Core.Constants.Github.RepositoryName);
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
            else if (latestVersion < currentVersion)
            {
                logger.LogWarning("Downgrade detected. Current version: v{currentVersion} New version: v{latestVersion}", currentVersion, latestVersion);
                return;
            }
            else if (latestVersion == currentVersion)
            {
                logger.LogInformation("Jackett is already updated. Current version: v{currentVersion}", currentVersion);
                return;
            }

            logger.LogInformation("New release found. Current version: v{currentVersion} New version: v{latestVersion}", currentVersion, latestVersion);
            logger.LogInformation("Downloading release v{latestVersion} It could take a while...", latestVersion);

            IReadOnlyList<ReleaseAsset> releaseAssets = await gitHubClient.Repository.Release
                .GetAllAssets(Libs.Core.Constants.Github.OwnerName, Libs.Core.Constants.Github.RepositoryName, latestRelease.Id);

            try
            {
                string? tempDir = await DownloadReleaseAsset(gitHubClient, releaseAssets, latestRelease.Name);

                if (tempDir != null)
                    StartUpdate(tempDir);
            }
            catch (Exception e) when (logger.Handle(e, "Unhandled exception.")) { }
        }
        catch (TaskCanceledException e) when (logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }
    }

    private void StartUpdate(string updateFolder)
    {
        string UpdaterFullPath = Path.Combine(EnvironmentUtil.InstallationPath(), EnvironmentUtil.GetUpdaterFileName());

        try
        {
            System.Diagnostics.ProcessStartInfo processStartInfo = new()
            {
                Arguments = $"-{nameof(ConsoleOptions.UpdateFilesFolder)} \"{updateFolder}\"",
                CreateNoWindow = true,
                ErrorDialog = false,
                FileName = UpdaterFullPath,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            };
            using System.Diagnostics.Process process = new() { StartInfo = processStartInfo, };
            {
                if (!process.Start())
                {
                    logger.LogWarning("Cannot start process {UpdaterFullPath}.", UpdaterFullPath);
                    return;
                }
            }

            Environment.Exit(0);
        }
        catch (Exception e) { logger.LogError(e, "Error updating {UpdaterFullPath}.", UpdaterFullPath); }
    }

    protected internal async Task<GitHubClient?> ConnectAsync()
    {
        GitHubClient gitHubClient = new(new ProductHeaderValue(Libs.Core.Constants.Github.RepositoryName))
        {
            Credentials = new Credentials(serviceProvider.GetRequiredService<UpdateSettings>().GithubPat)
        };

        return (await gitHubClient.Repository.Release.GetAll(Libs.Core.Constants.Github.OwnerName, Libs.Core.Constants.Github.RepositoryName)).Any()
            ? gitHubClient
            : null;
    }

    private async Task<string?> DownloadReleaseAsset(GitHubClient gitHubClient, IEnumerable<ReleaseAsset> releaseAssets, string version)
    {
        string architecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
        releaseAssets = releaseAssets.Where(x => x.Name.Contains(architecture, StringComparison.InvariantCultureIgnoreCase));
        if (!releaseAssets.Any())
        {
            logger.LogInformation("Unsopported architecture: {Architecture}", System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);
            return await Task.FromResult<string?>(null);
        }

        string platform;
        string extractorFileName;
        if (EnvironmentUtil.IsWindows)
        {
            extractorFileName = @"C:\Program Files\7-Zip\7z.exe";
            platform = "win";
        }
        else
        {
            extractorFileName = "7zr";
            platform = "linux";
        }

        releaseAssets = releaseAssets.Where(x => x.Name.StartsWith(platform, StringComparison.InvariantCultureIgnoreCase));

        string tempDir = Path.Combine(Path.GetTempPath(), $"Update-{version}-{DateTime.Now.Ticks}");
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
        tempDir = Directory.CreateDirectory(tempDir).FullName;
        tempDir = @"C:\Users\alfon\AppData\Local\Temp\Update-Release v1.1.1.6-638565582644439810";

        for (int i = 0; i < (releaseAssets.TryGetNonEnumeratedCount(out int assetsCount) ? assetsCount : releaseAssets.Count()); i++)
        {
            ReleaseAsset asset = releaseAssets.ElementAt(i);

            string localFilename = Path.Combine(tempDir, asset.Name);
            if (File.Exists(localFilename))
                continue;

            using HttpClient client = new() { Timeout = TimeSpan.FromMinutes(5) };
            {
                string token = gitHubClient.Connection.Credentials.GetToken();
                client.DefaultRequestHeaders.Accept.ParseAdd("application/octet-stream");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(Libs.Core.Constants.Github.RepositoryName);

                using HttpResponseMessage response = await
                    client.GetAsync($"https://api.github.com/repos/{Libs.Core.Constants.Github.OwnerName}/{Libs.Core.Constants.Github.RepositoryName}/releases/assets/{asset.Id}");
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

            if (!ExtractFile(extractorFileName, localFilename, tempDir))
                return null;
        }

        return tempDir;
    }

    protected internal bool ExtractFile(string extractorFileName, string sourceFileName, string destinationFolder)
    {
        try
        {
            System.Diagnostics.ProcessStartInfo processStartInfo = new()
            {
                Arguments = $"x -bsp1 \"{sourceFileName}\" -o\"{destinationFolder}\"",
                CreateNoWindow = true,
                ErrorDialog = false,
                FileName = extractorFileName,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            };
            using System.Diagnostics.Process process = new()
            {
                EnableRaisingEvents = true,
                StartInfo = processStartInfo,
            };
            {
                process.ErrorDataReceived += (sender, e)
                    => logger.LogError("Received error from {extractorFileName}: {data}", extractorFileName, e.Data);
                process.OutputDataReceived += (sender, e)
                    => logger.LogInformation("Received info from {extractorFileName}: {data}", extractorFileName, e.Data);

                if (!process.Start())
                {
                    logger.LogWarning("Cannot start process {extractorFileName}.", extractorFileName);
                    return false;
                }

                process.WaitForExit();

                return true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error extracting file {source} to {destination}", sourceFileName, destinationFolder);
        }

        return false;
    }
}

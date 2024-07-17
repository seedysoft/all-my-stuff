using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Update.Lib.Services;

public class UpdateBackgroundServiceCron : Libs.BackgroundServices.Cron
{
    private readonly ILogger<UpdateBackgroundServiceCron> logger;
    private readonly IServiceProvider serviceProvider;

    public UpdateBackgroundServiceCron(IServiceProvider serviceProvider) : base(
        serviceProvider,
        serviceProvider.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>())
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        logger = serviceProvider.GetRequiredService<ILogger<UpdateBackgroundServiceCron>>();

        Config = ServiceProvider.GetRequiredService<Settings.UpdateSettings>();
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            Version? currentVersion = Libs.Update.EnvironmentUtil.ParseVersion(Libs.Update.EnvironmentUtil.Version());
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

            Version? latestVersion = Libs.Update.EnvironmentUtil.ParseVersion(latestRelease.Name);
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
                    StartUpdater(tempDir);
            }
            catch (Exception e) when (logger.Handle(e, "Unhandled exception.")) { }
        }
        catch (TaskCanceledException e) when (logger.Handle(e, "Task cancelled.")) { }
        catch (Exception e) when (logger.Handle(e, "Unhandled exception.")) { }
        finally { await Task.CompletedTask; }
    }

    private void StartUpdater(string directory)
    {
        string UpdaterFullPath = Path.Combine(directory, Libs.Update.EnvironmentUtil.GetUpdaterFileName());
        if (!File.Exists(UpdaterFullPath))
        {
            logger.LogError("Updater file {UpdaterFullPath} does NOT found.", UpdaterFullPath);
            return;
        }

        Settings.UpdateConsoleOptions upd = Settings.UpdateConsoleOptions.Default;
        upd.LaunchDebugger = true;
        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            Arguments = upd.ToString(),
            CreateNoWindow = true,
            ErrorDialog = false,
            FileName = UpdaterFullPath,
            //RedirectStandardError = true,
            //RedirectStandardInput = true,
            //RedirectStandardOutput = true,
            UseShellExecute = false,
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
        };

        System.Diagnostics.Process process = new() { StartInfo = processStartInfo, };
        if (!process.Start())
        {
            logger.LogWarning("Cannot start process {UpdaterFullPath}.", UpdaterFullPath);
            return;
        }

        Environment.Exit(0);
    }

    protected internal async Task<GitHubClient?> ConnectAsync()
    {
        GitHubClient gitHubClient = new(new ProductHeaderValue(Libs.Core.Constants.Github.RepositoryName))
        {
            Credentials = new Credentials(serviceProvider.GetRequiredService<Settings.UpdateSettings>().GithubPat)
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
            logger.LogInformation("Unsopported architecture: {Architecture}.", System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);
            return null;
        }

        string platform;
        string extractorFileName;
        if (Libs.Update.EnvironmentUtil.IsWindows)
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

        string destinationDirectory = Path.Combine(Path.GetTempPath(), $"Update-{version}-{DateTime.Now.Ticks}");
        if (Directory.Exists(destinationDirectory))
            Directory.Delete(destinationDirectory, true);
        destinationDirectory = Directory.CreateDirectory(destinationDirectory).FullName;

        for (int i = 0; i < (releaseAssets.TryGetNonEnumeratedCount(out int assetsCount) ? assetsCount : releaseAssets.Count()); i++)
        {
            ReleaseAsset releaseAsset = releaseAssets.ElementAt(i);

            string localFilename = Path.Combine(destinationDirectory, releaseAsset.Name);
            if (File.Exists(localFilename))
                continue;

            using HttpClient httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };
            {
                string token = gitHubClient.Connection.Credentials.GetToken();
                httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/octet-stream");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Libs.Core.Constants.Github.RepositoryName);

                using HttpResponseMessage response = await httpClient
                    .GetAsync($"{gitHubClient.BaseAddress}repos/{Libs.Core.Constants.Github.OwnerName}/{Libs.Core.Constants.Github.RepositoryName}/releases/assets/{releaseAsset.Id}");
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Cannot download release {asset}. Received code {StatusCode} with response {response}", releaseAsset, response.StatusCode, response);
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

            ExtractFile(extractorFileName, localFilename, destinationDirectory);
        }

        return destinationDirectory;
    }

    protected internal void ExtractFile(string extractorFileName, string sourceFileName, string destinationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extractorFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        if (!File.Exists(extractorFileName))
            return;

        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            Arguments = $"x -bsp1 \"{sourceFileName}\" -o\"{destinationDirectory}\"",
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

            if (process.Start())
                process.WaitForExit();
            else
                logger.LogWarning("Cannot start process {extractorFileName}.", extractorFileName);
        }
    }

    public async Task CopyUpdatesAsync(string destinationDirectory, CancellationToken cancellationToken)
    {
        string[] allFileNames = Directory.GetFiles(Libs.Update.EnvironmentUtil.ExecutablePath(), "*.*", SearchOption.AllDirectories);
        logger.LogInformation("{Length} update files found", allFileNames.Length);

        try
        {
            for (int i = 0; i < allFileNames.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                string fileName = Path.GetFileName(allFileNames[i]).ToLowerInvariant();

                if (fileName.EndsWith(".7z"))
                    continue;

                await CopyUpdateFileAsync(destinationDirectory, fileName, destinationDirectory, cancellationToken);
            }
        }
        catch (Exception e) { logger.LogError(e, "Error copying update files to {destination}", destinationDirectory); }
    }

    private async Task CopyUpdateFileAsync(string destinationDirectory, string fullSourceFilePath, string sourceDirectory, CancellationToken cancellationToken)
    {
        string fileName = Path.GetFileName(fullSourceFilePath);
        string fullDestinationFilePath = Path.Combine(destinationDirectory, fullSourceFilePath[sourceDirectory.Length..]);
        string fileDestinationDirectory = Path.GetDirectoryName(fullDestinationFilePath)!;

        logger.LogInformation("Attempting to copy {fileName} from source: {fullSourceFilePath} to destination: {fullDestinationFilePath}", fileName, fullSourceFilePath, fullDestinationFilePath);

        for (int i = 0; i < 2; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                if (!Directory.Exists(fileDestinationDirectory))
                {
                    logger.LogInformation("Creating directory {fileDestinationDirectory}", fileDestinationDirectory);
                    _ = Directory.CreateDirectory(fileDestinationDirectory);
                }

                File.Copy(fullSourceFilePath, fullDestinationFilePath, i > 0);
                logger.LogInformation("Copied {fileName}", fileName);

                break;
            }
            catch (Exception e) { logger.LogError(e, "Error copying update files to {destination}", destinationDirectory); }

            logger.LogInformation("The first attempt copying file: {fileName} failed. Retrying and will delete old file first.", fileName);

            try
            {
                if (File.Exists(fullDestinationFilePath))
                {
                    logger.LogInformation("{fullDestinationFilePath} was found.", fullDestinationFilePath);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    File.Delete(fullDestinationFilePath);
                    logger.LogInformation("Deleted {fullDestinationFilePath}.", fullDestinationFilePath);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                else
                {
                    logger.LogInformation("{fullDestinationFilePath} was NOT found", fullDestinationFilePath);
                }
            }
            catch (Exception e) { logger.LogError(e, "Error deleting {fullDestinationFilePath}.", fullDestinationFilePath); }
        }
    }
}

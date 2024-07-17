using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.Update.Lib.Services;

public class UpdateBackgroundServiceCron : Libs.BackgroundServices.Cron
{
    private readonly ILogger<UpdateBackgroundServiceCron> logger;

    public UpdateBackgroundServiceCron(IServiceProvider serviceProvider) : base(
        serviceProvider,
        serviceProvider.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>())
    {
        logger = serviceProvider.GetRequiredService<ILogger<UpdateBackgroundServiceCron>>();

        Config = ServiceProvider.GetRequiredService<Settings.UpdateSettings>();
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            Version? currentVersion = Libs.Update.EnvironmentUtil.ParseVersion(Libs.Update.EnvironmentUtil.MyVersion());
            if (currentVersion == new Version(1, 0, 0))
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

    protected internal async Task<GitHubClient?> ConnectAsync()
    {
        GitHubClient gitHubClient = new(new ProductHeaderValue(Libs.Core.Constants.Github.RepositoryName))
        {
            Credentials = new Credentials(ServiceProvider.GetRequiredService<Settings.UpdateSettings>().GithubPat)
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
        string extractorFileNameWithPath;
        if (Libs.Update.EnvironmentUtil.IsWindows)
        {
            extractorFileNameWithPath = @"C:\Program Files\7-Zip\7z.exe";
            platform = "win";
        }
        else
        {
            extractorFileNameWithPath = "7zr";
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

            string localFilenameWithPath = Path.Combine(destinationDirectory, releaseAsset.Name);
            if (File.Exists(localFilenameWithPath))
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

                    using FileStream fileStream = File.Create(localFilenameWithPath);
                    {
                        await response.Content.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        fileStream.Close();
                    }
                }
            }

            ExtractFile(extractorFileNameWithPath, localFilenameWithPath, destinationDirectory);
        }

        return destinationDirectory;
    }

    protected internal void ExtractFile(string extractorFileNameWithPath, string sourceFileNameWithPath, string destinationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extractorFileNameWithPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFileNameWithPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        if (!File.Exists(extractorFileNameWithPath))
            return;

        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            // x : eXtract files with full paths
            // -aoa : Overwrite All existing files without prompt.
            // -bsp1 : -bs{o|e|p}{0|1|2} : set output stream for output/error/progress line
            // -o{Directory} : set Output directory
            Arguments = $"x -aoa -bsp1 \"{sourceFileNameWithPath}\" -o\"{destinationDirectory}\"",
            CreateNoWindow = true,
            ErrorDialog = false,
            FileName = extractorFileNameWithPath,
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
                => logger.LogError("Received error from {extractor}: {data}", extractorFileNameWithPath, e.Data);
            process.OutputDataReceived += (sender, e)
                => logger.LogInformation("Received info from {extractor}: {data}", extractorFileNameWithPath, e.Data);

            if (process.Start())
                process.WaitForExit();
            else
                logger.LogWarning("Cannot start process {extractor}.", extractorFileNameWithPath);
        }
    }

    protected internal async Task CopyUpdateFilesAsync(string destinationDirectory, CancellationToken cancellationToken)
    {
        // Delete main program as cannot restart automatically

        string sourceDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string[] allFileNamesWithPath = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
        logger.LogInformation("{Length} update files found", allFileNamesWithPath.Length);

        try
        {
            for (int i = 0; i < allFileNamesWithPath.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                string current = allFileNamesWithPath[i];

                if (current.EndsWith(".7z", StringComparison.InvariantCultureIgnoreCase) ||
                    current.EndsWith(Libs.Update.EnvironmentUtil.GetMainProgramFileName(), StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                await CopyFileAsync(destinationDirectory, current, sourceDirectory, cancellationToken);
            }

            // Now copy main program file
            await CopyFileAsync(
                destinationDirectory,
                allFileNamesWithPath.First(x => x.EndsWith(Libs.Update.EnvironmentUtil.GetMainProgramFileName())),
                sourceDirectory,
                cancellationToken);
        }
        catch (Exception e) { logger.LogError(e, "Error copying update files to {destination}", destinationDirectory); }
    }

    private async Task CopyFileAsync(string destinationDirectory, string sourceFileNameWithPath, string sourceDirectory, CancellationToken cancellationToken)
    {
        string fileName = Path.GetFileName(sourceFileNameWithPath);
        string destinationFileNameWithPath = Path.Combine(destinationDirectory, sourceFileNameWithPath[(sourceDirectory.Length + 1)..]);
        string fileDestinationDirectory = Path.GetDirectoryName(destinationFileNameWithPath)!;

        logger.LogInformation("Attempting to copy {fileName} from source: {source} to destination: {destination}", fileName, sourceFileNameWithPath, destinationFileNameWithPath);

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

                File.Copy(sourceFileNameWithPath, destinationFileNameWithPath, true);
                logger.LogInformation("Copied {fileName}", fileName);

                break; // done on first attempt
            }
            catch (Exception e) { logger.LogError(e, "Error copying update files to {destination}", destinationDirectory); }

            logger.LogInformation("The first attempt copying file: {fileName} failed. Retrying and will delete old file first.", fileName);

            try
            {
                if (File.Exists(destinationFileNameWithPath))
                {
                    logger.LogInformation("{fullDestinationFilePath} was found.", destinationFileNameWithPath);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    File.Delete(destinationFileNameWithPath);
                    logger.LogInformation("Deleted {fullDestinationFilePath}.", destinationFileNameWithPath);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                else
                {
                    logger.LogInformation("{fullDestinationFilePath} was NOT found", destinationFileNameWithPath);
                }
            }
            catch (Exception e) { logger.LogError(e, "Error deleting {fullDestinationFilePath}.", destinationFileNameWithPath); }
        }
    }

    public async Task UpdateAndStartAsync(string destinationDirectory, CancellationToken token)
    {
        await CopyUpdateFilesAsync(destinationDirectory, token);

        StartMainProgram();
    }

    private void StartUpdater(string directory)
    {
        string UpdaterFileNameWithPath = Path.Combine(directory, Libs.Update.EnvironmentUtil.GetUpdaterFileName());
        if (!File.Exists(UpdaterFileNameWithPath))
        {
            logger.LogError("Updater file {UpdaterFullPath} does NOT found.", UpdaterFileNameWithPath);
            return;
        }

        Settings.UpdateConsoleOptions upd = Settings.UpdateConsoleOptions.Default;
        upd.InstallDirectory = Libs.Update.EnvironmentUtil.CurrentExecutablePath();
        System.Diagnostics.ProcessStartInfo processStartInfo = new()
        {
            Arguments = upd.ToString(),
            CreateNoWindow = true,
            ErrorDialog = false,
            FileName = UpdaterFileNameWithPath,
            UseShellExecute = true,
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
        };

        System.Diagnostics.Process process = new() { StartInfo = processStartInfo, };

        if (!process.Start())
        {
            logger.LogWarning("Cannot start process {UpdaterFullPath}.", UpdaterFileNameWithPath);
            return;
        }

        Environment.Exit(0);
    }

    private void StartMainProgram()
    {
        // As it is a service, it starts itself

        Environment.Exit(0);
    }
}

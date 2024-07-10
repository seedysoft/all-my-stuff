using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

    //    private async Task CheckForUpdatesAsync()
    //    {
    //        logger.LogInformation("Checking for updates... Seedysoft variant: {variant}", variant);

    //        if (System.Diagnostics.Debugger.IsAttached)
    //        {
    //            logger.LogInformation("Skipping checking for new releases as the debugger is attached.");
    //            return;
    //        }

    //        string currentVersion = EnvironmentUtil.Version();

    //        bool trayIsRunning =
    //            Environment.OSVersion.Platform != PlatformID.Unix &&
    //            System.Diagnostics.Process.GetProcessesByName("SeedysoftTray").Length > 0;

    //        try
    //        {
    //            Octokit.GitHubClient gitHubClient = new(Octokit.ProductHeaderValue.Parse(""));
    //            Octokit.Release latestRelease = await gitHubClient.Repository.Release.GetLatest("", "");

    //            if (latestRelease.Name == currentVersion)
    //                logger.LogInformation("Seedysoft is already updated. Current version: {currentVersion}", currentVersion);
    //            else
    //            {
    //                logger.LogInformation("New release found. Current version: {currentVersion} New version: {latestRelease}", currentVersion, latestRelease.Name);
    //                logger.LogInformation("Downloading release {latestRelease} It could take a while...", latestRelease.Name);
    //                try
    //                {
    //                    string tempDir = await DownloadReleaseAsync(latestRelease.Assets, latestRelease.Name);
    //                    // Copy updater
    //                    string updaterPath = GetUpdaterPath(tempDir);
    //                    if (updaterPath != null)
    //                        StartUpdate(updaterPath, EnvironmentUtil.InstallationPath(), trayIsRunning);
    //                }
    //                catch (Exception e) { logger.LogError("Error performing update.\n{e}", e); }
    //            }
    //        }
    //        catch (Exception e) { logger.LogError("Error checking for updates.\n{e}", e); }
    //        finally { await Task.CompletedTask; }
    //    }

    //    private string GetUpdaterPath(string tempDirectory)
    //    {
    //        return variant switch
    //        {
    //            Variants.Variant.CoreWindows => Path.Combine(tempDirectory, nameof(Seedysoft), "SeedysoftUpdater.exe"),
    //            _ => Path.Combine(tempDirectory, nameof(Seedysoft), "SeedysoftUpdater"),
    //        };
    //    }

    //    //public void CleanupTempDir()
    //    //{
    //    //    string tempDir = Path.GetTempPath();

    //    //    if (!Directory.Exists(tempDir))
    //    //    {
    //    //        logger.LogError("Temp dir doesn't exist: {tempDir}", tempDir);
    //    //        return;
    //    //    }

    //    //    try
    //    //    {
    //    //        var d = new DirectoryInfo(tempDir);
    //    //        foreach (DirectoryInfo dir in d.GetDirectories("SeedysoftUpdate-*"))
    //    //        {
    //    //            try
    //    //            {
    //    //                logger.LogInformation("Deleting SeedysoftUpdate temp files from {dir}", dir.FullName);
    //    //                dir.Delete(true);
    //    //            }
    //    //            catch (Exception e)
    //    //            {
    //    //                logger.LogError("Error while deleting temp files from: {dir}\n{e}", variant, e);
    //    //            }
    //    //        }
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        logger.LogError("Unexpected error while deleting temp files from: {tempDir}\n{e}", tempDir, e);
    //    //    }
    //    //}

    //    //public void CheckUpdaterLock()
    //    //{
    //    //    // check .lock file to detect errors in the update process
    //    //    string lockFilePath = Path.Combine(EnvironmentUtil.InstallationPath(), ".lock");
    //    //    if (File.Exists(lockFilePath))
    //    //    {
    //    //        logger.LogError(
    //    //            "An error occurred during the last update. If this error occurs again, you need to reinstall " +
    //    //                   "Jackett following the documentation. If the problem continues after reinstalling, " +
    //    //                   "report the issue and attach the Jackett and Updater logs.");
    //    //        File.Delete(lockFilePath);
    //    //    }
    //    //}

    //    private async Task<string> DownloadReleaseAsync(IReadOnlyList<Octokit.ReleaseAsset> assets, string version)
    //    {
    //        var variants = new Variants();
    //        string artifactFileName = Variants.GetArtifactFileName(variant);
    //        Octokit.ReleaseAsset? targetAsset = assets.FirstOrDefault(a => a.BrowserDownloadUrl.EndsWith(artifactFileName, StringComparison.OrdinalIgnoreCase) && artifactFileName.Length > 0);

    //        if (targetAsset == null)
    //        {
    //            logger.LogError("Failed to find asset to download!");
    //            return null!;
    //        }

    //        string url = targetAsset.BrowserDownloadUrl;

    //        _ = client.DefaultRequestHeaders.Accept.TryParseAdd("application/octet-stream");
    //        System.Diagnostics.Debug.Assert(client.DefaultRequestHeaders.AcceptLanguage.Any(x => x.Value == "*"));
    //        client.DefaultRequestHeaders.ExpectContinue = false;

    //        HttpResponseMessage response = await client.GetAsync(url);

    //        while (response.IsRedirect())
    //            response = await client.GetAsync(response.Headers.Location);

    //        string tempDir = Path.Combine(Path.GetTempPath(), "SeedysoftUpdate-" + version + "-" + DateTime.Now.Ticks);

    //        if (Directory.Exists(tempDir))
    //            Directory.Delete(tempDir, true);

    //        _ = Directory.CreateDirectory(tempDir);

    //        if (Environment.OSVersion.Platform == PlatformID.Unix)
    //        {
    //            string gzPath = Path.Combine(tempDir, "Update.tar.gz");
    //            File.WriteAllBytes(gzPath, await response.Content.ReadAsByteArrayAsync());
    //            Stream inStream = File.OpenRead(gzPath);
    //            Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inStream);

    //            var tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream, null);
    //            tarArchive.ExtractContents(tempDir);
    //            tarArchive.Close();
    //            gzipStream.Close();
    //            inStream.Close();

    //            switch (variant)
    //            {
    //                case Variants.Variant.CoreLinuxAmdx64:
    //                case Variants.Variant.CoreLinuxArm64:
    //                {
    //                    // When the files get extracted, the execute permission don't get carried across

    //                    string SeedysoftPath = tempDir + "/Seedysoft/Seedysoft.HomeCloud.Server";
    //                    MakeFileExecutable(SeedysoftPath);

    //                    string SeedysoftUpdaterPath = tempDir + "/Seedysoft/SeedysoftUpdater";
    //                    MakeFileExecutable(SeedysoftUpdaterPath);

    //                    string SeedysoftSystemdPath = tempDir + "/Seedysoft/create-HomeCloudServer-daemon.sh";
    //                    MakeFileExecutable(SeedysoftSystemdPath);

    //                    string SeedysoftLauncherPath = tempDir + "/Seedysoft/seedysoft_launcher.sh";
    //                    MakeFileExecutable(SeedysoftLauncherPath);
    //                    break;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            string zipPath = Path.Combine(tempDir, "Update.zip");
    //            File.WriteAllBytes(zipPath, await response.Content.ReadAsByteArrayAsync());
    //            var fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
    //            fastZip.ExtractZip(zipPath, tempDir, null);
    //        }

    //        return tempDir;
    //    }

    //    private static void MakeFileExecutable(string path)
    //    {
    //        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
    //        {
    //            _ = new FileInfo(path)
    //            {
    //                UnixFileMode = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute | UnixFileMode.GroupRead | UnixFileMode.OtherRead
    //            };
    //        }
    //    }

    //    private void StartUpdate(string updaterExePath, string installLocation, bool trayIsRunning)
    //    {
    //        const string SERVICE_NAME = "";

    //        string appType = "Console";

    //#pragma warning disable CA1416 // Validate platform compatibility
    //        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) &&
    //            ServiceControllerStatus.Running == ServiceController.GetServices().FirstOrDefault(c => string.Equals(c.ServiceName, SERVICE_NAME, StringComparison.InvariantCultureIgnoreCase))?.Status)
    //        {
    //            appType = "WindowsService";
    //        }
    //#pragma warning restore CA1416 // Validate platform compatibility

    //        string args = string.Join(" ", Environment.GetCommandLineArgs().Skip(1).Select(a => a.Contains(' ') ? "\"" + a + "\"" : a)).Replace("\"", "\\\"");

    //        System.Diagnostics.ProcessStartInfo startInfo = new()
    //        {
    //            UseShellExecute = false,
    //            CreateNoWindow = true,
    //            // Note: add a leading space to the --Args argument to avoid parsing as arguments
    //            Arguments = $"--Path \"{installLocation}\" --Type \"{appType}\" --Args \" {args}\"",
    //            FileName = Path.Combine(updaterExePath)
    //        };

    //        try
    //        {
    //            int pid = Environment.ProcessId;
    //            startInfo.Arguments += $" --KillPids \"{pid}\"";
    //        }
    //        catch (Exception e)
    //        {
    //            logger.LogError("Unexpected error while retriving the PID.\n{e}", e);
    //        }

    //        if (trayIsRunning && appType == "Console")
    //            startInfo.Arguments += " --StartTray";

    //        // create .lock file to detect errors in the update process
    //        string lockFilePath = Path.Combine(installLocation, ".lock");
    //        if (!File.Exists(lockFilePath))
    //            File.Create(lockFilePath).Dispose();

    //        logger.LogInformation("Starting updater: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);
    //        var procInfo = System.Diagnostics.Process.Start(startInfo);
    //        logger.LogInformation("Updater started process id: {procInfoId}", procInfo?.Id);

    //        if (Environment.OSVersion.Platform != PlatformID.Unix)
    //        {
    //            logger.LogInformation("Signal sent to lock service");
    //            Thread.Sleep(TimeSpan.FromSeconds(2));
    //        }

    //        logger.LogInformation("Exiting Seedysoft..");
    //        Environment.Exit(0);
    //    }

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
                logger.LogInformation("Skipping checking for new releases because Jackett is runing in IDE.");
                return;
            }

            using IServiceScope ServiceScope = serviceProvider.CreateAsyncScope();

            Octokit.GitHubClient gitHubClient = new(new Octokit.ProductHeaderValue(Core.Constants.Github.RepositoryName))
            {
                Credentials = new Octokit.Credentials(serviceProvider.GetRequiredService<Settings.UpdateSettings>().GithubPat)
            };

            //Octokit.ApiOptions options = new() {  };
            Octokit.Release? latestRelease = (await gitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName/*, options*/))
                .Where(x => x.Draft == false && x.Prerelease == false)
                .OrderByDescending(x => x.PublishedAt)
                .FirstOrDefault();

            if (latestRelease == null)
            {
                logger.LogInformation("No release obtained");
                return;
            }

            Version latestVersion = EnvironmentUtil.ParseVersion(latestRelease.Name)
                ?? throw new Exception("Failed to parse latest version.");

            if (latestVersion < currentVersion)
                logger.LogWarning("Downgrade detected. Current version: v{currentVersion} New version: v{latestVersion}", currentVersion, latestVersion);

            if (latestVersion == currentVersion)
            {
                logger.LogInformation("Jackett is already updated. Current version: v{currentVersion}", currentVersion);
                return;
            }

            logger.LogInformation("New release found. Current version: v{currentVersion} New version: v{latestVersion}", currentVersion, latestVersion);
            logger.LogInformation("Downloading release v{latestVersion} It could take a while...", latestVersion);

            IReadOnlyList<Octokit.ReleaseAsset> releaseAssets = await gitHubClient.Repository.Release
                .GetAllAssets(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName, latestRelease.Id);

            try
            {
                string? tempDir = await DownloadReleaseAsset(releaseAssets, EnvironmentUtil.IsWindows, latestRelease.Name);

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

    protected internal async Task<bool> ConnectAsync()
    {
        Octokit.GitHubClient gitHubClient = new(new Octokit.ProductHeaderValue(Core.Constants.Github.RepositoryName))
        {
            Credentials = new Octokit.Credentials(serviceProvider.GetRequiredService<Settings.UpdateSettings>().GithubPat)
        };

        return (await gitHubClient.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName)).Any();
    }

    private async Task<string?> DownloadReleaseAsset(IReadOnlyList<Octokit.ReleaseAsset> releaseAssets, bool isWindows, string version)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), $"Update-{version}-{DateTime.Now.Ticks}");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        _ = Directory.CreateDirectory(tempDir);

        IList<Octokit.ReleaseAsset> releaseAssetList = (isWindows
            ? releaseAssets.Where(x => x.Url.EndsWith(".zip"))
            : releaseAssets.Where(x => x.Url.EndsWith(".tar.gz"))
            ).ToArray();

        for (int i = 0; i < releaseAssetList.Count; i++)
        {
            Octokit.ReleaseAsset asset = releaseAssetList[i];

            string localFilename = isWindows 
                ? Path.Combine(tempDir, "Update.zip") 
                : Path.Combine(tempDir, "Update.tar.gz");

            HttpResponseMessage httpResult = await httpClient.GetAsync(asset.Url);
            using Stream resultStream = await httpResult.Content.ReadAsStreamAsync();
            using FileStream fileStream = File.Create(localFilename);
            await resultStream.CopyToAsync(fileStream);

            if (isWindows)
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

                //switch (variant)
                //{
                //    case Variants.Variant.CoreLinuxAmdx64:
                //    case Variants.Variant.CoreLinuxArm64:
                //    {
                //        // When the files get extracted, the execute permission for jackett and JackettUpdater don't get carried across

                //        string jackettPath = tempDir + "/Jackett/jackett";
                //        new UnixFileInfo(jackettPath)
                //        {
                //            FileAccessPermissions = FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupRead | FileAccessPermissions.OtherRead
                //        };

                //        string jackettUpdaterPath = tempDir + "/Jackett/JackettUpdater";
                //        new UnixFileInfo(jackettUpdaterPath)
                //        {
                //            FileAccessPermissions = FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupRead | FileAccessPermissions.OtherRead
                //        };

                //        string systemdPath = tempDir + "/Jackett/install_service_systemd.sh";
                //        new UnixFileInfo(systemdPath)
                //        {
                //            FileAccessPermissions = FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupRead | FileAccessPermissions.OtherRead
                //        };

                //        string launcherPath = tempDir + "/Jackett/jackett_launcher.sh";
                //        new UnixFileInfo(launcherPath)
                //        {
                //            FileAccessPermissions = FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupRead | FileAccessPermissions.OtherRead
                //        };

                //        break;
                //    }
                //}
            }
        }

        return tempDir;
    }
}

using Microsoft.Extensions.Logging;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;

namespace Seedysoft.Libs.Update;

public class UpdateService : IDisposable
{
    private readonly ILogger<UpdateService> logger;
    private readonly Variants.Variant variant;
    private readonly HttpClient client = new();
    private readonly ManualResetEvent locker = new(false);
    private bool disposedValue;

    public UpdateService(ILogger<UpdateService> l)
    {
        logger = l;

        variant = Variants.GetVariant();

        // Increase the HTTP client timeout just for update download (not other requests)
        // The update is heavy and can take longer time for slow connections. Fix #12711
        client.Timeout = TimeSpan.FromMinutes(5);
    }

    public void StartUpdateChecker() => Task.Factory.StartNew(UpdateWorkerThreadAsync);

    private async void UpdateWorkerThreadAsync()
    {
        int delayHours = 1; // first check after 1 hour (for users not running 24/7)
        while (true)
        {
            _ = System.Diagnostics.Debugger.IsAttached
                ? locker.WaitOne((int)TimeSpan.FromMinutes(30).TotalMilliseconds)
                : locker.WaitOne((int)TimeSpan.FromHours(delayHours).TotalMilliseconds);
            _ = locker.Reset();
            await CheckForUpdatesAsync();
            delayHours = 24; // following checks only once/24 hours
        }
    }

    private bool AcceptCert(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true;

    private async Task CheckForUpdatesAsync()
    {
        logger.LogInformation("Checking for updates... Seedysoft variant: {variant}", variant);

        if (System.Diagnostics.Debugger.IsAttached)
        {
            logger.LogInformation("Skipping checking for new releases as the debugger is attached.");
            return;
        }

        string currentVersion = EnvironmentUtil.Version();

        bool trayIsRunning =
            Environment.OSVersion.Platform != PlatformID.Unix &&
            System.Diagnostics.Process.GetProcessesByName("SeedysoftTray").Length > 0;

        try
        {
            Octokit.GitHubClient gitHubClient = new(Octokit.ProductHeaderValue.Parse(""));
            Octokit.Release latestRelease = await gitHubClient.Repository.Release.GetLatest("", "");

            if (latestRelease.Name != currentVersion)
            {
                logger.LogInformation("New release found. Current version: {currentVersion} New version: {latestRelease}", currentVersion, latestRelease.Name);
                logger.LogInformation("Downloading release {latestRelease} It could take a while...", latestRelease.Name);
                try
                {
                    string tempDir = await DownloadReleaseAsync(latestRelease.Assets, latestRelease.Name);
                    // Copy updater
                    string updaterPath = GetUpdaterPath(tempDir);
                    if (updaterPath != null)
                        StartUpdate(updaterPath, EnvironmentUtil.InstallationPath(), trayIsRunning);
                }
                catch (Exception e)
                {
                    logger.LogError("Error performing update.\n{e}", e);
                }
            }
            else
            {
                logger.LogInformation("Seedysoft is already updated. Current version: {currentVersion}", currentVersion);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error checking for updates.\n{e}", e);
        }
        finally
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                System.Net.ServicePointManager.ServerCertificateValidationCallback -= AcceptCert;
        }
    }

    private string GetUpdaterPath(string tempDirectory)
    {
        return variant switch
        {
            Variants.Variant.CoreWindows => Path.Combine(tempDirectory, "Seedysoft", "SeedysoftUpdater.exe"),
            _ => Path.Combine(tempDirectory, "Seedysoft", "SeedysoftUpdater"),
        };
    }

    //public void CleanupTempDir()
    //{
    //    string tempDir = Path.GetTempPath();

    //    if (!Directory.Exists(tempDir))
    //    {
    //        logger.LogError("Temp dir doesn't exist: {tempDir}", tempDir);
    //        return;
    //    }

    //    try
    //    {
    //        var d = new DirectoryInfo(tempDir);
    //        foreach (DirectoryInfo dir in d.GetDirectories("SeedysoftUpdate-*"))
    //        {
    //            try
    //            {
    //                logger.LogInformation("Deleting SeedysoftUpdate temp files from {dir}", dir.FullName);
    //                dir.Delete(true);
    //            }
    //            catch (Exception e)
    //            {
    //                logger.LogError("Error while deleting temp files from: {dir}\n{e}", variant, e);
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        logger.LogError("Unexpected error while deleting temp files from: {tempDir}\n{e}", tempDir, e);
    //    }
    //}

    //public void CheckUpdaterLock()
    //{
    //    // check .lock file to detect errors in the update process
    //    string lockFilePath = Path.Combine(EnvironmentUtil.InstallationPath(), ".lock");
    //    if (File.Exists(lockFilePath))
    //    {
    //        logger.LogError(
    //            "An error occurred during the last update. If this error occurs again, you need to reinstall " +
    //                   "Jackett following the documentation. If the problem continues after reinstalling, " +
    //                   "report the issue and attach the Jackett and Updater logs.");
    //        File.Delete(lockFilePath);
    //    }
    //}

    private async Task<string> DownloadReleaseAsync(IReadOnlyList<Octokit.ReleaseAsset> assets, string version)
    {
        var variants = new Variants();
        string artifactFileName = Variants.GetArtifactFileName(variant);
        Octokit.ReleaseAsset? targetAsset = assets.FirstOrDefault(a => a.BrowserDownloadUrl.EndsWith(artifactFileName, StringComparison.OrdinalIgnoreCase) && artifactFileName.Length > 0);

        if (targetAsset == null)
        {
            logger.LogError("Failed to find asset to download!");
            return null!;
        }

        string url = targetAsset.BrowserDownloadUrl;

        _ = client.DefaultRequestHeaders.Accept.TryParseAdd("application/octet-stream");
        System.Diagnostics.Debug.Assert(client.DefaultRequestHeaders.AcceptLanguage.Any(x => x.Value == "*"));
        client.DefaultRequestHeaders.ExpectContinue = false;

        HttpResponseMessage response = await client.GetAsync(url);

        while (response.IsRedirect())
            response = await client.GetAsync(response.Headers.Location);

        string tempDir = Path.Combine(Path.GetTempPath(), "SeedysoftUpdate-" + version + "-" + DateTime.Now.Ticks);

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        _ = Directory.CreateDirectory(tempDir);

        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            string gzPath = Path.Combine(tempDir, "Update.tar.gz");
            File.WriteAllBytes(gzPath, await response.Content.ReadAsByteArrayAsync());
            Stream inStream = File.OpenRead(gzPath);
            Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inStream);

            var tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream, null);
            tarArchive.ExtractContents(tempDir);
            tarArchive.Close();
            gzipStream.Close();
            inStream.Close();

            switch (variant)
            {
                case Variants.Variant.CoreLinuxAmdx64:
                case Variants.Variant.CoreLinuxArm64:
                {
                    // When the files get extracted, the execute permission don't get carried across

                    string SeedysoftPath = tempDir + "/Seedysoft/Seedysoft.HomeCloud.Server";
                    MakeFileExecutable(SeedysoftPath);

                    string SeedysoftUpdaterPath = tempDir + "/Seedysoft/SeedysoftUpdater";
                    MakeFileExecutable(SeedysoftUpdaterPath);

                    string SeedysoftSystemdPath = tempDir + "/Seedysoft/create-HomeCloudServer-daemon.sh";
                    MakeFileExecutable(SeedysoftSystemdPath);

                    string SeedysoftLauncherPath = tempDir + "/Seedysoft/seedysoft_launcher.sh";
                    MakeFileExecutable(SeedysoftLauncherPath);
                    break;
                }
            }
        }
        else
        {
            string zipPath = Path.Combine(tempDir, "Update.zip");
            File.WriteAllBytes(zipPath, await response.Content.ReadAsByteArrayAsync());
            var fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip();
            fastZip.ExtractZip(zipPath, tempDir, null);
        }

        return tempDir;
    }

    private static void MakeFileExecutable(string path)
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
        {
            _ = new FileInfo(path)
            {
                UnixFileMode = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute | UnixFileMode.GroupRead | UnixFileMode.OtherRead
            };
        }
    }

    private void StartUpdate(string updaterExePath, string installLocation, bool trayIsRunning)
    {
        const string SERVICE_NAME = "";

        string appType = "Console";

#pragma warning disable CA1416 // Validate platform compatibility
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) &&
            ServiceControllerStatus.Running == ServiceController.GetServices().FirstOrDefault(c => string.Equals(c.ServiceName, SERVICE_NAME, StringComparison.InvariantCultureIgnoreCase))?.Status)
        {
            appType = "WindowsService";
        }
#pragma warning restore CA1416 // Validate platform compatibility

        string args = string.Join(" ", Environment.GetCommandLineArgs().Skip(1).Select(a => a.Contains(' ') ? "\"" + a + "\"" : a)).Replace("\"", "\\\"");

        System.Diagnostics.ProcessStartInfo startInfo = new()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            // Note: add a leading space to the --Args argument to avoid parsing as arguments
            Arguments = $"--Path \"{installLocation}\" --Type \"{appType}\" --Args \" {args}\"",
            FileName = Path.Combine(updaterExePath)
        };

        try
        {
            int pid = Environment.ProcessId;
            startInfo.Arguments += $" --KillPids \"{pid}\"";
        }
        catch (Exception e)
        {
            logger.LogError("Unexpected error while retriving the PID.\n{e}", e);
        }

        if (trayIsRunning && appType == "Console")
            startInfo.Arguments += " --StartTray";

        // create .lock file to detect errors in the update process
        string lockFilePath = Path.Combine(installLocation, ".lock");
        if (!File.Exists(lockFilePath))
            File.Create(lockFilePath).Dispose();

        logger.LogInformation("Starting updater: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);
        var procInfo = System.Diagnostics.Process.Start(startInfo);
        logger.LogInformation("Updater started process id: {procInfoId}", procInfo?.Id);

        if (Environment.OSVersion.Platform != PlatformID.Unix)
        {
            logger.LogInformation("Signal sent to lock service");
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        logger.LogInformation("Exiting Seedysoft..");
        Environment.Exit(0);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                client?.Dispose();
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

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

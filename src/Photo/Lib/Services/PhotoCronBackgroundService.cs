using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Photo.Lib.Services;

public sealed class PhotoCronBackgroundService : Libs.BackgroundServices.Cron
{
    private readonly ILogger<PhotoCronBackgroundService> Logger;
    public Settings.PhotoSettings Settings => (Settings.PhotoSettings)Config;

    public PhotoCronBackgroundService(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<ILogger<PhotoCronBackgroundService>>();

        Config = ServiceProvider.GetRequiredService<IConfiguration>()
            .GetSection(nameof(Lib.Settings.PhotoSettings)).Get<Settings.PhotoSettings>()!;
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();

        string? AppName = GetType().FullName;

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            if (!Directory.Exists(Settings.PhotoRootPath))
            {
                if (Logger.IsEnabled(LogLevel.Critical))
                    Logger.LogCritical("Principal photo directory '{PhotoRootPath}' does not exists", Settings.PhotoRootPath);

                return;
            }

            DbContexts.PhotoCtx photoCtx = ServiceProvider.GetRequiredService<DbContexts.PhotoCtx>();

            DirectoryInfo directoryInfo = new(Settings.PhotoRootPath);

            //IEnumerable<DirectoryInfo> photoDirectories = directoryInfo.EnumerateDirectories(Settings.PhotoRootPath, new EnumerationOptions() { });
            DirectoryInfo[] photoDirectories = directoryInfo.GetDirectories();

            for (int i = photoDirectories.Length - 1; i > 0; i--)
            {
                DirectoryInfo subDirectory = photoDirectories.ElementAt(i);

                if (Logger.IsEnabled(LogLevel.Information))
                    Logger.LogInformation("Processing directory '{Name}'", subDirectory.Name);

                if (!int.TryParse(subDirectory.Name.AsSpan(subDirectory.Name.Length - 4), out int photoYear))
                    continue;

                await photoCtx.Photos
                    .Include((x) => x.Tags)
                    //.Where(x => x.UpdatedAtDateTimeOffset.HasValue && x.UpdatedAtDateTimeOffset.Value.Year == photoYear)
                    .Where(x => x.Year == photoYear)
                    .Where(x => !x.Tags.Any() || (x.Tags.Count == 1 && x.Tags.First().TagGuid == Entities.Tag.NoGeoLocation.TagGuid))
                    .LoadAsync(stoppingToken);

                //IEnumerable<FileInfo> photos = subDirectory.EnumerateFiles(Settings.PhotoRootPath, new EnumerationOptions() { });
                FileInfo[] photoFiles = subDirectory.GetFiles("*.*", new EnumerationOptions() { });

                for (int j = 0; j < photoFiles.Length; j++)
                {
                    FileInfo photoFile = photoFiles.ElementAt(j);

                    string photoRelativePath = Path.GetRelativePath(Settings.PhotoRootPath, photoFile.FullName);

                    Entities.Photo? photoDb = await photoCtx.Photos.FirstOrDefaultAsync(x => x.FilePath == photoRelativePath, stoppingToken);
                    if (photoDb == null)
                    {
                        photoDb = new Entities.Photo()
                        {
                            FilePath = photoRelativePath,
                            //PhotoGuid = Guid.NewGuid(),
                            //UpdatedAtDateTimeOffset = DateTimeOffset.UtcNow,
                            Year = photoYear,
                        };
                        _ = photoCtx.Add(photoDb);
                    }

                    _ = TryToSyncPhotoMetadata(photoFile, photoDb, photoCtx.Tags.First(x => x.TagGuid == Entities.Tag.NoGeoLocation.TagGuid));
                }

                _ = await photoCtx.SaveChangesAsync(stoppingToken);
            }
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("End {ApplicationName}", AppName);
    }

    internal bool TryToSyncPhotoMetadata(FileInfo photoFile, Entities.Photo photoDb, Entities.Tag noGeoLocationTag)
    {
        bool IsSuccess = false;

        try
        {
            Exif.ExifService exifService = new(photoFile.FullName);
            if (exifService.GetGpsLocation(out Libs.Core.Models.Location location))
            {
                photoDb.Location = location;
                //photoDb.UpdatedAtDateTimeOffset = DateTimeOffset.UtcNow;
            }
            else
            {
                photoDb.Tags.Add(noGeoLocationTag);
            }

            //if (exifService.GetTagRawData(
            //    tagSpec: Exif.ExifTag.ImageUniqueId,
            //    tagType: out Exif.ExifTagType exifTagType,
            //    valueCount: out int valueCount,
            //    rawData: out byte[] rawValue))
            //{
            //    photoDb.PhotoGuid = new Guid(rawValue);

            //    IsSuccess = true;
            //}
            //else
            //{
            //    byte[] myGuid = photoDb.PhotoGuid.ToByteArray();
            //    if (exifService.SetTagRawData(Exif.ExifTag.ImageUniqueId, Exif.ExifTagType.Byte, myGuid.Length, myGuid))
            //    {
            //        exifService.Save();

            //        IsSuccess = true;
            //    }
            //}
        }
        catch (Exception e)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError(e, "Cannot obtain metadata from '{Name}'", photoFile.Name);

            IsSuccess = false;
        }

        return IsSuccess;
    }
}

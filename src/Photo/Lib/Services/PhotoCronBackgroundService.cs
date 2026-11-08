using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Photo.Lib.Services;

public sealed class PhotoCronBackgroundService : Libs.BackgroundServices.Cron
{
    private readonly ILogger<PhotoCronBackgroundService> Logger;
    private Settings.PhotoSettings Settings => (Settings.PhotoSettings)Config;

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
                    //.Where(x => x.UpdatedAtDateTimeOffset.HasValue && x.UpdatedAtDateTimeOffset.Value.Year == photoYear)
                    .Where(x => x.Year == photoYear)
                    .Include((x) => x.Tags)
                    .LoadAsync(stoppingToken);

                //IEnumerable<FileInfo> photos = subDirectory.EnumerateFiles(Settings.PhotoRootPath, new EnumerationOptions() { });
                FileInfo[] photoFiles = subDirectory.GetFiles("*.*", new EnumerationOptions() { });

                for (int j = 0; j < photoFiles.Length; j++)
                {
                    FileInfo photoFile = photoFiles.ElementAt(j);

                    Entities.Photo? photoDb = await photoCtx.Photos.FirstOrDefaultAsync(x => x.FilePath == photoFile.FullName, stoppingToken);
                    if (photoDb == null)
                    {
                        photoDb = new Entities.Photo()
                        {
                            FilePath = photoFile.FullName,
                            //UpdatedAtDateTimeOffset = DateTimeOffset.UtcNow,
                            Year = photoYear,
                        };
                        _ = photoCtx.Add(photoDb);
                    }

                    if (photoDb.Latitude.HasValue && photoDb.Longitude.HasValue)
                        continue;

                    try
                    {
                        IReadOnlyList<MetadataExtractor.Directory> directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(photoFile.FullName);
                        //for (int d = 0; d < directories.Count; d++)
                        //{
                        //    MetadataExtractor.Directory directory = directories[d];
                        //    Console.WriteLine("Processing directory '{0}'", directory.Name);
                        //    for (int t = 0; t < directory.Tags.Count; t++)
                        //    {
                        //        MetadataExtractor.Tag tag = directory.Tags[t];
                        //        Console.WriteLine("\tProcessing tag '{0}' of type '{1}' with value '{2}'", tag.Name, tag.Type, tag.Description);
                        //    }
                        //}

                        MetadataExtractor.Formats.Exif.GpsDirectory? gpsDirectory =
                            directories.OfType<MetadataExtractor.Formats.Exif.GpsDirectory>().FirstOrDefault();
                        if (gpsDirectory != null && gpsDirectory.TryGetGeoLocation(out MetadataExtractor.GeoLocation geoLocation))
                        {
                            photoDb.Latitude = geoLocation.Latitude;
                            photoDb.Longitude = geoLocation.Longitude;
                            //photoDb.UpdatedAtDateTimeOffset = DateTimeOffset.UtcNow;
                        }

                        //MetadataExtractor.Formats.FileSystem.FileMetadataDirectory? fileMetadataDirectory =
                        //    directories.OfType<MetadataExtractor.Formats.FileSystem.FileMetadataDirectory>().FirstOrDefault();
                        //string? TagFileModifiedDate = fileMetadataDirectory?.GetDescription(MetadataExtractor.Formats.FileSystem.FileMetadataDirectory.TagFileModifiedDate);
                        //if (!string.IsNullOrWhiteSpace(TagFileModifiedDate) &&
                        //    DateTimeOffset.TryParse(TagFileModifiedDate, out DateTimeOffset fileModifiedDate))
                        //{
                        //    photoDb.Year = fileModifiedDate.Year;
                        //}
                        //else
                        //{
                        //    MetadataExtractor.Formats.Exif.ExifSubIfdDirectory? exifSubIfdDirectory =
                        //        directories.OfType<MetadataExtractor.Formats.Exif.ExifSubIfdDirectory>().FirstOrDefault();
                        //    string? TagDateTime = exifSubIfdDirectory?.GetDescription(MetadataExtractor.Formats.Exif.ExifDirectoryBase.TagDateTime);
                        //    if (!string.IsNullOrWhiteSpace(TagDateTime) &&
                        //        DateTime.TryParseExact(TagDateTime, "yyyy:MM:dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime tagDateTime))
                        //    {
                        //        photoDb.Year = tagDateTime.Year;
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {
                        if (Logger.IsEnabled(LogLevel.Error))
                            Logger.LogError(e, "Cannot obtain metadata from '{Name}'", photoFile.Name);
                    }
                }

                _ = await photoCtx.SaveChangesAsync(stoppingToken);
            }
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("End {ApplicationName}", AppName);
    }
}

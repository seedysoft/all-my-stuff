using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Libs.Update.Services;

public sealed class UpdaterCronBackgroundService : BackgroundServices.Cron
{
    private readonly ILogger<UpdaterCronBackgroundService> Logger;

    public UpdaterCronBackgroundService(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
        => Logger = ServiceProvider.GetRequiredService<ILogger<UpdaterCronBackgroundService>>();

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        string? AppName = GetType().FullName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            if (await IsNewVersionAvailableAsync())
            {

            }
            //Libs.Infrastructure.DbContexts.DbCxt dbCtx = ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>();

            //Libs.Core.Entities.Outbox[] PendingMessages = await dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset == null).ToArrayAsync(stoppingToken);

            //if (PendingMessages.Length == 0)
            //{
            //    Logger.LogInformation("NO pending messages");
            //}
            //else
            //{
            //    Logger.LogInformation("Obtained {PendingMessages} pending messages", PendingMessages.Length);

            //    Libs.Core.Entities.Subscriber[]? AllSubscribers = await dbCtx.Subscribers
            //        .Include(x => x.Subscriptions)
            //        .AsNoTracking()
            //        .ToArrayAsync(stoppingToken);
            //    Logger.LogInformation("Obtained {AllSubscribers} subscribers", AllSubscribers.Length);

            //    Libs.TelegramBot.Services.TelegramHostedService telegramHostedService = ServiceProvider.GetRequiredService<Libs.TelegramBot.Services.TelegramHostedService>();

            //    for (int i = 0; i < PendingMessages.Length; i++)
            //    {
            //        Libs.Core.Entities.Outbox PendingMessage = PendingMessages[i];

            //        Libs.Core.Entities.Subscriber[] Subscribers = AllSubscribers
            //            .Where(x => x.Subscriptions.Any(s => s.SubscriptionName == PendingMessage.SubscriptionName))
            //            .Where(x => PendingMessage.SubscriptionId == null || x.Subscriptions.Any(y => y.SubscriptionId == PendingMessage.SubscriptionId))
            //            .ToArray();

            //        for (int j = 0; j < Subscribers.Length; j++)
            //        {
            //            Libs.Core.Entities.Subscriber subscriber = Subscribers[j];

            //            if (subscriber.TelegramUserId.HasValue)
            //                await telegramHostedService.SendMessageToSubscriberAsync(PendingMessage, subscriber.TelegramUserId.Value, stoppingToken);

            //            if (!string.IsNullOrEmpty(subscriber.MailAddress))
            //            {
            //                string Message = PendingMessage.SubscriptionName switch
            //                {
            //                    Libs.Core.Enums.SubscriptionName.electricidad => GetHtmlBodyMail(PendingMessage.Payload),

            //                    Libs.Core.Enums.SubscriptionName.webComparer => PendingMessage.Payload,

            //                    //Enums.SubscriptionName.amazon => ,

            //                    _ => throw new ApplicationException($"Unexpected SubscriptionName: '{PendingMessage.SubscriptionName}'"),
            //                };

            //                await ServiceProvider.GetRequiredService<Libs.SmtpService.Services.SmtpService>().SendMailAsync(
            //                    subscriber.MailAddress,
            //                    PendingMessage.SubscriptionName.ToString(),
            //                    Message,
            //                    Libs.Core.Enums.SubscriptionName.electricidad == PendingMessage.SubscriptionName || Message.ContainsHtml(),
            //                    stoppingToken);
            //            }
            //        }

            //        PendingMessage.SentAtDateTimeOffset = DateTimeOffset.Now;

            //        _ = await dbCtx.SaveChangesAsync(stoppingToken);
            //    }
            //}

            //const int KeepDays = 20;
            //DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddDays(-KeepDays);
            //await dbCtx.BulkDeleteAsync(dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset < dateTimeOffset), cancellationToken: stoppingToken);
            //Logger.LogDebug("Removed {Entities} old entities", await dbCtx.SaveChangesAsync(stoppingToken));
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    internal async Task<bool> IsNewVersionAvailableAsync()
    {
        // System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        // System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        // string version = fvi.FileVersion;

        Version? CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        Version? NewVersion = await GetLatestVersionFromGithubAsync();

        return NewVersion > CurrentVersion;
    }

    internal async Task<Version?> GetLatestVersionFromGithubAsync()
    {
        var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(Core.Constants.Github.RepositoryName));
        //string GitHubToken = "--- token goes here ---";
        //var tokenAuth = new Octokit.Credentials(GitHubToken);
        //client.Credentials = tokenAuth;

        // Retrieve a List of Releases in the Repository, and get latest using [0]-subscript
        IReadOnlyList<Octokit.Release> releases =
            await client.Repository.Release.GetAll(Core.Constants.Github.OwnerName, Core.Constants.Github.RepositoryName);
        Octokit.Release latest = releases[0];

        return new Version(latest.Name);

        //// Get a HttpResponse from the Zipball URL, which is then converted to a byte array
        //Octokit.IApiResponse<object> response =
        //    await client.Connection.Get<object>(new Uri(latest.ZipballUrl), new Dictionary<string, string>(), "application/json");
        //byte[] releaseBytes = System.Text.Encoding.ASCII.GetBytes(response.HttpResponse.Body.ToString());

        //// Create the resulting file using the byte array
        //await File.WriteAllBytesAsync(filename, releaseBytes);
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;

namespace Seedysoft.Photo.Lib.Services;

public sealed class PhotoCronBackgroundService : Libs.BackgroundServices.Cron
{
    private readonly ILogger<PhotoCronBackgroundService> Logger;
    //private Settings.PhotoSettings Settings => (Settings.PhotoSettings)Config;

    public PhotoCronBackgroundService(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<ILogger<PhotoCronBackgroundService>>();

        Config = ServiceProvider.GetRequiredService<IConfiguration>()
            .GetSection(nameof(Settings.PhotoSettings)).Get<Settings.PhotoSettings>()!;
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
            Libs.Infrastructure.DbContexts.PhotoCtx photoCtx = ServiceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.PhotoCtx>();

            //Libs.Core.Entities.Outbox[] PendingMessages = await dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset == null).ToArrayAsync(stoppingToken);

            //if (PendingMessages.Length == 0)
            //{
            //    if (Logger.IsEnabled(LogLevel.Information))
            //        Logger.LogInformation("NO pending messages");
            //}
            //else
            //{
            //    if (Logger.IsEnabled(LogLevel.Information))
            //        Logger.LogInformation("Obtained {PendingMessages} pending messages", PendingMessages.Length);

            //    Libs.Core.Entities.Subscriber[]? AllSubscribers = await dbCtx.Subscribers
            //        .Include(x => x.Subscriptions)
            //        .AsNoTracking()
            //        .ToArrayAsync(stoppingToken);
            //    if (Logger.IsEnabled(LogLevel.Information))
            //        Logger.LogInformation("Obtained {AllSubscribers} subscribers", AllSubscribers.Length);

            //    Libs.TelegramBot.Services.TelegramHostedService telegramHostedService =
            //        ServiceProvider.GetRequiredService<Libs.TelegramBot.Services.TelegramHostedService>();

            //    Libs.SmtpService.Services.SmtpService smtpService =
            //        ServiceProvider.GetRequiredService<Libs.SmtpService.Services.SmtpService>();

            //    for (int i = 0; i < PendingMessages.Length; i++)
            //    {
            //        Libs.Core.Entities.Outbox PendingMessage = PendingMessages[i];

            //        Libs.Core.Entities.Subscriber[] Subscribers = [.. AllSubscribers
            //            .Where(x => x.Subscriptions.Any(s => s.SubscriptionName == PendingMessage.SubscriptionName))
            //            .Where(x => PendingMessage.SubscriptionId == null || x.Subscriptions.Any(y => y.SubscriptionId == PendingMessage.SubscriptionId))];

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

            //                await smtpService.SendMailAsync(
            //                    to: subscriber.MailAddress,
            //                    subject: PendingMessage.SubscriptionName.ToString(),
            //                    body: Message,
            //                    isBodyHtml: Libs.Core.Enums.SubscriptionName.electricidad == PendingMessage.SubscriptionName || Message.ContainsHtml(),
            //                    cancellationToken: stoppingToken);
            //            }
            //        }

            //        PendingMessage.SentAtDateTimeOffset = DateTimeOffset.Now;

            //        _ = await dbCtx.SaveChangesAsync(stoppingToken);
            //    }
            //}

            //const int KeepDays = 20;
            //DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddDays(-KeepDays);
            //await dbCtx.BulkDeleteAsync(dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset < dateTimeOffset), cancellationToken: stoppingToken);
            //if (Logger.IsEnabled(LogLevel.Debug))
            //    Logger.LogDebug("Removed {Entities} old entities", await dbCtx.SaveChangesAsync(stoppingToken));
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation("End {ApplicationName}", AppName);
    }
}

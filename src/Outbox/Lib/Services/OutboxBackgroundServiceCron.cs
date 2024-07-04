using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;
using System.Collections.Immutable;
// TODO Remove Immutable

namespace Seedysoft.Outbox.Lib.Services;

public sealed class OutboxBackgroundServiceCron : Libs.BackgroundServices.Cron
{
    private readonly ILogger<OutboxBackgroundServiceCron> Logger;

    public OutboxBackgroundServiceCron(
        IServiceProvider serviceProvider,
        Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime) : base(serviceProvider, hostApplicationLifetime)
    {
        Logger = ServiceProvider.GetRequiredService<ILogger<OutboxBackgroundServiceCron>>();

        Config = new Libs.BackgroundServices.ScheduleConfig() { CronExpression = "*/4 * * * *" /*At every 4th minute*/  };
    }

    public override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        string? AppName = GetType().FullName;

        logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            Libs.Infrastructure.DbContexts.DbCxt dbCtx = serviceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>();

            Libs.Core.Entities.Outbox[] PendingMessages = await dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset == null).ToArrayAsync(stoppingToken);

            if (PendingMessages.Length == 0)
            {
                logger.LogInformation("NO pending messages");
            }
            else
            {
                logger.LogInformation("Obtained {PendingMessages} pending messages", PendingMessages.Length);

                Libs.Core.Entities.Subscriber[]? AllSubscribers = await dbCtx.Subscribers
                    .Include(x => x.Subscriptions)
                    .AsNoTracking()
                    .ToArrayAsync(stoppingToken);
                logger.LogInformation("Obtained {AllSubscribers} subscribers", AllSubscribers.Length);

                Libs.TelegramBot.Services.TelegramHostedService telegramHostedService = serviceProvider.GetRequiredService<Libs.TelegramBot.Services.TelegramHostedService>();

                for (int i = 0; i < PendingMessages.Length; i++)
                {
                    Libs.Core.Entities.Outbox PendingMessage = PendingMessages[i];

                    var Subscribers = AllSubscribers
                        .Where(x => x.Subscriptions.Any(s => s.SubscriptionName == PendingMessage.SubscriptionName))
                        .Where(x => PendingMessage.SubscriptionId == null || x.Subscriptions.Any(y => y.SubscriptionId == PendingMessage.SubscriptionId))
                        .ToImmutableArray();

                    for (int j = 0; j < Subscribers.Length; j++)
                    {
                        Libs.Core.Entities.Subscriber subscriber = Subscribers[j];

                        if (subscriber.TelegramUserId.HasValue)
                            await telegramHostedService.SendMessageToSubscriberAsync(PendingMessage, subscriber.TelegramUserId.Value, stoppingToken);

                        if (!string.IsNullOrEmpty(subscriber.MailAddress))
                        {
                            string Message = PendingMessage.SubscriptionName switch
                            {
                                Libs.Core.Enums.SubscriptionName.electricidad => GetHtmlBodyMail(PendingMessage.Payload),

                                Libs.Core.Enums.SubscriptionName.webComparer => PendingMessage.Payload,

                                //Enums.SubscriptionName.amazon => ,

                                _ => throw new ApplicationException($"Unexpected SubscriptionName: '{PendingMessage.SubscriptionName}'"),
                            };

                            await serviceProvider.GetRequiredService<Libs.SmtpService.Services.SmtpService>().SendMailAsync(
                                subscriber.MailAddress,
                                PendingMessage.SubscriptionName.ToString(),
                                Message,
                                Libs.Core.Enums.SubscriptionName.electricidad == PendingMessage.SubscriptionName || Message.ContainsHtml(),
                                stoppingToken);
                        }
                    }

                    PendingMessage.SentAtDateTimeOffset = DateTimeOffset.Now;

                    _ = await dbCtx.SaveChangesAsync(stoppingToken);
                }
            }

            const int KeepDays = 20;
            DateTimeOffset dateTimeOffset = DateTimeOffset.Now.AddDays(-KeepDays);
            await dbCtx.BulkDeleteAsync(dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset < dateTimeOffset), cancellationToken: stoppingToken);
            logger.LogDebug("Removed {Entities} old entities", await dbCtx.SaveChangesAsync(stoppingToken));
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        logger.LogInformation("End {ApplicationName}", AppName);
    }

    private static string GetHtmlBodyMail(string payload)
    {
        IEnumerable<Libs.Core.Entities.Pvpc> entities = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Libs.Core.Entities.Pvpc>>(payload)!;
        if (!entities.Any())
            return string.Empty;

        System.Text.StringBuilder sb = new(10_000);

        decimal Avg = entities.Average(x => x.KWhPriceInEuros);
        sb = sb.Append("<!DOCTYPE html><html><body><div>");
        sb = sb.Append($"<h2>Día: {entities.First().AtDateTimeOffset.Date.ToString("dd-MM-yy (dddd)", Libs.Utils.Constants.Formats.ESCultureInfo.DateTimeFormat)}</h2>");
        sb = sb.Append($"<h3>Precio medio: {Avg:N5} € / Kwh</h3>");

        for (int h = 0; h < 3; h++)
        {
            sb = sb.Append("<table style='background-color:#000;padding:10px;'>");

            var RangoMediaPair = entities
                .Where(x => x.AtDateTimeOffset.Hour < 24 - h)
                .Select((x, i) => new
                {
                    Rango = $"{x.AtDateTimeOffset.Hour:00}+{h + 1:0}",
                    Media = entities.Skip(i).Take(h + 1).Average(x => x.KWhPriceInEuros),
                })
                .OrderBy(x => x.Rango)
                .ToImmutableArray();

            const int HowMany = 6;
            decimal Min = entities.OrderBy(x => x.KWhPriceInEuros).Take(HowMany).Max(x => x.KWhPriceInEuros);
            decimal Max = entities.OrderByDescending(x => x.KWhPriceInEuros).Take(HowMany).Min(x => x.KWhPriceInEuros);

            const int Rows = 6;
            const int Cols = 4;
            for (int row = 0; row < Rows; row++)
            {
                sb = sb.Append("<tr>");

                for (int col = 0; col < Cols; col++)
                {
                    int CurrentIndex = (col * Rows) + row;
                    if (CurrentIndex >= RangoMediaPair.Length)
                    {
                        sb = sb.Append("<td></td>");
                        continue;
                    }

                    var CurrentPair = RangoMediaPair[CurrentIndex];

                    string CellPriceColor =
                        CurrentPair.Media >= Max ? "#F00" :
                        CurrentPair.Media <= Min ? "#0F0" :
                        CurrentPair.Media <= Avg ? "#FF0" : "#F90";

                    sb = sb.Append($"<td style='color:#FFF;padding:5px 5px 0px {(col == 0 ? "0" : " 20")}px;'>{CurrentPair.Rango}</td>");
                    sb = sb.Append($"<td style='color:{CellPriceColor};padding:5px 5px 0px 0px;text-align:right;'>{CurrentPair.Media:N5}</td>");
                    /*
                     * padding:10px 5px 15px 20px;
                     *      top padding is 10px
                     *      right padding is 5px
                     *      bottom padding is 15px
                     *      left padding is 20px
                     */
                }

                sb = sb.Append("</tr>");
            }

            sb = sb.Append("</table>");
        }

        sb = sb.Append("</div></body></html>");

        return sb.ToString();
    }
}

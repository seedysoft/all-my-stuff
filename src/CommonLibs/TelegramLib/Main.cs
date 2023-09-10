﻿using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using System.Collections.Immutable;

namespace Seedysoft.TelegramLib;

public static class Main
{
    public static async Task SendMessagesAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken stoppingToken)
    {
        try
        {
            DbContexts.DbCxt dbCtx = serviceProvider.GetRequiredService<DbContexts.DbCxt>();

            CoreLib.Entities.Outbox[] PendingMessages = await dbCtx.Outbox.Where(x => x.SentAtDateTimeOffset == null).ToArrayAsync(stoppingToken);

            if (PendingMessages.Any())
            {
                logger.LogInformation("Obtained {PendingMessages} pending messages", PendingMessages.Length);

                CoreLib.Entities.Subscriber[]? AllSubscribers = await dbCtx.Subscribers
                    .Include(x => x.Subscriptions)
                    .AsNoTracking()
                    .ToArrayAsync(stoppingToken);
                logger.LogInformation("Obtained {AllSubscribers} subscribers", AllSubscribers.Length);

                Services.TelegramService TelegramService = serviceProvider.GetRequiredService<Services.TelegramService>();

                for (int i = 0; i < PendingMessages.Length; i++)
                {
                    CoreLib.Entities.Outbox PendingMessage = PendingMessages[i];

                    var Subscribers = AllSubscribers
                        .Where(x => x.Subscriptions.Any(s => s.SubscriptionName == PendingMessage.SubscriptionName))
                        .Where(x => PendingMessage.SubscriptionId == null || x.Subscriptions.Any(y => y.SubscriptionId == PendingMessage.SubscriptionId))
                        .ToImmutableArray();

                    for (int j = 0; j < Subscribers.Length; j++)
                    {
                        CoreLib.Entities.Subscriber subscriber = Subscribers[j];

                        if (subscriber.TelegramUserId.HasValue)
                        {
                            _ = PendingMessage.SubscriptionName switch
                            {
                                CoreLib.Enums.SubscriptionName.electricidad => await TelegramService.MessageSendTextAsync(
                                    subscriber.TelegramUserId.Value,
                                    Services.TelegramService.MessageGetMarkdownV2TextForPrices(System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CoreLib.Entities.Pvpc>>(PendingMessage.Payload)!),
                                    Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                                    stoppingToken),

                                CoreLib.Enums.SubscriptionName.webComparer => await TelegramService.MessageSendTextAsync(
                                    subscriber.TelegramUserId.Value,
                                    PendingMessage.Payload[new Range(0, Math.Min(UtilsLib.Constants.Telegram.MessageLengthLimit, PendingMessage.Payload.Length))],
                                    null,
                                    stoppingToken),

                                //Enums.SubscriptionName.amazon => await TelegramService.MessageSendTextAsync(
                                //    subscriber.TelegramUserId.Value,
                                //    null,
                                //    stoppingToken),

                                _ => throw new ApplicationException($"Unexpected SubscriptionName: '{PendingMessage.SubscriptionName}?"),
                            };
                        }

                        if (!string.IsNullOrEmpty(subscriber.MailAddress))
                        {
                            string Message = PendingMessage.SubscriptionName switch
                            {
                                CoreLib.Enums.SubscriptionName.electricidad => GetHtmlBodyMail(System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CoreLib.Entities.Pvpc>>(PendingMessage.Payload)!),

                                CoreLib.Enums.SubscriptionName.webComparer => PendingMessage.Payload,

                                //Enums.SubscriptionName.amazon => ,

                                _ => throw new ApplicationException($"Unexpected SubscriptionName: '{PendingMessage.SubscriptionName}'"),
                            };

                            await serviceProvider.GetRequiredService<SmtpServiceLib.Services.SmtpService>().SendMailAsync(
                                subscriber.MailAddress,
                                PendingMessage.SubscriptionName.ToString(),
                                Message,
                                CoreLib.Enums.SubscriptionName.electricidad == PendingMessage.SubscriptionName || Message.ContainsHtml(),
                                stoppingToken);
                        }
                    }

                    PendingMessage.SentAtDateTimeOffset = DateTimeOffset.Now;

                    _ = await dbCtx.SaveChangesAsync(stoppingToken);
                }
            }
            else
            {
                logger.LogInformation("NO pending messages");
            }

            long MonthAgoUnixTimeSeconds = DateTimeOffset.Now.AddDays(-20).ToUnixTimeSeconds();
            await dbCtx.BulkDeleteAsync(dbCtx.OutboxView.Where(x => x.SentAtDateTimeUnix < MonthAgoUnixTimeSeconds), cancellationToken: stoppingToken);
            logger.LogDebug("Removed {Entities} old entities", await dbCtx.SaveChangesAsync(stoppingToken));
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }
    }

    private static string GetHtmlBodyMail(IEnumerable<CoreLib.Entities.Pvpc> entities)
    {
        if (!entities.Any())
            return string.Empty;

        System.Text.StringBuilder sb = new(10_000);

        decimal Avg = entities.Average(x => x.KWhPriceInEuros);
        sb = sb.Append("<!DOCTYPE html><html><body><div>");
        sb = sb.Append($"<h2>Día: {entities.First().AtDateTimeOffset.Date.ToString("dd-MM-yy (dddd)", UtilsLib.Constants.Formats.ESCultureInfo.DateTimeFormat)}</h2>");
        sb = sb.Append($"<h3>Precio medio: {Avg:N5} € / Kwh</h3>");

        for (int h = 0; h < 3; h++)
        {
            sb = sb.Append("<table style='background-color:#000;padding:10px;'>");

            var RangoMediaPair = entities
                .Where(x => x.AtDateTimeOffset.Hour < 24 - h)
                .Select((x, i) => new
                {
                    Rango = $"{x.AtDateTimeOffset.Hour:00}+{h + 1:0}",
                    Media = entities.Skip(i).Take(h + 1).Average(x => x.KWhPriceInEuros)
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.UtilsLib.Extensions;
using System.Collections.Immutable;
using Telegram.Bot;

namespace Seedysoft.TelegramLib.Services;

public partial class TelegramService
{
    private readonly ILogger<TelegramService> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly TelegramBotClient TelegramBotClient;

    public TelegramService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Logger = ServiceProvider.GetRequiredService<ILogger<TelegramService>>();

        TelegramBotClientOptions telegramBotClientOptions = new(
            token: $"{Bots.Current.Id}:{ServiceProvider.GetRequiredService<Settings.TelegramSettings>().Tokens[Bots.Current.Id.ToString()]}");

        TelegramBotClient = new TelegramBotClient(telegramBotClientOptions);
    }

    public async Task StartReceivingAsync(
        IEnumerable<Telegram.Bot.Types.BotCommand>? myCommands
        , CancellationToken stoppingToken)
    {
        Bots.Current.SetMe(await TelegramBotClient.GetMeAsync(stoppingToken));

        if (myCommands != null)
            //_TelegramBotClient.DeleteMyCommandsAsync
            await TelegramBotClient.SetMyCommandsAsync(commands: myCommands, cancellationToken: stoppingToken);

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        Telegram.Bot.Polling.ReceiverOptions OptionsReceiver = new()
        {
            AllowedUpdates = default,
            Limit = default,
            Offset = default,
            ThrowPendingUpdates = default,
        };
        //Telegram.Bot.Types.Enums.UpdateType[] UpdatesAllowed = { Telegram.Bot.Types.Enums.UpdateType.Message | Telegram.Bot.Types.Enums.UpdateType.Unknown };
        //receiverOptions.AllowedUpdates = UpdatesAllowed;

        TelegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, OptionsReceiver, stoppingToken);
    }

    private async Task HandleErrorAsync(
        ITelegramBotClient botClient
        , Exception exception
        , CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case Telegram.Bot.Exceptions.ApiRequestException apiRequestException:
                Logger.LogError("Telegram API Error:\n[{ErrorCode}]\n{Message}", apiRequestException.ErrorCode, apiRequestException.Message);
                break;

            case Telegram.Bot.Exceptions.RequestException requestException:
                Logger.LogError("Telegram API Error:\n[{HttpStatusCode}]\n{Message}", requestException.HttpStatusCode, requestException.Message);
                break;

            default:
                Logger.LogError("{ex}", exception);
                break;
        };

        await Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient
        , Telegram.Bot.Types.Update update
        , CancellationToken cancellationToken)
    {
        try
        {
            Task handler = update.Type switch
            {
                Telegram.Bot.Types.Enums.UpdateType.CallbackQuery => BotOnCallbackQueryReceivedAsync(botClient, update.CallbackQuery!, cancellationToken),

                Telegram.Bot.Types.Enums.UpdateType.ChannelPost => BotOnMessageReceivedAsync(update.ChannelPost!, cancellationToken),

                Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceivedAsync(botClient, update.ChosenInlineResult!, cancellationToken),

                Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost => BotOnMessageReceivedAsync(update.EditedChannelPost!, cancellationToken),

                Telegram.Bot.Types.Enums.UpdateType.EditedMessage => BotOnMessageReceivedAsync(update.EditedMessage!, cancellationToken),

                Telegram.Bot.Types.Enums.UpdateType.InlineQuery => BotOnInlineQueryReceivedAsync(botClient, update.InlineQuery!, cancellationToken),

                Telegram.Bot.Types.Enums.UpdateType.Message => BotOnMessageReceivedAsync(update.Message!, cancellationToken),

                _ => throw new NotImplementedException(update.Type.ToString()),
            };

            await handler;
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected Error")) { }
    }

    internal async Task<Telegram.Bot.Types.Message> MessageSendSimpleTextAsync(
        long to
        , string text
        , CancellationToken cancellationToken) =>
        await MessageSendTextAsync(to, text, null, cancellationToken);

    internal static string MessageGetMarkdownV2TextForPrices(
        IEnumerable<CoreLib.Entities.PvpcBase> prices)
    {
        if (prices == null || !prices.Any())
            return "No hay datos";

        var HorasRestantes = prices.ToImmutableArray();

        DateTime ObtainedDate = prices.First().AtDateTimeOffset.Date;

        int HowMany = Math.Min(6, HorasRestantes.Length - 1);
        decimal Min = HorasRestantes.OrderBy(x => x.KWhPriceInEuros).Take(HowMany).Max(x => x.KWhPriceInEuros);
        decimal Max = HorasRestantes.OrderByDescending(x => x.KWhPriceInEuros).Take(HowMany).Min(x => x.KWhPriceInEuros);
        decimal Avg = prices.Average(x => x.KWhPriceInEuros);

        System.Text.StringBuilder sb = new(500);
        sb = sb.AppendLine($"Media del *{ObtainedDate.ToString(UtilsLib.Constants.Formats.LongDateFormat, UtilsLib.Constants.Formats.ESCultureInfo)}*");
        sb = sb.AppendLine($"*{Avg.ToString("N5", UtilsLib.Constants.Formats.ESCultureInfo)} € / kWh*");

        for (int i = 0; i < HorasRestantes.Length; i++)
        {
            string emoji =
                HorasRestantes[i].KWhPriceInEuros >= Max ? Constants.Emojis.RedCircle :
                HorasRestantes[i].KWhPriceInEuros <= Min ? Constants.Emojis.GreenCircle :
                HorasRestantes[i].KWhPriceInEuros <= Avg ? Constants.Emojis.YellowCircle : Constants.Emojis.OrangeCircle;

            sb = sb.AppendLine($"{HorasRestantes[i].AtDateTimeOffset.Hour:00}  {emoji}  {HorasRestantes[i].KWhPriceInEuros.ToString("N5", UtilsLib.Constants.Formats.ESCultureInfo)}");
        }

        return sb.ToString();
    }

    internal async Task<Telegram.Bot.Types.Message> MessageSendQueryAsync(
        long to
        , string text
        , Telegram.Bot.Types.ReplyMarkups.IReplyMarkup replyMarkup
        , CancellationToken cancellationToken)
    {
        text = text[..Math.Min(text.Length, UtilsLib.Constants.Telegram.MessageLengthLimit)];
        Telegram.Bot.Types.ChatId ToChatId = new(to);

        try
        {
            return await TelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException e) when (e.Message?.StartsWith("Bad Request: can't parse entities:", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            // Sample:
            //  "Telegram.Bot.Exceptions.ApiRequestException: Bad Request: can't parse entities: Unexpected end tag at byte offset 4120"
            //  +  Descargar Modelo 790
            //  =  Ver más
            //  =  </form>
            // Retry without HTML
            return await TelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                cancellationToken: cancellationToken);
        }
    }

    internal async Task<Telegram.Bot.Types.Message> MessageSendTextAsync(
        long to
        , string text
        , Telegram.Bot.Types.Enums.ParseMode? parseMode
        , CancellationToken cancellationToken)
    {
        text = text[..Math.Min(text.Length, UtilsLib.Constants.Telegram.MessageLengthLimit)];
        Telegram.Bot.Types.ChatId ToChatId = new(to);

        try
        {
            return await TelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                parseMode: parseMode ?? (text.ContainsHtml() ? Telegram.Bot.Types.Enums.ParseMode.Html : null),
                cancellationToken: cancellationToken);
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException e) when (e.Message?.StartsWith("Bad Request: can't parse entities:", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            // Sample:
            //  "Telegram.Bot.Exceptions.ApiRequestException: Bad Request: can't parse entities: Unexpected end tag at byte offset 4120"
            //  +  Descargar Modelo 790
            //  =  Ver más
            //  =  </form>
            // Retry without HTML
            return await TelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                cancellationToken: cancellationToken);
        }
    }

    private static async Task<CoreLib.Entities.Subscriber> SubscriberWithSubscriptionsGetOrCreateAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.User user
        , CancellationToken cancellationToken)
    {
        CoreLib.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(user.Id, cancellationToken);
        if (SubscriberWithSubscriptions == null)
        {
            SubscriberWithSubscriptions = new CoreLib.Entities.Subscriber(user.Username ?? user.FirstName) { TelegramUserId = user.Id };
            _ = await dbCtx.Subscribers.AddAsync(SubscriberWithSubscriptions, cancellationToken);
        }

        return SubscriberWithSubscriptions;
    }

    private static async Task<string[]?> SubscriberGetWebUrlsAsync(
        DbContexts.DbCxt dbCtx
        , long telegramUserId
        , CancellationToken cancellationToken)
    {
        CoreLib.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(telegramUserId, cancellationToken);
        if (SubscriberWithSubscriptions == null)
            return null;

        string[]? WebUrls = await dbCtx.WebDatas
            .Where(x => SubscriberWithSubscriptions.Subscriptions.Select(y => y.SubscriptionId).Contains(x.SubscriptionId))
            .OrderBy(x => x.SubscriptionId)
            .Select(x => $"{x.SubscriptionId} ({x.WebUrl})")
            .ToArrayAsync(cancellationToken);

        return WebUrls;
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceivedAsync(
        ITelegramBotClient botClient
        , Telegram.Bot.Types.CallbackQuery callbackQuery
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Received CallbackQuery: {Data}", callbackQuery.Data);

        var CallbackQueryDatas = CallbackData.Parse(callbackQuery.Data);
        string? ResponseText = (CallbackQueryDatas?.BotActionName) switch
        {
            Enums.BotActionName.email_edit => await GetResponseTextLocal(callbackQuery, cancellationToken),
            _ => null,
        };
        _ = await botClient.EditMessageReplyMarkupAsync(
            chatId: callbackQuery.Message!.Chat.Id
            , messageId: callbackQuery.Message!.MessageId
            , replyMarkup: Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup.Empty()
            , cancellationToken: cancellationToken);

        //await botClient.AnswerCallbackQueryAsync(
        //    callbackQueryId: callbackQuery.Id,
        //    text: ResponseText ?? $"No he podido saber qué hacer con {callbackQuery.Data ?? "Nulo"}",
        //    showAlert: false,
        //    cancellationToken: cancellationToken);

        _ = await botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id
            , text: ResponseText ?? $"No he podido saber qué hacer con {callbackQuery.Data ?? "Nulo"}"
            , cancellationToken: cancellationToken);

        async Task<string?> GetResponseTextLocal(Telegram.Bot.Types.CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await SubscriberSetEmailAsync(ServiceProvider.GetRequiredService<DbContexts.DbCxt>(), callbackQuery.Message!, null, cancellationToken);

            return "Se ha borrado su correo electrónico";
        }
    }

    private static async Task SubscriberSetEmailAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , System.Net.Mail.MailAddress? mailAddress
        , CancellationToken cancellationToken)
    {
        CoreLib.Entities.Subscriber TheSubscriber = await SubscriberWithSubscriptionsGetOrCreateAsync(dbCtx, message.From!, cancellationToken);

        TheSubscriber.MailAddress = mailAddress?.Address;

        _ = await dbCtx.SaveChangesAsync(cancellationToken);
    }

    private async Task BotOnChosenInlineResultReceivedAsync(
        ITelegramBotClient botClient
        , Telegram.Bot.Types.ChosenInlineResult chosenInlineResult
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Received ChosenInlineResult: {ResultId}", chosenInlineResult.ResultId);

        _ = await botClient.SendTextMessageAsync(
            chatId: chosenInlineResult.From.Id,
            text: $"Received {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    private async Task BotOnInlineQueryReceivedAsync(
        ITelegramBotClient botClient
        , Telegram.Bot.Types.InlineQuery inlineQuery
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Received inline query from: {Id}", inlineQuery.From.Id);

        // displayed result
        Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
            new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent("hello")) };

        await botClient.AnswerInlineQueryAsync(
            inlineQueryId: inlineQuery.Id,
            results: results,
            isPersonal: true,
            cacheTime: 0,
            cancellationToken: cancellationToken);
    }

    private Task BotOnMessageAutoDeleteTimerChangedReceived(
        Telegram.Bot.Types.Message message)
    {
        Logger.LogInformation("Establecido tiempo de autoborrado en {MessageAutoDeleteTime} segundos", message.MessageAutoDeleteTimerChanged!.MessageAutoDeleteTime);

        return Task.CompletedTask;
    }

    private async Task BotOnMessageDocumentReceivedAsync(
        Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        await ChatActionSendAsync(message, Telegram.Bot.Types.Enums.ChatAction.Typing, cancellationToken);

        //await ProcesarDocumentoConsumosAsync(message, cancellationToken); // TODO Manejar todos los tipos de documentos

        Task<Telegram.Bot.Types.Message>? handler = message.Caption switch
        {
            //BotCommandNames.SubirConsumos => ProcesarDocumentoConsumos(),
            _ => MessageSendTextAsync(message.Chat.Id, "No sé qué hacer con eso...", null, cancellationToken)
        };

        Telegram.Bot.Types.Message sentMessage = await handler;

        Logger.LogDebug("The message was sent with Id: {MessageId}", sentMessage.MessageId);
    }

    private async Task BotOnMessageReceivedAsync(
        Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Receive message type: {Type}", message.Type);

        Task handler = message.Type switch
        {
            Telegram.Bot.Types.Enums.MessageType.Document => BotOnMessageDocumentReceivedAsync(message, cancellationToken),

            Telegram.Bot.Types.Enums.MessageType.MessageAutoDeleteTimerChanged => BotOnMessageAutoDeleteTimerChangedReceived(message),

            Telegram.Bot.Types.Enums.MessageType.Text => BotOnMessageTextReceivedAsync(message, cancellationToken),

            _ => throw new NotImplementedException(message.Type.ToString()),
        };

        try
        {
            await handler;
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected Error")) { }
    }

    private async Task BotOnMessageTextReceivedAsync(
        Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        await ChatActionSendAsync(message, Telegram.Bot.Types.Enums.ChatAction.Typing, cancellationToken);

        string FirstWordReceived = message.Text!.Split(' ').First();

        if (FirstWordReceived.StartsWith('/') &&
            Enum.TryParse(FirstWordReceived[1..].ToLowerInvariant(), out Enums.BotActionName ReceivedCommand))
        {
            await ManageCommandReceivedAsync(ServiceProvider.GetRequiredService<DbContexts.DbCxt>(), message, ReceivedCommand, cancellationToken);
        }
        else if (FirstWordReceived.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
        {
            await ManageNewSubscriptionAsync(ServiceProvider.GetRequiredService<DbContexts.DbCxt>(), message, FirstWordReceived, cancellationToken);
        }
        else
        {
            await ManageNoCommandReceivedAsync(message, cancellationToken);
        }
    }

    private async Task<Telegram.Bot.Types.Message> PvpcGetAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || !DateTime.TryParseExact(
            data[1],
            UtilsLib.Constants.Formats.YearMonthDayFormat,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out DateTime dateTimeToObtain))
        {
            // Si pedimos después de las 20:30, nos devolverá los datos del día siguiente.
            dateTimeToObtain = DateTime.Now.TimeOfDay > new TimeSpan(20, 30, 00) ? DateTime.Today.AddDays(1) : DateTime.Today;
        }

        _ = await ObtainPvpcLib.Main.ObtainPricesAsync(
            dbCtx,
            ServiceProvider.GetRequiredService<ObtainPvpcLib.Settings.ObtainPvpcSettings>(),
            Logger,
            dateTimeToObtain,
            cancellationToken);

        CoreLib.Entities.PvpcView[]? Prices = await dbCtx.GetPvpcBetweenDatesAsync(
            dateTimeToObtain.Date,
            dateTimeToObtain.Date.AddDays(1),
            cancellationToken);

        string responseText = MessageGetMarkdownV2TextForPrices(Prices);

        return await MessageSendTextAsync(message.Chat.Id, responseText, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, cancellationToken);
    }

    private async Task<Telegram.Bot.Types.Message> MailSetAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        string ResponseText;

        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || string.IsNullOrWhiteSpace(data[1]))
        {
            CallbackData RemoveEmailCallbackData = new (Enums.BotActionName.email_edit);
            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup ReplyKeyboard = new(
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Sí", RemoveEmailCallbackData.ToString()));
            ResponseText = "¿Realmente quieres quitar tu correo electrónico asociado?";

            return await MessageSendQueryAsync(message.Chat.Id, ResponseText, ReplyKeyboard, cancellationToken);
        }
        else
        {
            // TODO MailSetAsync
            long TelegramUserId = message.From!.Id;
            CoreLib.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(TelegramUserId, cancellationToken);
            if (SubscriberWithSubscriptions == null)
                return await MessageSendUsageAsync(TelegramUserId, cancellationToken);

            //Entities.Subscription? Subscription = SubscriberWithSubscriptions.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);
            //if (Subscription == null)
            //{
            //    ResponseText = $"{Constants.Emojis.FaceWithRaisedEyebrow} No estás suscrito a {subscriptionId}";
            //}
            //else
            //{
            //    _ = SubscriberWithSubscriptions.Subscriptions.Remove(Subscription);

            //    _ = await dbCtx.SaveChangesAsync(cancellationToken);

            ResponseText = $"Te has dado de baja correctamente";
            //}
        }

        return await MessageSendTextAsync(message.Chat.Id, ResponseText, null, cancellationToken);
    }

    private async Task<Telegram.Bot.Types.Message> MailShowAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        long TelegramUserId = message.From!.Id;

        CoreLib.Entities.Subscriber? Subscriber = await dbCtx.Subscribers.FirstOrDefaultAsync(x => x.TelegramUserId == TelegramUserId, cancellationToken);

        if (Subscriber == null)
            return await MessageSendUsageAsync(message.Chat.Id, cancellationToken);

        string TextToSend = string.IsNullOrEmpty(Subscriber.MailAddress) ? "No tienes correo electrónico asociado" : Subscriber.MailAddress;

        return await MessageSendTextAsync(TelegramUserId, TextToSend, null, cancellationToken);
    }

    private async Task ManageCommandReceivedAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , Enums.BotActionName receivedCommand
        , CancellationToken cancellationToken)
    {
        Task<Telegram.Bot.Types.Message> NextAction = receivedCommand switch
        {
            Enums.BotActionName.start => StartAsync(dbCtx, message, cancellationToken),

            Enums.BotActionName.email_show => MailShowAsync(dbCtx, message, cancellationToken),
            Enums.BotActionName.email_edit => MailSetAsync(dbCtx, message, cancellationToken),

            Enums.BotActionName.pvpc_fill => PvpcGetAsync(dbCtx, message, cancellationToken),

            Enums.BotActionName.amaz_find => AmazFindAsync(dbCtx, message, cancellationToken),

            Enums.BotActionName.subs_list => SubscriptionsListAsync(dbCtx, message, cancellationToken),
            Enums.BotActionName.subs_quit => UnsubscribeAsync(dbCtx, message, cancellationToken),

            _ => MessageSendUsageAsync(message.Chat.Id, cancellationToken)
        };

        Telegram.Bot.Types.Message sentMessage = await NextAction;

        Logger.LogDebug("The message was sent with Id: {MessageId}", sentMessage.MessageId);
    }

    private Task<Telegram.Bot.Types.Message> AmazFindAsync(
        DbContexts.DbCxt dbCtx,
        Telegram.Bot.Types.Message message,
        CancellationToken cancellationToken)
    {
#pragma warning disable IDE0022 // Use expression body for method
        throw new NotImplementedException();
#pragma warning restore IDE0022 // Use expression body for method
    }

    private async Task ManageNewSubscriptionAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , string FirstWordReceived
        , CancellationToken cancellationToken)
    {
        long SenderChatId = message.Chat.Id;

        Entities.WebData? webData = await dbCtx.WebDatas.FirstOrDefaultAsync(x => x.WebUrl == FirstWordReceived, cancellationToken);
        if (webData == null)
        {
            webData = new Entities.WebData(FirstWordReceived, $"Received by Telegram on {message.Date}");
            _ = await dbCtx.WebDatas.AddAsync(webData, cancellationToken);

            _ = await MessageSendSimpleTextAsync(SenderChatId, "Added new WebData", cancellationToken);
        }

        _ = await dbCtx.SaveChangesAsync(cancellationToken);

        CoreLib.Entities.Subscriber TheSubscriber = await SubscriberWithSubscriptionsGetOrCreateAsync(dbCtx, message.From!, cancellationToken);
        if (dbCtx.ChangeTracker.Entries().FirstOrDefault(x => x.Entity == TheSubscriber)?.State == EntityState.Added)
            _ = await MessageSendSimpleTextAsync(SenderChatId, "Added new Subscriber", cancellationToken);

        CoreLib.Entities.Subscription? TheSubscription =
            await dbCtx.Subscriptions
            .FirstOrDefaultAsync(x => x.SubscriptionId == webData.SubscriptionId && x.SubscriptionName == CoreLib.Enums.SubscriptionName.webComparer, cancellationToken);
        if (TheSubscription == null)
        {
            TheSubscription = new CoreLib.Entities.Subscription(CoreLib.Enums.SubscriptionName.webComparer) { SubscriptionId = webData.SubscriptionId };
            _ = await dbCtx.Subscriptions.AddAsync(TheSubscription, cancellationToken);

            _ = await MessageSendSimpleTextAsync(SenderChatId, "Added new Subscription", cancellationToken);
        }

        if (!TheSubscriber!.Subscriptions.Any(x => x.SubscriptionId == webData.SubscriptionId))
        {
            TheSubscriber!.Subscriptions.Add(TheSubscription);

            _ = await MessageSendSimpleTextAsync(SenderChatId, "Added Subscription to Subscriber", cancellationToken);
        }

        _ = await dbCtx.SaveChangesAsync(cancellationToken);
    }

    private async Task ManageNoCommandReceivedAsync(
        Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        Logger.LogInformation("{Text}", message.Text);

        await Task.Delay(UtilsLib.Constants.Time.OneTenthOfSecondTimeSpan, cancellationToken);
    }

    private async Task ChatActionSendAsync(
        Telegram.Bot.Types.Message message
        , Telegram.Bot.Types.Enums.ChatAction chatAction
        , CancellationToken cancellationToken) =>
        await TelegramBotClient.SendChatActionAsync(
            chatId: message.Chat.Id,
            chatAction: chatAction,
            cancellationToken: cancellationToken);

    private async Task<Telegram.Bot.Types.Message> StartAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || !System.Net.Mail.MailAddress.TryCreate(data[1], out System.Net.Mail.MailAddress? mailAddress))
            mailAddress = null;

        await SubscriberSetEmailAsync(dbCtx, message, mailAddress, cancellationToken);

        string responseText = $"Bienvenid@ {message.From!.Username ?? message.From!.FirstName}";

        return await MessageSendTextAsync(message.Chat.Id, responseText, null, cancellationToken);
    }

    private async Task<Telegram.Bot.Types.Message> SubscriptionsListAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        long TelegramUserId = message.From!.Id;

        string[]? Subscriptions = await SubscriberGetWebUrlsAsync(dbCtx, TelegramUserId, cancellationToken);
        if (Subscriptions == null)
            return await MessageSendUsageAsync(TelegramUserId, cancellationToken);

        string TextToSend = Subscriptions.Any()
            ? "Estás suscrito a: \n" + string.Join("\n", Subscriptions.Select(x => $"{x}"))
            : "No tienes suscripciones";

        return await MessageSendTextAsync(TelegramUserId, TextToSend, null, cancellationToken);
    }

    private async Task<Telegram.Bot.Types.Message> MessageSendUsageAsync(
        long to
        , CancellationToken cancellationToken)
    {
        Telegram.Bot.Types.BotCommand[] MyCommands = await TelegramBotClient.GetMyCommandsAsync(cancellationToken: cancellationToken);

        // TODO                 Cambiar el mensaje de ayuda, incluyendo que pueden enviar direcciones http[s].
        return await MessageSendTextAsync(
            to,
            "Comandos disponibles:\n" + string.Join("\n", MyCommands.Select(x => $"/{x.Command} {x.Description}")),
            null,
            cancellationToken);
    }

    private async Task<Telegram.Bot.Types.Message> UnsubscribeAsync(
        DbContexts.DbCxt dbCtx
        , Telegram.Bot.Types.Message message
        , CancellationToken cancellationToken)
    {
        string ResponseText;

        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || !int.TryParse(data[1], out int subscriptionId))
        {
            ResponseText = $"No sé de qué suscripción te quieres dar de baja {Constants.Emojis.PersonShrugging}";
        }
        else
        {
            long TelegramUserId = message.From!.Id;
            CoreLib.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(TelegramUserId, cancellationToken);
            if (SubscriberWithSubscriptions == null)
                return await MessageSendUsageAsync(TelegramUserId, cancellationToken);

            CoreLib.Entities.Subscription? Subscription = SubscriberWithSubscriptions.Subscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId);
            if (Subscription == null)
            {
                ResponseText = $"{Constants.Emojis.FaceWithRaisedEyebrow} No estás suscrito a {subscriptionId}";
            }
            else
            {
                _ = SubscriberWithSubscriptions.Subscriptions.Remove(Subscription);

                _ = await dbCtx.SaveChangesAsync(cancellationToken);

                ResponseText = $"Te has dado de baja correctamente";
            }
        }

        return await MessageSendTextAsync(message.Chat.Id, ResponseText, null, cancellationToken);
    }

    //private async Task ProcesarDocumentoConsumosAsync(
    //  Message message
    //  , CancellationToken cancellationToken)
    //{
    //    using MemoryStream memoryStream = new();

    //    Telegram.Bot.Types.File? file = await _TelegramBotClient.GetInfoAndDownloadFileAsync(message.Document!.FileId, memoryStream, cancellationToken);

    //    byte[] fileBytes = memoryStream.ToArray();

    //    string[]? FileLines = System.Text.Encoding.UTF8.GetString(fileBytes).Split("\n");

    //    await GuardarConsumosEnBaseDatosAsync(FileLines!, cancellationToken);
    //}

    //public async Task GuardarConsumosEnBaseDatosAsync(
    //  string[] fileLines
    //  , CancellationToken cancellationToken)
    //{
    //    ElectricidadLib.ConsumoEnergiaActivaExportado[] Consumos = ElectricidadLib.ConsumoEnergiaActivaExportado.ParseLineas(fileLines);

    //    ElectricidadLib.ElectricidadDbContext electricidadDbContext = new();

    //    await electricidadDbContext.Consumos
    //        .Where(x => Consumos.Select(y => y.Dia).Distinct().Contains(x.Dia))
    //        .LoadAsync(cancellationToken);

    //    for (int i = 0; i < Consumos.Length; i++)
    //    {
    //        ElectricidadLib.ConsumoEnergiaActivaExportado? Consumo = Consumos[i];
    //        ElectricidadLib.ConsumoEnergiaActivaExportado? ConsumoEnBd =
    //            await electricidadDbContext.Consumos.FindAsync(new object?[] { Consumo.Cups, Consumo.Dia, Consumo.Hora }, cancellationToken);

    //        if (ConsumoEnBd == null)
    //            ConsumoEnBd = (await electricidadDbContext.Consumos.AddAsync(Consumo, cancellationToken)).Entity;

    //        ConsumoEnBd.KWh = Consumo.KWh;

    //        if (electricidadDbContext.Entry(ConsumoEnBd).State == EntityState.Modified)
    //            _ = electricidadDbContext.Update(ConsumoEnBd);
    //    }

    //    _ = await electricidadDbContext.SaveChangesAsync(cancellationToken);
    //}
}

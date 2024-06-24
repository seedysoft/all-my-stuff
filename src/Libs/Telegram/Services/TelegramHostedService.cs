#if !SKIP_DEBUG_TELEGRAM
using Microsoft.EntityFrameworkCore;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Utils.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Seedysoft.Libs.Telegram.Services;

public partial class TelegramHostedService : Microsoft.Extensions.Hosting.IHostedService
{
    private readonly ILogger<TelegramHostedService> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly Settings.TelegramSettings TelegramSettings;
    private readonly TelegramBotClient LocalTelegramBotClient;

    public TelegramHostedService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Logger = ServiceProvider.GetRequiredService<ILogger<TelegramHostedService>>();

        TelegramSettings = ServiceProvider.GetRequiredService<Settings.TelegramSettings>();

        TelegramBotClientOptions telegramBotClientOptions = new(
            token: $"{TelegramSettings.CurrentBot.Id}:{TelegramSettings.CurrentBot.Token}");

        LocalTelegramBotClient = new TelegramBotClient(telegramBotClientOptions);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Called {ApplicationName} version {Version}", GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        BotCommand[] Commands = GetMyCommands();

        await StartReceivingAsync(Commands, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("End {ApplicationName}", GetType().FullName);

        await Task.CompletedTask;
    }

    private static BotCommand[] GetMyCommands()
    {
        return Enum.GetValues<Enums.BotActionName>().
            Select(x => new BotCommand()
            {
                Command = x.ToString(),
                Description = x.GetEnumDescription()
            })
            .ToArray();
    }

    private async Task StartReceivingAsync(
        IEnumerable<BotCommand>? myCommands
        , CancellationToken stoppingToken)
    {
#if SKIP_DEBUG_TELEGRAM
        Logger.ToString();

        await Task.CompletedTask;
    }

    private async Task<Message> MessageSendTextAsync(
        long to
        , string text
        , ParseMode? parseMode
        , CancellationToken cancellationToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            to = long.Parse(TelegramSettings.Users.UserTest.Id);
        ChatId ToChatId = new(to);

        text = text[..Math.Min(text.Length, Utils.Constants.Telegram.MessageLengthLimit)];

        try
        {
            return await LocalTelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                parseMode: parseMode ?? (text.ContainsHtml() ? ParseMode.Html : null),
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException e) when (e.Message?.StartsWith("Bad Request: can't parse entities:", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            // Sample:
            //  "Telegram.Bot.Exceptions.ApiRequestException: Bad Request: can't parse entities: Unexpected end tag at byte offset 4120"
            //  +  Descargar Modelo 790
            //  =  Ver más
            //  =  </form>
            // Retry without HTML
            return await LocalTelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                cancellationToken: cancellationToken);
        }
    }
#else
        TelegramSettings.CurrentBot.SetMe(await LocalTelegramBotClient.GetMeAsync(stoppingToken));

        if (myCommands != null)
            //TelegramBotClient.DeleteMyCommandsAsync
            await LocalTelegramBotClient.SetMyCommandsAsync(commands: myCommands, cancellationToken: stoppingToken);

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions OptionsReceiver = new()
        {
            AllowedUpdates = default,
            Limit = default,
            Offset = default,
            ThrowPendingUpdates = default,
        };
        //Enums.UpdateType[] UpdatesAllowed = { Enums.UpdateType.Message | Enums.UpdateType.Unknown };
        //receiverOptions.AllowedUpdates = UpdatesAllowed;

        LocalTelegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, OptionsReceiver, stoppingToken);
    }

    private async Task HandleErrorAsync(
        ITelegramBotClient botClient
        , Exception exception
        , CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ApiRequestException apiRequestException:
                Logger.LogError("Telegram API Error:\n[{ErrorCode}]\n{Message}", apiRequestException.ErrorCode, apiRequestException.Message);
                break;

            case RequestException requestException:
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
        , Update update
        , CancellationToken cancellationToken)
    {
        try
        {
            Task handler = update.Type switch
            {
                UpdateType.CallbackQuery => BotOnCallbackQueryReceivedAsync(botClient, update.CallbackQuery!, cancellationToken),

                UpdateType.ChannelPost => BotOnMessageReceivedAsync(update.ChannelPost!, cancellationToken),

                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceivedAsync(botClient, update.ChosenInlineResult!, cancellationToken),

                UpdateType.EditedChannelPost => BotOnMessageReceivedAsync(update.EditedChannelPost!, cancellationToken),

                UpdateType.EditedMessage => BotOnMessageReceivedAsync(update.EditedMessage!, cancellationToken),

                UpdateType.InlineQuery => BotOnInlineQueryReceivedAsync(botClient, update.InlineQuery!, cancellationToken),

                UpdateType.Message => BotOnMessageReceivedAsync(update.Message!, cancellationToken),

                _ => throw new NotImplementedException(update.Type.ToString()),
            };

            await handler;
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected Error")) { }
    }

    private async Task<Message> MessageSendSimpleTextAsync(
        long to
        , string text
        , CancellationToken cancellationToken) =>
        await MessageSendTextAsync(to, text, null, cancellationToken);

    private async Task<Message> MessageSendQueryAsync(
        long to
        , string text
        , IReplyMarkup replyMarkup
        , CancellationToken cancellationToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            to = long.Parse(TelegramSettings.Users.UserTest.Id);

        text = text[..Math.Min(text.Length, Utils.Constants.Telegram.MessageLengthLimit)];
        ChatId ToChatId = new(to);

        try
        {
            return await LocalTelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException e) when (e.Message?.StartsWith("Bad Request: can't parse entities:", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            // Sample:
            //  "Telegram.Bot.Exceptions.ApiRequestException: Bad Request: can't parse entities: Unexpected end tag at byte offset 4120"
            //  +  Descargar Modelo 790
            //  =  Ver más
            //  =  </form>
            // Retry without HTML
            return await LocalTelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                cancellationToken: cancellationToken);
        }
    }

    private async Task<Message> MessageSendTextAsync(
        long to
        , string text
        , ParseMode? parseMode
        , CancellationToken cancellationToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            to = long.Parse(TelegramSettings.Users.UserTest.Id);

        text = text[..Math.Min(text.Length, Utils.Constants.Telegram.MessageLengthLimit)];
        ChatId ToChatId = new(to);

        try
        {
            return await LocalTelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                parseMode: parseMode ?? (text.ContainsHtml() ? ParseMode.Html : null),
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException e) when (e.Message?.StartsWith("Bad Request: can't parse entities:", StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            // Sample:
            //  "Telegram.Bot.Exceptions.ApiRequestException: Bad Request: can't parse entities: Unexpected end tag at byte offset 4120"
            //  +  Descargar Modelo 790
            //  =  Ver más
            //  =  </form>
            // Retry without HTML
            return await LocalTelegramBotClient.SendTextMessageAsync(
                chatId: ToChatId,
                text: text,
                cancellationToken: cancellationToken);
        }
    }

    private static async Task<Core.Entities.Subscriber> SubscriberWithSubscriptionsGetOrCreateAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , User user
        , CancellationToken cancellationToken)
    {
        Core.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(user.Id, cancellationToken);
        if (SubscriberWithSubscriptions == null)
        {
            SubscriberWithSubscriptions = new Core.Entities.Subscriber(user.Username ?? user.FirstName) { TelegramUserId = user.Id };
            _ = await dbCtx.Subscribers.AddAsync(SubscriberWithSubscriptions, cancellationToken);
        }

        return SubscriberWithSubscriptions;
    }

    private static async Task<string[]?> SubscriberGetWebUrlsAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , long telegramUserId
        , CancellationToken cancellationToken)
    {
        Core.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(telegramUserId, cancellationToken);
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
        , CallbackQuery callbackQuery
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Received CallbackQuery: {Data}", callbackQuery.Data);

        var CallbackQueryDatas = CallbackData.Parse(callbackQuery.Data);
        string? ResponseText = (CallbackQueryDatas?.BotActionName) switch
        {
            Enums.BotActionName.email_edit => await ParseResponseTextAsync(callbackQuery, cancellationToken),

            _ => null,
        };
        _ = await botClient.EditMessageReplyMarkupAsync(
            chatId: callbackQuery.Message!.Chat.Id
            , messageId: callbackQuery.Message!.MessageId
            , replyMarkup: InlineKeyboardMarkup.Empty()
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

        async Task<string?> ParseResponseTextAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await SubscriberSetEmailAsync(ServiceProvider.GetRequiredService<Infrastructure.DbContexts.DbCxt>(), callbackQuery.Message!, null, cancellationToken);

            return "Se ha borrado su correo electrónico";
        }
    }

    private static async Task SubscriberSetEmailAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , System.Net.Mail.MailAddress? mailAddress
        , CancellationToken cancellationToken)
    {
        Core.Entities.Subscriber TheSubscriber = await SubscriberWithSubscriptionsGetOrCreateAsync(dbCtx, message.From!, cancellationToken);

        TheSubscriber.MailAddress = mailAddress?.Address;

        _ = await dbCtx.SaveChangesAsync(cancellationToken);
    }

    private async Task BotOnChosenInlineResultReceivedAsync(
        ITelegramBotClient botClient
        , ChosenInlineResult chosenInlineResult
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Received ChosenInlineResult: {ResultId}", chosenInlineResult.ResultId);

        _ = await botClient.SendTextMessageAsync(
            chatId: chosenInlineResult.From.Id,
            text: $"Recibido {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    private async Task BotOnInlineQueryReceivedAsync(
        ITelegramBotClient botClient
        , InlineQuery inlineQuery
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Received inline query from: {Id}", inlineQuery.From.Id);

        // displayed result
        InlineQueryResult[] results = [
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent("hello"))
            ];

        await botClient.AnswerInlineQueryAsync(
            inlineQueryId: inlineQuery.Id,
            results: results,
            isPersonal: true,
            cacheTime: 0,
            cancellationToken: cancellationToken);
    }

    private Task BotOnMessageAutoDeleteTimerChangedReceived(
        Message message)
    {
        Logger.LogInformation("Establecido tiempo de autoborrado en {MessageAutoDeleteTime} segundos", message.MessageAutoDeleteTimerChanged!.MessageAutoDeleteTime);

        return Task.CompletedTask;
    }

    private async Task BotOnMessageDocumentReceivedAsync(
        Message message
        , CancellationToken cancellationToken)
    {
        await ChatActionSendAsync(message, ChatAction.Typing, cancellationToken);

        //await ProcesarDocumentoConsumosAsync(message, cancellationToken); // TODO Manejar todos los tipos de documentos

        Task<Message>? handler = message.Caption switch
        {
            //BotCommandNames.SubirConsumos => ProcesarDocumentoConsumos(),
            _ => MessageSendTextAsync(message.Chat.Id, "No sé qué hacer con eso...", null, cancellationToken)
        };

        Message sentMessage = await handler;

        Logger.LogDebug("The message was sent with Id: {MessageId}", sentMessage.MessageId);
    }

    private async Task BotOnMessageReceivedAsync(
        Message message
        , CancellationToken cancellationToken)
    {
        Logger.LogDebug("Receive message type: {Type}", message.Type);

        Task handler = message.Type switch
        {
            MessageType.Document => BotOnMessageDocumentReceivedAsync(message, cancellationToken),

            MessageType.MessageAutoDeleteTimerChanged => BotOnMessageAutoDeleteTimerChangedReceived(message),

            MessageType.Text => BotOnMessageTextReceivedAsync(message, cancellationToken),

            _ => throw new NotImplementedException(message.Type.ToString()),
        };

        try
        {
            await handler;
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected Error")) { }
    }

    private async Task BotOnMessageTextReceivedAsync(
        Message message
        , CancellationToken cancellationToken)
    {
        await ChatActionSendAsync(message, ChatAction.Typing, cancellationToken);

        string FirstWordReceived = message.Text!.Split(' ').First();

        if (FirstWordReceived.StartsWith('/') &&
            Enum.TryParse(FirstWordReceived[1..].ToLowerInvariant(), out Enums.BotActionName ReceivedCommand))
        {
            await ManageCommandReceivedAsync(ServiceProvider.GetRequiredService<Infrastructure.DbContexts.DbCxt>(), message, ReceivedCommand, cancellationToken);
        }
        else if (FirstWordReceived.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
        {
            await ManageNewSubscriptionAsync(ServiceProvider.GetRequiredService<Infrastructure.DbContexts.DbCxt>(), message, FirstWordReceived, cancellationToken);
        }
        else
        {
            await ManageNoCommandReceivedAsync(message, cancellationToken);
        }
    }

    private async Task<Message> PvpcGetAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , CancellationToken cancellationToken)
    {
        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || !DateTime.TryParseExact(
            data[1],
            Libs.Utils.Constants.Formats.YearMonthDayFormat,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out DateTime dateTimeToObtain))
        {
            // Si pedimos después de las 20:30, nos devolverá los datos del día siguiente.
            dateTimeToObtain = DateTime.Now.TimeOfDay > new TimeSpan(20, 30, 00) ? DateTime.Today.AddDays(1) : DateTime.Today;
        }

        await ServiceProvider.GetRequiredService<Pvpc.Lib.Services.PvpcCronBackgroundService>()
            .GetPvpcFromReeForDateAsync(dateTimeToObtain, cancellationToken);

        Core.Entities.Pvpc[]? Prices = await dbCtx.Pvpcs.AsNoTracking()
            .Where(x => x.AtDateTimeOffset >= dateTimeToObtain)
            .Where(x => x.AtDateTimeOffset < dateTimeToObtain.AddDays(1))
            .ToArrayAsync(cancellationToken: cancellationToken);

        string responseText = MessageGetMarkdownV2TextForPrices(Prices);

        return await MessageSendTextAsync(message.Chat.Id, responseText, ParseMode.MarkdownV2, cancellationToken);
    }

    private async Task<Message> MailSetAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , CancellationToken cancellationToken)
    {
        string ResponseText;

        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || string.IsNullOrWhiteSpace(data[1]))
        {
            CallbackData RemoveEmailCallbackData = new(Enums.BotActionName.email_edit);
            InlineKeyboardMarkup ReplyKeyboard = new(
                InlineKeyboardButton.WithCallbackData("Sí", RemoveEmailCallbackData.ToString()));
            ResponseText = "¿Realmente quiere quitar su correo electrónico asociado?";

            return await MessageSendQueryAsync(message.Chat.Id, ResponseText, ReplyKeyboard, cancellationToken);
        }
        else
        {
            // TODO MailSetAsync
            long TelegramUserId = message.From!.Id;
            Core.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(TelegramUserId, cancellationToken);
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

            ResponseText = $"Usted se ha dado de baja correctamente";
            //}
        }

        return await MessageSendTextAsync(message.Chat.Id, ResponseText, null, cancellationToken);
    }

    private async Task<Message> MailShowAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , CancellationToken cancellationToken)
    {
        long TelegramUserId = message.From!.Id;

        Core.Entities.Subscriber? Subscriber = await dbCtx.Subscribers.FirstOrDefaultAsync(x => x.TelegramUserId == TelegramUserId, cancellationToken);

        if (Subscriber == null)
            return await MessageSendUsageAsync(message.Chat.Id, cancellationToken);

        string TextToSend = string.IsNullOrEmpty(Subscriber.MailAddress) ? "No tiene usted correo electrónico asociado" : Subscriber.MailAddress;

        return await MessageSendTextAsync(TelegramUserId, TextToSend, null, cancellationToken);
    }

    private async Task ManageCommandReceivedAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , Enums.BotActionName receivedCommand
        , CancellationToken cancellationToken)
    {
        Task<Message> NextAction = receivedCommand switch
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

        Message sentMessage = await NextAction;

        Logger.LogDebug("The message was sent with Id: {MessageId}", sentMessage.MessageId);
    }

    private Task<Message> AmazFindAsync(
        Infrastructure.DbContexts.DbCxt dbCtx,
        Message message,
        CancellationToken cancellationToken)
    {
#pragma warning disable IDE0022 // Use expression body for method
        throw new NotImplementedException();
#pragma warning restore IDE0022 // Use expression body for method
    }

    private async Task ManageNewSubscriptionAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , string FirstWordReceived
        , CancellationToken cancellationToken)
    {
        long SenderChatId = message.Chat.Id;

        Core.Entities.WebData? webData = await dbCtx.WebDatas.FirstOrDefaultAsync(x => x.WebUrl == FirstWordReceived, cancellationToken);
        if (webData == null)
        {
            webData = new Core.Entities.WebData(FirstWordReceived, $"Recibido a través de Telegram el {message.Date}");
            _ = await dbCtx.WebDatas.AddAsync(webData, cancellationToken);

            _ = await MessageSendSimpleTextAsync(SenderChatId, "Añadida URL para seguimiento", cancellationToken);
        }

        _ = await dbCtx.SaveChangesAsync(cancellationToken);

        Core.Entities.Subscriber TheSubscriber = await SubscriberWithSubscriptionsGetOrCreateAsync(dbCtx, message.From!, cancellationToken);
        if (dbCtx.ChangeTracker.Entries().FirstOrDefault(x => x.Entity == TheSubscriber)?.State == EntityState.Added)
            _ = await MessageSendSimpleTextAsync(SenderChatId, "Gracias por usar este bot", cancellationToken);

        Core.Entities.Subscription? TheSubscription =
            await dbCtx.Subscriptions
            .FirstOrDefaultAsync(x => x.SubscriptionId == webData.SubscriptionId && x.SubscriptionName == Core.Enums.SubscriptionName.webComparer, cancellationToken);
        if (TheSubscription == null)
        {
            TheSubscription = new Core.Entities.Subscription(Core.Enums.SubscriptionName.webComparer) { SubscriptionId = webData.SubscriptionId };
            _ = await dbCtx.Subscriptions.AddAsync(TheSubscription, cancellationToken);
        }

        if (!TheSubscriber!.Subscriptions.Any(x => x.SubscriptionId == webData.SubscriptionId))
        {
            TheSubscriber!.Subscriptions.Add(TheSubscription);

            _ = await MessageSendSimpleTextAsync(SenderChatId, "Recibirá actualizaciones cuando se produzcan", cancellationToken);
        }
        else
        {
            _ = await MessageSendSimpleTextAsync(SenderChatId, "Usted ya está recibiendo actualizaciones", cancellationToken);
        }

        _ = await dbCtx.SaveChangesAsync(cancellationToken);
    }

    private async Task ManageNoCommandReceivedAsync(
        Message message
        , CancellationToken cancellationToken)
    {
        Logger.LogInformation("{Text}", message.Text);

        await Task.Delay(Utils.Constants.Time.OneTenthOfSecondTimeSpan, cancellationToken);
    }

    private async Task ChatActionSendAsync(
        Message message
        , ChatAction chatAction
        , CancellationToken cancellationToken) =>
        await LocalTelegramBotClient.SendChatActionAsync(
            chatId: message.Chat.Id,
            chatAction: chatAction,
            cancellationToken: cancellationToken);

    private async Task<Message> StartAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , CancellationToken cancellationToken)
    {
        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || !System.Net.Mail.MailAddress.TryCreate(data[1], out System.Net.Mail.MailAddress? mailAddress))
            mailAddress = null;

        await SubscriberSetEmailAsync(dbCtx, message, mailAddress, cancellationToken);

        string responseText = $"Bienvenid@ {message.From!.Username ?? message.From!.FirstName}";

        return await MessageSendTextAsync(message.Chat.Id, responseText, null, cancellationToken);
    }

    private async Task<Message> SubscriptionsListAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , CancellationToken cancellationToken)
    {
        long TelegramUserId = message.From!.Id;

        string[]? Subscriptions = await SubscriberGetWebUrlsAsync(dbCtx, TelegramUserId, cancellationToken);
        if (Subscriptions == null)
            return await MessageSendUsageAsync(TelegramUserId, cancellationToken);

        string TextToSend = Subscriptions.Length != 0
            ? "Estás suscrit@ a:\n" + string.Join("\n", Subscriptions.Select(x => $"{x}"))
            : "Usted no tiene suscripciones";

        return await MessageSendTextAsync(TelegramUserId, TextToSend, null, cancellationToken);
    }

    private async Task<Message> MessageSendUsageAsync(
        long to
        , CancellationToken cancellationToken)
    {
        BotCommand[] MyCommands = await LocalTelegramBotClient.GetMyCommandsAsync(cancellationToken: cancellationToken);

        // TODO                 Cambiar el mensaje de ayuda, incluyendo que pueden enviar direcciones http[s].
        return await MessageSendTextAsync(
            to,
            "Comandos disponibles:\n" + string.Join("\n", MyCommands.Select(x => $"/{x.Command} {x.Description}")),
            null,
            cancellationToken);
    }

    private async Task<Message> UnsubscribeAsync(
        Infrastructure.DbContexts.DbCxt dbCtx
        , Message message
        , CancellationToken cancellationToken)
    {
        string ResponseText;

        string[]? data = message.Text!.Split(' ');
        if (data.Length < 2 || !int.TryParse(data[1], out int subscriptionId))
        {
            ResponseText = $"No sé de qué suscripción quiere darse de baja {Constants.Emojis.PersonShrugging}";
        }
        else
        {
            long TelegramUserId = message.From!.Id;
            Core.Entities.Subscriber? SubscriberWithSubscriptions = await dbCtx.GetSubscriberWithSubscriptionsAsync(TelegramUserId, cancellationToken);
            if (SubscriberWithSubscriptions == null)
                return await MessageSendUsageAsync(TelegramUserId, cancellationToken);

            Core.Entities.Subscription? Subscription = SubscriberWithSubscriptions.Subscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId);
            if (Subscription == null)
            {
                ResponseText = $"{Constants.Emojis.FaceWithRaisedEyebrow} No estás suscrit@ a {subscriptionId}";
            }
            else
            {
                _ = SubscriberWithSubscriptions.Subscriptions.Remove(Subscription);

                _ = await dbCtx.SaveChangesAsync(cancellationToken);

                ResponseText = $"Usted se ha dado de baja correctamente";
            }
        }

        return await MessageSendTextAsync(message.Chat.Id, ResponseText, null, cancellationToken);
    }

    //private async Task ProcesarDocumentoConsumosAsync(
    //  Message message
    //  , CancellationToken cancellationToken)
    //{
    //    using MemoryStream memoryStream = new();

    //    File? file = await _TelegramBotClient.GetInfoAndDownloadFileAsync(message.Document!.FileId, memoryStream, cancellationToken);

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
#endif

    public async Task SendMessageToSubscriberAsync(
        Core.Entities.Outbox pendingMessage
        , long telegramUserId
        , CancellationToken stoppingToken)
    {
        _ = pendingMessage.SubscriptionName switch
        {
            Core.Enums.SubscriptionName.electricidad => await MessageSendTextAsync(
                telegramUserId,
                MessageGetMarkdownV2TextForPrices(System.Text.Json.JsonSerializer.Deserialize<Core.Entities.Pvpc[]>(pendingMessage.Payload)!),
                ParseMode.MarkdownV2,
                stoppingToken),

            Core.Enums.SubscriptionName.webComparer => await MessageSendTextAsync(
                telegramUserId,
                pendingMessage.Payload[new Range(0, Math.Min(Utils.Constants.Telegram.MessageLengthLimit, pendingMessage.Payload.Length))],
                null,
                stoppingToken),

            //Enums.SubscriptionName.amazon => await TelegramService.MessageSendTextAsync(
            //    subscriber.TelegramUserId.Value,
            //    null,
            //    stoppingToken),

            _ => throw new ApplicationException($"Unexpected SubscriptionName: '{pendingMessage.SubscriptionName}?"),
        };
    }

    private static string MessageGetMarkdownV2TextForPrices(Core.Entities.Pvpc[] prices)
    {
        if (prices == null || prices.Length == 0)
            return "No hay datos";

        DateTime ObtainedDate = prices.First().AtDateTimeOffset.Date;

        int HowMany = Math.Min(6, prices.Length - 1);
        decimal Min = prices.OrderBy(x => x.KWhPriceInEuros).Take(HowMany).Max(x => x.KWhPriceInEuros);
        decimal Max = prices.OrderByDescending(x => x.KWhPriceInEuros).Take(HowMany).Min(x => x.KWhPriceInEuros);
        decimal Avg = prices.Average(x => x.KWhPriceInEuros);

        System.Text.StringBuilder sb = new(500);
        sb = sb.AppendLine($"Media del *{ObtainedDate.ToString(Utils.Constants.Formats.LongDateFormat, Utils.Constants.Formats.ESCultureInfo)}*");
        sb = sb.AppendLine($"*{Avg.ToString("N5", Utils.Constants.Formats.ESCultureInfo)} € / kWh*");

        for (int i = 0; i < prices.Length; i++)
        {
            string emoji =
                prices[i].KWhPriceInEuros >= Max ? Constants.Emojis.RedCircle :
                prices[i].KWhPriceInEuros <= Min ? Constants.Emojis.GreenCircle :
                prices[i].KWhPriceInEuros <= Avg ? Constants.Emojis.YellowCircle : Constants.Emojis.OrangeCircle;

            sb = sb.AppendLine($"{prices[i].AtDateTimeOffset.Hour:00}  {emoji}  {prices[i].KWhPriceInEuros.ToString("N5", Utils.Constants.Formats.ESCultureInfo)}");
        }

        return sb.ToString();
    }

    //private static string MessageGetMarkdownV2TextForPrices(string payload)
    //   => MessageGetMarkdownV2TextForPrices(System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CoreLib.Entities.Pvpc>>(payload)!);
}

using Microsoft.Extensions.DependencyInjection;
using Seedysoft.Libs.Infrastructure.Extensions;

namespace Seedysoft.Libs.TelegramBot.Tests.Services;

public sealed class TelegramHostedServiceTests : Infrastructure.Tests.TestClassBase
{
    private readonly TelegramBot.Services.TelegramHostedService telegramHostedService = default!;

    public TelegramHostedServiceTests() : base()
    {
        Microsoft.Extensions.Hosting.HostApplicationBuilder appBuilder = new();
        _ = appBuilder.AddAllMyDependencies();
        ServiceProvider serviceProvider = appBuilder.Services.BuildServiceProvider();

        telegramHostedService = serviceProvider.GetRequiredService<TelegramBot.Services.TelegramHostedService>();
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithValidPlainTextSendsMessageSuccessfullyTest()
    {
        // Arrange
        const string text = "Hello, World!";
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: text,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
        _ = await Assert.That(result.Text).IsEqualTo(text);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithHtmlContentAutomaticallyDetectsHtmlParseModeTest()
    {
        // Arrange
        const string text = "Bold Text";
        const string htmlText = $"<b>{text}</b>";
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: htmlText,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
        _ = await Assert.That(result.Text).IsEqualTo(text);
        _ = await Assert.That(result.Entities).Any(static x => x.Type == Telegram.Bot.Types.Enums.MessageEntityType.Bold);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithExplicitParseModeUsesProvidedModeTest()
    {
        // Arrange
        const string text = "Some text";
        Telegram.Bot.Types.Enums.ParseMode parseMode = Telegram.Bot.Types.Enums.ParseMode.Markdown;
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: text,
            parseMode: parseMode,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithTextExceedingLimitTruncatesTextTest()
    {
        // Arrange
        string longText = new('A', Core.Constants.Telegram.MessageLengthLimit + 100);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: longText,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
        _ = await Assert.That(result.Text!.Length).IsLessThanOrEqualTo(Core.Constants.Telegram.MessageLengthLimit);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithEmptyTextThrowsApiRequestExceptionTest()
    {
        // Arrange
        const string text = "";
        CancellationToken cancellationToken = CancellationToken.None;

        // Act & Assert (Bad Request: message text is empty)
        _ = await Assert.ThrowsAsync<Telegram.Bot.Exceptions.ApiRequestException>(
            () => telegramHostedService.MessageSendTextAsync(
                to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
                text: text,
                parseMode: null,
                cancellationToken: cancellationToken));
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWhenCancellationRequestedThrowsOperationCanceledExceptionTest()
    {
        // Arrange
        const string text = "Hello";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        _ = await Assert.ThrowsAsync<OperationCanceledException>(
            () => telegramHostedService.MessageSendTextAsync(
                to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
                text: text,
                parseMode: null,
                cancellationToken: cts.Token));
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithVariousHtmlTagsHandlesProperlyTest()
    {
        // Arrange
        const string text = "Italic Underline Strikethrough";
#pragma warning disable IDE0043 // Invalid format string
        string htmlText = string.Format("<i>{0}</i> <u>{1}</u> <s>{2}</s>", text.Split(" "));
#pragma warning restore IDE0043 // Invalid format string
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: htmlText,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
        _ = await Assert.That(result.Text).IsEqualTo(text);
        _ = await Assert.That(result.Entities).Any(static x => x.Type == Telegram.Bot.Types.Enums.MessageEntityType.Italic);
        _ = await Assert.That(result.Entities).Any(static x => x.Type == Telegram.Bot.Types.Enums.MessageEntityType.Underline);
        _ = await Assert.That(result.Entities).Any(static x => x.Type == Telegram.Bot.Types.Enums.MessageEntityType.Strikethrough);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithPlainTextAndExplicitParseModeUsesExplicitModeTest()
    {
        // Arrange
        const string plainText = "Plain text without HTML";
        Telegram.Bot.Types.Enums.ParseMode parseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: plainText,
            parseMode: parseMode,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithLongTextAtExactLimitSendsSuccessfullyTest()
    {
        // Arrange
        string exactLimitText = new('A', Core.Constants.Telegram.MessageLengthLimit);
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: exactLimitText,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
        _ = await Assert.That(result.Text!.Length).IsEqualTo(Core.Constants.Telegram.MessageLengthLimit);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithSpecialCharactersSendsSuccessfullyTest()
    {
        // Arrange
        const string textWithSpecialChars = "Hello! @user #hashtag 😀 \n\r\t";
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: textWithSpecialChars,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithUnicodeCharactersSendsSuccessfullyTest()
    {
        // Arrange
        const string unicodeText = "Привет мир 你好世界 مرحبا العالم";
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: unicodeText,
            parseMode: null,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithParseModeNoneSendsSucessfullyTest()
    {
        // Arrange
        const string text = "Plain text";
        Telegram.Bot.Types.Enums.ParseMode parseMode = Telegram.Bot.Types.Enums.ParseMode.None;
        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
            to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            text: text,
            parseMode: parseMode,
            cancellationToken: cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithMultipleCallsSendsAllSuccessfullyTest()
    {
        // Arrange
        string[] texts = ["First", "Second", "Third"];
        CancellationToken cancellationToken = CancellationToken.None;

        // Act & Assert
        foreach (string? text in texts)
        {
            Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
                to: telegramHostedService.Settings.KnownUserForTest.IdAsLong,
                text: text,
                parseMode: null,
                cancellationToken: cancellationToken);

            Assert.NotNull(result);
            _ = await Assert.That(result.Text).IsEqualTo(text);
        }
    }

    [Test]
    [Explicit]
    public async Task MessageSendTextAsyncWithDifferentChatIdsSendsToCorrectChatTest()
    {
        // Arrange
        long[] chatIds = [
            telegramHostedService.Settings.KnownUserForTest.IdAsLong,
            //Constants.TelegramIds.BotFather,
            // TODO Find more user ids to test
        ];
        const string text = "Test message";
        CancellationToken cancellationToken = CancellationToken.None;

        // Act & Assert
        foreach (long chatId in chatIds)
        {
            Telegram.Bot.Types.Message result = await telegramHostedService.MessageSendTextAsync(
                to: chatId,
                text: text,
                parseMode: null,
                cancellationToken: cancellationToken);

            Assert.NotNull(result);
            _ = await Assert.That(result.Chat.Id).IsEqualTo(chatId);
        }
    }
}

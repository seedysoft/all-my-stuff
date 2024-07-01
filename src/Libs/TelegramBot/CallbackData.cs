using Seedysoft.Libs.TelegramBot.Enums;

namespace Seedysoft.Libs.TelegramBot;

internal sealed class CallbackData(BotActionName botActionName)
{
    private const char Separator = '~';

    public BotActionName BotActionName { get; init; } = botActionName;

    public override string ToString() => string.Join(Separator, (int)BotActionName);

    public static CallbackData? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        string[] Data = text.Split(Separator);

        return new((BotActionName)int.Parse(Data[0]));
    }
}

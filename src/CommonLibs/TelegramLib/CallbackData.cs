namespace Seedysoft.TelegramLib;

internal sealed class CallbackData(Enums.BotActionName botActionName)
{
    private const string Separator = "~";

    public Enums.BotActionName BotActionName { get; init; } = botActionName;

    public override string ToString() => string.Join(Separator, (int)BotActionName);

    public static CallbackData? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        string[] Data = text.Split(Separator);

        return new((Enums.BotActionName)int.Parse(Data[0]));
    }
}

namespace Seedysoft.TelegramLib;

internal class CallbackData
{
    private const string Separator = "~";

    public CallbackData(Enums.BotActionName botActionName) => BotActionName = botActionName;

    public Enums.BotActionName BotActionName { get; init; }

    public override string ToString() => string.Join(Separator, (int)BotActionName);

    public static CallbackData? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        string[] Data = text.Split(Separator);

        return new((Enums.BotActionName)int.Parse(Data[0]));
    }
}

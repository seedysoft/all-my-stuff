namespace Seedysoft.TelegramLib;

public record Bots
{
    public long Id { get; init; }
    public string Username { get; init; } = default!;

    public Telegram.Bot.Types.User? Me { get; private set; }
    public static Bots Current => System.Diagnostics.Debugger.IsAttached ? Test : Prod;

    private static readonly Bots Test = new(5025594021L, "Raspberrypi4FokyTestbot");
    private static readonly Bots Prod = new(2099959399L, "Raspberrypi4Foky_bot");

    private Bots(long id, string username)
    {
        Id = id;
        Username = username ?? throw new ArgumentNullException(nameof(username));
    }

    internal void SetMe(Telegram.Bot.Types.User user) => Me = user;
}

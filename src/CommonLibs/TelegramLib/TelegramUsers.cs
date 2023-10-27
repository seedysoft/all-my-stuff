namespace Seedysoft.TelegramLib;

public record TelegramUserBase
{
    private static readonly TelegramUserBase TestBotTelegramUser = new(5025594021L, "Raspberrypi4FokyTestbot");
    private static readonly TelegramUserBase ProdBotTelegramUser = new(2099959399L, "Raspberrypi4Foky_bot");

    public long Id { get; init; }
    public string Username { get; init; }

    //12849423	Yo
    public static TelegramUserBase TestTelegramUser { get; set; } = new(12849423L, "Yo probando");

    public Telegram.Bot.Types.User? SenderUser { get; protected set; }

    protected internal TelegramUserBase(long id, string username)
    {
        Id = id;
        Username = username ?? throw new ArgumentNullException(nameof(username));
    }

    public static TelegramUserBase Current => System.Diagnostics.Debugger.IsAttached ? TestBotTelegramUser : ProdBotTelegramUser;

    public void SetMe(Telegram.Bot.Types.User user) => SenderUser = user;
}

public record BotTelegramUser : TelegramUserBase
{
    protected internal BotTelegramUser(long id, string username) : base(id, username) { }
}

public record KnowTelegramUser : TelegramUserBase
{
    protected internal KnowTelegramUser(long id, string username) : base(id, username) { }
}
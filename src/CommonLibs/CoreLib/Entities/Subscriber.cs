namespace Seedysoft.CoreLib.Entities;

//286475043	Rake

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Subscriber
{
    private Subscriber() { }
    public Subscriber(string firstname) => Firstname = firstname;

    public long SubscriberId { get; set; }

    public string Firstname { get; set; } = default!;

    public long? TelegramUserId { get; set; }

    public string? MailAddress { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    // Coalesce expression raises "Error CS0019  Operator '??' cannot be applied to operands of type 'long?' and 'string'".
    private string GetDebuggerDisplay() => $"{Firstname} - {(TelegramUserId.HasValue ? TelegramUserId : "Sin Telegram")} - {MailAddress ?? "Sin correo electrónico"}";
}

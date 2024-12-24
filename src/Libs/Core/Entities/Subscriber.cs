namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
public sealed class Subscriber
{
    public long SubscriberId { get; set; }

    public required string Firstname { get; init; }

    public long? TelegramUserId { get; set; }

    public string? MailAddress { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = [];

    // Coalesce expression raises "Error CS0019  Operator '??' cannot be applied to operands of type 'long?' and 'string'".
    private string GetDebuggerDisplay
        => $"{Firstname} - {(TelegramUserId.HasValue ? TelegramUserId : "Sin Telegram")} - {MailAddress ?? "Sin correo electrónico"}";
}

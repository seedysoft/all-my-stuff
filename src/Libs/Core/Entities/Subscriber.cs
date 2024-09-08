﻿namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Subscriber
{
    private Subscriber() { }
    public Subscriber(string firstname) => Firstname = firstname;

    public long SubscriberId { get; set; }

    public string Firstname { get; set; } = default!;

    public long? TelegramUserId { get; set; }

    public string? MailAddress { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = [];

    // Coalesce expression raises "Error CS0019  Operator '??' cannot be applied to operands of type 'long?' and 'string'".
    private string GetDebuggerDisplay() => $"{Firstname} - {(TelegramUserId.HasValue ? TelegramUserId : "Sin Telegram")} - {MailAddress ?? "Sin correo electrónico"}";
}

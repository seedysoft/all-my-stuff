namespace Seedysoft.SmtpServiceLib.Settings;

public record SmtpServiceSettings
{
    public string Host { get; init; } = default!;
    public int Port { get; init; }
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}

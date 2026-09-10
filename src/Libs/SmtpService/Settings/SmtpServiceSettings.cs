namespace Seedysoft.Libs.SmtpService.Settings;

public record SmtpServiceSettings
{
    public required string Host { get; init; }

    public int Port { get; init; }

    public required string Username { get; init; }

    public string Password
    {
        get;
        init => field = Core.Helpers.EnvironmentHelper.Decrypt(value);
    } = default!;
}

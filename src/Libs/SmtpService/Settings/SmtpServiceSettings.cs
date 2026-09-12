namespace Seedysoft.Libs.SmtpService.Settings;

public record SmtpServiceSettings
{
    public required string Host
    {
        get;
        init => field = Core.Helpers.EnvironmentHelper.Decrypt(value);
    } = default!;

    public required int Port { get; init; } = default!;

    public required string Username { get; init; } = default!;

    public required string Password
    {
        get;
        init => field = Core.Helpers.EnvironmentHelper.Decrypt(value);
    } = default!;
}

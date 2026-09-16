namespace Seedysoft.Libs.SmtpService.Settings;

public record SmtpServiceSettings
{
    public required string Host
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }

    public required int Port { get; init; } = default!;

    public required string Username { get; init; } = default!;

    public required string Password
    {
        get;
        init => field = Cryptography.Crypto.DecryptText(value, Core.Helpers.EnvironmentHelper.GetMasterKey());
    }
}

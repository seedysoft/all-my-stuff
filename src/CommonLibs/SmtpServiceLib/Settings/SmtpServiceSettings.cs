using Seedysoft.UtilsLib.Helpers;

namespace Seedysoft.SmtpServiceLib.Settings;

public record SmtpServiceSettings
{
    public required string Host { get; init; }

    public int Port { get; init; }

    public required string Username { get; init; }

    private string password = default!;
    public string Password
    {
        get => password;
        init => password = CryptoLib.Crypto.DecryptText(value, EnvironmentHelper.GetMasterKey());
    }
}

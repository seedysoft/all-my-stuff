namespace Seedysoft.Libs.Update.Settings;

public record UpdateSettings
{
    private string githubPat = default!;
    public string GithubPat
    {
        get => githubPat;
        init => githubPat = Cryptography.Crypto.DecryptText(value, Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
}

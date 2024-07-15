namespace Seedysoft.Update.Lib.Settings;

public record UpdateSettings : Libs.BackgroundServices.ScheduleConfig
{
    private string githubPat = default!;

    public string GithubPat
    {
        get => githubPat;
        init => githubPat = Libs.Cryptography.Crypto.DecryptText(value, Libs.Utils.Helpers.EnvironmentHelper.GetMasterKey());
    }
}

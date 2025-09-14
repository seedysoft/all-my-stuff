namespace Seedysoft.Libs.Update.Enums;

public enum UpdateResults : int
{
    Ok = 0,

    LatestReleaseFromGithubIsNull,

    AssetNotFound,

    NoNewVersionFound,

    ErrorExecutingUpdateScript,
}

namespace Seedysoft.Libs.Update.Enums;

public enum UpdateResults : int
{
    Ok = 0,

    NoNewVersionFound,

    LatestReleaseFromGithubIsNull,

    AssetNotFound,

    ErrorExecutingUpdateScript,
}

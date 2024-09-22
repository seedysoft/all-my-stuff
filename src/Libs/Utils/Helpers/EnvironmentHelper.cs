namespace Seedysoft.Libs.Utils.Helpers;

public static class EnvironmentHelper
{
    private static Lazy<string?> MasterKey = null!;
    public static string GetMasterKey()
    {
        const string MasterKeyEnvironmentVariableName = "SEEDY_MASTER_KEY";

        MasterKey ??= new(Environment.GetEnvironmentVariable(MasterKeyEnvironmentVariableName));

        return string.IsNullOrWhiteSpace(MasterKey.Value)
            ? throw new Exception($"Environment variable {MasterKeyEnvironmentVariableName} is not established.")
            : MasterKey.Value;
    }
}

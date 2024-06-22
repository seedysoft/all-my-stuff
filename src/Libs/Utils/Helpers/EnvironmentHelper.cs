namespace Seedysoft.Libs.Utils.Helpers;

public static class EnvironmentHelper
{
    public static string GetMasterKey()
    {
        const string MasterKeyEnvironmentVariableName = "SEEDY_MASTER_KEY";

        string? MasterKey = Environment.GetEnvironmentVariable(MasterKeyEnvironmentVariableName);

        return string.IsNullOrWhiteSpace(MasterKey)
            ? throw new Exception($"Environment variable {MasterKeyEnvironmentVariableName} is not established.")
            : MasterKey;
    }
}

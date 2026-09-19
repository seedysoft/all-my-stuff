namespace Seedysoft.Libs.Core.Helpers;

public static class EnvironmentHelper
{
    private static readonly SemaphoreSlim semaphoreSlim = new(initialCount: 1, maxCount: 1);

    private static string? MasterKey = null;
    public static string GetMasterKey()
    {
        const string MasterKeyEnvironmentVariableName = "SEEDY_MASTER_KEY";

        semaphoreSlim.Wait();

        if (MasterKey == null)
        {
            MasterKey = Environment.GetEnvironmentVariable(MasterKeyEnvironmentVariableName);
            System.Diagnostics.Trace.Assert(!string.IsNullOrWhiteSpace(MasterKey));
        }

        _ = semaphoreSlim.Release();

        return MasterKey;
    }
}

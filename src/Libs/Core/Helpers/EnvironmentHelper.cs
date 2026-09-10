namespace Seedysoft.Libs.Core.Helpers;

public static class EnvironmentHelper
{
    private static readonly SemaphoreSlim semaphoreSlim = new(initialCount: 1, maxCount: 1);

    private static byte[]? MasterKey = null;
    internal static ReadOnlySpan<byte> GetMasterKey()
    {
        const string MasterKeyEnvironmentVariableName = "SEEDY_MASTER_KEY";

        semaphoreSlim.Wait();

        if (MasterKey == null)
        {
            string? MasterKeyEnvironmentVariableValue = Environment.GetEnvironmentVariable(MasterKeyEnvironmentVariableName);
            System.Diagnostics.Trace.Assert(!string.IsNullOrWhiteSpace(MasterKeyEnvironmentVariableValue));

            MasterKey = Convert.FromBase64String(MasterKeyEnvironmentVariableValue);
            System.Diagnostics.Trace.Assert(MasterKey != null);
        }

        _ = semaphoreSlim.Release();

        return MasterKey.AsSpan();
    }

    public static string Decrypt(string text) => Cryptography.Crypto.Decrypt(GetMasterKey(), text);

    internal static string Encrypt(string text) => Cryptography.Crypto.Encrypt(GetMasterKey(), text);
}

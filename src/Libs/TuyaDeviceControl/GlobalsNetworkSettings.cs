namespace Seedysoft.Libs.TuyaDeviceControl;

internal class GlobalsNetworkSettings
{
    /// <summary>
    /// How many tries before stopping.
    /// </summary>
    public const int MAXCOUNT = 15;
    /// <summary>
    /// How many seconds to wait before stopping device discovery.
    /// </summary>
    public const int SCANTIME = 18;
    /// <summary>
    /// Tuya 3.1 UDP Port.
    /// </summary>
    public const int UDPPORT = 6666;
    /// <summary>
    /// Tuya 3.3 encrypted UDP Port.
    /// </summary>
    public const int UDPPORTS = 6667;
    /// <summary>
    /// Tuya app encrypted UDP Port.
    /// </summary>
    public const int UDPPORTAPP = 7000;
    /// <summary>
    /// Tuya TCP Local Port.
    /// </summary>
    public const int TCPPORT = 6668;
    /// <summary>
    /// Seconds to wait for a broadcast.
    /// </summary>
    public const decimal TIMEOUT = 3.0M;
    /// <summary>
    /// Seconds to wait for socket open for scanning.
    /// </summary>
    public const decimal TCPTIMEOUT = 0.4M;
    public const string DEFAULT_NETWORK = "'192.168.0.0/24";
}

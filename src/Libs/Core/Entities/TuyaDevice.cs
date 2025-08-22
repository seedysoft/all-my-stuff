namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
public sealed class TuyaDevice
{
    public required string Id { get; set; }  // bf73f8d9933c68f98az7hb

    public required string Address { get; set; }  // 192.168.1.72
    public System.Net.IPAddress IPAddress => System.Net.IPAddress.Parse(Address);

    public required float Version { get; set; } // 3.4f

    public required string LocalKey { get; set; }

    private string GetDebuggerDisplay => $"{IPAddress}";
}

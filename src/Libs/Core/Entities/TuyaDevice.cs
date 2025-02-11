﻿namespace Seedysoft.Libs.Core.Entities;

[System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay,nq}")]
public sealed class TuyaDevice
{
    public string Id { get; set; } = default!; // bf73f8d9933c68f98az7hb

    public string Address { get; set; } = default!; // 192.168.1.72
    public System.Net.IPAddress IPAddress => System.Net.IPAddress.Parse(Address);

    public float Version { get; set; } // 3.4f

    public string LocalKey { get; set; } = default!;

    private string GetDebuggerDisplay => $"{IPAddress}";
}

using Seedysoft.CoreLib.Entities;
using Seedysoft.TuyaDeviceControlLib;

namespace Seedysoft.PvpcLib.Extensions;

public static class PvpcLibExtensions
{
    internal static TuyaDeviceBase ToTuyaDeviceBase(this TuyaDevice self)
        => new(self.Id, self.Address, self.LocalKey, version: self.Version);
}

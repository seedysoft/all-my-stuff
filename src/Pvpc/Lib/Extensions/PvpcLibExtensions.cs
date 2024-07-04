namespace Seedysoft.Pvpc.Lib.Extensions;

public static class PvpcLibExtensions
{
    internal static Libs.TuyaDeviceControl.TuyaDeviceBase ToTuyaDeviceBase(this Libs.Core.Entities.TuyaDevice self) =>
        new(self.Id, self.Address, self.LocalKey, version: self.Version);
}

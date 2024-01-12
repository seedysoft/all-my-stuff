namespace Seedysoft.TuyaDeviceControlLib;

/// <summary>
/// namedtuple("MessagePayload", "cmd payload")
/// </summary>
/// <param name="Cmd">Command from <see cref="TuyaCommandTypes"/>.</param>
/// <param name="Payload"></param>
public record MessagePayload(uint Cmd, byte[] Payload) { }

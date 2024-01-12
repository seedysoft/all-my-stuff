namespace Seedysoft.TuyaDeviceControlLib;

/// <summary>
/// TuyaHeader = namedtuple('TuyaHeader', 'prefix seqno cmd length total_length')
/// </summary>
/// <param name="Prefix"></param>
/// <param name="SeqNo"></param>
/// <param name="Cmd"></param>
/// <param name="Length"></param>
/// <param name="TotalLength"></param>
public record TuyaHeader(uint Prefix, uint SeqNo, uint Cmd, uint Length, uint TotalLength);

namespace Seedysoft.Libs.TuyaDeviceControl;

/// <summary>
/// TuyaMessage = namedtuple("TuyaMessage", "seqno cmd retcode payload crc crc_good prefix iv", defaults=(True,0x55AA,None))
/// </summary>
/// <param name="SeqNo"></param>
/// <param name="Cmd"></param>
/// <param name="RetCode"></param>
/// <param name="Payload"></param>
/// <param name="Crc"></param>
/// <param name="IsCrcGood"></param>
/// <param name="Prefix"></param>
/// <param name="InitVector"></param>
public record class TuyaMessage(
    uint SeqNo,
    uint Cmd,
    uint RetCode,
    byte[] Payload,
    byte[]? Crc = null,
    bool IsCrcGood = true,
    uint Prefix = ProtocolVersionsAndHeaders.PREFIX_VALUE,
    byte[]? InitVector = null)
    : MessagePayload(Cmd, Payload)
{
    public TuyaMessage(
        uint SeqNo,
        uint Cmd,
        byte[] payload) : this(
            SeqNo,
            Cmd,
            RetCode: default,
            Payload: payload,
            Crc: default,
            IsCrcGood: true,
            Prefix: ProtocolVersionsAndHeaders.PREFIX_VALUE,
            InitVector: null)
    { }
}

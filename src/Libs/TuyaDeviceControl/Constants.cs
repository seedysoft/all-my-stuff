namespace Seedysoft.Libs.TuyaDeviceControl;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
public sealed class Constants
{
    private Constants() { }

    private const string UDP_KEY = "yGAdlopoPVldABfn";

    // UDP packet payload decryption - credit to tuya-convert
    public static readonly byte[] UpdKey = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.ASCII.GetBytes(UDP_KEY));

    public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> PayloadDefDict { get; } = new()
    {
        {
            "default", new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "AP_CONFIG", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "gwId", string.Empty }, { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } } },
                    }
                },
                {
                    "CONTROL", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } } },
                    }
                },
                {
                    "STATUS", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "gwId", string.Empty }, { "devId", string.Empty } } },
                    }
                },
                {
                    "HEART_BEAT", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "gwId", string.Empty }, { "devId", string.Empty } } },
                    }
                },
                {
                    "DP_QUERY", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "gwId", string.Empty }, { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } } },
                    }
                },
                {
                    "CONTROL_NEW", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } } },
                    }
                },
                {
                    "DP_QUERY_NEW", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } } },
                    }
                },
                {
                    "UPDATEDPS", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "dpId", new List<int> { 18, 19, 20 } } } },
                    }
                },
                {
                    "LAN_EXT_STREAM", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "reqType", string.Empty }, { "data", new Dictionary<string, object>() } } },
                    }
                },
            }
        },
        {
            "device22", new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "DP_QUERY", new Dictionary<string, object>
                    {
                        { "command_override", "CONTROL_NEW" },
                        { "command", new Dictionary<string, object> { { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } } },
                    }
                },
            }
        },
        {
            "v3.4", new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "CONTROL", new Dictionary<string, object>
                    {
                        { "command_override", "CONTROL_NEW" },
                        { "command", new Dictionary<string, object> { { "protocol", 5 }, { "t", "int" }, { "data", new Dictionary<string, object>() } } },
                    }
                },
                {
                    "CONTROL_NEW", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "protocol", 5 }, { "t", "int" }, { "data", new Dictionary<string, object>() } } },
                    }
                },
                {
                    "DP_QUERY", new Dictionary<string, object>
                    {
                        { "command_override", "DP_QUERY_NEW" },
                        { "command", new Dictionary<string, object>() },
                    }
                },
                {
                    "DP_QUERY_NEW", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object>() },
                    }
                },
            }
        },
        {
            "v3.5", new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "CONTROL", new Dictionary<string, object>
                    {
                        { "command_override", "CONTROL_NEW" },
                        { "command", new Dictionary<string, object> { { "protocol", 5 }, { "t", "int" }, { "data", new Dictionary<string, object>() } } },
                    }
                },
                {
                    "CONTROL_NEW", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object> { { "protocol", 5 }, { "t", "int" }, { "data", new Dictionary<string, object>() } } },
                    }
                },
                {
                    "DP_QUERY", new Dictionary<string, object>
                    {
                        { "command_override", "DP_QUERY_NEW" },
                        { "command", new Dictionary<string, object>() },
                    }
                },
                {
                    "DP_QUERY_NEW", new Dictionary<string, object>
                    {
                        { "command", new Dictionary<string, object>() },
                    }
                },
            }
        },
    };

    public static readonly Dictionary<ErrorCode, string> ERROR_CODES = new() {
        { ErrorCode.ERR_JSON,       "Invalid JSON Response from Device"},
        { ErrorCode.ERR_CONNECT,    "Network Error: Unable to Connect"},
        { ErrorCode.ERR_TIMEOUT,    "Timeout Waiting for Device"},
        { ErrorCode.ERR_RANGE,      "Specified Value Out of Range"},
        { ErrorCode.ERR_PAYLOAD,    "Unexpected Payload from Device"},
        { ErrorCode.ERR_OFFLINE,    "Network Error: Device Unreachable"},
        { ErrorCode.ERR_STATE,      "Device in Unknown State"},
        { ErrorCode.ERR_FUNCTION,   "Function Not Supported by Device"},
        { ErrorCode.ERR_DEVTYPE,    "Device22 Detected: Retry Command"},
        { ErrorCode.ERR_CLOUDKEY,   "Missing Tuya Cloud Key and Secret"},
        { ErrorCode.ERR_CLOUDRESP,  "Invalid JSON Response from Cloud"},
        { ErrorCode.ERR_CLOUDTOKEN, "Unable to Get Cloud Token"},
        { ErrorCode.ERR_PARAMS,     "Missing Function Parameters"},
        { ErrorCode.ERR_CLOUD,      "Error Response from Tuya Cloud"},
        { ErrorCode.ERR_KEY_OR_VER, "Check device key or version"},
        { default,                  "Unknown Error"},
    };

    public static readonly uint[] HEADER_COMMANDS_WITHOUT_PROTOCOL = [
        (uint)TuyaCommandTypes.DP_QUERY,
        (uint)TuyaCommandTypes.DP_QUERY_NEW,
        (uint)TuyaCommandTypes.UPDATEDPS,
        (uint)TuyaCommandTypes.HEART_BEAT,
        (uint)TuyaCommandTypes.SESS_KEY_NEG_START,
        (uint)TuyaCommandTypes.SESS_KEY_NEG_RESP,
        (uint)TuyaCommandTypes.SESS_KEY_NEG_FINISH,
        (uint)TuyaCommandTypes.LAN_EXT_STREAM,
    ];
}

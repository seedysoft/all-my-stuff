namespace Seedysoft.Libs.TuyaDeviceControl;

public enum ErrorCode : uint
{
    NoError = 0,
    ERR_JSON = 900,
    ERR_CONNECT = 901,
    ERR_TIMEOUT = 902,
    ERR_RANGE = 903,
    ERR_PAYLOAD = 904,
    ERR_OFFLINE = 905,
    ERR_STATE = 906,
    ERR_FUNCTION = 907,
    ERR_DEVTYPE = 908,
    ERR_CLOUDKEY = 909,
    ERR_CLOUDRESP = 910,
    ERR_CLOUDTOKEN = 911,
    ERR_PARAMS = 912,
    ERR_CLOUD = 913,
    ERR_KEY_OR_VER = 914,
}

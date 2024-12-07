namespace Seedysoft.Libs.TuyaDeviceControl;

/// <summary>
/// Reference: https://github.com/tuya/tuya-iotos-embeded-sdk-wifi-ble-bk7231n/blob/master/sdk/include/lan_protocol.h
/// </summary>
public enum TuyaCommandTypes : uint
{
    /// <summary>
    /// FRM_TP_CFG_WF      # only used for ap 3.0 network config
    /// </summary>
    AP_CONFIG = 1,
    /// <summary>
    /// FRM_TP_ACTV (discard) # WORK_MODE_CMD
    /// </summary>
    ACTIVE = 2,
    /// <summary>
    /// FRM_SECURITY_TYPE3 # negotiate session key
    /// </summary>
    SESS_KEY_NEG_START = 3,
    /// <summary>
    /// FRM_SECURITY_TYPE4 # negotiate session key response
    /// </summary>
    SESS_KEY_NEG_RESP = 4,
    /// <summary>
    /// FRM_SECURITY_TYPE5 # finalize session key negotiation
    /// </summary>
    SESS_KEY_NEG_FINISH = 5,
    /// <summary>
    /// FRM_TP_UNBIND_DEV  # DATA_QUERT_CMD - issue command
    /// </summary>
    UNBIND = 6,
    /// <summary>
    /// FRM_TP_CMD         # STATE_UPLOAD_CMD
    /// </summary>
    CONTROL = 7,
    /// <summary>
    /// FRM_TP_STAT_REPORT # STATE_QUERY_CMD
    /// </summary>
    STATUS = 8,
    /// <summary>
    /// FRM_TP_HB
    /// </summary>
    HEART_BEAT = 9,
    /// <summary>
    /// 10 # FRM_QUERY_STAT      # UPDATE_START_CMD - get data points
    /// </summary>
    DP_QUERY = 0x0a,
    /// <summary>
    /// 11 # FRM_SSID_QUERY (discard) # UPDATE_TRANS_CMD
    /// </summary>
    QUERY_WIFI = 0x0b,
    /// <summary>
    /// 12 # FRM_USER_BIND_REQ   # GET_ONLINE_TIME_CMD - system time (GMT)
    /// </summary>
    TOKEN_BIND = 0x0c,
    /// <summary>
    /// 13 # FRM_TP_NEW_CMD      # FACTORY_MODE_CMD
    /// </summary>
    CONTROL_NEW = 0x0d,
    /// <summary>
    /// 14 # FRM_ADD_SUB_DEV_CMD # WIFI_TEST_CMD
    /// </summary>
    ENABLE_WIFI = 0x0e,
    /// <summary>
    /// 15 # FRM_CFG_WIFI_INFO
    /// </summary>
    WIFI_INFO = 0x0f,
    /// <summary>
    /// 16 # FRM_QUERY_STAT_NEW
    /// </summary>
    DP_QUERY_NEW = 0x10,
    /// <summary>
    /// 17 # FRM_SCENE_EXEC
    /// </summary>
    SCENE_EXECUTE = 0x11,
    /// <summary>
    /// 18 # FRM_LAN_QUERY_DP    # Request refresh of DPS
    /// </summary>
    UPDATEDPS = 0x12,
    /// <summary>
    /// 19 # FR_TYPE_ENCRYPTION
    /// </summary>
    UDP_NEW = 0x13,
    /// <summary>
    /// 20 # FRM_AP_CFG_WF_V40
    /// </summary>
    AP_CONFIG_NEW = 0x14,
    /// <summary>
    /// 35 # FR_TYPE_BOARDCAST_LPV34
    /// </summary>
    BOARDCAST_LPV34 = 0x23,
    /// <summary>
    /// 64 # FRM_LAN_EXT_STREAM
    /// </summary>
    LAN_EXT_STREAM = 0x40,
}

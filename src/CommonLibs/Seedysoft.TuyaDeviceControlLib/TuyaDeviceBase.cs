using Seedysoft.TuyaDeviceControlLib.Exceptions;
using Seedysoft.TuyaDeviceControlLib.Extensions;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Seedysoft.TuyaDeviceControlLib;

public class TuyaDeviceBase
{
    #region Properties

    private string Id { get; set; }
    private string Address { get; set; }
    private string DevType { get; set; }
    private bool DevTypeAuto { get; set; }
    private string LastDevType { get; set; }

    private int ConnTimeout;
    public int ConnectionTimeout
    {
        get => ConnTimeout;
        set
        {
            ConnTimeout = value;
            if (Socket != null)
                Socket.ReceiveTimeout = Socket.SendTimeout = (int)TimeSpan.FromSeconds(value).TotalMilliseconds;
        }
    }

    private bool IsRetryEnabled { get; set; }
    private bool IsDetectDisabled { get; set; }
    private int Port { get; set; }
    private Socket? Socket { get; set; }

    private bool IsSockPersistant;
    private bool IsSocketPersistent
    {
        get => IsSockPersistant;
        set
        {
            IsSockPersistant = value;
            if (Socket != null && !value)
            {
                Socket.Close();
                Socket = null;
            }
        }
    }

    private bool IsSockNoDelay;
    private bool IsSocketNoDelay
    {
        get => IsSockNoDelay;
        set
        {
            IsSockNoDelay = value;
            Socket?.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.NoDelay, value);
        }
    }

    private int SocketRetryLimit { get; set; }
    private int SocketRetryDelay { get; set; }

    private float Vers;
    private float Version
    {
        get => Vers;
        set
        {
            Vers = value;
            PayloadDict = null;
        }
    }

    private string VersionStr => string.Create(System.Globalization.CultureInfo.InvariantCulture, $"v{Vers:0.0}");
    private byte[] VersionBytes => JsonSerializer.SerializeToUtf8Bytes(Vers);
    private byte[] VersionHeader => [.. VersionBytes, .. ProtocolVersionsAndHeaders.PROTOCOL_3x_HEADER];

    private Dictionary<string, object?> DpsToRequest { get; set; }
    private uint SeqNo { get; set; }
    private float? SendWait { get; set; }
    private byte[] RemoteNonce { get; set; }
    private Dictionary<string, Dictionary<string, object>>? PayloadDict { get; set; }

    private byte[] LocalKey { get; set; }
    private byte[] RealLocalKey { get; set; }
    private AESCipher? Cipher { get; set; }

    private static byte[] LocalNonce => Encoding.UTF8.GetBytes("0123456789abcdef");

    #endregion

    public TuyaDeviceBase(
        string devId,
        string address,
        string localKey,
        string devType = "default",
        int connectionTimeout = 5,
        float? version = null,
        bool persist = false,
        int connectionRetryLimit = 5,
        int connectionRetryDelay = 5)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentException.ThrowIfNullOrWhiteSpace(localKey);

        Id = devId;
        Address = address;
        DevType = devType;
        DevTypeAuto = DevType == "default";
        LastDevType = string.Empty;
        ConnectionTimeout = connectionTimeout;
        IsRetryEnabled = true;
        IsDetectDisabled = false;
        Port = GlobalsNetworkSettings.TCPPORT;
        IsSocketPersistent = persist;
        IsSocketNoDelay = true;
        SocketRetryLimit = connectionRetryLimit;
        SocketRetryDelay = connectionRetryDelay;
        DpsToRequest = [];
        SeqNo = 1;
        SendWait = 0.01f;
        RemoteNonce = [];

        LocalKey = Encoding.Latin1.GetBytes(localKey);//.encode("latin1");
        RealLocalKey = LocalKey;

        // #make sure we call our set_version() and not a subclass since some of
        // #them (such as BulbDevice) make connections when called
        Version = version ?? float.Parse(ProtocolVersionsAndHeaders.VERSION_31);
    }

    // TODO          Wrap results into class/record
    public object? GetStatus(bool noWait = false)
    {
        Debug.WriteLine($"{nameof(GetStatus)}() called ({nameof(DevType)} is '{DevType}')");
        MessagePayload? payload = GenerateMessagePayload(TuyaCommandTypes.DP_QUERY);

        object? data = SendReceive(payload, getResponse: !noWait);
        Debug.WriteLine($"{nameof(GetStatus)} received data={JsonSerializer.Serialize(data)}");
        if (!noWait && data != null && data is Dictionary<string, object> errorJson && errorJson.TryGetValue("err", out object? value))
        {
            if (value.ToString() == ErrorCode.ERR_DEVTYPE.ToString())
            {
                Debug.WriteLine($"{nameof(GetStatus)}() rebuilding payload for device22");
                payload = GenerateMessagePayload(TuyaCommandTypes.DP_QUERY);
                data = SendReceive(payload);
            }
            else if (value.ToString() == ErrorCode.ERR_PAYLOAD.ToString())
            {
                Debug.WriteLine($"Status request returned an error, is version {VersionStr} and local key {Convert.ToHexString(LocalKey)} correct?");
            }
        }

        return data;
    }

    public object? TurnOn(uint switchNumber = 1, bool noWait = false) => SetStatus(true, switchNumber, noWait);
    public object? TurnOff(uint switchNumber = 1, bool noWait = false) => SetStatus(false, switchNumber, noWait);

    #region Socket

    private void CheckSocketClose(bool force = false)
    {
        if ((force || !IsSocketPersistent) && Socket != null)
        {
            Socket.Close();
            Socket = null;
        }
    }

    private ErrorCode GetSocket(bool renew = false)
    {
        if (renew && Socket is not null)
        {
            // #this.socket.shutdown(socket.SHUT_RDWR)
            Socket.Close();
            Socket = null;
        }

        if (Socket is null)
        {
            // #Set up Socket
            int retries = 0;
            ErrorCode err = ErrorCode.ERR_OFFLINE;
            while (retries < SocketRetryLimit)
            {
                // #this.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                if (IsSocketNoDelay)
                    // #this.socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
                    Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                // #this.socket.settimeout(this.connection_timeout)
                Socket.ReceiveTimeout = Socket.SendTimeout = (int)TimeSpan.FromSeconds(ConnectionTimeout).TotalMilliseconds;

                try
                {
                    retries++;
                    Socket.Connect(System.Net.IPAddress.Parse(Address), Port);
                    if (Version < 3.4f)
                        return ErrorCode.NoError;

                    // restart session key negotiation
                    if (NegotiateSessionKey())
                        return ErrorCode.NoError;

                    if (Socket != null)
                    {
                        Socket.Close();
                        Socket = null;
                    }

                    return ErrorCode.ERR_KEY_OR_VER;
                }
                catch (Exception e) when (e is SocketException)
                {
                    // unable to open socket
                    Debug.WriteLine($"socket unable to connect (timeout) - retry {retries}/{SocketRetryLimit}");
                    err = ErrorCode.ERR_OFFLINE;
                }
                catch (Exception)
                {
                    // unable to open socket
                    Debug.WriteLine($"socket unable to connect (exception) - retry {retries}/{SocketRetryLimit}");
                    err = ErrorCode.ERR_CONNECT;
                }

                if (Socket != null)
                {
                    Socket.Close();
                    Socket = null;
                }

                if (retries < SocketRetryLimit)
                    Task.Delay(TimeSpan.FromSeconds(SocketRetryDelay)).Wait();
            }
            // unable to get connection
            return err;
        }

        // existing socket active
        return ErrorCode.NoError;
    }

    #endregion

    #region Negotiate Session Key

    private bool NegotiateSessionKey()
    {
        RemoteNonce = [];
        LocalKey = RealLocalKey;

        MessagePayload messagePayload = new((uint)TuyaCommandTypes.SESS_KEY_NEG_START, LocalNonce);

        TuyaMessage? rkey = SendReceiveQuick(payload: messagePayload, recvRetries: 2);
        if (rkey == null)
            return false;

        MessagePayload? step3 = NegotiateSessionKeyGenerateStep3(rkey);
        if (step3 == null)
            return false;

        _ = SendReceiveQuick(step3, 0);

        _ = NegotiateSessionKeyGenerateFinalize();

        return true;
    }
    private MessagePayload? NegotiateSessionKeyGenerateStep3(MessagePayload rkey)
    {
        if (rkey == null || rkey is not MessagePayload mp || mp.Payload.Length < 48)
        {
            Debug.WriteLine($"session key negotiation failed on step 1");
            return null;
        }

        if (rkey.Cmd != (uint)TuyaCommandTypes.SESS_KEY_NEG_RESP)
        {
            Debug.WriteLine($"session key negotiation step 2 returned wrong command={(TuyaCommandTypes)rkey.Cmd}");
            return null;
        }

        byte[] payload = rkey.Payload;
        if (Version == 3.4f)
        {
            try
            {
                Debug.WriteLine($"decrypting={Convert.ToHexString(payload)}");
                AESCipher cipher = new(RealLocalKey);
                payload = cipher.Decrypt(payload, useBase64: false, decodeText: false);
            }
            catch
            {
                Debug.WriteLine($"session key step 2 decrypt failed, payload={Convert.ToHexString(payload)} (len={payload.Length})");
                return null;
            }
        }

        Debug.WriteLine($"decrypted session key negotiation step 2 payload={Convert.ToHexString(payload)}");

        if (payload.Length < 48)
        {
            Debug.WriteLine($"session key negotiation step 2 failed, too short response");
            return null;
        }

        RemoteNonce = payload[..16];
        byte[] hmacCheck = HMACSHA256.HashData(LocalKey, LocalNonce);
        Debug.WriteLine($"{nameof(hmacCheck)} = {Convert.ToHexString(hmacCheck)} len = {hmacCheck.Length}");
        if (!hmacCheck.SequenceEqual(payload[16..48]))
        {
            Debug.WriteLine($"session key negotiation step 2 failed HMAC check! wanted={Convert.ToHexString(hmacCheck)} but got={Convert.ToHexString(payload[16..48])}");
            return null;
        }

        Debug.WriteLine($"Session Local nonce={Convert.ToHexString(LocalNonce)}");
        Debug.WriteLine($"Session Rmote nonce={Convert.ToHexString(RemoteNonce)}");
        byte[] rkeyHmac = HMACSHA256.HashData(LocalKey, RemoteNonce);

        return new MessagePayload((uint)TuyaCommandTypes.SESS_KEY_NEG_FINISH, rkeyHmac);
    }
    private bool NegotiateSessionKeyGenerateFinalize()
    {
        LocalKey = LocalNonce.Zip(RemoteNonce, (a, b) => Convert.ToByte(a ^ b)).ToArray();

        Debug.WriteLine($"{nameof(LocalNonce)}={Convert.ToHexString(LocalNonce)}");
        Debug.WriteLine($"{nameof(RemoteNonce)}={Convert.ToHexString(RemoteNonce)}");
        Debug.WriteLine($"Session nonce XOR'd={Convert.ToHexString(LocalKey)}");

        AESCipher cipher = new(RealLocalKey);
        if (Version == 3.4f)
        {
            LocalKey = cipher.Encrypt(LocalKey, useBase64: false, pad: false);
        }
        else
        {
            byte initVector = LocalNonce[12];
            Debug.WriteLine($"Session Initialization Vector={Convert.ToHexString([initVector])}");
            LocalKey = [.. cipher.Encrypt(LocalKey, useBase64: false, pad: false, initialVector: [initVector])[12..(12 + 28)]];
        }

        Debug.WriteLine($"Session key negotiate success! session key={Convert.ToHexString(LocalKey)}");

        return true;
    }

    // similar to _send_receive() but never retries sending and does not decode the response
    private TuyaMessage? SendReceiveQuick(MessagePayload payload, int recvRetries)
    {
        Debug.WriteLine($"sending payload quick");
        if (GetSocket(false) != ErrorCode.NoError)
            return null;

        byte[] encodedPayload = EncodeMessage(payload);
        //Debug.WriteLine($"payload={Convert.ToHexString(encodedPayload)}");
        try
        {
            _ = (Socket?.Send(encodedPayload));
        }
        catch
        {
            CheckSocketClose(true);
            return null;
        }

        if (recvRetries == 0)
            return null;

        TuyaMessage? msg;
        while (recvRetries > 0)
        {
            try
            {
                msg = Receive();
            }
            catch
            {
                msg = null;
            }

            if (msg != null && msg.Payload != null && msg.Payload.Length > 0)
                return msg;

            recvRetries -= 1;
            if (recvRetries == 0)
                Debug.WriteLine($"received null payload ({msg}) but out of recv retries, giving up");
            else
                Debug.WriteLine($"received null payload ({msg}), fetch new one - {recvRetries} retries remaining");
        }

        return null;
    }

    #endregion

    private Dictionary<string, object>? DecodeTuyaMessagePayload(byte[] payload)
    {
        Debug.WriteLine($"decode payload={Convert.ToHexString(payload)}");
        AESCipher cipher = new(LocalKey);
        if (Version == 3.4f)
        {
            // 3.4 devices encrypt the version header in addition to the payload
            try
            {
                Debug.WriteLine($"decrypting={Convert.ToHexString(payload)}");
                payload = cipher.Decrypt(payload, useBase64: false, decodeText: false);
            }
            catch
            {
                Debug.WriteLine($"incomplete payload={Convert.ToHexString(payload)} (len={payload.Length})");
                return ErrorJson(ErrorCode.ERR_PAYLOAD);
            }

            Debug.WriteLine($"decrypted 3.x payload={Convert.ToHexString(payload)}");
        }

        if (payload.Take(ProtocolVersionsAndHeaders.PROTOCOL_VERSION_BYTES_31.Length).SequenceEqual(ProtocolVersionsAndHeaders.PROTOCOL_VERSION_BYTES_31))
        {
            // Received an encrypted payload
            // Remove version header
            payload = payload[ProtocolVersionsAndHeaders.PROTOCOL_VERSION_BYTES_31.Length..];
            // Decrypt payload
            // Remove 16-bytes of MD5 hexdigest of payload
            payload = cipher.Decrypt(payload[16..]);
        }
        else if (Version >= 3.2f)
        {
            // 3.2 or 3.3 or 3.4 or 3.5
            // Trim header for non-default device type
            if (payload.Take(VersionBytes.Length).SequenceEqual(VersionBytes))
            {
                payload = payload[VersionHeader.Length..];
                Debug.WriteLine($"removing 3.x={Convert.ToHexString(payload)}");
            }
            else if (DevType == "device22" && (payload.Length & 0x0F) != 0)
            {
                payload = payload[VersionHeader.Length..];
                Debug.WriteLine($"removing device22 3.x header={Convert.ToHexString(payload)}");
            }

            if (Version < 3.4f)
            {
                try
                {
                    Debug.WriteLine($"decrypting={Convert.ToHexString(payload)}");
                    payload = cipher.Decrypt(payload, false);
                }
                catch
                {
                    Debug.WriteLine($"incomplete payload={Convert.ToHexString(payload)} (len={payload.Length})");
                    return ErrorJson(ErrorCode.ERR_PAYLOAD);
                }

                Debug.WriteLine($"decrypted 3.x payload={Convert.ToHexString(payload)}");
            }

            string spayload;
            try
            {
                spayload = Encoding.UTF8.GetString(payload);
            }
            catch
            {
                Debug.WriteLine($"payload was not string type and decoding failed");
                return ErrorJson(ErrorCode.ERR_JSON, payload);
            }

            if (!IsDetectDisabled && spayload.Contains("data unvalid"))
            {
                DevType = "device22";
                // set at least one DPS
                DpsToRequest = new Dictionary<string, object?> { { "1", null } };
                Debug.WriteLine($"'data unvalid' error detected. Switching to DevType {DevType}");
                return null;
            }
        }
        else if (payload.Take("{"u8.ToArray().Length) != "{"u8.ToArray())
        {
            Debug.WriteLine($"Unexpected payload={Convert.ToHexString(payload)}");
            return ErrorJson(ErrorCode.ERR_PAYLOAD, payload);
        }

        Debug.WriteLine($"decoded results={Convert.ToHexString(payload)}");

        Dictionary<string, object>? jsonPayload;
        try
        {
            jsonPayload = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
        }
        catch
        {
            jsonPayload = ErrorJson(ErrorCode.ERR_JSON, payload);
        }

        if (jsonPayload != null &&
            !jsonPayload.ContainsKey("dps") &&
            jsonPayload.TryGetValue("data", out object? dataDict) &&
            (dataDict is Dictionary<string, object> jp) &&
            jp.TryGetValue("dps", out _))
        {
            jsonPayload["dps"] = dataDict;
        }

        return jsonPayload;
    }
    /// <summary>
    /// Adds protocol header (if needed) and encrypts.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private byte[] EncodeMessage(MessagePayload msg)
    {
        byte[]? hmacKey = null;
        byte[]? initVector = null;
        byte[] payload = msg.Payload;
        Cipher = new AESCipher(LocalKey);

        TuyaMessage mess;
        if (Version >= 3.4f)
        {
            hmacKey = LocalKey;
            if (!Constants.HEADER_COMMANDS_WITHOUT_PROTOCOL.Contains(msg.Cmd))
                // add the 3.x header
                payload = [.. VersionHeader, .. payload];
            Debug.WriteLine($"final payload='{Convert.ToHexString(payload)}'");

            if (Version >= 3.5f)
            {
                initVector = [12];
                // TuyaMessage = namedtuple("TuyaMessage", "seqno cmd retcode payload crc crc_good prefix iv", defaults = (True, 0x55AA, None))
                // seqno cmd retcode payload crc crc_good, prefix, iv
                mess = new TuyaMessage(
                    SeqNo: SeqNo,
                    Cmd: msg.Cmd,
                    RetCode: 0,
                    Payload: payload,
                    Crc: [0],
                    IsCrcGood: true,
                    Prefix: ProtocolVersionsAndHeaders.PREFIX_6699_VALUE,
                    InitVector: initVector);
                SeqNo += 1;
                byte[] data = PackMessage(mess, hmacKey: LocalKey);
                Debug.WriteLine($"payload encrypted={Convert.ToHexString(data)}");

                return data;
            }

            payload = Cipher.Encrypt(payload, false);
        }
        else if (Version >= 3.2f)
        {
            // expect to connect and then disconnect to set new
            payload = Cipher.Encrypt(payload, false);
            if (!Constants.HEADER_COMMANDS_WITHOUT_PROTOCOL.Contains(msg.Cmd))
                // add the 3.x header
                payload = [.. VersionHeader, .. payload];
        }
        else if (msg.Cmd == (uint)TuyaCommandTypes.CONTROL)
        {
            // need to encrypt
            payload = Cipher.Encrypt(payload);
            byte[] preMd5String =
                [.. "data="u8.ToArray(), .. payload, .. "||lpv="u8.ToArray(), .. ProtocolVersionsAndHeaders.PROTOCOL_VERSION_BYTES_31, .. "||"u8.ToArray(), .. LocalKey];

            string hexDigest = BitConverter.ToString(MD5.HashData(preMd5String)).Replace("-", string.Empty);
            // #some tuya libraries strip 8: to :24
            // #payload = (PROTOCOL_VERSION_BYTES_31 + hexdigest[8:][:16].encode("latin1") + payload)
            payload = [.. ProtocolVersionsAndHeaders.PROTOCOL_VERSION_BYTES_31, .. Encoding.Latin1.GetBytes(hexDigest[8..]).Concat(payload)];
        }

        Cipher = null;
        mess = new TuyaMessage(
            SeqNo: SeqNo,
            Cmd: msg.Cmd,
            RetCode: 0,
            Payload: payload,
            Crc: [0],
            IsCrcGood: true,
            Prefix: ProtocolVersionsAndHeaders.PREFIX_55AA_VALUE,
            InitVector: initVector);
        SeqNo += 1;

        return PackMessage(mess, hmacKey: hmacKey);
    }
    private Dictionary<string, object>? ProcessMessage(TuyaMessage? msg, string? devType = null)
    {
        // null packet, nothing to decode
        if (msg == null || msg.Payload.Length == 0)
        {
            Debug.WriteLine($"raw unpacked message={JsonSerializer.Serialize(msg)}");
            // legacy/default mode avoids persisting socket across commands
            CheckSocketClose();
            return null;
        }

        Dictionary<string, object>? result;
        // option - decode Message with hard coded offsets
        // result = this._decode_payload(data[20:-8])
        // Unpack Message into TuyaMessage format
        // and return payload decrypted
        try
        {
            // #Data available: seqno cmd retcode payload crc
            Debug.WriteLine($"raw unpacked message={JsonSerializer.Serialize(msg)}");
            result = DecodeTuyaMessagePayload(msg.Payload);
            if (result is null)
                Debug.WriteLine($"DecodePayload() failed!");
        }
        catch
        {
            Debug.WriteLine($"error unpacking or decoding tuya JSON payload");
            result = ErrorJson(ErrorCode.ERR_PAYLOAD);
        }

        if (devType != null && devType != DevType)
        {
            Debug.WriteLine($"Device22 detected and updated ({devType} -> {DevType}) - Update payload and try again");
            result = ErrorJson(ErrorCode.ERR_DEVTYPE);
        }

        // legacy/default mode avoids persisting socket across commands
        CheckSocketClose();

        return result;
    }

    /// <summary>
    /// Generate the payload to send.
    /// </summary>
    /// <param name="command">The type of command.</param>
    /// <param name="data">The data to send. This is what will be passed via the 'dps' entry</param>
    /// <param name="gwId">Will be used for gwId</param>
    /// <param name="devId">Will be used for devId</param>
    /// <param name="uid">Will be used for uid</param>
    /// <param name="rawData"></param>
    /// <param name="reqType"></param>
    /// <returns></returns>
    private MessagePayload GenerateMessagePayload(
        TuyaCommandTypes commandType,
        Dictionary<string, object>? data = null,
        object? gwId = null,
        object? devId = null,
        object? uid = null,
        object? rawData = null,
        object? reqType = null)
    {
        uint command = (uint)commandType;

        // dicts will get referenced instead of copied if we don't do this
        Dictionary<string, object> _deepcopy(Dictionary<string, object> dict1)
        {
            Dictionary<string, object> result = [];

            foreach (string k in dict1.Keys)
                result[k] = dict1[k] is Dictionary<string, object> secDict ? _deepcopy(secDict) : dict1[k];

            return result;
        }
        // dict2 will be merged into dict1
        // as dict2 is payload_dict['...'] we only need to worry about copying 2 levels deep,
        //  the command id and "command"/"command_override" keys: i.e. dict2[CMD_ID]["command"]
        void _merge_payload_dicts(Dictionary<string, Dictionary<string, object>> dict1, Dictionary<string, Dictionary<string, object>> dict2)
        {
            foreach (string cmd in dict2.Keys)
            {
                if (!dict1.TryGetValue(cmd, out Dictionary<string, object>? value))
                {
                    dict1[cmd] = _deepcopy(dict2[cmd]);
                }
                else
                {
                    foreach (string var in dict2[cmd].Keys)
                        value[var] = dict2[cmd][var] is Dictionary<string, object> d ? _deepcopy(d) : dict2[cmd][var];
                }
            }
        }

        // start merging down to the final payload dict
        // later merges overwrite earlier merges
        // "default" - ("gateway" if gateway) - ("zigbee" if sub-device) - [version string] - ('gateway_'+[version string] if gateway) -
        //   'zigbee_'+[version string] if sub-device - [dev_type if not "default"]
        if (PayloadDict == null || !(PayloadDict.Count > 0) || LastDevType != DevType)
        {
            PayloadDict = [];
            _merge_payload_dicts(PayloadDict, Constants.PayloadDefDict["default"]);

            if (Constants.PayloadDefDict.TryGetValue(VersionStr, out Dictionary<string, Dictionary<string, object>>? versionDict))
                _merge_payload_dicts(PayloadDict, versionDict);
            if (DevType != "default")
                _merge_payload_dicts(PayloadDict, Constants.PayloadDefDict[DevType]);
            Debug.WriteLine($"final {nameof(PayloadDict)} for '{Id}' ('{VersionStr}'/'{DevType}')={JsonSerializer.Serialize(PayloadDict)}");
            // #save it so we don't have to calculate this again unless something changes
            LastDevType = DevType;
        }

        Dictionary<string, object>? jsonData = null;
        uint? commandOverride = null;
        string tuyaCommand = commandType.ToString();
        if (PayloadDict?.ContainsKey(tuyaCommand) ?? false)
        {
            if (PayloadDict[tuyaCommand].TryGetValue("command", out object? commandDict))
                jsonData = commandDict as Dictionary<string, object>;

            if (PayloadDict[tuyaCommand].TryGetValue("command_override", out object? commandOverrideDict))
                commandOverride = (uint)Enum.Parse<TuyaCommandTypes>((string)commandOverrideDict);
        }

        commandOverride ??= command;

        // I have yet to see a device complain about included but unneeded attribs, but they *will*
        // complain about missing attribs, so just include them all unless otherwise specified
        jsonData ??= new Dictionary<string, object> { { "gwId", string.Empty }, { "devId", string.Empty }, { "uid", string.Empty }, { "t", string.Empty } };

        if (jsonData.ContainsKey("gwId"))
            jsonData["gwId"] = gwId ?? Id;
        if (jsonData.ContainsKey("devId"))
            jsonData["devId"] = devId ?? Id;
        if (jsonData.ContainsKey("uid"))
            jsonData["uid"] = uid ?? Id;
        if (jsonData.TryGetValue("t", out object? value))
        {
            // time.time(): Return the time in seconds since the epoch as a floating point number.
            jsonData["t"] = value is int
                ? Convert.ToInt32((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds)
                : (object)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
        }

        if (rawData is not null && jsonData.ContainsKey("data"))
        {
            jsonData["data"] = rawData;
        }
        else if (data is not null)
        {
            if (jsonData.ContainsKey("dpId"))
                jsonData["dpId"] = data;
            else if (jsonData.TryGetValue("data", out object? dataDict))
                ((Dictionary<string, object>)dataDict)["dps"] = data;
            else
                jsonData["dps"] = data;
        }
        else if (DevType == "device22" && command == (uint)TuyaCommandTypes.DP_QUERY)
        {
            jsonData["dps"] = DpsToRequest;
        }

        if (reqType != null && jsonData.ContainsKey("reqType"))
            jsonData["reqType"] = reqType;

        string payload = jsonData != null && jsonData.Count > 0 && !string.IsNullOrWhiteSpace(jsonData.FirstOrDefault().Value?.ToString())
            ? JsonSerializer.Serialize(jsonData)
            : string.Empty;
        // Create byte buffer from hex data

        // if spaces are not removed device does not respond!
        payload = payload.Replace(" ", string.Empty);
        //payload = payload.encode("utf-8")
        Debug.WriteLine($"building command '{command}({tuyaCommand})' payload='{payload}'");

        // create Tuya message packet
        return new MessagePayload(commandOverride.Value, Encoding.UTF8.GetBytes(payload));
    }

    private TuyaMessage Receive()
    {
        // message consists of header + retcode + [data] + crc (4 or 32) + footer
        int minLength_55AA = StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_55AA) + 4 + 4 + ProtocolVersionsAndHeaders.SUFFIX_BIN.Length;
        // message consists of header + iv + retcode + [data] + crc (16) + footer
        int minLength_6699 = StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_6699) + 12 + 4 + 16 + ProtocolVersionsAndHeaders.SUFFIX_BIN.Length;
        int minLength = minLength_55AA < minLength_6699 ? minLength_55AA : minLength_6699;

        byte[] data = RecvAll(minLength);

        // search for the prefix.  if not found, delete everything except
        // the last (prefix_len - 1) bytes and recv more to replace it
        int prefixOffset_55AA = data.SkipWhile(x => !data.SequenceEqual(ProtocolVersionsAndHeaders.PREFIX_55AA_BIN)).Count();
        //var prefix_offset_6699 = data.find(ProtocolVersionsAndHeaders.PREFIX_6699_BIN);
        int prefixOffset_6699 = data.SkipWhile(x => !data.SequenceEqual(ProtocolVersionsAndHeaders.PREFIX_6699_BIN)).Count();

        while (prefixOffset_55AA != 0 && prefixOffset_6699 != 0)
        {
            Debug.WriteLine($"Message prefix not at the beginning of the received data!");
            Debug.WriteLine($"Offset 55AA={prefixOffset_55AA}, 6699={prefixOffset_6699}, Received data={Convert.ToHexString(data)}");
            if (prefixOffset_55AA < 0 && prefixOffset_6699 < 0)
            {
                int prefixLength = ProtocolVersionsAndHeaders.PREFIX_55AA_BIN.Length;
                data = data[(1 - prefixLength)..];
            }
            else
            {
                int prefixOffset = prefixOffset_55AA < 0 ? prefixOffset_6699 : prefixOffset_55AA;
                data = data[prefixOffset..];
            }

            data = [.. data, .. RecvAll(minLength - data.Length)];
            prefixOffset_55AA = data.SkipWhile(x => !data.SequenceEqual(ProtocolVersionsAndHeaders.PREFIX_55AA_BIN)).Count();
            prefixOffset_6699 = data.SkipWhile(x => !data.SequenceEqual(ProtocolVersionsAndHeaders.PREFIX_6699_BIN)).Count();
        }

        TuyaHeader header = TuyaExtensions.ParseHeader(data);
        int remaining = (int)(header.TotalLength - data.Length);
        if (remaining > 0)
            data = [.. data, .. RecvAll(remaining)];
        Debug.WriteLine($"received data={Convert.ToHexString(data)}");
        byte[]? hmacKey = Version >= 3.4f ? LocalKey : null;

        return UnpackMessage(data, header: header, hmacKey: hmacKey, noRetcode: false);
    }

    private byte[] RecvAll(int length)
    {
        int tries = 2;
        byte[] data = [];

        while (length > 0)
        {
            byte[] newData = new byte[length];
            _ = (Socket?.Receive(newData));
            if (newData.Length == 0 || newData.All(x => x == 0))
            {
                Debug.WriteLine($"{nameof(RecvAll)}(): no data? {Convert.ToHexString(newData)}");
                // #connection closed?
                if (--tries == 0)
                    throw new DecodeException("No data received - connection closed?");

                if (SendWait.HasValue)
                    Task.Delay(TimeSpan.FromSeconds(SendWait.Value)).Wait();

                continue;
            }
            //Debug.WriteLine($"{nameof(RecvAll)}() data='{Convert.ToHexString(newdata)}'");
            data = [.. data, .. newData];
            length -= newData.Length;
            tries = 2;
        }

        return data;
    }

    /// <summary>
    /// Send single buffer `payload` and receive a single buffer.
    /// </summary>
    /// <param name="payload">payload(bytes): Data to send. Set to 'None' to receive only.</param>
    /// <param name="getResponse">getresponse(bool): If True, wait for and return response.</param>
    /// <param name="decodeResponse"></param>
    /// <returns></returns>
    private object? SendReceive(MessagePayload? payload, bool getResponse = true, bool decodeResponse = true)
    {
        bool success = false;
        bool partialSuccess = false;
        int retries = 0;
        int recvRetries = 0;
        int maxRecvRetries = !IsRetryEnabled ? 0 : SocketRetryLimit;
        string devType = DevType;
        bool doSend = true;

        TuyaMessage? msg = null;

        while (!success)
        {
            // open up socket if device is available
            ErrorCode sockResult = GetSocket(false);
            if (sockResult != ErrorCode.NoError)
            {
                // unable to get a socket - device likely offline
                CheckSocketClose(true);
                return ErrorJson(sockResult, JsonSerializer.SerializeToUtf8Bytes(payload));
            }

            // send request to device
            try
            {
                if (payload is not null && doSend)
                {
                    Debug.WriteLine($"sending payload");
                    byte[] encodedPayload = EncodeMessage(payload);
                    Debug.WriteLine($"{nameof(encodedPayload)}='{Convert.ToHexString(encodedPayload)}'");
                    _ = (Socket?.Send(encodedPayload));
                    if (SendWait.HasValue)
                        Task.Delay(TimeSpan.FromSeconds(SendWait.Value)).Wait();
                }

                if (getResponse)
                {
                    doSend = false;
                    TuyaMessage rmsg = Receive();
                    // #device may send null ack (28 byte) response before a full response
                    // #consider it an ACK and do not retry the send even if we do not get a full response
                    if (rmsg != null)
                    {
                        payload = null;
                        partialSuccess = true;
                        msg = rmsg;
                    }

                    if ((msg == null || msg.Payload.Length == 0) && recvRetries <= maxRecvRetries)
                    {
                        Debug.WriteLine($"received null payload ({msg}), fetch new one - retry {recvRetries} / {maxRecvRetries}");
                        recvRetries += 1;
                        if (recvRetries > maxRecvRetries)
                            success = true;
                    }
                    else
                    {
                        success = true;
                        Debug.WriteLine($"received message={JsonSerializer.Serialize(msg)}");
                    }
                }
                else
                {
                    // # legacy/default mode avoids persisting socket across commands
                    CheckSocketClose();
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"Keyboard Interrupt - Exiting");
                break;
            }
            catch (SocketException se) when (se.SocketErrorCode == SocketError.TimedOut)
            {
                // a socket timeout occurred
                if (payload is null)
                {
                    // Receive only mode - return None
                    CheckSocketClose();
                    return null;
                }

                doSend = true;
                retries += 1;
                // toss old socket and get new one
                CheckSocketClose(true);
                Debug.WriteLine($"Timeout in {nameof(SendReceive)}() - retry {retries} / {SocketRetryLimit}");
                // if we exceed the limit of retries then lets get out of here
                if (retries > SocketRetryLimit)
                {
                    Debug.WriteLine($"Exceeded tinytuya retry limit ({SocketRetryLimit})");
                    // timeout reached - return error
                    return ErrorJson(ErrorCode.ERR_KEY_OR_VER, JsonSerializer.SerializeToUtf8Bytes(payload));
                }
                // wait a bit before retrying
                Task.Delay(TimeSpan.FromSeconds(0.1)).Wait();
            }
            catch (DecodeException)
            {
                Debug.WriteLine($"Error decoding received data - read retry {recvRetries}/{maxRecvRetries}");
                recvRetries += 1;
                if (recvRetries > maxRecvRetries)
                {
                    // we recieved at least 1 valid message with a null payload, so the send was successful
                    if (partialSuccess)
                    {
                        CheckSocketClose();
                        return null;
                    }
                    // no valid messages received
                    CheckSocketClose(true);
                    return ErrorJson(ErrorCode.ERR_KEY_OR_VER, JsonSerializer.SerializeToUtf8Bytes(payload));
                }
            }
            catch (Exception)
            {
                // likely network or connection error
                doSend = true;
                retries += 1;
                // toss old socket and get new one
                CheckSocketClose(true);
                Debug.WriteLine($"Network connection error in {nameof(SendReceive)}() - retry {retries}/{SocketRetryLimit}");
                // if we exceed the limit of retries then lets get out of here
                if (retries > SocketRetryLimit)
                {
                    Debug.WriteLine($"Exceeded tinytuya retry limit ({SocketRetryLimit})");
                    Debug.WriteLine($"Unable to connect to device ");
                    // timeout reached - return error
                    return ErrorJson(ErrorCode.ERR_CONNECT, JsonSerializer.SerializeToUtf8Bytes(payload));
                }
                // wait a bit before retrying
                Task.Delay(TimeSpan.FromSeconds(0.1)).Wait();
            }
        }

        if (!decodeResponse)
        {
            // legacy/default mode avoids persisting socket across commands
            CheckSocketClose();
            return msg;
        }

        return ProcessMessage(msg, devType);
    }

    /// <summary>
    /// Set status of the device to 'on' or 'off'.
    /// </summary>
    /// <param name="isOn">true for 'on', false for 'off'.</param>
    /// <param name="switchNumber">The switch to set</param>
    /// <param name="noWait">true to send without waiting for response.</param>
    /// <returns></returns>
    private object? SetStatus(bool isOn, uint switchNumber = 1, bool noWait = false)
    {
        // #open device, send request, then close connection
        // #if isinstance(switch, int) :
        //     #switch = str(switch)  # index and payload is a string
        MessagePayload payload = GenerateMessagePayload(
            TuyaCommandTypes.CONTROL,
            new Dictionary<string, object>() { { $"{switchNumber}", isOn } });

        object? data = SendReceive(payload, getResponse: !noWait);
        //log.debug("set_status received data=%r", data);
        Debug.WriteLine($"{nameof(SetStatus)} received data='{JsonSerializer.Serialize(data)}'");

        return data;
    }

    #region Pack & Unpack

    /// <summary>
    /// Packs a TuyaMessage() into a network packet, encrypting or adding a CRC if protocol requires.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="hmacKey"></param>
    /// <returns></returns>
    /// <exception cref="TypeException"></exception>
    /// <exception cref="ValueException"></exception>
    protected internal byte[] PackMessage(
        TuyaMessage msg,
        byte[]? hmacKey = null)
    {
        string headerFormat;
        string endFormat;
        int msgLength;
        object[]? headerData = null;

        if (msg.Prefix == ProtocolVersionsAndHeaders.PREFIX_55AA_VALUE)
        {
            headerFormat = ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_55AA;
            endFormat = hmacKey == null ? ProtocolVersionsAndHeaders.MESSAGE_END_FMT_55AA : ProtocolVersionsAndHeaders.MESSAGE_END_FMT_HMAC;
            msgLength = msg.Payload.Length + StructConverter.CalcSize(endFormat);
            headerData = [msg.Prefix, msg.SeqNo, msg.Cmd, msgLength];
        }
        else if (msg.Prefix == ProtocolVersionsAndHeaders.PREFIX_6699_VALUE)
        {
            if (hmacKey == null)
                throw new TypeException("key must be provided to pack 6699-format messages");

            headerFormat = ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_6699;
            endFormat = ProtocolVersionsAndHeaders.MESSAGE_END_FMT_6699;
            msgLength = msg.Payload.Length + (StructConverter.CalcSize(endFormat) - 4) + 12;
            if (msg.RetCode.GetType() == typeof(uint))
                msgLength += StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT);
            headerData = [msg.Prefix, 0U, msg.SeqNo, msg.Cmd, msgLength];
        }
        else
        {
            throw new ValueException($"{nameof(PackMessage)}() cannot handle message format {msg.Prefix:X8}");
        }

        // #Create full message excluding CRC and suffix
        byte[] data = StructConverter.Pack(headerFormat, headerData);
        if (msg.Prefix == ProtocolVersionsAndHeaders.PREFIX_6699_VALUE)
        {
            AESCipher cipher = new(hmacKey ?? []);
            byte[] raw = [.. StructConverter.Pack(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT, [msg.RetCode]), .. msg.Payload];

            byte[] data2 = cipher.Encrypt(raw, pad: false, initialVector: msg.InitVector, header: data.Skip(4).ToArray());
            data = [.. data, .. data2, .. ProtocolVersionsAndHeaders.SUFFIX_6699_BIN];
        }
        else
        {
            data = [.. data, .. msg.Payload];
            byte[] crc = hmacKey == null
                ? BitConverter.GetBytes(System.IO.Hashing.Crc32.HashToUInt32(data) & 0xFFFFFFFF)
                : HMACSHA256.HashData(hmacKey, data);
            //Debug.WriteLine($"crc={Convert.ToHexString(crc)}");
            // #Calculate CRC, add it together with suffix
            byte[] CrcAndSuffixPacked = StructConverter.Pack(endFormat, crc, ProtocolVersionsAndHeaders.SUFFIX_VALUE);
            //Debug.WriteLine($"CrcAndSuffixPacked={Convert.ToHexString(CrcAndSuffixPacked)}");
            data = [.. data, .. CrcAndSuffixPacked];
        }

        return data;
    }
    /// <summary>
    /// Unpacks a TuyaMessage() from a network packet, decrypting or checking the CRC if protocol requires.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="hmacKey"></param>
    /// <param name="header"></param>
    /// <param name="noRetcode"></param>
    /// <returns><see cref="TuyaMessage"/></returns>
    /// <exception cref="TypeException"></exception>
    /// <exception cref="ValueException"></exception>
    /// <exception cref="DecodeException"></exception>
    protected internal TuyaMessage UnpackMessage(
        byte[] data,
        byte[]? hmacKey = null,
        TuyaHeader? header = null,
        bool? noRetcode = false)
    {
        if (header == null)
            header = TuyaExtensions.ParseHeader(data);

        int headerLength;
        string endFormat;
        int retcodeLength;
        int msgLength;

        switch (header.Prefix)
        {
            case ProtocolVersionsAndHeaders.PREFIX_55AA_VALUE:
                headerLength = StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_55AA);
                endFormat = hmacKey == null ? ProtocolVersionsAndHeaders.MESSAGE_END_FMT_55AA : ProtocolVersionsAndHeaders.MESSAGE_END_FMT_HMAC;
                retcodeLength = noRetcode.GetValueOrDefault() ? 0 : StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT);
                msgLength = (int)(headerLength + header.Length);
                break;

            case ProtocolVersionsAndHeaders.PREFIX_6699_VALUE:
                if (hmacKey == null)
                    throw new TypeException("key must be provided to unpack 6699-format messages");
                headerLength = StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_HEADER_FMT_6699);
                endFormat = ProtocolVersionsAndHeaders.MESSAGE_END_FMT_6699;
                retcodeLength = 0;
                msgLength = (int)(headerLength + header.Length + 4);
                break;

            default:
                throw new ValueException($"{nameof(UnpackMessage)}() cannot handle message format '{header.Prefix:X8}'");
        }

        if (data.Length < msgLength)
            throw new DecodeException($"{nameof(UnpackMessage)}(): not enough data to unpack payload! need {headerLength + header.Length} but only have {data.Length}");

        int endLength = StructConverter.CalcSize(endFormat);

        uint retcode = retcodeLength == 0
            ? 0U
            : Convert.ToUInt32(Convert.ToHexString(StructConverter.Unpack(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT, data[headerLength..(headerLength + retcodeLength)])[0]), 16);

        //     collection[start                 :stop   :step=1(optional)]
        // #payload = data[header_len+retcode_len:msg_len]
        byte[] payload = data[(headerLength + retcodeLength)..msgLength];

        // Negative indexes refer to the positions of elements within an array-like object such as a list, tuple, or string, counting from the end of the data structure rather than the beginning
        // crc, suffix = struct.unpack(end_fmt, payload[-end_len:])
        byte[] real = payload.TakeLast(endLength).ToArray();
        byte[][] crcAndSuffix = StructConverter.Unpack(endFormat, real);

        byte[]? crc = crcAndSuffix[0];
        uint suffix = Convert.ToUInt32(Convert.ToHexString(crcAndSuffix[1]), 16);

        // payload = payload[:-end_len]
        payload = payload.SkipLast(endLength).ToArray();

        bool isCrcGood = false;
        byte[]? initVector = null;

        switch (header.Prefix)
        {
            case ProtocolVersionsAndHeaders.PREFIX_55AA_VALUE:
            {
                byte[] haveCrc = hmacKey == null
                    ? BitConverter.GetBytes(System.IO.Hashing.Crc32.HashToUInt32(new(data.Take((int)(headerLength + header.Length) - endLength).ToArray())))
                    : new HMACSHA256(hmacKey).ComputeHash(data.Take((int)(headerLength + header.Length) - endLength).ToArray());

                if (suffix != ProtocolVersionsAndHeaders.SUFFIX_55AA_VALUE)
                    Debug.WriteLine($"Suffix prefix wrong! {suffix:X8} != {ProtocolVersionsAndHeaders.SUFFIX_VALUE:X8}");

                isCrcGood = crc != null && haveCrc.SequenceEqual(crc);
                if (!isCrcGood)
                {
                    if (hmacKey == null)
                        Debug.WriteLine($"CRC wrong! {Convert.ToHexString(haveCrc)} != {Convert.ToHexString(crc ?? [])}");
                    else
                        Debug.WriteLine($"HMAC checksum wrong! {Convert.ToHexString(haveCrc)} != {Convert.ToHexString(crc ?? [])}");
                }

                initVector = null;
                break;
            }

            case ProtocolVersionsAndHeaders.PREFIX_6699_VALUE:
                initVector = payload[..12];
                payload = payload[12..];
                try
                {
                    Cipher = new AESCipher(hmacKey ?? []);
                    payload = Cipher.Decrypt(
                        enc: payload,
                        useBase64: false,
                        decodeText: false,
                        verifyPadding: false,
                        initialVector: initVector,
                        header: data[4..headerLength],
                        tag: crc);
                    isCrcGood = true;
                }
                catch
                {
                    isCrcGood = false;
                }

                retcodeLength = StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT);
                if (noRetcode.HasValue && noRetcode.Value == false)
                {
                    /* do nothing */
                }
                else
                {
                    retcodeLength = noRetcode == null && payload[0] != '{' && payload[retcodeLength..(retcodeLength + 1)][0] == '{'
                        ? StructConverter.CalcSize(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT)
                        : 0;
                }

                if (retcodeLength > 0)
                {
                    retcode = Convert.ToUInt32(Convert.ToHexString(StructConverter.Unpack(ProtocolVersionsAndHeaders.MESSAGE_RETCODE_FMT, payload[..retcodeLength])[0]), 16);
                    payload = payload[retcodeLength..];
                }

                break;
        }

        return new TuyaMessage(
            SeqNo: header.SeqNo,
            Cmd: header.Cmd,
            RetCode: retcode,
            Payload: payload,
            Crc: crc,
            IsCrcGood: isCrcGood,
            Prefix: header.Prefix,
            InitVector: initVector);
    }

    #endregion

    private static Dictionary<string, object> ErrorJson(ErrorCode number, byte[]? payload = null)
    {
        string spayload;
        try
        {
            spayload = Encoding.UTF8.GetString(payload ?? []);
        }
        catch (Exception) { spayload = string.Empty; }

        return new Dictionary<string, object>() { { "error", Constants.ERROR_CODES[number] }, { "err", (uint)number }, { "payload", spayload } };
    }
}

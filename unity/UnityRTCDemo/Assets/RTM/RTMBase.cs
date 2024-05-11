using System;
using System.Runtime.InteropServices;

namespace LJ.RTM
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ApiRelayInfo
    {
        public string relayIP;
        public int relayPort;
        public int relayId;
        public int sessionId;
        public bool bgp;
        public ApiRelayInfo(string ip, int port, int _relayId,
            int _sessionId, bool _bgp) {
            relayIP = ip;
            relayPort = port;
            relayId = _relayId;
            sessionId = _sessionId;
            bgp = _bgp;
        }
    };


    public enum DataWorkMode
    {
        SEND_AND_RECV = 0, // 既发送又接收
        SEND_ONLY = 1, // 只是发送
        RECV_ONLY = 2, // 只是接收
        LOCK_STEP_SEND_RECV = 3,
    };

    public enum RUDPMode
    {
        RUDP_REALTIME_ULTRA, //RTM
        RUDP_REALTIME_NORMAL // RTC
    };

    public enum RUDPRole
    {
        RUDP_NORMAL, // 
        RUDP_CONTROLLER // 一般客户端设置为controller
    };


    [StructLayout(LayoutKind.Sequential)]
    public struct RUDPConfig
    {
        public string token;     // 正式环境不能为空，测试环境使用默认的token
        public UInt64 appId;        // 服务器分配的APPID
        public int mode;         // RUDPMode 0 RTM 1 RTC
        public int role;         // RUDPRole 0 normal 1 controller
        public bool isDebug;     // 测试环境还是正式环境
        public int dataWorkMode; // DataWorkMode
    };


    internal enum MsgType
    {
        MsgData,
        JoinChannel,
        LeaveChannel,
        LinkStatus,
        RemoteUserJoin,
        RemoteUserLeave
    };

    public enum LinkStatus
    {
        STATUS_CONNECTED = 1,
        STATUS_DISCONNECTED = 2,
        STATUS_LOST = 3,
    };

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void MessageCallback(IntPtr msg, int size, UInt64 uid, IntPtr content);

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void MessageCallbackEx(int type, IntPtr msg, int size, UInt64 uid, IntPtr content);

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void EventCallbackEx(int type, IntPtr msg, int size, int result, IntPtr context);

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void EventCallback(int type, IntPtr msg, int size, int result, IntPtr context);
    public class IRTMEngineEventHandler
    {
        /// <summary>
        /// 1V1 RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvMessage(byte[] message, long uid, string channelId)
        {
            //UnityEngine.Debug.Log("OnRecvMessage:" + System.Text.Encoding.UTF8.GetString(message));
        }

        /// <summary>
        /// 多人RTM 接收对端消息回调
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <param name="channelId"></param>
        public virtual void OnRecvExtMessage(byte[] message, long uid, string channelId)
        {
            //UnityEngine.Debug.Log("OnRecvExtMessage:" + System.Text.Encoding.UTF8.GetString(message));
        }

        /// <summary>
        /// 加入频道失败
        /// </summary>
        public virtual void OnJoinChannelFail()
        {
        }

        /// <summary>
        /// 加入频道成功
        /// </summary>
        public virtual void OnJoinChannelSuccess()
        {
           
        }

        /// <summary>
        /// 离开频道成功
        /// </summary>
        public virtual void OnLeaveChannelSuccess()
        {
            
        }

        /// <summary>
        /// 离开频道失败
        /// </summary>
        public virtual void OnLeaveChannelFail()
        {
            
        }
        /// <summary>
        ///STATUS_CONNECTED, 1
        ///STATUS_DISCONNECTED, 2
        ///STATUS_LOST, 3
        ///STATUS_CLOSE 4
        /// </summary>
        /// <param name="status">LinkStatus</param>
        public virtual void OnLinkStatusChange(int status)
        {

        }

        public virtual void OnRemoteUserJoined(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserJoined:" + userId);
        }

        public virtual void OnRemoteUserOffLine(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserOffLine:" + userId);
        }
    }
}

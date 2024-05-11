using AOT;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace LJ.RTM
{
    public class RTMChannel
    {
        private IRTMEngineEventHandler mEventHandler;
        private IntPtr mNativeRtm = IntPtr.Zero;
        private string localChannelId;
        private UInt64 _localUid;
        private static ConcurrentDictionary<IntPtr, RTMChannel> _cbList = new ConcurrentDictionary<IntPtr, RTMChannel>();
        public RTMChannel(RUDPRole role, DataWorkMode mode, UInt64 uid, string token, UInt64 appId,
                    bool isDebug, string channelId, IRTMEngineEventHandler handler) {
            localChannelId = channelId;
            mEventHandler = handler;
            RUDPConfig config;
            config.token = token;
            config.appId = appId;
            config.mode = (int)RUDPMode.RUDP_REALTIME_ULTRA;
            config.role = (int)role;
            config.isDebug = isDebug;
            config.dataWorkMode = (int)mode;
            _localUid = uid;
            mNativeRtm = RTMChannelNative.NativeCreateRTMEngine(config);
            _cbList.TryAdd(mNativeRtm, this);

            RTMChannelNative.NativeRegisterExtMsgCallback(mNativeRtm, MsgCallback, mNativeRtm);
            RTMChannelNative.NativeRegisterExtEventCallback(mNativeRtm, EventCallback, mNativeRtm);
        }

        private void HandleEventCallback(int type, IntPtr msg, int size, int result, IntPtr context) {

            if (type == (int)MsgType.JoinChannel)
            {
                if (result == 0)
                {
                    mEventHandler.OnJoinChannelSuccess();
                }
                else
                {
                    mEventHandler.OnJoinChannelFail();
                }
            }
            else if (type == (int)MsgType.LeaveChannel)
            {
                if (result == 0)
                {
                    mEventHandler.OnLeaveChannelSuccess();
                }
                else
                {
                    mEventHandler.OnLeaveChannelFail();
                }
            }
            else if (type == (int)MsgType.LinkStatus)
            {
                mEventHandler.OnLinkStatusChange((int)result);
            }
            else if (type == (int)MsgType.RemoteUserJoin || type == (int)MsgType.RemoteUserLeave)
            {
                byte[] userIdByte = new byte[size];
                Marshal.Copy(msg, userIdByte, 0, size);
                String UserIdStr = System.Text.Encoding.UTF8.GetString(userIdByte);
                UInt64 UserId = UInt64.Parse(UserIdStr);
                if (type == (int)MsgType.RemoteUserJoin)
                {
                    mEventHandler.OnRemoteUserJoined(UserId);
                }
                else {
                    mEventHandler.OnRemoteUserOffLine(UserId);
                }
            }
        }

        public int Join() {
            if (mNativeRtm == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("JoinChannelExt RTMChannel mNativeRtm == IntPtr.Zero");
                return -1;
            }
            return RTMChannelNative.NativeJoinChannelEx(mNativeRtm, (UInt64)_localUid, localChannelId);
        }

        public int Join(string token)
        {
            if (mNativeRtm == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("JoinChannelExt RTMChannel mNativeRtm == IntPtr.Zero");
                return -1;
            }
            return RTMChannelNative.NativeJoinChannelExWithToken(mNativeRtm, token,(UInt64)_localUid, localChannelId);
        }

        

        public int Leave() {
            if (mNativeRtm == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("Destroy RTMChannel mNativeRtm == IntPtr.Zero");
                return -1;
            }
            RTMChannelNative.NativeLeaveCannel(mNativeRtm);
            return 0;
        }

        [Obsolete("This method is deprecated. Use the newMethod instead.")]
        public void SendMsg(string msg) {
            if (mNativeRtm == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("SendMsg RTMEngine mNativeRtm == IntPtr.Zero");
                return;
            }
            if (msg == null || msg.Length == 0) {
                UnityEngine.Debug.Log("SendMsg RTMEngine msg is null");
                return;
            }
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(msg);
            RTMChannelNative.NativeSend(mNativeRtm, byteArray, byteArray.Length);
        }

        public int SendMsg(byte[] msg)
        {
            if (msg == null || msg.Length == 0)
            {
                return -2;
            }
            if (mNativeRtm == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("SendMsg byte RTMEngine mNativeRtm == IntPtr.Zero");
                return -1;
            }
            return RTMChannelNative.NativeSend(mNativeRtm, msg, msg.Length);
        }

        private void HandleMsgCallback(int type, IntPtr msg, int size, long uid, IntPtr content)
        {
            if (mEventHandler != null)
            {
                if (type == (int)MsgType.MsgData)
                {
                    byte[] managedArray = new byte[size];
                    Marshal.Copy(msg, managedArray, 0, size);
                    mEventHandler.OnRecvExtMessage(managedArray, uid, localChannelId);
                }
            }
        }


        public int Dispose() {
            if (mNativeRtm == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("Destroy RTMChannel mNativeRtm == IntPtr.Zero");
                return -1;
            }
            RTMChannelNative.NativeDestroyRTMEengine(mNativeRtm);
            RTMChannel remove;
            _cbList.TryRemove(mNativeRtm, out remove);
            mNativeRtm = IntPtr.Zero;
            mEventHandler = null;
            return 0;
        }

        [MonoPInvokeCallback(typeof(MessageCallbackEx))]
        public static void MsgCallback(int type, IntPtr msg, int size, UInt64 uid, IntPtr content)
        {
            RTMChannel rtm;
            if (_cbList.TryGetValue(content, out rtm))
            {
                rtm.HandleMsgCallback(type, msg, size, (long)uid, content);
            }
        }


        [MonoPInvokeCallback(typeof(EventCallbackEx))]
        public static void EventCallback(int type, IntPtr msg, int size, int result, IntPtr content)
        {
            RTMChannel rtm;
            if (_cbList.TryGetValue(content, out rtm))
            {
                rtm.HandleEventCallback(type, msg, size, result, content);
            }
        }
    }
}

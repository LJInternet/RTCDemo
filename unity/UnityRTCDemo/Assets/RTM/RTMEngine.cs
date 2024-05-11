using AOT;
using LJ.Log;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
namespace LJ.RTM
{
    public class RTMEngine
    {
        private IntPtr mNativeRtm = IntPtr.Zero;
        private string localChannelId;
        private IRTMEngineEventHandler mEventHandler;
        private static ConcurrentDictionary<IntPtr, RTMEngine> _eventList = new ConcurrentDictionary<IntPtr, RTMEngine>();
        public RTMEngine(RUDPConfig config, IRTMEngineEventHandler handler)
        {
            config.mode = (int)RUDPMode.RUDP_REALTIME_ULTRA;
            SetRTMENgineEventHandler(handler);
            if (mNativeRtm != IntPtr.Zero)
            {
                FLog.Error("init RTMEngine mNativeRtm != IntPtr.Zero");
                return;
            }
            mNativeRtm = RTMNative.NativeCreateRTMEngine(config);
            _eventList.TryAdd(mNativeRtm, this);
            if (handler != null)
            {
                RTMNative.NativeRegisterMsgCallback(mNativeRtm, RTMMsgCallback, mNativeRtm);
                RTMNative.NativeRegisterEventCallback(mNativeRtm, RTMEventCallback, mNativeRtm);
            }
        }
        public void SetRTMENgineEventHandler(IRTMEngineEventHandler eventHandler)
        {
            mEventHandler = eventHandler;
        }
        public int Destroy()
        {
            if (mNativeRtm == IntPtr.Zero)
            {
                FLog.Error("Destroy RTMEngine mNativeRtm -= IntPtr.Zero");
                return -1;
            }
            RTMNative.NativeDestroyRTMEengine(mNativeRtm);
            RTMEngine remove;
            _eventList.TryRemove(mNativeRtm, out remove);
            mNativeRtm = IntPtr.Zero;
            mEventHandler = null;
            return 0;
        }
        public int JoinChannel(long uid, string channelId)
        {
            if (mNativeRtm == IntPtr.Zero)
            {
                FLog.Error("JoinChannel RTMEngine mNativeRtm == IntPtr.Zero");
                return -1;
            }
            localChannelId = channelId;
            return RTMNative.NativeJoinChannel(mNativeRtm, (UInt64)uid, channelId);
        }
        public int LeaveChannel()
        {
            if (mNativeRtm == IntPtr.Zero)
            {
                FLog.Error("LeaveChannel RTMEngine mNativeRtm == IntPtr.Zero");
                return -1;
            }
            return RTMNative.NativeLeaveCannel(mNativeRtm);
        }

        [Obsolete("This method is deprecated. Use the newMethod instead.")]
        public int SendMsg(string msg)
        {
             return SendMsg2Channel(msg);
        }

        public int SendMsg(byte[] msg)
        {
            if (msg == null || msg.Length == 0) {
                return -2;
            }
            if (mNativeRtm == IntPtr.Zero)
            {
                FLog.Error("SendMsg byte RTMEngine mNativeRtm == IntPtr.Zero");
                return -1;
            }
            return RTMNative.NativeSend(mNativeRtm, msg, msg.Length);
        }

        private int SendMsg2Channel(string msg)
        {
            if (mNativeRtm == IntPtr.Zero)
            {
                FLog.Error("SendMsg byte RTMEngine mNativeRtm == IntPtr.Zero");
                return -1;
            }
            if (msg == null)
            {
                return -2;
            }
            if (msg == null || msg.Length == 0)
            {
                UnityEngine.Debug.Log("SendMsg RTMChannel msg is null");
                return -2; ;
            }
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(msg);
            return RTMNative.NativeSend(mNativeRtm, byteArray, byteArray.Length);
        }

        public void CallNativeCallback(IntPtr msg, int size, long uid)
        {
            if (mEventHandler != null)
            {
                byte[] managedArray = new byte[size];
                Marshal.Copy(msg, managedArray, 0, size);
                mEventHandler.OnRecvMessage(managedArray, uid, localChannelId);
            }
        }
        public void CallNativeEventCallback(int type, IntPtr msg, int size, int result)
        {
            if (mEventHandler != null)
            {
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
            }
        }
        [MonoPInvokeCallback(typeof(MessageCallback))]
        public static void RTMMsgCallback(IntPtr msg, int size, UInt64 uid, IntPtr content)
        {
            RTMEngine rtm;
            if (_eventList.TryGetValue(content, out rtm))
            {
                rtm.CallNativeCallback(msg, size, (long)uid);
            }
        }
        [MonoPInvokeCallback(typeof(EventCallback))]
        public static void RTMEventCallback(int type, IntPtr msg, int size, int result, IntPtr context)
        {
            RTMEngine rtm;
            if (_eventList.TryGetValue(context, out rtm))
            {
                rtm.CallNativeEventCallback(type, msg, size, result);
            }
        }
    }
}

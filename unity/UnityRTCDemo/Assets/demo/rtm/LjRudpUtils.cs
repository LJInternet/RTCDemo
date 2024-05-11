using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using AOT;

using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections;
using LJ.RTC;
using LJ.Log;

namespace Fancy
{

    public class LjRudpUtils
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string RudpLibName = "rudp";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string RudpLibName = "rudp";
#elif UNITY_IOS
		private const string RudpLibName = "__Internal";
#else
        private const string RudpLibName = "rudp";
#endif
        //Summary

        #region dll
        [DllImport(RudpLibName)]
        private static extern int FancyJingMsgInit(int uid_, int virIP, int mode_);
        [DllImport(RudpLibName)]
        private static extern int FancyJingMsgAddConnAddr(string local, string server, short port, int sessionId, long relayId, byte prio);
        [DllImport(RudpLibName)]
        private static extern int FancyJingMsgRun();
        [DllImport(RudpLibName)]
        private static extern int FancyJingMsgSendToPeer(uint dest_id, IntPtr data, int len);
        [DllImport(RudpLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FancyJingMsgRegisterConnCallback(ConnDelegate connDelegate);

        [DllImport(RudpLibName)]
        private static extern int FancyJingMsgStop();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RecvDelegate(IntPtr message, int a, int b);
        [DllImport(RudpLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FancyJingMsgRegisterRecvCallback(RecvDelegate recvDelegate);

        [MonoPInvokeCallback(typeof(RecvDelegate))]
        public static void RecvMessageFromCppAsync(IntPtr message, int msgLength, int b)
        {
            byte[] managedArray = new byte[msgLength];
            Marshal.Copy(message, managedArray, 0, msgLength);
            //await RespReceiver(managedArray);
            //  LjRudpUtils.GetInstance().RespReceiver(managedArray);
            LjRudpUtils.RespReceiver(managedArray);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConnDelegate(int a, bool b);


        [MonoPInvokeCallback(typeof(RecvDelegate))]
        public static void ConnMessageFromCpp(int a, bool b)
        {
            FLog.Info("ConnMessageFromCpp:" + a + "," + b);
        }


        [DllImport(RudpLibName)]
        private static extern int FancyJingMsgRelease();
        [DllImport(RudpLibName)]
        private static extern IntPtr FancyJingMsgVersion();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RUDP_CALLBACK(int a, bool b);

        #endregion


        private bool isJoinLjRtm = false;
        public RecvDelegate Callback;

        #region life
        private void Awake()
        {

        }


        #endregion




        public Boolean isAndroid()
        {

            if (Application.platform == RuntimePlatform.Android
         || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return true;
            }
            return false;

        }




        // private BaseRudp.OnMessage DelegateReceiveMessage;
        //public delegate void OnMessage(RudpToFancyAdapter adapter);




        [MonoPInvokeCallback(typeof(RecvDelegate))]
        public static void RecvCallback(IntPtr message, int msgLength, int b)
        {
          
        }


        public static void RespReceiver(byte[] managedArray)
        {

        }


        //public void EndToEndTime(long[] endToEndTime) {

        //    long[] receiverTime = new long[11];
        //    Array.Copy(endToEndTime, 0, receiverTime, 0, endToEndTime.Length);
        //    receiverTime[10] = TimeUtils.GetTimeStamp();
        //    listLongTime.Add(receiverTime);
        //}


        public  void StartAndroidRUDP()
        {
            if (isJoinLjRtm)
            {
                StopRudp();
            }
            isJoinLjRtm = true;
            if (isAndroid())
            {
                FancyJingMsgRun();
            }
            else
            {
                FancyJingMsgRun();
            }
        }

        public void StopRudp()
        {
            FLog.Info("mRtm StopRudp: ");
            FancyJingMsgStop();
            FancyJingMsgRelease();
            isJoinLjRtm = false;
        }

        public void InitRudp()
        {
            throw new NotImplementedException();
        }

        public void JoinChannelRudp(int uuid, string rtcChannel, List<UdpInitConfig> relayServers, string user_id)
        {
            if (Application.platform == RuntimePlatform.Android
             || Application.platform == RuntimePlatform.IPhonePlayer)
            {
            }

            if (isAndroid())
            {
                FancyJingMsgInit(uuid, 0, 0);
                for (int i = 0; i < relayServers.Count; i++)
                {
                    UdpInitConfig receivedWsRelayServersBean = relayServers[i];
                    FancyJingMsgAddConnAddr("0.0.0.0", receivedWsRelayServersBean.remoteIP, (short)receivedWsRelayServersBean.remotePort, int.Parse(rtcChannel), receivedWsRelayServersBean.relayId, 1);

                }
                FancyJingMsgRegisterRecvCallback(RecvMessageFromCppAsync);
            }
            else
            {
                Callback = RecvCallback;
                FancyJingMsgInit(uuid, 0, 0);
                for (int i = 0; i < relayServers.Count; i++)
                {
                    UdpInitConfig receivedWsRelayServersBean = relayServers[i];

                    if (receivedWsRelayServersBean.remoteIP == "61.155.136.209")
                    {
                        FLog.Info("ReceivedWsRelayServersBean=====ip=" + receivedWsRelayServersBean.remoteIP + ",port=" + receivedWsRelayServersBean.remotePort + ",channel=" + int.Parse(rtcChannel));
                    }

                    FancyJingMsgAddConnAddr("0.0.0.0", receivedWsRelayServersBean.remoteIP, (short)receivedWsRelayServersBean.remotePort, int.Parse(rtcChannel), receivedWsRelayServersBean.relayId, 1);
                    FLog.Info("ReceivedWsRelayServersBean=====ip=" + receivedWsRelayServersBean.remoteIP + ",port=" + receivedWsRelayServersBean.remotePort + ",channel=" + int.Parse(rtcChannel));


                }

                FancyJingMsgRegisterRecvCallback(Callback);
            }
            StartAndroidRUDP();
        }

        enum RUDP_CB_TYPE
        {
            RUDP_CB_DATA = 1,
            RUDP_CB_AVAILABLE_BW,
            RUDP_CB_PACKET_DROPPED,
            RUDP_CB_LINK_OK,
            RUDP_CB_LINK_FAILURE,
            RUDP_CB_NET_REPORT = 7,
        }
    }



}
//#endif

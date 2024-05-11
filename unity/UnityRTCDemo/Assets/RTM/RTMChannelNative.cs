using System;
using System.Runtime.InteropServices;
namespace LJ.RTM
{


    internal class RTMChannelNative
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
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_create_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr NativeCreateRTMEngine(RUDPConfig config);

        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_destroy_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeDestroyRTMEengine(IntPtr engine);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_join_channel_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeJoinChannelEx(IntPtr engine, UInt64 uid, string channelId);

        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_join_channel_with_token_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeJoinChannelExWithToken(IntPtr engine, string token, UInt64 uid, string channelId);

        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_leave_channel_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeLeaveCannel(IntPtr engine);

        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_send_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeSend(IntPtr engine, byte[] data, int dataLen);

        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_register_msg_callback_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeRegisterExtMsgCallback(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] MessageCallbackEx callback, IntPtr context);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_register_event_callback_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeRegisterExtEventCallback(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] EventCallbackEx callback, IntPtr context);
    }
}

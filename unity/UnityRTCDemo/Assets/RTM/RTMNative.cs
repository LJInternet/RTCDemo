using System;
using System.Runtime.InteropServices;
namespace LJ.RTM
{
    internal class RTMNative
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
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_create", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr NativeCreateRTMEngine(RUDPConfig config);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_destroy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeDestroyRTMEengine(IntPtr engine);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_join_channel", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeJoinChannel(IntPtr engine, UInt64 uid, string channelId);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_leave_channel", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeLeaveCannel(IntPtr engine);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_send", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeSend(IntPtr engine, byte[] data, int dataLen);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_register_msg_callback", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeRegisterMsgCallback(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] MessageCallback callback, IntPtr context);
        [DllImport(RudpLibName, CharSet = CharSet.Ansi, EntryPoint = "rudp_engine_register_event_callback", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeRegisterEventCallback(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] EventCallback callback, IntPtr context);
    }
}
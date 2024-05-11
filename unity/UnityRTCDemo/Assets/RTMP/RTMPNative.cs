using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTMP
{
    internal class RTMPNative
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string libName = "mediatransfer";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string libName = "mediatransfer";
#elif UNITY_IOS
		private const string libName = "__Internal";
#else
        private const string libName = "mediatransfer";
#endif


        [DllImport(libName, CharSet = CharSet.Ansi, EntryPoint = "rtmp_engine_open", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeOpen(string url, int width, int height, int fps, int bitrate);

        [DllImport(libName, CharSet = CharSet.Ansi, EntryPoint = "rtmp_engine_write_raw_video", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeWriteVideo(byte[] buf, int size, byte[] msg, int msgSize, int pixel_fmt);

        [DllImport(libName, CharSet = CharSet.Ansi, EntryPoint = "rtmp_engine_write_raw_audio", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeWriteAudio(byte[] buf, int frame_num, int sampleRate, int channelCount, int bytePerSample);

        [DllImport(libName, CharSet = CharSet.Ansi, EntryPoint = "rtmp_engine_close", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void NativeClose();
        [DllImport(libName, CharSet = CharSet.Ansi, EntryPoint = "rtmp_engine_is_connected", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NativeRTMPIsConnected();
        
    }
}

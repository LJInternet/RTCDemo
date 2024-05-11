

using AOT;
using LJ.RTC.Common;
using LJ.RTC.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LJ.RTC
{
    internal class RtcEngineNavite
    {

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string RtcLibName = "mediatransfer";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private const string RtcLibName = "mediatransfer";
#elif UNITY_IPHONE
		private const string RtcLibName = "__Internal";
#else
        private const string RtcLibName = "mediatransfer";
#endif
        ///////////////////////////RTC EX START/////////////////////////////////////////

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void audio_callback_inner_ex(IntPtr buf, int size, int pts, int simple_rate, int channelCont, IntPtr channelId, int channelIdLen, UInt64 uid, IntPtr context);
        public delegate void audio_callback_ex(IntPtr buf, int size, int pts, int simple_rate, int channelCont, IntPtr channelId, int channelIdLen, long uid, System.Object context);

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void video_callback_inner_ex(IntPtr buf, Int32 len, Int32 width, Int32 height, int pixel_fmt, IntPtr channelId, int channelIdLen, UInt64 uid, UInt64 localUid, IntPtr context);
        public delegate void video_callback_ex(IntPtr buf, Int32 len, Int32 width, Int32 height, int pixel_fmt, IntPtr channelId, int channelIdLen, UInt64 uid, UInt64 localUid);

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void event_callback_inner_ex(int type, IntPtr buf, int len, IntPtr channelId, int channelIdLen, UInt64 localUid, IntPtr context);
        public delegate void event_callback_ex(int type, IntPtr buf, int len, IntPtr channelId, int channelIdLen, UInt64 localUid);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_video_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SubscribeVideoNativeEx(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] video_callback_inner_ex cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_audio_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SubscribeAudioNativeEx(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] audio_callback_inner_ex cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_register_event_ex_listener", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SubscribeEventNativeEx(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] event_callback_inner_ex cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_push_audio_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PushAudioCaptureFrameNativeEx(IntPtr engine, byte[] buffer, int frameNum, int sampleRate, int channelCount, int bytePerSample, string key);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_push_video_ex", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PushVideoFrameNativeEx(IntPtr engine, byte[] buf, int size, byte[] msg, int msgSize, int pixel_fmt, string key);

        ///////////////////////////RTC EX END/////////////////////////////////////////
        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate bool audio_callback_inner(IntPtr buf, int size, int pts, int simple_rate, int channelCont, IntPtr context);

        public delegate bool audio_callback(byte[] buf, out byte[] outBuffer, int pts, int simple_rate, int channelCont, System.Object context);

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public delegate void video_callback_inner(IntPtr buf, Int32 len, Int32 width, Int32 height, int pixel_fmt, IntPtr context);

        public delegate void video_callback(IntPtr buf, Int32 len, Int32 width, Int32 height, int pixel_fmt, System.Object context);

        [UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void event_callback_inner(int type, IntPtr buf, int size, IntPtr context);

        public delegate void event_callback(int type, byte[] buf, System.Object context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_set_debug_env", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SetDebugEnvNative(bool debug);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_create", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr CreateRtcEngineNative(byte[] msg, int msgSize);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_destroy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DestroyRtcEngineNative(IntPtr engine);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_push_video", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PushVideoCaptureFrameNative(IntPtr engine, byte[] buf, int size, byte[] msg, int msgSize, int pixel_fmt);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_push_audio", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PushAudioCaptureFrameNative(IntPtr engine, byte[] buf, int frame_num, int sampleRate, int channelCount, int bytePerSample);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_video", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeVideoNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] video_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_encoded_video", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeEncodedVideoNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] video_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_audio", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeAudioNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] audio_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_capture_audio", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeCaptureAudioNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] audio_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_mic_audio", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeMicAudioNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] audio_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_submix_audio", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeSubMixAudioNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] audio_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_unsubscribe_callback", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UnSubscribeCallbckNative(IntPtr engine, int type);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_send_event", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SendMediaEventNative(IntPtr engine, int eventType, byte[] mediaData, int len);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_get_event", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetMediaEventNative(IntPtr engine, int eventType, byte[] mediaData, int len, ref int retLen);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_register_event_listener", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterEventListenerNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] event_callback_inner cb, IntPtr context);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_camera_list", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetCameraListNative(byte[] out_buf, int len);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_start_camera_capture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartVideoCaptureNative(IntPtr engine, string deviceName, int width, int height, int fps);
        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_start_camera_capture_with_config", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartVideoCaptureWithConfigNative(IntPtr engine, string deviceName, byte[] config, int length);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_stop_camera_capture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopVideoCaptureNative(IntPtr engine);

        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_capture_video", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeCaptureVideoNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] video_callback_inner cb, IntPtr context);
        //////////////////////////////RTC SREEN CAPTURE START/////////////////////////////////////////
        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_subscribe_screen_capture_video", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SubscribeSreenCaptureVideoNative(IntPtr engine, [MarshalAs(UnmanagedType.FunctionPtr)] video_callback_inner cb, IntPtr context);
        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_start_screen_capture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartScreenCaptureNative(IntPtr engine, int enableAudio, int enableVideo, int width, int height, int fps, int bitrate);
        [DllImport(RtcLibName, CharSet = CharSet.Ansi, EntryPoint = "media_engine_stop_screen_capture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StopScreenCaptureNative(IntPtr engine);
        //////////////////////////////RTC SREEN CAPTURE END  /////////////////////////////////////////

        private IntPtr mIntPtr = IntPtr.Zero;

        private audio_callback audioCb = null;
        private System.Object audioCtx = null;

        private audio_callback captureAudioCb = null;
        private System.Object captureAudioCtx = null;
        private byte[] mCaptureDataCache = new byte[1];
        private AudioFrameOpType mCaptureFrameOpType = AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_ONLY;

        private audio_callback micAudioCb = null;
        private System.Object micAudioCtx = null;
        private byte[] mMicDataCache = new byte[1];
        private AudioFrameOpType mMicFrameOpType = AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_ONLY;

        private audio_callback submixAudioCb = null;
        private System.Object submixAudioCtx = null;
        private byte[] mSubmixDataCache = new byte[1];
        private AudioFrameOpType mSubmixFrameOpType = AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_ONLY;

        private video_callback videoCb = null;
        private System.Object videoCtx = null;

        private video_callback encodedVideoCb = null;
        private System.Object encodedVideoCtx = null;

        private video_callback captureVideoCb = null;
        private System.Object captureVideoCtx = null;

        private event_callback eventCb = null;
        private System.Object eventCtx = null;

        private event_callback_ex eventExCb = null;

        private video_callback screenCpatureVideoCb = null;
        private System.Object screenCpatureVideoCtx = null;

        private video_callback_ex decodeVideoCbEx = null;

        private RtcEngineConfig mRtcConfig;

        private static System.Collections.Concurrent.ConcurrentDictionary<IntPtr, RtcEngineNavite> dict = new System.Collections.Concurrent.ConcurrentDictionary<IntPtr, RtcEngineNavite>();

        public RtcEngineNavite(RtcEngineConfig config) {
            mRtcConfig = config;
        }

        public static void SetDebugEnv(bool debug) {
            SetDebugEnvNative(debug);
        }

        public void CreateRtcEngine() {
            byte[] msg = null;
            if (mRtcConfig != null) {
                msg = mRtcConfig.HPmarshall();
            }
            mIntPtr = CreateRtcEngineNative(msg, msg == null ? 0 : msg.Length);

            dict.TryAdd(mIntPtr, this);

        }

        public void DestroyRtcEngine()
        {
            if (mIntPtr != IntPtr.Zero)
            {
                DestroyRtcEngineNative(mIntPtr);
                RtcEngineNavite remove;
                dict.TryRemove(mIntPtr, out remove);
                mIntPtr = IntPtr.Zero;
            }
        }

        public static String[] GetCameraList()
        {
            byte[] buffer = new byte[1024];
            GetCameraListNative(buffer, 1024);

            string msg = System.Text.Encoding.UTF8.GetString(buffer);

            //JLog.Info("GetCameraList:" + msg);

            String[] devices = msg.Split(";");
            
            return devices;
        }

        public void StartVideoCapture(string deviceName, int width, int height, int fps)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                StartVideoCaptureNative(mIntPtr, deviceName, width, height, fps);
            }
        }

        public void StartVideoCaptureWithConfig(string deviceName, byte[] config)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                StartVideoCaptureWithConfigNative(mIntPtr, deviceName, config, config.Length);
            }
        }

        public void StopVideoCapture()
        {
            if (mIntPtr != IntPtr.Zero)
            {
                StopVideoCaptureNative(mIntPtr);
            }
        }

        public void StartVideoScreenCapture(ScreenCaptureParam param) {
            if (mIntPtr != IntPtr.Zero)
            {
                StartScreenCaptureNative(mIntPtr, param.enableAudio, param.enableVideo, param.videoWidth,
                    param.videoHeight, param.videoFps, param.videoBitrate);
            }   
        }

        public void StopScreenCapture()
        {
            if (mIntPtr != IntPtr.Zero)
            {
                StopScreenCaptureNative(mIntPtr);
            }
        }

        public void SubscribeCaptureVideo(video_callback cb, System.Object context)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.captureVideoCb = cb;
                this.captureVideoCtx = context;

                SubscribeCaptureVideoNative(mIntPtr, CaptureVideoCallbackFunc, mIntPtr);
            }
        }

        public int SendMediaEvent(int eventType, byte[] msg, UnityEngine.Object extraObject = null)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                return SendMediaEventNative(mIntPtr, eventType, msg, msg.Length);
            }
            return 0;
        }

        public byte[] GetMediaEvent(int eventType, byte[] msg)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                int resultLen = 0;
                IntPtr intPtr = GetMediaEventNative(mIntPtr, eventType, msg, msg == null ? 0 : msg.Length, ref resultLen);
                if (intPtr == IntPtr.Zero) {
                    return null;
                }
                string str = Marshal.PtrToStringAnsi(intPtr, resultLen);
                if (str == null || str.Length == 0) {
                    return null;
                }
                return System.Text.Encoding.UTF8.GetBytes(str);
            }
            return null;
        }

        public void PushVideoCaptureFrame(CaptureVideoFrame videoFrame)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                byte[] msg = videoFrame.HPmarshall();
                PushVideoCaptureFrameNative(mIntPtr, videoFrame.buffer, 
                    videoFrame.buffer == null ? 0 : videoFrame.buffer.Length, msg, msg.Length, GetPixelFormat(videoFrame));
            }
        }

        private int GetPixelFormat(CaptureVideoFrame videoFrame) {
            if (videoFrame.type == VIDEO_BUFFER_TYPE.VIDEO_BUFFER_TEXTURE) {
                return 1;
            }
            if (videoFrame.format == VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA) {
                return 1;
            }
            if (videoFrame.format == VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_NV21)
            {
                return 3;
            }
            if (videoFrame.format == VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_I420)
            {
                return 2;
            }
            if (videoFrame.format == VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_NV12)
            {
                return 5;
            }
            return 1;
        }

        //TODO
        public void PushAudioCaptureFrame(byte[] buffer, int sampleRate, int channelCount, int bytePerSample)
        {
            if (buffer == null || buffer.Length == 0) {
                return;
            }
            if (mIntPtr != IntPtr.Zero)
            {
                PushAudioCaptureFrameNative(mIntPtr, buffer, buffer.Length / (channelCount * bytePerSample),
                    sampleRate, channelCount, bytePerSample);
            }
        }

        public void PushAudioCaptureFrameEx(LJRtcConnection connection, byte[] buffer, int sampleRate, int channelCount, int bytePerSample)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return;
            }
            if (mIntPtr != IntPtr.Zero)
            {
                PushAudioCaptureFrameNativeEx(mIntPtr, buffer, buffer.Length / (channelCount * bytePerSample), sampleRate, channelCount, bytePerSample, connection.GetKey());
            }
        }

        public void PushVideoCaptureFrameEx(CaptureVideoFrame videoFrame, string key)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                byte[] msg = videoFrame.HPmarshall();
                PushVideoFrameNativeEx(mIntPtr, videoFrame.buffer,
                    videoFrame.buffer == null ? 0 : videoFrame.buffer.Length, msg, msg.Length, GetPixelFormat(videoFrame), key);
            }
        }

        

        public void SubscribeVideoNative(video_callback cb, System.Object context)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.videoCb = cb;
                this.videoCtx = context;

                SubscribeVideoNative(mIntPtr, VideoCallbackFunc, mIntPtr);
            }
        }

        public void SubscribeEncodeVideoNative(video_callback cb, System.Object context)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.encodedVideoCb = cb;
                this.encodedVideoCtx = context;

                SubscribeEncodedVideoNative(mIntPtr, EncodedVideoCallbackFunc, mIntPtr);
            }
        }

        public void SubscribeScreenCaptureVideoCallbck(video_callback cb, System.Object context)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.screenCpatureVideoCb = cb;
                this.screenCpatureVideoCtx = context;

                SubscribeSreenCaptureVideoNative(mIntPtr, SubscribeScreenCpatureVideoFunc, mIntPtr);
            }
        }

        public void SubscribeDecodeAudioCallback(audio_callback cb, System.Object context)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.audioCb = cb;
                this.audioCtx = context;

                SubscribeAudioNative(mIntPtr, AudioCallbackFunc, mIntPtr);
            }
        }

        public void SubscribeCaptureAudioCallback(audio_callback cb, System.Object context, AudioFrameOpType type)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.captureAudioCb = cb;
                this.captureAudioCtx = context;
                this.mCaptureFrameOpType = type;
                SubscribeCaptureAudioNative(mIntPtr, CaptureAudioCallbackFunc, mIntPtr);
            }
        }

        public void SubscribeMicAudioCallback(audio_callback cb, System.Object context, AudioFrameOpType type)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.micAudioCb = cb;
                this.micAudioCtx = context;
                this.mMicFrameOpType = type;
                SubscribeMicAudioNative(mIntPtr, MicAudioCallbackFunc, mIntPtr);
            }
        }

        public void SubscribeSubMixAudioCallback(audio_callback cb, System.Object context, AudioFrameOpType type)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.submixAudioCb = cb;
                this.submixAudioCtx = context;
                this.mSubmixFrameOpType = type;
                SubscribeSubMixAudioNative(mIntPtr, SubMixAudioCallbackFunc, mIntPtr);
            }
        }


        internal void UnSubscribeCallback(CallbackType type)
        {
            if (mIntPtr != IntPtr.Zero) {
                UnSubscribeCallbckNative(mIntPtr, (int)type);
            }
        }

        public void RegisterEventListener(event_callback cb, System.Object context)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.eventCb = cb;
                this.eventCtx = context;

                RegisterEventListenerNative(mIntPtr, EventCallbackFunc, mIntPtr);
            }
        }

        public void RegisterDecodeVideoEx(video_callback_ex cb)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.decodeVideoCbEx = cb;
                SubscribeVideoNativeEx(mIntPtr, DecodeVideoCallbackExFunc, mIntPtr);
            }
        }

        public void RegisterEventExListener(event_callback_ex cb)
        {
            if (mIntPtr != IntPtr.Zero)
            {
                this.eventExCb = cb;
                SubscribeEventNativeEx(mIntPtr, EventCallbackExFunc, mIntPtr);
            }
        }

        [MonoPInvokeCallback(typeof(video_callback_inner_ex))]
        public static void DecodeVideoCallbackExFunc(IntPtr buf, Int32 len, Int32 width,
            Int32 height, int pixel_fmt, IntPtr channelId, int channelIdLen, UInt64 uid, UInt64 localUid, IntPtr context)
        {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                rtc.decodeVideoCbEx(buf, len, width, height, pixel_fmt, channelId, channelIdLen, uid, localUid);
            }
        }

        [MonoPInvokeCallback(typeof(event_callback_inner_ex))]
        public static void EventCallbackExFunc(int type, IntPtr buf, int len, IntPtr channelId, int channelIdLen, UInt64 localUid, IntPtr context)
        {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                rtc.eventExCb(type, buf, len, channelId, channelIdLen, localUid);
            }
        }

        [MonoPInvokeCallback(typeof(event_callback_inner))]
        public static void EventCallbackFunc(int type, IntPtr buf, int size, IntPtr context)
        {
            //JLog.Info("event callback:" + type);

            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                byte[] data = new byte[size];
                Marshal.Copy(buf, data, 0, size);
                rtc.eventCb(type, data, rtc.eventCtx);
            }
        }

        [MonoPInvokeCallback(typeof(audio_callback_inner))]
        public static bool AudioCallbackFunc(IntPtr buf, int size, int pts, int simple_rate, int channelCount, IntPtr context)
        {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                byte[] data = new byte[size];
                Marshal.Copy(buf, data, 0, size);
                byte[] outBuffer;
                rtc.audioCb(data, out outBuffer, pts, simple_rate, channelCount, rtc.audioCtx);
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(audio_callback_inner))]
        public static bool CaptureAudioCallbackFunc(IntPtr buf, int size, int pts, int simple_rate, int channelCount, IntPtr context)
        {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                if (rtc.mCaptureDataCache.Length != size) {
                    rtc.mCaptureDataCache = new byte[size];
                }
                
                Marshal.Copy(buf, rtc.mCaptureDataCache, 0, size);
                byte[] outBuffer;
                bool result = rtc.captureAudioCb(rtc.mCaptureDataCache, out outBuffer, pts, simple_rate, channelCount, rtc.captureAudioCtx);
                // copy the result data to the buffer
                if (outBuffer != null && rtc.mCaptureFrameOpType
                    == AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE) {
                    Marshal.Copy(outBuffer, 0, buf, size);
                }
                return result;
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(audio_callback_inner))]
        private static bool MicAudioCallbackFunc(IntPtr buf, int size, int pts, int simple_rate, int channelCount, IntPtr context)
        {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                if (rtc.mMicDataCache.Length != size)
                {
                    rtc.mMicDataCache = new byte[size];
                }

                Marshal.Copy(buf, rtc.mMicDataCache, 0, size);
                byte[] outBuffer;
                bool result = rtc.micAudioCb(rtc.mMicDataCache, out outBuffer, pts, simple_rate, channelCount, rtc.micAudioCtx);
                // copy the result data to the buffer
                if (outBuffer != null && rtc.mMicFrameOpType
                    == AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE)
                {
                    Marshal.Copy(outBuffer, 0, buf, size);
                }
                return result;
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(audio_callback_inner))]
        private static bool SubMixAudioCallbackFunc(IntPtr buf, int size, int pts, int simple_rate, int channelCount, IntPtr context)
        {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                if (rtc.mSubmixDataCache.Length != size)
                {
                    rtc.mSubmixDataCache = new byte[size];
                }

                Marshal.Copy(buf, rtc.mSubmixDataCache, 0, size);
                byte[] outBuffer;
                bool result = rtc.submixAudioCb(rtc.mSubmixDataCache, out outBuffer, pts, simple_rate, channelCount, rtc.submixAudioCtx);
                // copy the result data to the buffer
                if (outBuffer != null && rtc.mSubmixFrameOpType
                    == AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE)
                {
                    Marshal.Copy(outBuffer, 0, buf, size);
                }
                return result;
            }
            return false;
        }

        [MonoPInvokeCallback(typeof(video_callback_inner))]
        public static void VideoCallbackFunc(IntPtr buf, Int32 size, Int32 width, Int32 height, int pixel_fmt, IntPtr context)
        {
            //JLog.Info("video callback:");

            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                //byte[] data = new byte[size];
                //Marshal.Copy(buf, data, 0, size);
                rtc.videoCb(buf, size, width, height, pixel_fmt, rtc.videoCtx);
            }
        }

        [MonoPInvokeCallback(typeof(video_callback_inner))]
        public static void EncodedVideoCallbackFunc(IntPtr buf, Int32 size, Int32 width, Int32 height, int pixel_fmt, IntPtr context)
        {
            //JLog.Info("video callback:");

            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                //byte[] data = new byte[size];
                //Marshal.Copy(buf, data, 0, size);
                rtc.encodedVideoCb(buf, size, width, height, pixel_fmt, rtc.encodedVideoCtx);
            }
        }

        [MonoPInvokeCallback(typeof(video_callback_inner))]
        public static void SubscribeScreenCpatureVideoFunc(IntPtr buf, Int32 size, Int32 width, Int32 height, int pixel_fmt, IntPtr context) {
            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                //byte[] data = new byte[size];
                //Marshal.Copy(buf, data, 0, size);
                rtc.screenCpatureVideoCb(buf, size, width, height, pixel_fmt, rtc.screenCpatureVideoCtx);
            }
        }

        [MonoPInvokeCallback(typeof(video_callback_inner))]
        public static void CaptureVideoCallbackFunc(IntPtr buf, Int32 size, Int32 width, Int32 height, int pixel_fmt, IntPtr context)
        {
            //JLog.Info("capture video callback:");

            RtcEngineNavite rtc;
            if (dict.TryGetValue(context, out rtc))
            {
                //byte[] data = new byte[size];
                //Marshal.Copy(buf, data, 0, size);
                rtc.captureVideoCb(buf, size, width, height, pixel_fmt, rtc.captureVideoCtx);
            }
        }

        private void test_event_callbakc(int type, byte[] buf, System.Object context)
        {
            JLog.Info("test_event_callbakc:");
        }
    }
}

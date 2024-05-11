using System;
using System.Collections.Generic;
using System.IO;
using LJ.RTC.Audio;
using LJ.RTC.Common;
using LJ.RTC.Video;

namespace LJ.RTC
{
    public class RtcEngineConfig : HPMarshaller
    {

        public long mAppId = -1;
        public LogConfig mLogConfig = new LogConfig();

        public string mAppUa;
        public ILog mJLog;
        public IReprot mReport;
        public bool isTestEv;
        public bool enableNativeLog = true;

        public override byte[] marshall()
        {
            pushBool(enableNativeLog);
            return base.marshall();
        }
    };

    public class LogConfig
    {
        public string filePath = null;
        public int fileSize = -1;
        public int level;
    };

    public enum LogLevel
    {

        LOG_LEVEL_NONE = 0,
        LOG_LEVEL_INFO = 1,
        LOG_LEVEL_WARN = 2,
        LOG_LEVEL_ERROR = 3,
        LOG_LEVEL_FATAL = 6,

    };

    public class ChannelConfig
    {
        public List<UdpInitConfig> configs = new List<UdpInitConfig>();
        public String p2pSignalServer = "61.155.136.209:9988";

        public int enableAudio = 1;
        public int enableVideo = 1;

        public Int64 appID;
        public Int64 userID;
        public string channelID;
        public string token;
    }

    public class RTCStatus
    {
        public bool isRUDPConnected = false;

    }

    public class UdpInitConfig : HPMarshaller
    {
        public long relayId = 12222;
        public int netType = 1;
        public string remoteIP = "61.155.136.209";
        public int remotePort = 30001;
        public int remoteSessionId = 10083;


        public override byte[] marshall()
        {
            pushInt64(relayId);
            pushString16(remoteIP);
            pushInt(remotePort);
            pushInt(remoteSessionId);
            pushInt(netType);
            return base.marshall();
        }

        public override void marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.marshall(stream, writer, reader);
            pushInt64(relayId);
            pushString16(remoteIP);
            pushInt(remotePort);
            pushInt(remoteSessionId);
            pushInt(netType);
        }

        public override void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.unmarshall(stream, writer, reader);
            relayId = popInt64();
            remoteIP = popString16();
            remotePort = popInt();
            remoteSessionId = popInt();
            netType = popInt();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            relayId = popInt64();
            remoteIP = popString16();
            remotePort = popInt();
            remoteSessionId = popInt();
            netType = popInt();
        }
    }

    public enum LinkStatus {
        CONNECTED = 1,
        DISCONNECTED = 2,
        LOST = 3,
        CLOSE = 4,
    }

    public abstract class IRtcEngineEventHandler
    {
        public delegate void OnCameraParam(int width, int height, int facing, int rotation, int fps);
        // 网络质量回调，localQuality是本地的网络质量，remoteQuality是对端的网络质量
        /**
        * @brief 网络质量级别枚举。
        */
        enum NetQualityLevel
        {
            QUALITY_GOOD = 1, /**< 网络质量好 */
            QUALITY_COMMON = 2, /**< 网络质量一般 */
            QUALITY_BAD = 3, /**< 勉强能沟通 */
            QUALITY_VBAD = 4, /**< 网络质量非常差，基本不能沟通。 */
            QUALITY_BLOCK = 5, /**< 链路不通 */
        };
        public virtual void onNetworkQuality(int uid, int localQuality, int remoteQuality)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"> 枚举值如下</param>
        /// #define API_STATUS_CONNECTED 	1
        /// #define API_STATUS_DISCONNECTED 2
        /// #define API_STATUS_LOST 		3
        /// #define API_STATUS_CLOSE		4 
        public virtual void onLinkStatus(int status)
        {

        }

        /// <summary>
        /// 采集音量提示回调
        /// </summary>
        /// <param name="info"></param>
        public virtual void onAudioVolumeIndication(AudioVolumeEvent info)
        {

        }

        public virtual void onJoinChannelSuccess(string channelId, long uid, string msg)
        {

        }

        public virtual void onJoinChannelFail(string channelId, long uid, string msg)
        {

        }

        public virtual void onLeavehannelSuccess(string channelId, long uid, string msg)
        {

        }

        public virtual void onLeaveChannelFail(string channelId, long uid, string msg)
        {
        }

        /// <summary>
        /// 相机操作结果回调
        /// </summary>
        /// <param name="action">CAMERA_ACTION</param>
        /// <param name="result">CAMERA_CAPTURE_ERROR</param>
        /// <param name="msg">result 是CAMERA_BUSY时是占用进程名</param>
        public virtual void OnCameraActionResult(int action, int result, string msg) { 
        
        }
    };


    public delegate void OnCaptureVideoFrame(CaptureVideoFrame captrueVideo);

    public delegate void OnCaptureVideoFrameInternel(CaptureVideoFrame captrueVideo, bool push);
    public delegate void OnDecodedVideoFrame(VideoFrame videoFrame);

    public delegate void OnEncodedVideoFrame(VideoFrame videoFrame);

    public delegate void OnDecodeVideoInternel(IntPtr buf, Int32 len, Int32 width, Int32 height,
        int pixel_fmt, IntPtr channelId, int channelIdLen, UInt64 uid, UInt64 localUid);

    public delegate void OnEventExCallbackInternel(int type, IntPtr buf, int len, IntPtr channelId, int channelIdLen, UInt64 localUid);
    /// <summary>
    /// 当开了内录后，回调的是麦克风和内录的混音
    /// </summary>
    /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
    /// <returns>true 拦截当前采集流程，不进行编码推流，需要额外调用PushAudioFrame进行编码推流， false 不拦截当前流程</returns>
    public delegate bool OnCaptureAudioFrame(AudioFrame audioFrame);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
    /// <returns>true 当前麦克风声音将被拦截，麦克风声音将静音，false 不拦截当前流程</returns>
    public delegate bool OnCaptureMicAudioFrame(AudioFrame audioFrame);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
    /// <returns>true 拦截当前内录流程，内录音频将不会被编码推流到远端， false 不拦截当前流程</returns>
    public delegate bool OnCaptureSubMixAudioFrame(AudioFrame audioFrame);

    public delegate void OnDecodedAudioFrame(AudioFrame audioFrame);
};

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LJ.RTMP
{
    class MediaInfo {
        public int widthAlignment = 16;
        public int heightAlignment =16;
        public int widthRangeLower =96;
        public int widthRangeUpper = 999999;
        public int heightRangeLower = 96;
        public int heightRangeUpper = 999999;
    }

    public enum RTMPStatus {
        NONE,
        INITIALIZED, // 初始化成功
        CLOSED, // 已关闭
        CONNECT_LOST, // 连接断开
        INITIAL_FAILED // 初始化失败
    }

    public enum RTMPResult
    {
        RTMP_SUCCESS = 0,
        RTMP_READ_DONE = -1,
        RTMP_ERROR_OPEN_ALLOC = -2,
        RTMP_ERROR_OPEN_CONNECT_STREAM = -3,
        RTMP_ERROR_UNKNOWN_RTMP_OPTION = -4,
        RTMP_ERROR_UNKNOWN_RTMP_AMF_TYPE = -5,
        RTMP_ERROR_DNS_NOT_REACHABLE = -6,
        RTMP_ERROR_SOCKET_CONNECT_FAIL = -7,
        RTMP_ERROR_SOCKS_NEGOTIATION_FAIL = -8,
        RTMP_ERROR_SOCKET_CREATE_FAIL = -9,
        RTMP_ERROR_NO_SSL_TLS_SUPP = -10,
        RTMP_ERROR_HANDSHAKE_CONNECT_FAIL = -11,
        RTMP_ERROR_HANDSHAKE_FAIL = -12,
        RTMP_ERROR_CONNECT_FAIL = -13,
        RTMP_ERROR_CONNECTION_LOST = -14,
        RTMP_ERROR_KEYFRAME_TS_MISMATCH = -15,
        RTMP_ERROR_READ_CORRUPT_STREAM = -16,
        RTMP_ERROR_MEM_ALLOC_FAIL = -17,
        RTMP_ERROR_STREAM_BAD_DATASIZE = -18,
        RTMP_ERROR_PACKET_TOO_SMALL = -19,
        RTMP_ERROR_SEND_PACKET_FAIL = -20,
        RTMP_ERROR_AMF_ENCODE_FAIL = -21,
        RTMP_ERROR_URL_MISSING_PROTOCOL = -22,
        RTMP_ERROR_URL_MISSING_HOSTNAME = -23,
        RTMP_ERROR_URL_INCORRECT_PORT = -24,
        RTMP_ERROR_IGNORED = -25,
        RTMP_ERROR_GENERIC = -26,
        RTMP_ERROR_SANITY_FAIL = -27,
        RTMP_ERROR_NOT_INITIAL = -28,
    };

    public delegate void OnRtmpStatusCallback(RTMPStatus status);
    public class RTMPEngine
    {
        private static RTMPEngine engine = new RTMPEngine();
        private MediaInfo _MediaInfo;
        private RTMPStatus _status = RTMPStatus.NONE;
        private OnRtmpStatusCallback _callback;

        public static RTMPEngine getInstance() {
            return engine;
        }

        public void SetStatusCallback(OnRtmpStatusCallback callback) {
            _callback = callback;
        }

        public bool isSupportWH(int width, int height) {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_MediaInfo == null) {
                string MiediaInfo = getMediaInfo();
                if (MiediaInfo == null) {
                    Debug.LogError("can not get media codec info");
                    return true;
                }
                _MediaInfo = JsonConvert.DeserializeObject<MediaInfo>(MiediaInfo);
                if (_MediaInfo == null) {
                    Debug.LogError("DeserializeObjectmedia codec info error");
                    return true;
                }
            }
            return width % _MediaInfo.widthAlignment == 0 && height % _MediaInfo.heightAlignment == 0
                && width > _MediaInfo.widthRangeLower && width < _MediaInfo.widthRangeUpper
                && height > _MediaInfo.heightRangeLower && height < _MediaInfo.heightRangeUpper;
#endif
            return true;
        }

        private string getMediaInfo() {

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.linjing.sdk.encode.UnityMediaInfoHelper");
                string mediaInfo = androidJavaClass.CallStatic<string>("getH264MediaInfo");
                Debug.Log("MediaCodecInfo " + mediaInfo);
                return mediaInfo;
            }
            catch (Exception e) {
                Debug.LogError("MediaCodecInfo " + e.ToString());
            }
#endif
            return null;
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="url">rtmp url</param>
        /// <param name="width">rtmp 推流的宽</param>
        /// <param name="height">rtmp 推流的高</param>
        /// <param name="fps">rtmp 推流的帧率</param>
        /// <param name="bitrate">rtmp 推流的码率</param>
        /// <returns></returns>
        public int open(string url, int width, int height, int fps, int bitrate) {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isSupportWH(width, height)) {
                Debug.LogError("width and height must be 2 * N width : " + width + ":height:" + height);
                return -1;
            }
#endif
            int ret = RTMPNative.NativeOpen(url, width, height, fps, bitrate);
            if (ret > 0) {
                _status = RTMPStatus.INITIALIZED;
                CallbackRtmpStatus(_status);
            }
            return ret;
        }

        private void CallbackRtmpStatus(RTMPStatus status) {
            if (_callback != null) {
                _callback(status);
            }
        }

        /// <summary>
        /// 发送视频数据
        /// </summary>
        /// <param name="buf">除android平台使用纹理编码外，其他平台这个值 不能为null，而且是有效的视频数据</param>
        /// <param name="size">视频数据长度</param>
        /// <param name="msg">CaptureVideoFrame序列化后的数据</param>
        /// <param name="msgSize">CaptureVideoFrame序列化后的数据长度</param>
        /// <param name="pixel_fmt">视频数据的格式</param>
        /// <returns></returns>
        public int WriteVideo(byte[] buf, int size, byte[] msg, int msgSize, int pixel_fmt) {
            if (_status == RTMPStatus.CLOSED)
            {
                return (int)RTMPResult.RTMP_ERROR_IGNORED;
            }
            if (_status == RTMPStatus.CONNECT_LOST) {
                return (int)RTMPResult.RTMP_ERROR_CONNECTION_LOST;
            }
            if (_status != RTMPStatus.INITIALIZED) {
                return (int)RTMPResult.RTMP_ERROR_NOT_INITIAL;
            }
            int ret = RTMPNative.NativeWriteVideo(buf, size, msg, msgSize, pixel_fmt);
            if ((int)RTMPResult.RTMP_ERROR_CONNECTION_LOST == ret && RTMPStatus.CONNECT_LOST != _status) {
                Debug.Log("WriteVideo ret " + ret);
                _status = RTMPStatus.CONNECT_LOST;
                CallbackRtmpStatus(_status);
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf">PCM 数据</param>
        /// <param name="frame_num">PCM数据样本数，一般是buf.Length / (bytePerSample * channelCount) </param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="bytePerSample">每个音频数据暂多少个字节，例如：int16 2 int8 1 int32 4</param>
        /// <returns></returns>
        public int WriteAudio(byte[] buf, int frame_num, int sampleRate, int channelCount, int bytePerSample) {
            if (_status == RTMPStatus.CLOSED)
            {
                return (int)RTMPResult.RTMP_ERROR_IGNORED;
            }
            if (_status == RTMPStatus.CONNECT_LOST)
            {
                return (int)RTMPResult.RTMP_ERROR_CONNECTION_LOST;
            }
            if (_status != RTMPStatus.INITIALIZED)
            {
                return (int)RTMPResult.RTMP_ERROR_NOT_INITIAL;
            }
            int ret = RTMPNative.NativeWriteAudio(buf, frame_num, sampleRate, channelCount, bytePerSample);
            if ((int)RTMPResult.RTMP_ERROR_CONNECTION_LOST == ret && RTMPStatus.CONNECT_LOST != _status)
            {
                Debug.Log("WriteAudio ret " + ret);
                _status = RTMPStatus.CONNECT_LOST;
                CallbackRtmpStatus(_status);
            }
            return ret;
        }

        public void close() {
            RTMPNative.NativeClose();
            _status = RTMPStatus.CLOSED;
            CallbackRtmpStatus(_status);
        }

        public bool IsConnected() {
            return RTMPNative.NativeRTMPIsConnected() != 0;
        }
    }
}

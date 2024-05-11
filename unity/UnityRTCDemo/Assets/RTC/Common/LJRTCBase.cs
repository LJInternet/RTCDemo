using System;
using UnityEngine;
using lj_view_t = System.UInt64;
using lj_video_track_id_t = System.UInt32;
using System.IO;

namespace LJ.RTC.Common
{
    ///
    /// <summary>
    /// Compression preference for video encoding.
    /// </summary>
    ///
    public enum COMPRESSION_PREFERENCE
    {
        ///
        /// <summary>
        /// 0: Low latency preference. The SDK compresses video frames to reduce latency. This preference is suitable for scenarios where smoothness is prioritized and reduced video quality is acceptable.
        /// </summary>
        ///
        PREFER_LOW_LATENCY = 0,

        ///
        /// <summary>
        /// 1: (Default) High quality preference. The SDK compresses video frames while maintaining video quality. This preference is suitable for scenarios where video quality is prioritized.
        /// </summary>
        ///
        PREFER_QUALITY = 1
    };

    public enum ENCODING_PREFERENCE
    {
        ///
        /// <summary>
        /// -1: Adaptive preference. The SDK automatically selects the optimal encoding type for encoding based on factors such as platform and device type.
        /// </summary>
        ///
        PREFER_AUTO = -1,

        ///
        /// <summary>
        /// 0: Software coding preference. The SDK prefers software encoders for video encoding.
        /// </summary>
        ///
        PREFER_SOFTWARE = 0,

        ///
        /// <summary>
        /// 1: Hardware encoding preference. The SDK prefers a hardware encoder for video encoding. When the device does not support hardware encoding, the SDK automatically uses software encoding and reports the currently used video encoder type through hwEncoderAccelerating in the OnLocalVideoStats callback.
        /// </summary>
        ///
        PREFER_HARDWARE = 1,
    };

    public class AdvanceOptions
    {
        ///
        /// <summary>
        /// Video encoder preference. See ENCODING_PREFERENCE .
        /// </summary>
        ///
        public ENCODING_PREFERENCE encodingPreference;
        ///
        /// <summary>
        /// Compression preference for video encoding. See COMPRESSION_PREFERENCE .
        /// </summary>
        ///
        public COMPRESSION_PREFERENCE compressionPreference;

        public AdvanceOptions()
        {
            encodingPreference = ENCODING_PREFERENCE.PREFER_AUTO;
            compressionPreference = COMPRESSION_PREFERENCE.PREFER_LOW_LATENCY;
        }

        public AdvanceOptions(ENCODING_PREFERENCE encoding_preference, COMPRESSION_PREFERENCE compression_preference)
        {
            encodingPreference = encoding_preference;
            compressionPreference = compression_preference;
        }
    }

    /// <summary>
    /// Video mirror mode.
    /// </summary>
    ///
    public enum VIDEO_MIRROR_MODE_TYPE
    {
        ///
        /// <summary>
        /// 0: (Default) The SDK determines the mirror mode.
        /// </summary>
        ///
        VIDEO_MIRROR_MODE_AUTO = 0,

        ///
        /// <summary>
        /// 1: Enable mirror mode.
        /// </summary>
        ///
        VIDEO_MIRROR_MODE_ENABLED = 1,

        ///
        /// <summary>
        /// 2: Disable mirror mode.
        /// </summary>
        ///
        VIDEO_MIRROR_MODE_DISABLED = 2,
    };

    public enum DEGRADATION_PREFERENCE
    {
        ///
        /// <summary>
        /// 0: (Default) Prefers to reduce the video frame rate while maintaining video resolution during video encoding under limited bandwidth. This degradation preference is suitable for scenarios where video quality is prioritized.
        /// </summary>
        ///
        MAINTAIN_QUALITY = 0,

        ///
        /// <summary>
        /// 1: Reduces the video resolution while maintaining the video frame rate during video encoding under limited bandwidth. This degradation preference is suitable for scenarios where smoothness is prioritized and video quality is allowed to be reduced.
        /// </summary>
        ///
        MAINTAIN_FRAMERATE = 1,

        ///
        /// <summary>
        /// 2: Reduces the video frame rate and video resolution simultaneously during video encoding under limited bandwidth. The MAINTAIN_BALANCED has a lower reduction than MAINTAIN_QUALITY and MAINTAIN_FRAMERATE, and this preference is suitable for scenarios where both smoothness and video quality are a priority.The resolution of the video sent may change, so remote users need to handle this issue. See OnVideoSizeChanged .
        /// </summary>
        ///
        MAINTAIN_BALANCED = 2,

        ///
        /// <summary>
        /// 3: Reduces the video frame rate while maintaining the video resolution during video encoding under limited bandwidth. This degradation preference is suitable for scenarios where video quality is prioritized.
        /// </summary>
        ///
        MAINTAIN_RESOLUTION = 3,

        ///
        /// @ignore
        ///
        DISABLED = 100,
    };

    public class VideoDimensions
    {
        public VideoDimensions()
        {
            width = 640;
            height = 480;
            encodeWidth = width;
            encodeHeight = height;
        }

        public VideoDimensions(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.encodeWidth = width;
            this.encodeHeight = height;
        }
        public VideoDimensions(int width, int height, int mVideoBitrate, int mMinVideoBitrate, int mVideoFrameRate)
        {
            this.width = width;
            this.height = height;
            this.mVideoBitrate = mVideoBitrate;
            this.mMinVideoBitrate = mMinVideoBitrate;
            this.mVideoFrameRate = mVideoFrameRate;
        }
        public VideoDimensions(int width, int height, int encodeWidth, int encodeHeight)
        {
            this.width = width;
            this.height = height;
            this.encodeWidth = encodeWidth;
            this.encodeHeight = encodeHeight;
        }
        ///
        /// <summary>
        /// The width (pixels) of the video.
        /// </summary>
        ///
        private int width;

        ///
        /// <summary>
        /// The height (pixels) of the video.
        /// </summary>
        ///
        private int height;

        ///
        /// <summary>
        /// The width (pixels) of the video.
        /// </summary>
        ///
        private int encodeWidth;

        ///
        /// <summary>
        /// The height (pixels) of the video.
        /// </summary>
        ///
        private int encodeHeight;

        public int mVideoBitrate = -1;
        public int mMinVideoBitrate = -1;
        public int mVideoFrameRate = -1;


        public int GetWidth() {
            return width;
        }
        public int GetHeight() {
            return height;
        }

        public int GetEncodeWidth() {
            return encodeWidth;
        }

        public int getEncodeHeight() {
            return encodeHeight;
        }

        public int GetMinBitRate()
        {
            if (mMinVideoBitrate != -1)
            {
                return mMinVideoBitrate;
            }
            int minWidth = Math.Min(encodeWidth, encodeHeight);
            if (minWidth == 120 || minWidth == 160 || minWidth == 180)
            {
                return 200;
            }
            if (minWidth == 240 || minWidth == 320 || minWidth == 360)
            {
                return 300;
            }
            if (minWidth == 480)
            {
                return 400;
            }
            if (minWidth == 540 || minWidth == 544)
            {
                return 650;
            }
            if (minWidth == 720)
            {
                return 850;
            }
            if (minWidth > 720)
            {
                return 2000;
            }
            return 450;
        }

        public int GetBitRate()
        {
            if (mVideoBitrate != -1)
            {
                return mVideoBitrate;
            }
            int minWidth = Math.Min(encodeWidth, encodeHeight);
            if (minWidth == 120 || minWidth == 160 || minWidth == 180)
            {
                return 300;
            }
            if (minWidth == 240 || minWidth == 320)
            {
                return 400;
            }
            if (minWidth == 360)
            {
                return 500;
            }
            if (minWidth == 480)
            {
                return 800;
            }
            if (minWidth == 540 || minWidth == 544)
            {
                return 1200;
            }
            if (minWidth == 720)
            {
                return 1800;
            }
            if (minWidth > 720)
            {
                return 2000;
            }
            return 1000;
        }
    }

    public enum VIDEO_CODEC_TYPE
    {
        ///
        /// @ignore
        ///
        VIDEO_CODEC_NONE = -1,
        ///
        /// <summary>
        /// 2: Standard H.264.
        /// </summary>
        ///
        VIDEO_CODEC_H264 = 0,

        ///
        /// <summary>
        /// 3: Standard H.265.
        /// </summary>
        ///
        VIDEO_CODEC_H265 = 1,
        ///
        /// <summary>
        /// 1: Standard VP8.
        /// </summary>
        ///
        VIDEO_CODEC_VP8 = 2,
        ///
        /// @ignore
        ///
        VIDEO_CODEC_VP9 = 5,

        ///
        /// <summary>
        /// 6: Generic.This type is used for transmitting raw video data, such as encrypted video frames. The SDK returns this type of video frames in callbacks, and you need to decode and render the frames yourself.
        /// </summary>
        ///
        VIDEO_CODEC_GENERIC = 6,

        ///
        /// @ignore
        ///
        VIDEO_CODEC_GENERIC_H264 = 7,

        ///
        /// @ignore
        ///
        VIDEO_CODEC_AV1 = 12,

        ///
        /// <summary>
        /// 20: Generic JPEG.This type consumes minimum computing resources and applies to IoT devices.
        /// </summary>
        ///
        VIDEO_CODEC_GENERIC_JPEG = 20,


        VIDEO_CODEC_H264_SOFT = 21,
    };

    ///
    /// <summary>
    /// Video output orientation mode.
    /// </summary>
    ///
    public enum ORIENTATION_MODE
    {
        ///
        /// <summary>
        /// 0: (Default) The output video always follows the orientation of the captured video. The receiver takes the rotational information passed on from the video encoder. This mode applies to scenarios where video orientation can be adjusted on the receiver.If the captured video is in landscape mode, the output video is in landscape mode.If the captured video is in portrait mode, the output video is in portrait mode.
        /// </summary>
        ///
        ORIENTATION_MODE_ADAPTIVE = 0,

        ///
        /// <summary>
        /// 1: In this mode, the SDK always outputs videos in landscape (horizontal) mode. If the captured video is in portrait mode, the video encoder crops it to fit the output. Applies to situations where the receiving end cannot process the rotational information. For example, CDN live streaming.
        /// </summary>
        ///
        ORIENTATION_MODE_FIXED_LANDSCAPE = 1,

        ///
        /// <summary>
        /// 2: In this mode, the SDK always outputs video in portrait (portrait) mode. If the captured video is in landscape mode, the video encoder crops it to fit the output. Applies to situations where the receiving end cannot process the rotational information. For example, CDN live streaming.
        /// </summary>
        ///
        ORIENTATION_MODE_FIXED_PORTRAIT = 2,
    };

    ///
    /// <summary>
    /// The encoding bitrate of the video.
    /// </summary>
    ///
    public enum BITRATE
    {
        ///
        /// <summary>
        /// (Recommended) Standard bitrate mode. In this mode, the video bitrate is twice the base bitrate.
        /// </summary>
        ///
        STANDARD_BITRATE = 0,

        ///
        /// <summary>
        /// Adaptive bitrate mode In this mode, the video bitrate is the same as the base bitrate. If you choose this mode in the interactive streaming profile, the video frame rate may be lower than the set value.
        /// </summary>
        ///
        COMPATIBLE_BITRATE = -1,

        ///
        /// @ignore
        ///
        DEFAULT_MIN_BITRATE = -1,

        ///
        /// @ignore
        ///
        DEFAULT_MIN_BITRATE_EQUAL_TO_TARGET_BITRATE = -2,
    };

    ///
    /// @ignore
    ///
    public enum FRAME_WIDTH
    {
        ///
        /// @ignore
        ///
        FRAME_WIDTH_640 = 640,
    };

    ///
    /// @ignore
    ///
    public enum FRAME_HEIGHT
    {
        ///
        /// @ignore
        ///
        FRAME_HEIGHT_360 = 360,
    };

    public enum FILL_MODE {
        COR_CENTER = 0, // 居中裁剪
        FIT_XY = 1 //按宽高视频
    }; 


    public class VideoEncoderConfiguration
    {
        ///
        /// <summary>
        /// The codec type of the local video stream. See VIDEO_CODEC_TYPE .
        /// </summary>
        ///
        public VIDEO_CODEC_TYPE codecType;

        ///
        /// <summary>
        /// The dimensions of the encoded video (px). See VideoDimensions . This parameter measures the video encoding quality in the format of length �� width. The default value is 640 �� 360. You can set a custom value.
        /// </summary>
        ///
        public VideoDimensions dimensions;

        ///
        /// <summary>
        /// The frame rate (fps) of the encoding video frame. The default value is 15. See FRAME_RATE .
        /// </summary>
        ///
        public int frameRate;

        ///
        /// <summary>
        /// The encoding bitrate (Kbps) of the video. See BITRATE .
        /// </summary>
        ///
        public int bitrate;

        ///
        /// <summary>
        /// The minimum encoding bitrate (Kbps) of the video.The SDK automatically adjusts the encoding bitrate to adapt to the network conditions. Using a value greater than the default value forces the video encoder to output high-quality images but may cause more packet loss and sacrifice the smoothness of the video transmission. Unless you have special requirements for image quality, Agora does not recommend changing this value.This parameter only applies to the interactive streaming profile.
        /// </summary>
        ///
        public int minBitrate;

        ///
        /// <summary>
        /// The orientation mode of the encoded video. See ORIENTATION_MODE .
        /// </summary>
        ///
        public ORIENTATION_MODE orientationMode;

        ///
        /// <summary>
        /// Video degradation preference under limited bandwidth. See DEGRADATION_PREFERENCE .
        /// </summary>
        ///
        public DEGRADATION_PREFERENCE degradationPreference;

        ///
        /// <summary>
        /// Sets the mirror mode of the published local video stream. It only affects the video that the remote user sees. See VIDEO_MIRROR_MODE_TYPE .By default, the video is not mirrored.
        /// </summary>
        ///
        public VIDEO_MIRROR_MODE_TYPE mirrorMode;


        ///
        /// <summary>
        /// Advanced options for video encoding. See AdvanceOptions .
        /// </summary>
        ///
        public AdvanceOptions advanceOptions;

        public FILL_MODE fillMode = FILL_MODE.COR_CENTER;

        public int mScreenOrientation = (int)ScreenOrientation.Portrait;

        public int keyFrameInterval = 3;

        public int bitrateMode = 2;

        public VideoEncoderConfiguration(ref VideoDimensions d, int f, int b, ORIENTATION_MODE m, VIDEO_MIRROR_MODE_TYPE mirror = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED)
        {
            codecType = VIDEO_CODEC_TYPE.VIDEO_CODEC_H264;
            dimensions = d;
            frameRate = f;
            bitrate = b;
            dimensions.mVideoBitrate = b;
            minBitrate = (int)BITRATE.DEFAULT_MIN_BITRATE;
            orientationMode = m;
            degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY;
            mirrorMode = mirror;
            advanceOptions = new AdvanceOptions(ENCODING_PREFERENCE.PREFER_AUTO, COMPRESSION_PREFERENCE.PREFER_LOW_LATENCY);
        }

        ///
        /// <summary>
        /// Video frame rate.
        /// </summary>
        ///
        public enum FRAME_RATE
        {
            ///
            /// <summary>
            /// 1: 1 fps
            /// </summary>
            ///
            FRAME_RATE_FPS_1 = 1,

            ///
            /// <summary>
            /// 7: 7 fps
            /// </summary>
            ///
            FRAME_RATE_FPS_7 = 7,

            ///
            /// <summary>
            /// 10: 10 fps
            /// </summary>
            ///
            FRAME_RATE_FPS_10 = 10,

            ///
            /// <summary>
            /// 15: 15 fps
            /// </summary>
            ///
            FRAME_RATE_FPS_15 = 15,

            ///
            /// <summary>
            /// 24: 24 fps
            /// </summary>
            ///
            FRAME_RATE_FPS_24 = 24,

            ///
            /// <summary>
            /// 30: 30 fps
            /// </summary>
            ///
            FRAME_RATE_FPS_30 = 30,

            ///
            /// <summary>
            /// 60: 60 fpsFor Windows and macOS only.
            /// </summary>
            ///
            FRAME_RATE_FPS_60 = 60,
        };

        public VideoEncoderConfiguration(int width, int height, int f, int b, ORIENTATION_MODE m, VIDEO_MIRROR_MODE_TYPE mirror = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED)
        {
            codecType = VIDEO_CODEC_TYPE.VIDEO_CODEC_H264;
            dimensions = new VideoDimensions(width, height);
            frameRate = f;
            bitrate = b;
            dimensions.mVideoBitrate = b;
            minBitrate = (int)BITRATE.DEFAULT_MIN_BITRATE;
            dimensions.mVideoBitrate = bitrate;
            orientationMode = m;
            degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY;
            mirrorMode = mirror;
            advanceOptions = new AdvanceOptions(ENCODING_PREFERENCE.PREFER_AUTO, COMPRESSION_PREFERENCE.PREFER_LOW_LATENCY);
        }

        public VideoEncoderConfiguration(ref VideoEncoderConfiguration config)
        {
            codecType = config.codecType;
            dimensions = config.dimensions;
            frameRate = config.frameRate;
            bitrate = config.bitrate;
            minBitrate = config.minBitrate;
            orientationMode = config.orientationMode;
            degradationPreference = config.degradationPreference;
            mirrorMode = config.mirrorMode;
            advanceOptions = new AdvanceOptions(config.advanceOptions.encodingPreference, config.advanceOptions.compressionPreference);
        }

        public VideoEncoderConfiguration()
        {
            codecType = VIDEO_CODEC_TYPE.VIDEO_CODEC_H264;
            dimensions = new VideoDimensions((int)FRAME_WIDTH.FRAME_WIDTH_640, (int)FRAME_HEIGHT.FRAME_HEIGHT_360);
            frameRate = (int)FRAME_RATE.FRAME_RATE_FPS_15;
            bitrate = (int)BITRATE.STANDARD_BITRATE;
            minBitrate = (int)BITRATE.DEFAULT_MIN_BITRATE;
            orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_ADAPTIVE;
            degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY;
            mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED;
            advanceOptions = new AdvanceOptions(ENCODING_PREFERENCE.PREFER_AUTO, COMPRESSION_PREFERENCE.PREFER_LOW_LATENCY);
        }
    };

    /// <summary>
    /// The channel profile.
    /// </summary>
    ///
    public enum CHANNEL_PROFILE_TYPE
    {
        ///
        /// <summary>
        /// 0: Communication. Use this profile when there are only two users in the channel.
        /// </summary>
        ///
        CHANNEL_PROFILE_COMMUNICATION = 0,

        ///
        /// <summary>
        /// 1: Live streaming. Live streaming. Use this profile when there are more than two users in the channel.
        /// </summary>
        ///
        CHANNEL_PROFILE_LIVE_BROADCASTING = 1,

        [Obsolete]
        ///
        /// <summary>
        /// 2: Gaming. This profile is deprecated.
        /// </summary>
        ///
        CHANNEL_PROFILE_GAME = 2,

        [Obsolete]
        ///
        /// <summary>
        /// Cloud gaming. The scenario is optimized for latency. Use this profile if the use case requires frequent interactions between users.
        /// </summary>
        ///
        CHANNEL_PROFILE_CLOUD_GAMING = 3,

        [Obsolete]
        ///
        /// @ignore
        ///
        CHANNEL_PROFILE_COMMUNICATION_1v1 = 4,
    };

    public enum CLIENT_ROLE_TYPE
    {
        ///
        /// <summary>
        /// 1: Host. A host can both send and receive streams.
        /// </summary>
        ///
        CLIENT_ROLE_BROADCASTER = 1,

        ///
        /// <summary>
        /// 2: (Default) Audience. An audience member can only receive streams.
        /// </summary>
        ///
        CLIENT_ROLE_AUDIENCE = 2,
    };

    public enum DEVICE_TYPE
    {
        RECORD = 0,
        SUBMIX_LOOPBACK = 2,
        PLAY_RENDER = 1,
    };

    public enum MediaInvokeEventType
    {
        MIET_INVALID = 0,
        CREATE_SESSION = 1, // 初始化JNI和JAVA的交互对象
        DESTROY_SESSION = 2, // 销毁JNI和JAVA的交互对象
        WEB_MSG_EVENT = 10,
        AUDIO_PLAYER_EVENT = 11, // 音频播放事件
        /**
         * soft decode
         */
        SOFT_DECODE_EVENT = 100,

        UPDATE_UPLOAD_CONFIG = 90,
        MIET_MUTE_MEDIA_EVENT = 80,
        SET_CHANNEL_PROFILE = 101,
        SET_CLIENT_ROLE = 102,
        JOIN_CHANNEL = 103,
        LEAVE_CHANNEL = 104,
        AUDIO_CREATE = 105, // 创建音频模块
        AUDIO_DESTROY = 106, // 销毁音频模块
        ///////////////////////multi channel start //////////////////////
        JOIN_CHANNEL_EX = 200,
        LEAVE_CHANNEL_EX = 201,
        MUTE_LOCAL_VIDEO_STREAM_EX = 202,
        MUTE_LOCAL_AUDIO_STREAM_EX = 203,
        MUTE_ALL_REMOTE_VIDEO_STREAM_EX = 204,
        MUTE_ALL_REMOTE_AUDIO_STREAM_EX = 205,
        SUBSCRIBE_AUDIO_STREAM_EX = 206,
        UNSUBSCRIBE_AUDIO_STREAM_EX = 207,
        SUBSCRIBE_VIDEO_STREAM_EX = 208,
        UNSUBSCRIBE_VIDEO_STREAM_EX = 209,
        ///////////////////////multi channel end //////////////////////
        ///////////////////////audio Events Start //////////////////////
        AUDIO_DEFAULT_EVENT = 1000,
        AUDIO_CAPTURE_EVENT = 1001, // 采集开始或者停止
        AUDIO_RENDER_EVENT = 1002, // 播放开始或者停止
        AUDIO_ENCODE_EVENT = 1003, //编码开始或者停止
        AUDIO_VOLUME_INDICATION_EVENT = 1004, //采集音量提示,开启或者停止
        AUDIO_SUBMIX_EVENT = 1005, // 内录开启或者停止
        AUDIO_ADJUST_MIC_VOLUME_EVENT = 1006, // 调整采集音量
        AUDIO_ADJUST_ENCODE_BITRATE = 1007, // 调整编码码率，再opus编码情况下，可以调整码率
        AUDIO_UPDATE_CONFIG_EVENT = 1008, // 更新配置
        AUDIO_SET_PROFILE_EVENT = 1011, // profile
        AUDIO_ENABLE_EVENT = 1012, // enableAudio
        AUDIO_ADJUST_SUBMIX_VOLUME_EVENT = 1013, // 调整内录音量

        AUDIO_ENUMERATE_DEVICES_EVENT = 1101, // 获取设备信息
        AUDIO_GET_SUBMIX_DEVICE_EVENT = 1102, // 获取伴奏、内录设备信息
        AUDIO_GET_DEFAULT_OUT_DEVICE_EVENT = 1103, // 默认播放设备
        AUDIO_GET_OUT_DEVICE_EVENT = 1104, // 当前播放设备
        AUDIO_GET_INPUT_DEVICE_EVENT = 1105, // 当前采集设备
        AUDIO_SET_DEVICE_EVENT = 1106, // 设置使用的设备@DEVICE_TYPE
        AUDIO_MUTE_DEVICE_EVENT = 1107, // 静音使用的设备@DEVICE_TYPE
        AUDIO_GET_DEVICE_MUTE_STATE_EVENT = 1108, // 设置使用的设备@DEVICE_TYPE
        AUDIO_GET_DEFAULT_INPUT_DEVICE_EVENT = 1109, // 当前采集设备
        AUDIO_DEVICE_SET_VOLUME = 1110, // 设置设备音量
        AUDIO_DEVICE_GET_VOLUME = 1111, // 获取设备音量
        AUDIO_MUTE_LOCAL_STREAM_EVENT = 1112, // 禁止本地音频推流

        AUDIO_MIXING_EVENT = 1200, // audioMixing
        AUDIO_DEBUG_SAVE_CALLBACK_FILE = 1950, // 保存回调给业务前后的音频数据，用于对比PCM波形
        AUDIO_EVENT_MAX = 1999, //
                                ///////////////////////audio Events End //////////////////////
        ///    ///////////////////////video Events Start //////////////////////
        VIDEO_CAMERA_BUSY_INFO = 2101, //
         ///////////////////////video Events End //////////////////////
                                     //////////////////////testEvent/////////////////////////////
        TEST_SAVE_VIDEO_FILE = 10001, //
        //////////////////////testEvent/////////////////////////////

    }

    public enum UDPMsgType
    {
        /**
         * 解码数据
         */
        DECODE_DATA = 1,
        /**
         * 可用带宽
         */
        AVAILABLE_BAND_WIDTH = 2,
        /**
         * 要求下一帧是I帧，同时会调上可用带宽
         */
        REQUEST_I_FRAME = 3,
        /**
         * 连接OK
         */
        UDP_LINK_OK = 4,

        /**
         * 从SDK发送信令消息到信令服务器
         */
        SEND_MSG = 6,


        /**
         * 网络等级回调
         */
        NET_QUALITY = 7,

        VIDEO_FRAME_RATE_CONTROL = 8, //视频帧率控制

        CB_AUDIO_CAPTURE_VOLUME = 9, //音频采集音量回调

        CB_JOIN_CHANNEL = 10, //加入频道结果回调
        CB_LEAVE_CHANNEL = 11, //离开频道结果回调
        CB_LINK_STATUS = 12, //RUDP 连接状态

        MUTI_CHANNEL_REMOTE_JOIN = 1000, //多人RTC远端有人加入
        MUTI_CHANNEL_REMOTE_LEAVE = 1001, //多人RTC远端有人退出
    }


    public enum AudioProfile
    {
        /**
         0：默认设置。
         通信场景下，该选项代表指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
         直播场景下，该选项代表指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
         */
        AUDIO_PROFILE_DEFAULT = 0,
        /**
         1：指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
         */
        AUDIO_PROFILE_SPEECH_STANDARD = 1,
        /**
         2：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
         */
        AUDIO_PROFILE_MUSIC_STANDARD = 2,
        /**
         3：指定 48 kHz采样率，音乐编码，双声道，编码码率最大值为 80 Kbps。
         */
        AUDIO_PROFILE_MUSIC_STANDARD_STEREO = 3,

        /**
         * 4：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 96 Kbps。
         */
        AUDIO_PROFILE_MUSIC_HIGH_QUALITY = 4,

        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 128 Kbps。
         */
        AUDIO_PROFILE_MUSIC_HIGH_QUALITY_STEREO = 5,
        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 80 Kbps,拿解码数据，不播放
         */
        AUDIO_PROFILE_CALLBACK_DATA_NORENDER = 6,

        AUDIO_PROFILE_NORENDER_NO_CALLBACK = 7,
    };

    public enum AudioScenario
    {
        /**
         * 0：默认的音频应用场景。
         */
        AUDIO_SCENARIO_DEFAULT = 0,
        /**
         * 1：娱乐场景，适用于用户需要频繁上下麦的场景。
         */
        AUDIO_SCENARIO_CHATROOM_ENTERTAINMENT = 1,
        /**
         * 2：教育场景，适用于需要高流畅度和稳定性的场景。
         */
        AUDIO_SCENARIO_EDUCATION = 2,
        /**
         * 3：高音质语聊房场景，适用于音乐为主的场景。
         */
        AUDIO_SCENARIO_GAME_STREAMING = 3,
        /**
         * 4：秀场场景，适用于需要高音质的单主播场景。
         */
        AUDIO_SCENARIO_SHOWROOM = 4,
        /**
         * 5：游戏开黑场景，适用于只有人声的场景。
         */
        AUDIO_SCENARIO_CHATROOM_GAMING = 5,
        /**
         * 6: IoT（物联网）场景，适用于使用低功耗 IoT 设备的场景。
         */
        AUDIO_SCENARIO_IOT = 6,
        /**
         * 8: 会议场景，适用于人声为主的多人会议。
         */
        AUDIO_SCENARIO_MEETING = 8,
        AUDIO_SCENARIO_NUM = 9,
    }

    public class DeviceInfo
    {
        ///
        /// <summary>
        /// The device name.
        /// </summary>
        ///
        public string deviceName;

        ///
        /// <summary>
        /// The device ID.
        /// </summary>
        ///
        public string deviceId;
    };

    /**
     * The use mode of the audio data.
     */
    public enum AudioFrameOpType
    {
        /** 0: Read-only mode: Users only read the data from `AudioFrame` without modifying anything. 
         * For example, when users acquire the data with the Agora SDK, then start the media push.
         */
        RAW_AUDIO_FRAME_OP_MODE_READ_ONLY = 0,

        /** 2: Read and write mode: Users read the data from `AudioFrame`, modify it, and then play it. 
         * For example, when users have their own audio-effect processing module and perform some voice pre-processing, such as a voice change.
         */
        RAW_AUDIO_FRAME_OP_MODE_READ_WRITE = 2,
    };

    public enum CallbackType
    {
      AUDIO_MIC = 1, // 麦克风
      AUDIO_SUBMIX = 2, // 内录
      AUDIO_MIXED = 3, // 当开了内录后，回调的是麦克风和内录的混音
      AUDIO_DECODED = 4, // 解码后数据
      AUDIO_DECODE_EX = 5,
      VIDEO_ENCODE = 50,
      VIDEO_DECODE = 51,
      VIDEO_CAPTURE = 52,
      VIDEO_SCREEN_CAPTURE = 53,
    };


    public class ChannelMediaOptions : HPMarshaller
    {
        ///
        /// <summary>
        /// Whether to publish the video captured by the camera:true: (Default) Publish the video captured by the camera.false: Do not publish the video captured by the camera.
        /// </summary>
        ///
        public bool publishCameraTrack = false;

        ///
        /// <summary>
        /// Whether to publish the video captured by the second camera:true: Publish the video captured by the second camera.false: (Default) Do not publish the video captured by the second camera.
        /// </summary>
        ///
        public bool publishSecondaryCameraTrack = false;

        ///
        /// <summary>
        /// Whether to publish the audio captured by the microphone:true: (Default) Publish the audio captured by the microphone.false: Do not publish the audio captured by the microphone.
        /// </summary>
        ///
        public bool publishMicrophoneTrack = false;

        ///
        /// <summary>
        /// Whether to publish the video captured from the screen:true: Publish the video captured from the screen.false: (Default) Do not publish the captured video from the screen.This parameter applies to Android and iOS only.
        /// </summary>
        ///
        public bool publishScreenCaptureVideo = false;

        ///
        /// <summary>
        /// Whether to publish the audio captured from the screen:true: Publish the audio captured from the screen.false: (Default) Do not publish the audio captured from the screen.This parameter applies to Android and iOS only.
        /// </summary>
        ///
        public bool publishScreenCaptureAudio = false;

        ///
        /// <summary>
        /// Whether to publish the video captured from the screen:true: Publish the video captured from the screen.false: (Default) Do not publish the captured video from the screen.
        /// </summary>
        ///
        public bool publishScreenTrack = false;

        ///
        /// <summary>
        /// Whether to publish the video captured from the second screen:true: Publish the captured video from the second screen.false: (Default) Do not publish the video captured from the second screen.
        /// </summary>
        ///
        public bool publishSecondaryScreenTrack = false;

        ///
        /// <summary>
        /// Whether to publish the audio captured from a custom source:true: Publish the captured audio from a custom source.false: (Default) Do not publish the audio captured from the custom source.
        /// </summary>
        ///
        public bool publishCustomAudioTrack = false;

        ///
        /// <summary>
        /// The ID of the custom audio source to publish. The default value is 0.If you have set the value of sourceNumber greater than 1 in SetExternalAudioSource , the SDK creates the corresponding number of custom audio tracks and assigns an ID to each audio track starting from 0.
        /// </summary>
        ///
        public int publishCustomAudioSourceId = 0;

        ///
        /// <summary>
        /// Whether to enable AEC when publishing the audio captured from a custom source:true: Enable AEC when publishing the captured audio from a custom source.false: (Default) Do not enable AEC when publishing the audio captured from the custom source.
        /// </summary>
        ///
        public bool publishCustomAudioTrackEnableAec = false;

        ///
        /// @ignore
        ///
        public bool publishDirectCustomAudioTrack = false;

        ///
        /// @ignore
        ///
        public bool publishCustomAudioTrackAec = false;

        ///
        /// <summary>
        /// Whether to publish the video captured from a custom source:true: Publish the captured video from a custom source.false: (Default) Do not publish the video captured from the custom source.
        /// </summary>
        ///
        public bool publishCustomVideoTrack = false;

        ///
        /// <summary>
        /// Whether to publish the encoded video:true: Publish the encoded video.false: (Default) Do not publish the encoded video.
        /// </summary>
        ///
        public bool publishEncodedVideoTrack = false;

        ///
        /// <summary>
        /// Whether to publish the audio from the media player:true: Publish the audio from the media player.false: (Default) Do not publish the audio from the media player.
        /// </summary>
        ///
        public bool publishMediaPlayerAudioTrack = false;

        ///
        /// <summary>
        /// Whether to publish the video from the media player:true: Publish the video from the media player.false: (Default) Do not publish the video from the media player.
        /// </summary>
        ///
        public bool publishMediaPlayerVideoTrack = false;

        ///
        /// <summary>
        /// Whether to publish the local transcoded video:true: Publish the local transcoded video.false: (Default) Do not publish the local transcoded video.
        /// </summary>
        ///
        public bool publishTrancodedVideoTrack = false;

        ///
        /// <summary>
        /// Whether to automatically subscribe to all remote audio streams when the user joins a channel:true: (Default) Automatically subscribe to all remote audio streams.false: Do not automatically subscribe to any remote audio streams.
        /// </summary>
        ///
        public bool autoSubscribeAudio = false;

        ///
        /// <summary>
        /// Whether to automatically subscribe to all remote video streams when the user joins the channel:true: (Default) Automatically subscribe to all remote video streams.false: Do not automatically subscribe to any remote video streams.
        /// </summary>
        ///
        public bool autoSubscribeVideo = false;

        ///
        /// <summary>
        /// Whether to enable audio capturing or playback:true: (Default) Enable audio capturing or playback.false: Do not enable audio capturing or playback.
        /// </summary>
        ///
        public bool enableAudioRecordingOrPlayout = false;

        ///
        /// <summary>
        /// The ID of the media player to be published. The default value is 0.
        /// </summary>
        ///
        public int publishMediaPlayerId = -1;

        ///
        /// <summary>
        /// The user role. See CLIENT_ROLE_TYPE .
        /// </summary>
        ///
        public CLIENT_ROLE_TYPE clientRoleType = CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER;

        ///
        /// <summary>
        /// The channel profile. See CHANNEL_PROFILE_TYPE .
        /// </summary>
        ///
        public CHANNEL_PROFILE_TYPE channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION;


        public RTCDataWorkMode dataWorkMode = RTCDataWorkMode.SEND_AND_RECV;

        ///
        /// @ignore
        ///
        public bool enableBuiltInMediaEncryption = false;

        ///
        /// <summary>
        /// Whether to publish the sound of a metronome to remote users:true: (Default) Publish the sound of the metronome. Both the local user and remote users can hear the metronome.false: Do not publish the sound of the metronome. Only the local user can hear the metronome.
        /// </summary>
        ///
        public bool publishRhythmPlayerTrack = false;

        ///
        /// <summary>
        /// Whether to enable interactive mode:true: Enable interactive mode. Once this mode is enabled and the user role is set as audience, the user can receive remote video streams with low latency.false: (Default) Do not enable interactive mode. If this mode is disabled, the user receives the remote video streams in default settings.This parameter only applies to scenarios involving cohosting across channels. The cohosts need to call the JoinChannelEx method to join the other host's channel as an audience member, and set isInteractiveAudience to true.This parameter takes effect only when the user role is CLIENT_ROLE_AUDIENCE.
        /// </summary>
        ///
        public bool isInteractiveAudience = false;

        ///
        /// <summary>
        /// The video track ID returned by calling the createCustomVideoTrack method. The default value is 0.
        /// </summary>
        ///
        public lj_video_track_id_t customVideoTrackId = 0;

        ///
        /// <summary>
        /// Whether the audio stream being published is filtered according to the volume algorithm:true: (Default) The audio stream is filtered. If the audio stream filter is not enabled, this setting does not takes effect.false: The audio stream is not filtered.If you need to enable this function, contact .
        /// </summary>
        ///
        public bool isAudioFilterable = false;

        public ChannelMediaOptions() { }

        public override void marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.marshall(stream, writer, reader);
            pushBool(publishCameraTrack);
            pushBool(publishSecondaryCameraTrack);
            pushBool(publishMicrophoneTrack);
            pushBool(publishScreenCaptureVideo);
            pushBool(publishScreenCaptureAudio);
            pushBool(publishScreenTrack);
            pushBool(publishSecondaryScreenTrack);
            pushBool(publishCustomAudioTrack);
            pushBool(enableBuiltInMediaEncryption);
            pushBool(publishRhythmPlayerTrack);
            pushBool(isInteractiveAudience);
            pushBool(isAudioFilterable);
            pushBool(publishCustomAudioTrackEnableAec);
            pushBool(publishDirectCustomAudioTrack);
            pushBool(publishCustomAudioTrackAec);
            pushBool(publishCustomVideoTrack);
            pushBool(publishEncodedVideoTrack);
            pushBool(publishMediaPlayerAudioTrack);
            pushBool(publishMediaPlayerVideoTrack);
            pushBool(publishTrancodedVideoTrack);
            pushBool(autoSubscribeAudio);
            pushBool(autoSubscribeVideo);
            pushBool(enableAudioRecordingOrPlayout);
            pushInt(publishMediaPlayerId);
            pushInt((int)clientRoleType);
            pushInt((int)channelProfile);
            pushInt((int)dataWorkMode);
            pushInt(publishCustomAudioSourceId);
        }
    }
}
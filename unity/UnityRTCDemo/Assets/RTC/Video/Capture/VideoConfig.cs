using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LJ.RTC.Common;
namespace LJ.RTC.Video
{
    public enum BitrateMode {
        /** Constant quality mode */
        BITRATE_MODE_CQ = 0,
        /** Variable bitrate mode */
        BITRATE_MODE_VBR = 1,
        /** Constant bitrate mode */
        BITRATE_MODE_CBR = 2,
    }

    public class VideoConfig
    {
        /**
     * 预览和编码分辨率，这里区分预览和编码是为了适配采集时的一些分辨率跟所需的分辨率不一致的情况。
     */
        public int previewWidth;
        public int previewHeight;

        public int encodeWidth;
        public int encodeHeight;
        public FILL_MODE fillMode = FILL_MODE.COR_CENTER;

        public int mScreenOrientation = (int)ScreenOrientation.Portrait;
        /**
         * 视频设定帧率，采集帧率和编码帧率（如果需要设定的话）都按照这个值来制定。
         */
        public int frameRate;

        /**
         * 编码帧率控制，如采集驱动和定时器驱动等，
         */
        public string frameRatePolicy;

        /**
         * 镜像
         */
        public VIDEO_MIRROR_MODE_TYPE mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED;

        /**
        * 摄像头前后置标识，为了统一camera1和camera2而引入 {@link com.linjing.capture.api.camera.CameraFaceType}
        */
        public int cameraFacing = (int)CameraFaceType.FRONT;

        public int orientationMode = 0;
        public VideoEncodeConfig encodeConfig;
    }

    public class VideoEncodeConfig
    {
        public int codecType = (int)VIDEO_CODEC_TYPE.VIDEO_CODEC_H264;

        /**
        * 编码器类型，如软编码器，同步硬编码器和异步硬编码器等，
        */
        public int encoderType = (int)EncoderType.soft;

        public int bitrateMode = (int)BitrateMode.BITRATE_MODE_CBR;

        public int frameRate = 30;

        public int keyFrameInterval = 3;

        public int encodeWidth;

        public int encodeHeight;

        /**
 * 编码码率（bps）
 */
        public int bitRate = 1200 * 1000;

        /**
         * 编码码率（max bps）
         */
        public int maxBitRate = 1500 * 1000;


        /**
         * 编码码率（min bps）
         */
        public int minBitRate = 800 * 1000;

        public VideoEncodeConfig(int encoderType, int codecType, int bitrateMode, int bitrate, int maxBitrate, int minBitRate)
        {
            this.encoderType = encoderType;
            this.codecType = codecType;
            this.bitrateMode = bitrateMode;
            bitRate = bitrate;
            maxBitRate = maxBitrate;
            this.minBitRate = minBitRate;
        }
    }

    public enum EncoderType
    {
        Hard = 0,
        AsyncHard = 1,
        AsyncHard2 = 2,
        soft = 3,
    }

    public enum CameraFaceType
    {
        FRONT = 1,
        BACK = 0,
    }
}



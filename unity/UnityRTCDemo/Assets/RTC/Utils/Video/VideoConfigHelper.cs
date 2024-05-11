using System;
using LJ.RTC.Video;
using LJ.RTC.Common;

namespace LJ.RTC.Utils
{
    public class VideoConfigHelper
    {
        public static VideoConfig CreateVideoConfig(VideoEncoderConfiguration config)
        {
            VideoConfig videoConfig = new VideoConfig();
 
            videoConfig.cameraFacing = (int)CameraFaceType.FRONT;
            // 编码16对齐
            int encodeWidth = (int)Math.Floor(videoConfig.previewWidth / 16f) * 16;
            int encodeHeight = (int)Math.Floor(videoConfig.previewHeight / 16f) * 16;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            config.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_LANDSCAPE;
#endif
            if (config.orientationMode == ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT)
            {
                videoConfig.previewWidth = Math.Min(config.dimensions.GetWidth(), config.dimensions.GetHeight());
                videoConfig.previewHeight = Math.Max(config.dimensions.GetWidth(), config.dimensions.GetHeight());
                encodeWidth = Math.Min(config.dimensions.GetEncodeWidth(), config.dimensions.getEncodeHeight());
                encodeHeight = Math.Max(config.dimensions.GetEncodeWidth(), config.dimensions.getEncodeHeight());
            }
            else
            {
                videoConfig.previewWidth = Math.Max(config.dimensions.GetWidth(), config.dimensions.GetHeight());
                videoConfig.previewHeight = Math.Min(config.dimensions.GetWidth(), config.dimensions.GetHeight());
                encodeWidth = Math.Max(config.dimensions.GetEncodeWidth(), config.dimensions.getEncodeHeight());
                encodeHeight = Math.Min(config.dimensions.GetEncodeWidth(), config.dimensions.getEncodeHeight());
            }

            videoConfig.encodeWidth = (int)(Math.Ceiling(1.0f * encodeWidth / 16)) * 16;
            videoConfig.encodeHeight = (int)(Math.Ceiling(1.0f * encodeHeight / 16)) * 16;
            videoConfig.fillMode = config.fillMode;
            videoConfig.frameRate = config.frameRate == -1 ? 30 : config.frameRate;
            videoConfig.mScreenOrientation = config.mScreenOrientation;
            videoConfig.mirrorMode = config.mirrorMode;
            videoConfig.orientationMode = (int)config.orientationMode;
            /**
             * 编码参数
             */
            int minBitrate = config.minBitrate > 0 ? config.minBitrate : config.dimensions.GetMinBitRate();
            int bitrate = config.bitrate > 0 ? config.bitrate : config.dimensions.GetBitRate();
            videoConfig.encodeConfig = CreateEncodeConfig(bitrate, bitrate, minBitrate);

            videoConfig.encodeConfig.encodeWidth = encodeWidth;
            videoConfig.encodeConfig.encodeHeight = encodeHeight;

            videoConfig.encodeConfig.codecType = (int)config.codecType;
            videoConfig.encodeConfig.keyFrameInterval = config.keyFrameInterval;
            videoConfig.encodeConfig.bitrateMode = config.bitrateMode;
            return videoConfig;
        }

        public static VideoEncodeConfig CreateEncodeConfig(int bitrate, int maxBitrate, int minBitRate)
        {
            int codecType = (int)VIDEO_CODEC_TYPE.VIDEO_CODEC_H264;
            int encoderType = (int)EncoderType.soft;
            int bitrateMode = (int)BitrateMode.BITRATE_MODE_CBR;
            return new VideoEncodeConfig(encoderType, codecType, bitrateMode, bitrate, maxBitrate, minBitRate);
        }

    }
}

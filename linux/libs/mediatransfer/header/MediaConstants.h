//
// Created by Administrator on 2022/6/14.
//

#ifndef LJSDK_MEDIACONSTANTS_H
#define LJSDK_MEDIACONSTANTS_H
#include <cstdint>
#include <stddef.h>
namespace LJMediaLibrary {
    enum VideoFrameType
    {
        kVideoUnknowFrame = 0xFF,  // 8bits
        kVideoIFrame = 0,
        kVideoPFrame = 1,
        kVideoBFrame = 2,
        kVideoPFrameSEI = 3,     // 0 - 3 is same with YY video packet's frame type.
        kVideoIDRFrame = 4,
        kVideoSPSFrame = 5,
        kVideoPPSFrame = 6,
        kVideoHeaderFrame = 7,
        kVideoEncodedDataFrame = 8,
        kVideoH265HeadFrame = 9,
    };

    struct DelayTimeInfo {
        uint64_t id;
        uint64_t value;
    };

    struct DelayTimeInfoList {
        DelayTimeInfo* timeInfos;
        uint32_t size;
        DelayTimeInfoList() : timeInfos(NULL), size(0) {}
    };

    enum EncodeType
    {
        ENTYPE_H264 = 0,
        ENTYPE_H265 = 1,
        VIDEO_CODEC_VP8 = 2,
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
        ENTYPE_RAW = 0xff,
    };

    enum VIDEO_BUFFER_TYPE
    {
        ///
        /// <summary>
        /// 1: The video buffer in the format of raw data.
        /// </summary>
        ///
        VIDEO_BUFFER_RAW_DATA = 1,

        ///
        /// <summary>
        /// 2: The video buffer in the format of raw data.
        /// </summary>
        ///
        VIDEO_BUFFER_ARRAY = 2,

        ///
        /// <summary>
        /// 3: The video buffer in the format of Texture.
        /// </summary>
        ///
        VIDEO_BUFFER_TEXTURE = 3,
    };

    enum VIDEO_PIXEL_FMT {
        VIDEO_PIXEL_FMT_RGBA 		= 1,
        VIDEO_PIXEL_FMT_YUV_I420  = 2,
        VIDEO_PIXEL_FMT_NV21 		= 3,
        VIDEO_PIXEL_FMT_RGB24 	= 4,
        VIDEO_PIXEL_FMT_NV12 		= 5,
    };

    enum VIDEO_PIXEL_FORMAT
    {
        ///
        /// <summary>
        /// 0: Raw video pixel format.
        /// </summary>
        ///
        VIDEO_PIXEL_DEFAULT = 0,

        ///
        /// <summary>
        /// 1: The format is I420.
        /// </summary>
        ///
        VIDEO_PIXEL_I420 = 1,

        ///
        /// @ignore
        ///
        VIDEO_PIXEL_BGRA = 2,

        ///
        /// @ignore
        ///
        VIDEO_PIXEL_NV21 = 3,

        ///
        /// <summary>
        /// 4: The format is RGBA.
        /// </summary>
        ///
        VIDEO_PIXEL_RGBA = 4,

        ///
        /// <summary>
        /// 8: The format is NV12.
        /// </summary>
        ///
        VIDEO_PIXEL_NV12 = 8,

        ///
        /// @ignore
        ///
        VIDEO_TEXTURE_2D = 10,

        ///
        /// @ignore
        ///
        VIDEO_TEXTURE_OES = 11,

        ///
        /// @ignore
        ///
        VIDEO_CVPIXEL_NV12 = 12,

        ///
        /// @ignore
        ///
        VIDEO_CVPIXEL_I420 = 13,

        ///
        /// @ignore
        ///
        VIDEO_CVPIXEL_BGRA = 14,

        ///
        /// <summary>
        /// 16: The format is I422.
        /// </summary>
        ///
        VIDEO_PIXEL_I422 = 16,
    };

}
#endif //LJSDK_MEDIACONSTANTS_H

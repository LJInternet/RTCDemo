//
// Created by Administrator on 2023/6/30.
//

#ifndef LJSDK_VIDEOMEDIAEVENT_H
#define LJSDK_VIDEOMEDIAEVENT_H

#include "LJPacket.h"
namespace LJMediaLibrary {
    struct GetCameraDeviceBusyInfo: public ljtransfer::mediaSox::Marshallable {
       std::string deviceName;
       GetCameraDeviceBusyInfo() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << deviceName;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> deviceName;
        }
    };

    struct GetCameraDeviceBusyInfoResult: public ljtransfer::mediaSox::Marshallable {
        bool result = false;
        std::string processName;

        GetCameraDeviceBusyInfoResult(bool ret, std::string& name) {
            result = ret;
            processName.assign(name);
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << result<< processName;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> result>> processName;
        }
    };

    enum FILL_MODE {
        COR_CENTER = 0, // 居中裁剪
        FIT_XY = 1 //按宽高视频
    };

    ///
    /// <summary>
    /// Video output orientation mode.
    /// </summary>
    ///
    enum ORIENTATION_MODE
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
    enum FITMODE
    {
        FIT_X = 2,
        FIT_Y = 1,
        FIT_NONE = 0,
    };

    struct CaptureConfig : public ljtransfer::mediaSox::Marshallable {
        uint32_t width;
        uint32_t height;
        uint32_t fps;
        uint32_t oriMode;
        uint32_t fillMode;
        uint32_t screenOriMode;
        CaptureConfig() : width(0), height(0), fps(0), oriMode(0), fillMode(0), screenOriMode(0) {

        }
        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {

            pak << width<< height<< fps<< oriMode<<fillMode<<screenOriMode;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> width>> height>>fps>>oriMode>>fillMode>>screenOriMode;
        };

        public: void assignConfig(CaptureConfig& config) {
            width = config.width;
            height = config.height;
            fps = config.fps;
            oriMode = config.oriMode;
            fillMode = config.fillMode;
            screenOriMode = config.screenOriMode;
        };
    };
}
#endif //LJSDK_VIDEOMEDIAEVENT_H


#include "media_engine.h"
#ifdef WIN32
//#include <capture/CaptureTest.cpp>
#endif
#include "RudpApi.h"
#include <iostream>
#include <fstream>
#include <string>

#include <Windows.h>
#include <iostream>
#include <string>   // C风格字符串函数
#include <wchar.h>    // 宽字符字符串函数
#include <tchar.h>

#include "TransConstants.h"
#include "rudp_proxy.h"
#include "VideoMediaEvent.h"
#include "MediaInvokeEvent.h"
#include "date_time.h"
#include "system_util.h"
//FILE* yuvFiles = nullptr;
using namespace LJMediaLibrary;
#pragma execution_character_set("UTF-8")

static media_engine* mMediaEngine = nullptr;


static void on_capture_video(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, void* context) {
    //printf("on_capture_video\n");
    CaptureVideoFrame frame;
    frame.stride = width;
    frame.width = width;
    frame.height = height;
    frame.rotation = 0;
    frame.type = VIDEO_BUFFER_RAW_DATA;
    frame.format = VIDEO_PIXEL_RGBA;
    frame.timestamp = 0;
    frame.mirror = 0;
    std::string data;
    ljtransfer::mediaSox::PacketToString(frame, data);

    //rtmp_engine_write_raw_video((const char*)buf, len, data.c_str(), data.length(), pixel_fmt);
}

static FILE* encodedFile = nullptr;
static void on_encode_video(uint8_t* buf, int32_t len, int32_t width, int32_t heigcdht, int pixel_fmt, void* context) {
    //if (encodedFile == nullptr) {
    //    encodedFile = fopen("encode_video.h264", "wb");
    //}
    //fwrite(buf, 1, len, encodedFile);
}

static bool on_capture_audio(void* audioData, int size, uint64_t pts, int sampleRate, int channelCount, void* context) {

    //printf("on_capture_audio\n");
    //rtmp_engine_write_raw_audio((const int8_t*)audioData, size/2/channelCount, sampleRate, channelCount, 2);
    return true;
}

static void onMsgCallback(const char* msg, uint32_t len, uint64_t uid, void* content) {
    printf("onMsgCallback %s \n", std::string(msg, len));
}

static void onEvnCallback(int type, const char* msg, uint32_t len, int result, void* content) {
    printf("onEvnCallback %d %d \n", type, result);
}

static FILE* file = nullptr;
static void OnDecodeVideoCallback(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, void* context) {
    printf("OnDecodeVideoCallback %d X %d pixel_fmt %d \n", width, height, pixel_fmt);

}

static void OnDecodeVideoWithDelayCallback(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, std::map<uint64_t, uint64_t> delayMap, void* context) {
    printf("OnDecodeVideoWithDelayCallback %d X %d pixel_fmt %d %lu \n", width, height, pixel_fmt, delayMap[KEY_CAPTURE_TIME]);

}

static RUDPEngine* startRtm(int role, long uid, std::string& channelId) {
    RUDPConfig config{};
    config.appId = 1;
    config.isDebug = true;
    config.token = "sss";
    config.dataWorkMode = SEND_AND_RECV;
    config.role = role;
    config.mode = RUDP_REALTIME_ULTRA;
    RUDPEngine* rtmEngine = rudp_engine_create(&config);
    rudp_engine_register_msg_callback(rtmEngine, onMsgCallback, nullptr);
    rudp_engine_register_event_callback(rtmEngine, onEvnCallback, nullptr);
    rudp_engine_join_channel(rtmEngine, uid, "channelId");
    return rtmEngine;
}

static void stopRtm(RUDPEngine* engine) {
    rudp_engine_leave_channel(engine);
    rudp_engine_destroy(engine);
}

static void onEventCallback(int type, const char* buf, int size, void* context) {
    //首次建连成功或者中途断开重连成功，这个时候需要发一个I帧
    if (type == RUDP_CB_TYPE_REQUEST_I_FRAME || type == RUDP_CB_TYPE_LINK_OK) {

    }
    else if (RUDP_CB_TYPE_AVAILABLE_BW == type) {
        AvailableBands bands;
        std::string bandStr(buf, size);
        ljtransfer::mediaSox::Unpack up(bandStr.data(), size);
        bands.unmarshal(up);
        int videoBands = bands.m_availableBands[VIDEO_DATA];
    }
    else if (CB_LINK_STATUS == type) {
        LinkStatusEvent linkStatus;
        std::string linkStatusStr(buf, size);
        ljtransfer::mediaSox::Unpack up(linkStatusStr.data(), size);
        linkStatus.unmarshal(up);
        // 1 connected 2 disconnectd 3 lost 4 close by peer
    }
	 else if (RUDP_CB_VIDEO_FRAME_RATE_CONTROL == type) {
        VideoFrameRateControl framecontrol;
        std::string framecontrolStr(buf, size);
        ljtransfer::mediaSox::Unpack up(framecontrolStr.data(), size);
        framecontrol.unmarshal(up);
        int frameRate = framecontrol.frameRate;
    }
}

static void testWindowPush() {
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    media_engine* nginx = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);

    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = 1;
    config.channelID = "954523111";
    config.userID = 3443434;
    config.token = "token";
    config.transferMode = 1;

    MIEVideoUploadConfig videoUploadConfig;
    videoUploadConfig.encodeWidth = 640;
    videoUploadConfig.encodeHeight = 480;
    videoUploadConfig.maxVideoBitrateInbps = 800000;
    videoUploadConfig.minVideoBitrateInbps = 700000;
    videoUploadConfig.realVideoBitrateInbps = 800000;
    c.transferConfig = config;
    c.videoUploadConfig = videoUploadConfig;

    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    media_engine_send_event(nginx, 103, (char*)cfgstr.c_str(), cfgstr.length());

    char devices[1000];
    media_engine_camera_list(devices, 1000);

    printf("设备列表 %s \n", devices);
    //media_engine_start_camera_capture(nginx, "C505e HD Webcam", 640, 480, 30);
    //media_engine_start_camera_capture(nginx, "e2eSoft iVCam", 640, 480, 30);
    CaptureConfig captureConfig;
    captureConfig.width = 640;
    captureConfig.height = 480;
    captureConfig.fps = 30;
    captureConfig.oriMode = ORIENTATION_MODE_FIXED_LANDSCAPE;// ORIENTATION_MODE_FIXED_LANDSCAPE;// ORIENTATION_MODE_FIXED_PORTRAIT;
    captureConfig.fillMode = FIT_XY;// COR_CENTER;// FIT_XY;
    std::string cfgstr11;
    ljtransfer::mediaSox::PacketToString(captureConfig, cfgstr11);
    media_engine_start_camera_capture_with_config(nginx, "Logi C270 HD WebCam", cfgstr11.c_str(), cfgstr11.length());
    media_engine_subscribe_capture_video(nginx, on_capture_video, nullptr);

    media_engine_subscribe_encoded_video(nginx, on_encode_video, nullptr);

    AudioEnableEvent createAudioEvent;
    createAudioEvent.evtType = AUDIO_CREATE;
    createAudioEvent.enabled = true;
    std::string audio_create_data;
    ljtransfer::mediaSox::PacketToString(createAudioEvent, audio_create_data);
    media_engine_send_event(nginx, createAudioEvent.evtType, (char*)audio_create_data.c_str(), audio_create_data.length());

    AudioEnableEvent captureEvent;
    captureEvent.evtType = AUDIO_ENABLE_EVENT;
    captureEvent.enabled = true;
    std::string audio_enable_data;
    ljtransfer::mediaSox::PacketToString(captureEvent, audio_enable_data);
    media_engine_send_event(nginx, captureEvent.evtType, (char*)audio_enable_data.c_str(), audio_enable_data.length());

    media_engine_subscribe_capture_audio(nginx, on_capture_audio, nullptr);


    // 订阅解码视频
    media_engine_subscribe_video_with_delay(nginx, OnDecodeVideoWithDelayCallback, nginx);

    //RUDPEngine* engine = startRtm(config.transferMode, config.userID, config.channelID);

   //while (true) {
   //    LJ::SystemUtil::sleep(500);
   //    std::string msgStr = "test";
   //    rudp_engine_send(engine, msgStr.c_str(), msgStr.size());
   //}
    //rtmp_engine_close();
    //media_engine_stop_camera_capture(nginx);

    LJ::SystemUtil::sleep(1000 * 60 * 500);
   // stopRtm(engine);
}

static uint8_t* yuvData = nullptr;
static void testWindowPull() {
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);
    // 订阅解码视频
    media_engine_subscribe_video(mMediaEngine, OnDecodeVideoCallback, mMediaEngine);

    // 加入频道
    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = 1;
    config.channelID = "channelId";
    config.userID = 34434341;
    config.token = "token";
    config.transferMode = 0; // 0设置为server模式
    c.transferConfig = config;
    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

    RUDPEngine* engine = startRtm(config.transferMode, config.userID, config.channelID);

    CaptureVideoFrame frame;
    frame.stride = 640;
    frame.width = 640;
    frame.height = 480;
    frame.rotation = 0;
    frame.type = VIDEO_BUFFER_RAW_DATA;
    frame.format = VIDEO_PIXEL_I420;
    frame.timestamp = LJ::DateTime::currentTimeMillis();
    frame.mirror = 0;
    std::string data;
    ljtransfer::mediaSox::PacketToString(frame, data);
    int size = 0;
    while (true) {
        LJ::SystemUtil::sleep(100);
        std::string msgStr = "test pull";
        rudp_engine_send(engine, msgStr.c_str(), msgStr.size());
        if (yuvData == nullptr) {
            std::ifstream yuvFileStream("640X480.yuv");
            std::string content((std::istreambuf_iterator<char>(yuvFileStream)),
                                (std::istreambuf_iterator<char>()));
            yuvFileStream.close();
            size = content.length();
            yuvData = new uint8_t[size];
            memcpy(yuvData, content.c_str(), size);
        }
        if (yuvData != nullptr && size != 0) {
        }
        media_engine_push_video(mMediaEngine, (const char*)yuvData, size, data.c_str(), data.length(), 2);
    }

    LJ::SystemUtil::sleep(1000 * 60 * 500);
    if (yuvData != nullptr) {
        delete[] yuvData;
        yuvData = nullptr;
    }
    media_engine_destroy(mMediaEngine);

    stopRtm(engine);
}

static void testLinuxPull() {
    // 创建RTC实例
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);

    // 订阅解码视频
    media_engine_subscribe_video(mMediaEngine, OnDecodeVideoCallback, mMediaEngine);

    // 加入频道
    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = 1;
    config.channelID = "channelId";
    config.userID = 34434341;
    config.token = "token";
    config.transferMode = 0; // 0设置为server模式
    c.transferConfig = config;
    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

    RUDPEngine* engine = startRtm(config.transferMode, config.userID, config.channelID);

    while (true) {
        LJ::SystemUtil::sleep(500);
        std::string msgStr = "test pull";
        rudp_engine_send(engine, msgStr.c_str(), msgStr.size());
    }

    LJ::SystemUtil::sleep(1000 * 60 * 500);

    media_engine_destroy(mMediaEngine);

    stopRtm(engine);
}

static void testLinuxPush() {
    // 创建RTC实例
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);

    // 加入频道
    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = 1;
    config.channelID = "channelId";
    config.userID = 3443434;
    config.token = "token";
    config.transferMode = 1; // 1设置为client模式
    c.transferConfig = config;
    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

    RUDPEngine* engine = startRtm(config.transferMode, config.userID, config.channelID);

    while (true) {
        LJ::SystemUtil::sleep(100);
        std::string msgStr = "test push";
        rudp_engine_send(engine, msgStr.c_str(), msgStr.size());
    }

    LJ::SystemUtil::sleep(1000 * 60 * 500);

    media_engine_destroy(mMediaEngine);

    stopRtm(engine);
}
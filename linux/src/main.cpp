#include <iostream>
#include <string>   // C风格字符串函数
#include <fstream>
#include "media_engine.h"
#include "RudpApi.h"
#include "TransConstants.h"
#include "rudp_proxy.h"
#include "VideoMediaEvent.h"
#include "MediaInvokeEvent.h"
#include "date_time.h"
#include "system_util.h"
#include <map>
using namespace LJMediaLibrary;
static media_engine* mMediaEngine = nullptr;
static uint8_t* yuvData = nullptr;
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

static void onMsgCallback(const char* msg, uint32_t len, uint64_t uid, void* content) {
    printf("onMsgCallback %s \n", std::string(msg, len).c_str());
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
    rudp_engine_join_channel(rtmEngine, uid, "954523111");
    return rtmEngine;
}

static void stopRtm(RUDPEngine* engine) {
    rudp_engine_leave_channel(engine);
    rudp_engine_destroy(engine);
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
    config.appID = xxx;
    config.channelID = "xxx";
    config.userID = xxx;
    config.token = "xxx";
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
    rtc_config.enableLog = true;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);

    // 加入频道
    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = xxx;
    config.channelID = "xxx";
    config.userID = xxx;
    config.token = "xxx";
    config.transferMode = 1; // 1设置为client模式
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
//        if (yuvData == nullptr) {
//            std::ifstream yuvFileStream("640X480.yuv");
//            std::string content((std::istreambuf_iterator<char>(yuvFileStream)),
//                                (std::istreambuf_iterator<char>()));
//            yuvFileStream.close();
//            size = content.length();
//            yuvData = new uint8_t[size];
//            memcpy(yuvData, content.c_str(), size);
//        }
//        media_engine_push_video(mMediaEngine, (const char*)yuvData, size, data.c_str(), data.length(), 2);
    }

    LJ::SystemUtil::sleep(1000 * 60 * 500);
    if (yuvData != nullptr) {
        delete[] yuvData;
        yuvData = nullptr;
    }

    media_engine_destroy(mMediaEngine);

    stopRtm(engine);
}

int main(int argc, char **argv){
    testLinuxPull();
}
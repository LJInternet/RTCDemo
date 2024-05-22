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
#include "WavPCMHeader.h"
#include <map>
using namespace LJMediaLibrary;
using namespace LJMediaLibrary::WAV;
static media_engine* mMediaEngine = nullptr;
static uint8_t* yuvData = nullptr;
static std::string channels = "954523112";
static std::string token = "token";
static uint64_t uid = LJ::DateTime::currentTimeMillis();
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

static FILE* testWavFile = nullptr;
static uint32_t totalAudioCount = 0;
static uint32_t audioChannels = 0;
static uint32_t audioSampleRate = 0;
static bool onDecodeAudioData(void* audioData, int size, uint64_t pts,
    int sampleRate, int channelCont, void* context) {
    //printf("onDecodeAudioData sampleRate %d channelCont %d %d \n", sampleRate, channelCont, sizeof(WavPCMHeader));
    if (testWavFile == nullptr) {
        testWavFile = fopen("decode_audio.wav", "wb");
        fseek(testWavFile, sizeof(WavPCMHeader), SEEK_SET);
    }
    fwrite(audioData, 1, size, testWavFile);
    totalAudioCount = totalAudioCount + size;
    audioChannels = channelCont;
    audioSampleRate = sampleRate;
    return false;
}

static void testSavePCMToWav() {
    // 创建RTC实例
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);

    // 订阅解码视频
    media_engine_subscribe_video(mMediaEngine, OnDecodeVideoCallback, mMediaEngine);

    AudioPlayerEvent audioPlayerEvent;
    audioPlayerEvent.directDecode = true;
    audioPlayerEvent.callbackDecodeData = true;
    std::string audio_enable_data;
    ljtransfer::mediaSox::PacketToString(audioPlayerEvent, audio_enable_data);
    media_engine_send_event(mMediaEngine, AUDIO_PLAYER_EVENT, (char*)audio_enable_data.c_str(), audio_enable_data.length());
    media_engine_subscribe_audio(mMediaEngine, onDecodeAudioData, nullptr);


    // 加入频道
    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = 1;
    config.channelID = channels;
    config.userID = uid;
    config.token = token.c_str();
    config.transferMode = 0; // 0设置为server模式
    c.transferConfig = config;
    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

    LJ::SystemUtil::sleep(1000 * 10);

    media_engine_destroy(mMediaEngine);

    if (testWavFile != nullptr && totalAudioCount > 0) {
        fseek(testWavFile, 0, SEEK_SET);
        WavPCMHeader header((uint16_t)audioChannels, audioSampleRate, 16, totalAudioCount);
        fwrite(&header, 1, sizeof(header), testWavFile);
        fclose(testWavFile);
        testWavFile = nullptr;
    }
}

static void testLinuxRTCCreateAndDestroy() {
    while (true) {
        // 创建RTC实例
        RTCEngineConfig rtc_config;
        rtc_config.enableLog = false;
        std::string rtccfgstr;
        ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
        mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length
());
        media_engine_set_debug_env(true);

        // 加入频道
        MIEUploadConfig c;
        MIETransferConfig config;
        config.appID = 1;
        config.channelID = channels;
        config.userID = uid;
        config.token = token.c_str();
        config.transferMode = 1; // 1设置为client模式
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

        media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.
c_str(), cfgstr.length());

        char devices[1000];
        media_engine_camera_list(devices, 1000);

        printf("设备列表 %s \n", devices);
        //media_engine_start_camera_capture(nginx, "C505e HD Webcam", 640, 480
, 30);
        //media_engine_start_camera_capture(nginx, "e2eSoft iVCam", 640, 480, 
30);
        CaptureConfig captureConfig;
        captureConfig.width = 640;
        captureConfig.height = 480;
        captureConfig.fps = 30;
        captureConfig.oriMode = ORIENTATION_MODE_FIXED_LANDSCAPE;// 
ORIENTATION_MODE_FIXED_LANDSCAPE;// ORIENTATION_MODE_FIXED_PORTRAIT;
        captureConfig.fillMode = FIT_XY;// COR_CENTER;// FIT_XY;
        std::string cfgstr11;
        ljtransfer::mediaSox::PacketToString(captureConfig, cfgstr11);
        media_engine_start_camera_capture_with_config(mMediaEngine, "Logi 
C270 HD WebCam", cfgstr11.c_str(), cfgstr11.length());

        int sampleCount = 48000 / 1000 * 10;
        const char* filename = "capture_in_debug.pcm"; // PCM文件路径
        std::ifstream inFile(filename, std::ios::binary);
        if (!inFile.is_open()) {
            std::cerr << "Failed to open file: " << filename << std::endl;
            return;
        }
        int16_t* pcmBuffer = new int16_t[sampleCount]; // 缓冲区用于存放10ms的音频数据
        while (!inFile.eof()) {

            inFile.read(reinterpret_cast<char*>(pcmBuffer), sampleCount * 
sizeof(int16_t));
            // 检查实际读取了多少样本
            size_t bytesRead = inFile.gcount();
            size_t actualSamplesRead = bytesRead / sizeof(int16_t);
            if (actualSamplesRead > 0) {
                // 处理读取到的数据，比如打印出来或进一步处理
                //std::cout << "Read " << actualSamplesRead << " samples." << 
std::endl;
                // 这里可以添加处理pcmBuffer的代码
                media_engine_push_audio(mMediaEngine, reinterpret_cast<const 
int8_t*>(pcmBuffer), sampleCount, 48000, 1, 2);
            }
            else {
                // 如果没有读取到数据，表示已经到达文件末尾
                break;
            }
            LJ::SystemUtil::sleep(9);
        }
        LJ::SystemUtil::sleep(10000);
        delete[] pcmBuffer; // 释放缓冲区
        inFile.close(); // 关闭文件
        media_engine_destroy(mMediaEngine);
    }

int main(int argc, char **argv){
    testLinuxPull();
}
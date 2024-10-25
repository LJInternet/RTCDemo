
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
#include <locale>
#include <codecvt>
#include "TransConstants.h"
#include "rudp_proxy.h"
#include "VideoMediaEvent.h"
#include "MediaInvokeEvent.h"
#include "date_time.h"
#include "system_util.h"
#include "WavPCMHeader.h"
#include <functional>
//FILE* yuvFiles = nullptr;
using namespace LJMediaLibrary;
using namespace LJMediaLibrary::WAV;
#pragma execution_character_set("UTF-8")

static std::string channels = "954523133";
static std::string token = "linjing@2023";
static uint64_t uid = LJ::DateTime::currentTimeMillis();
static media_engine* mMediaEngine = nullptr;


class RTMEngine {
public:
    RTMEngine(uint64_t appid, const char* token, bool isDebug) {
        config_.appId = appid;
        config_.isDebug = isDebug;
        config_.token = token;
        config_.dataWorkMode = 0;
        config_.role = 0;
        config_.mode = 0;
        rtmEngine_ = rudp_engine_create(&config_);
    }


    ~RTMEngine() {
        if (rtmEngine_) {
            rudp_engine_leave_channel(rtmEngine_);
            rudp_engine_destroy(rtmEngine_);
            rtmEngine_ = nullptr;
        }
    }

    static void onMsgCallback(const char* msg, uint32_t len, uint64_t uid, void* content) {
        printf("onMsgCallback %s \n", std::string(msg, len));
        if (content == nullptr) {
            return;
        }
        RTMEngine* engine = (RTMEngine*)content;
        engine->callbackMsg(msg, len, uid);

    }

    static void onEvnCallback(int type, const char* msg, uint32_t len, int result, void* content) {
        printf("onEvnCallback %d %d \n", type, result);
        if (content == nullptr) {
            return;
        }
        RTMEngine* engine = (RTMEngine*)content;
        if (type == 3) {
            engine->callbackLinkStatus(result);
        }
    }

    void joinChannel(uint64_t uid, const char* channelId) {
        if (rtmEngine_) {
            rudp_engine_join_channel(rtmEngine_, uid, channelId);
        }
    }

    void leaveChannel() {
        if (rtmEngine_) {
            rudp_engine_leave_channel(rtmEngine_);
        }
    }

    void registerMsgCallback(std::function<void(const char* msg, uint32_t len, uint64_t uid)> callback) {
        msgCallback_ = callback;
        if (rtmEngine_) {
            rudp_engine_register_msg_callback(rtmEngine_, onMsgCallback, this);
        }
    }

    void registerLinkStatusCallback(std::function<void(int status)> callback) {
        statusCallback_ = callback;
        if (rtmEngine_) {
            rudp_engine_register_event_callback(rtmEngine_, onEvnCallback, this);
        }
    }

private:
    RUDPEngine* rtmEngine_;
    RUDPConfig config_;
    std::function<void(const char* msg, uint32_t len, uint64_t uid)> msgCallback_;
    std::function<void(int status)> statusCallback_;

    void callbackMsg(const char* msg, uint32_t len, uint64_t uid) {
        if (msgCallback_) {
            msgCallback_(msg, len, uid);
        }
    }

    void callbackLinkStatus(int status) {
        if (statusCallback_) {
            statusCallback_(status);
        }
    }
};

static FILE* captureYuvFile = nullptr;
static void on_capture_video(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, void* context) {
    printf("on_capture_video %d %d %d\n", pixel_fmt, width, height);

    // if (captureYuvFile == nullptr) {
    //     captureYuvFile = fopen("640X480.yuv", "wb");
    //     fwrite(buf, 1, len, captureYuvFile);
    // }
    //fwrite(buf, 1, len, encodedFile);
    //CaptureVideoFrame frame;
    //frame.stride = width;
    //frame.width = width;
    //frame.height = height;
    //frame.rotation = 0;
    //frame.type = VIDEO_BUFFER_RAW_DATA;
    //frame.format = VIDEO_PIXEL_RGBA;
    //frame.timestamp = 0;
    //frame.mirror = 0;
    //std::string data;
    //ljtransfer::mediaSox::PacketToString(frame, data);

    //rtmp_engine_write_raw_video((const char*)buf, len, data.c_str(), data.length(), pixel_fmt);
}

static FILE* encodedFile = nullptr;
static void on_encode_video(uint8_t* buf, int32_t len, int32_t width, int32_t heigcdht, int pixel_fmt, void* context) {
    //if (encodedFile == nullptr) {
    //    encodedFile = fopen("encode_video.h264", "wb");
    //}
    //fwrite(buf, 1, len, encodedFile);
}

static FILE* testWavFile = nullptr;
static uint32_t totalAudioCount = 0;
static uint32_t audioChannels = 0;
static uint32_t audioSampleRate = 0;
static bool onDecodeAudioData(void* audioData, int size, uint64_t pts,
    int sampleRate, int channelCont, void* context) {
    printf("onDecodeAudioData sampleRate %d channelCont %d %d %lu\n", sampleRate, channelCont, sizeof(WavPCMHeader), pts);
    //if (testWavFile == nullptr) {
    //    testWavFile = fopen("decode_audio.wav", "wb");
    //    fseek(testWavFile, sizeof(WavPCMHeader), SEEK_SET);
    //}
    //fwrite(audioData, 1, size, testWavFile);
    //totalAudioCount = totalAudioCount + size;
    //audioChannels = channelCont;
    //audioSampleRate = sampleRate;
    return false;
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
    // if (captureYuvFile == nullptr) {
    //     captureYuvFile = fopen("640X480.yuv", "wb");
    //     fwrite(buf, 1, len, captureYuvFile);
    // }

}
static void OnDecodeVideoCallbackWithPts(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, uint32_t pts, void* context) {
    //printf("OnDecodeVideoCallback %d X %d pixel_fmt %d pts %d\n", width, height, pixel_fmt, pts);
    // if (captureYuvFile == nullptr) {
    //     captureYuvFile = fopen("640X480.yuv", "wb");
    //     fwrite(buf, 1, len, captureYuvFile);
    // }

}

static void OnDecodeVideoWithDelayCallback(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, std::map<uint64_t, uint64_t> delayMap, void* context) {
    //printf("OnDecodeVideoWithDelayCallback %d X %d pixel_fmt %d %lu \n", width, height, pixel_fmt, delayMap[KEY_CAPTURE_TIME]);

}

static RUDPEngine* startRtm(int role, long uid, std::string& channelId) {
    RUDPConfig config{};
    config.appId = 1;
    config.isDebug = true;
    config.token = token.c_str();
    config.dataWorkMode = SEND_AND_RECV;
    config.role = role;
    config.mode = RUDP_REALTIME_ULTRA;
    RUDPEngine* rtmEngine = rudp_engine_create(&config);
    rudp_engine_register_msg_callback(rtmEngine, onMsgCallback, nullptr);
    rudp_engine_register_event_callback(rtmEngine, onEvnCallback, nullptr);
    rudp_engine_join_channel(rtmEngine, uid, channels.c_str());
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
    config.channelID = channels;
    config.userID = uid;
    config.token = token.c_str();
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

enum AUDIO_DEVICE_TYPE {
    RECORD = 0,
    PLAY_RENDER = 1,
    SUBMIX_LOOPBACK = 2,
};
static void testWindowPull() {
    SetConsoleOutputCP(CP_UTF8);
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    media_engine* mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    media_engine_set_debug_env(true);

    AudioEnableEvent createAudioEvent;
    createAudioEvent.evtType = AUDIO_CREATE;
    createAudioEvent.enabled = true;
    std::string audio_create_data;
    ljtransfer::mediaSox::PacketToString(createAudioEvent, audio_create_data);
    media_engine_send_event(mMediaEngine, createAudioEvent.evtType, (char*)audio_create_data.c_str(), audio_create_data.length());

    // 获取电脑的所有音频播放设备列表
    EnumerateAudioDevicesEvent audioDeviceEvent;
    audioDeviceEvent.type = 1;
    std::string audioDeviceStr;
    ljtransfer::mediaSox::PacketToString(audioDeviceEvent, audioDeviceStr);
    int retLen = 0;
    char * retChar = media_engine_get_event(mMediaEngine, AUDIO_ENUMERATE_DEVICES_EVENT, (char*)audioDeviceStr.c_str(), audioDeviceStr.length(), &retLen);
    std::string retStr(retChar, retLen);
    printf("retLen %d, retChar %s , retStr %s \n", retLen, retChar, retStr.c_str());
    AudioDevicesEvent retDeviceEvent;
    ljtransfer::mediaSox::Unpack up(retChar, retLen);
    retDeviceEvent.unmarshal(up);
    for (AudioDevice palyDevice : retDeviceEvent.devices) {
        printf("palyDevice id = %d name = %s \n", palyDevice.id, palyDevice.name.c_str());
    }

    // 获取电脑的默认播放设备
    char* retChar1 = media_engine_get_event(mMediaEngine, AUDIO_GET_DEFAULT_OUT_DEVICE_EVENT, "", 0, &retLen);
    AudioDevice audioDevice;
    ljtransfer::mediaSox::Unpack up1(retChar1, retLen);
    audioDevice.unmarshal(up1);
    printf("default audioDevice %s id %d \n", audioDevice.name.c_str(), audioDevice.id);

    // 获取当前播放设备id
    char* retChar2 = media_engine_get_event(mMediaEngine, AUDIO_GET_OUT_DEVICE_EVENT, "", 0, &retLen);
    AudioDevice audioDevice2;
    ljtransfer::mediaSox::Unpack up2(retChar2, retLen);
    audioDevice2.unmarshal(up2);
    printf("current audioDevice %s id %d \n", audioDevice2.name.c_str(), audioDevice2.id);
    // 设置播放设备
    //SetDeviceInfoEvent setDeviceInfo;
    //AudioDevice setDevice;
    //setDevice.id = audioDevice.id;
    //setDevice.name = audioDevice.name;
    //setDeviceInfo.audioDevice = setDevice;
    //setDeviceInfo.type = 1;
    //std::string setdeviceInfoStr;
    //ljtransfer::mediaSox::PacketToString(setDeviceInfo, setdeviceInfoStr);
    //media_engine_send_event(mMediaEngine, AUDIO_SET_DEVICE_EVENT, (char*)setdeviceInfoStr.c_str(), setdeviceInfoStr.length());

    // 订阅解码视频
    //media_engine_subscribe_video(mMediaEngine, OnDecodeVideoCallback, mMediaEngine);
    media_engine_subscribe_video_with_pts(mMediaEngine, OnDecodeVideoCallbackWithPts, mMediaEngine);
    AudioPlayerEvent audioPlayerEvent;
    audioPlayerEvent.directDecode = false;
    audioPlayerEvent.callbackDecodeData = false;
    audioPlayerEvent.renderAudioData = true;
    std::string PlayerEnableStr;
    ljtransfer::mediaSox::PacketToString(audioPlayerEvent, PlayerEnableStr);
    media_engine_send_event(mMediaEngine, AUDIO_PLAYER_EVENT, (char*)PlayerEnableStr.c_str(), PlayerEnableStr.length());
    //media_engine_subscribe_audio(mMediaEngine, onDecodeAudioData, nullptr);
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


    LJ::SystemUtil::sleep(1000 * 60 * 500);

    media_engine_destroy(mMediaEngine);
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

    LJ::SystemUtil::sleep(1000 * 5);

    media_engine_destroy(mMediaEngine);

    if (testWavFile != nullptr && totalAudioCount > 0) {
        fseek(testWavFile, 0, SEEK_SET);
        WavPCMHeader header((uint16_t)audioChannels, audioSampleRate, 16, totalAudioCount);
        fwrite(&header, 1, sizeof(header), testWavFile);
        fclose(testWavFile);
        testWavFile = nullptr;
    }
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
    config.channelID = channels;
    config.userID = uid;
    config.token = token.c_str();
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

static void testLinuxRTCCreateAndDestroy() {
    while (true) {
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

        media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

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
        media_engine_start_camera_capture_with_config(mMediaEngine, "Logi C270 HD WebCam", cfgstr11.c_str(), cfgstr11.length());

        int sampleCount = 48000 / 1000 * 10;
        const char* filename = "capture_in_debug.pcm"; // PCM文件路径
        std::ifstream inFile(filename, std::ios::binary);
        if (!inFile.is_open()) {
            std::cerr << "Failed to open file: " << filename << std::endl;
            return;
        }
        int16_t* pcmBuffer = new int16_t[sampleCount]; // 缓冲区用于存放10ms的音频数据
        while (!inFile.eof()) {

            inFile.read(reinterpret_cast<char*>(pcmBuffer), sampleCount * sizeof(int16_t));
            // 检查实际读取了多少样本
            size_t bytesRead = inFile.gcount();
            size_t actualSamplesRead = bytesRead / sizeof(int16_t);
            if (actualSamplesRead > 0) {
                // 处理读取到的数据，比如打印出来或进一步处理
                //std::cout << "Read " << actualSamplesRead << " samples." << std::endl;
                // 这里可以添加处理pcmBuffer的代码
                media_engine_push_audio(mMediaEngine, reinterpret_cast<const int8_t*>(pcmBuffer), sampleCount, 48000, 1, 2);
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
}
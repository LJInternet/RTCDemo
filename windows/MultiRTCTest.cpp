#include <iostream>
#include <cstdlib> // 用于 atoi 函数
#include <cstring> // 用于 strcmp 函数
#include <sstream>
#include <string>
#include "TransConstants.h"
#include "rudp_proxy.h"
#include "VideoMediaEvent.h"
#include "MediaInvokeEvent.h"
#include "date_time.h"
#include "system_util.h"
#include "media_engine.h"
#include "MultiChannelEvent.h"

//FILE* yuvFiles = nullptr;
using namespace LJMediaLibrary;
#pragma execution_character_set("UTF-8")

static media_engine* mMediaEngine = nullptr;

static void onChannelExEventCallback(int type, const char* buf, int size,
    uint8_t* channelId, int channelIdLen, uint64_t localUid, void* context) {
    //首次建连成功或者中途断开重连成功，这个时候需要发一个I帧
    std::string bufStr(buf, size);
    if (CB_JOIN_CHANNEL == type || CB_LEAVE_CHANNEL == type) {
        MultiChannelEventResult joinResult;
        ljtransfer::mediaSox::Unpack up(bufStr.data(), size);
        joinResult.unmarshal(up);
        if (CB_JOIN_CHANNEL == type) {
            if (joinResult.result == 0) {
                printf("joinchannel success\n");
            }
            else {
                printf("joinchannel fail\n");
            }
        }
        else {
            if (joinResult.result == 0) {
                printf("leaveChannel success\n");
            }
            else {
                printf("leaveChannel fail\n");
            }
        }
    }
    else if (CB_LINK_STATUS == type) {
        LinkStatusEvent linstatus;
        ljtransfer::mediaSox::Unpack up(bufStr.data(), size);
        linstatus.unmarshal(up);
        printf("LinkStatusEvent %d\n", linstatus.result);
        
    }
    else if (MUTI_CHANNEL_REMOTE_JOIN == type) {
        MultiChannelEventResult remoteEvent;
        ljtransfer::mediaSox::Unpack up(bufStr.data(), size);
        remoteEvent.unmarshal(up);
        std::string channelIdStr((char*)channelId, channelIdLen);
        printf("MUTI_CHANNEL_REMOTE_JOIN %llu %s\n", remoteEvent.uid, channelIdStr.c_str());
    }
    else if (MUTI_CHANNEL_REMOTE_LEAVE == type) {
        MultiChannelEventResult remoteEvent;
        ljtransfer::mediaSox::Unpack up(bufStr.data(), size);
        remoteEvent.unmarshal(up);
        std::string channelIdStr((char*)channelId, channelIdLen);
        printf("MUTI_CHANNEL_REMOTE_LEAVE %llu %s\n", remoteEvent.uid, channelIdStr.c_str());
    }
    else if (RUDP_CB_TYPE_NET_REPORT == type) {
        NetworkQuality netQuality;
        ljtransfer::mediaSox::Unpack up(bufStr.data(), size);
        netQuality.unmarshal(up);
        printf("RUDP_CB_TYPE_NET_REPORT %d %d\n", netQuality.m_localQuality, netQuality.m_remoteQuality);
    }
    else if (RUDP_CB_TYPE_REQUEST_I_FRAME == type) {
        //首次建连成功或者中途断开重连成功，这个时候需要发一个I帧
        printf("RUDP_CB_TYPE_REQUEST_I_FRAME\n");
    }
}

static void onEventCallback(int type, const char* buf, int size, void* context) {
    //首次建连成功或者中途断开重连成功，这个时候需要发一个I帧
    if (type == RUDP_CB_TYPE_REQUEST_I_FRAME || type == RUDP_CB_TYPE_LINK_OK) {
        printf("onEventCallback RUDP_CB_TYPE_REQUEST_I_FRAME or RUDP_CB_TYPE_LINK_OK \n");
    }
    else if (RUDP_CB_TYPE_AVAILABLE_BW == type) {
        AvailableBands bands;
        std::string bandStr(buf, size);
        ljtransfer::mediaSox::Unpack up(bandStr.data(), size);
        bands.unmarshal(up);
        int videoBands = bands.m_availableBands[VIDEO_DATA];
    }
}

static void onChannelExDecodeVideo(uint8_t* buf, int32_t len, int32_t width,
    int32_t height, int pixel_fmt, uint8_t* channelId, int channelIdLen, uint64_t uid, uint64_t localUid, void* context) {
    //printf("onChannelExDecodeVideo channelId %s localUid %llu uid %llu WH %dX%d pixel_fmt %d\n",
    //    channelId, localUid, uid, width, height, pixel_fmt);
}

int main(int argc, char** argv) {

    int role = 0;
    std::string channelId = "954523222";
    uint64_t appId = 1;
    std::string token = "token";
    uint64_t uid = LJ::DateTime::currentTimeMillis();

    for (int i = 1; i < argc; ++i) {
        const char* arg = argv[i]; // 将 std::string 转换为 const char*
        std::istringstream iss(arg); // 使用 const char* 初始化 istringstream
        std::string key;
        std::string value;

        // 以等号分割参数名和值
        std::getline(iss, key, '=');

        // 从等号后的字符开始读取参数值
        std::getline(iss, value);

        // 去除值前的空格
        value.erase(0, value.find_first_not_of(' '));

        // 根据参数名进行赋值
        if (key == "channelId") {
            channelId = value;
        }
        else if (key == "uid") {
            uid = std::stoul(value);
        }
        else if (key == "appId") {
            appId = std::stoul(value);
        }
        else if (key == "token") {
            token = value;
        }
        else if (key == "role") {
            role = std::stoi(value);
        }
    }

    RTCEngineConfig rtc_config;
    // 是否把日志写入文件，目前日志文件默认存储在项目运行的根目录中，名字叫debug_unity.txt
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    // 创建RTCEngine
    media_engine* nginx = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
    // 设置为运行在测试环境
    media_engine_set_debug_env(true);

    MIEUploadConfig c;
    MIEVideoUploadConfig videoUploadConfig;
    videoUploadConfig.encodeWidth = 640;
    videoUploadConfig.encodeHeight = 480;
    videoUploadConfig.maxVideoBitrateInbps = 800000;
    videoUploadConfig.minVideoBitrateInbps = 700000;
    videoUploadConfig.realVideoBitrateInbps = 800000;
    c.videoUploadConfig = videoUploadConfig;
    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    // 设置推流视频宽高和码率
    media_engine_send_event(nginx, MIET_UPDATE_UPLOAD_CONFIG, (char*)cfgstr.c_str(), cfgstr.length());
    // 注册基础的RTC事件回调，主要处理RUDP_CB_TYPE_REQUEST_I_FRAME和RUDP_CB_TYPE_LINK_OK
    media_engine_register_event_listener(nginx, onEventCallback, nginx);
    // 注册多人RTC特有的事件回调，主要处理有用户加入或者退出频道，自己RTC链接的状态
    media_engine_register_event_ex_listener(nginx, onChannelExEventCallback, nginx);
    // 注册解码后YUV数据
    media_engine_subscribe_video_ex(nginx, onChannelExDecodeVideo, nginx);
    // 创建多人RTC的音视频配置
    ChannelMediaOptions option;
    option.autoSubscribeAudio = true;
    // 创建多人RTC加入频道所需参数，其中key是channelId+uid的string
    JoinChannelExConfig channelExConfig;
    channelExConfig.appId = appId;
    channelExConfig.isDebug = true;
    channelExConfig.channelId = channelId;
    channelExConfig.key = std::string(channelId.c_str()).append(std::to_string(uid));
    channelExConfig.uid = uid;
    channelExConfig._option = option;
    channelExConfig._token = token;

    std::string joinExStr;
    ljtransfer::mediaSox::PacketToString(channelExConfig, joinExStr);
    media_engine_send_event(nginx, JOIN_CHANNEL_EX, (char*)joinExStr.c_str(), joinExStr.length());

    LJ::SystemUtil::sleep(20 * 50000);

    // 离开多人RTC频道
    MultiChannelEvent multiChannelEvent;
    std::string leaveExStr;
    ljtransfer::mediaSox::PacketToString(multiChannelEvent, leaveExStr);
    media_engine_send_event(nginx, LEAVE_CHANNEL_EX, (char*)leaveExStr.c_str(), leaveExStr.length());
    // 销毁多人RTC频道
    media_engine_destroy(nginx);
}
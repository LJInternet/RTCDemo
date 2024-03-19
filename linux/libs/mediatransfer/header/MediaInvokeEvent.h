#pragma once
namespace LJMediaLibrary
{

enum MediaInvokeEventType
{
    MIET_INVALID                                                           = 0,
    MIET_CREATE                                                            = 1,
    MIET_RELEASE                                                           = 2,
    MIET_WEB_MSG_EVENT                                                     = 10,
    AUDIO_PLAYER_EVENT                                                     = 11,
    MIET_MUTE_MEDIA_EVENT                                                  = 80,
    MIET_UPDATE_UPLOAD_CONFIG                                              = 90,
    MIET_SOFT_DECODE                                                       = 100,
    MIET_SOFT_DECODE_SURFACE                                               = 101,
    SET_CLIENT_ROLE = 102,
    JOIN_CHANNEL = 103,
    LEAVE_CHANNEL = 104,
    AUDIO_CREATE = 105, // 创建音频模块
    AUDIO_DESTROY = 106, // 销毁音频模块
    SUBSCRIBE_CAPTURE_AUDIO = 107, // 订阅采集音频
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
    MULTI_CHANNEL_EVENT_END = 299,
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
    ///////////////////////video Events Start //////////////////////
    VIDEO_CAMERA_BUSY_INFO = 2101, // 获取设备信息
    ///////////////////////video Events End //////////////////////
    //////////////////////testEvent/////////////////////////////
    TEST_SAVE_VIDEO_FILE = 10001, //
    //////////////////////testEvent/////////////////////////////

};
} // namespace LJMediaLibrary

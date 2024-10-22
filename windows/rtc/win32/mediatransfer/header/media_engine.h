//
// Created by Administrator on 2023/1/9.
//
/**
 * @file media_engine.h
 * @brief This file contains the definition of the media engine interface.
 */

#ifndef LJSDK_MEDIA_ENGINE_H
#define LJSDK_MEDIA_ENGINE_H

#include <stdint.h>
#include <map>
#include "transfer_common.h"

/**
 * @defgroup PixelFormats Pixel Formats
 * @brief Constants representing different pixel formats.
 * @{
 */
#define PIXEL_FMT_RGBA      1 /**< RGBA pixel format. */
#define PIXEL_FMT_YUV_I420  2 /**< YUV I420 pixel format. */
#define PIXEL_FMT_NV21      3 /**< NV21 pixel format. */
#define PIXEL_FMT_RGB24     4 /**< RGB24 pixel format. */
#define PIXEL_FMT_NV12      5 /**< NV12 pixel format. */
#define PIXEL_FMT_YUY2      6 /**< YUY2 pixel format. */
#define PIXEL_FMT_H264      7 /**< H.264 pixel format. */
#define PIXEL_FMT_H265      8 /**< H.265 pixel format. */
/** @} */

/**
 * @defgroup AudioCallbacks Audio Callbacks
 * @brief Constants representing different audio callback types.
 * @{
 */
#define AUDIO_CALLBACK_MIC         1 /**< Microphone audio callback. */
#define AUDIO_CALLBACK_SUBMIX      2 /**< Submix audio callback. */
#define AUDIO_CALLBACK_MIXED       3 /**< Mixed audio callback. */
#define AUDIO_CALLBACK_DECODE      4 /**< Audio decode callback. */
#define AUDIO_CALLBACK_DECODE_EX   5 /**< Extended audio decode callback. */
/** @} */

/**
 * @defgroup VideoCallbacks Video Callbacks
 * @brief Constants representing different video callback types.
 * @{
 */
#define VIDEO_CALLBACK_ENCODE           50 /**< Video encode callback. */
#define VIDEO_CALLBACK_DECODE           51 /**< Video decode callback. */
#define VIDEO_CALLBACK_CAPTURE          52 /**< Video capture callback. */
#define VIDEO_CALLBACK_SCREEN_CAPTURE   53 /**< Screen capture callback. */
#define VIDEO_CALLBACK_DECODE_EX        54 /**< Extended video decode callback. */
/** @} */

#ifdef __cplusplus
extern "C" {
#endif

    /**
     * @brief Structure representing a video frame that needs to be decoded.
     */
    struct CNeedDecodeVideoFrame {
        uint8_t* buf; /**< Pointer to the video frame buffer. */
        int32_t len; /**< Length of the video frame buffer. */
        int32_t width; /**< Width of the video frame. */
        int32_t height; /**< Height of the video frame. */
        int32_t frameType; /**< Type of the video frame (VideoFrameType). */
        uint64_t pts; /**< Presentation timestamp of the video frame. */
        int32_t pixel_fmt; /**< Pixel format of the video frame (e.g., PIXEL_FMT_H264 or PIXEL_FMT_H265). */
        std::map<uint64_t, uint64_t> delayData; /**< Delay data associated with the video frame. */

        /**
         * @brief Constructor.
         */
        CNeedDecodeVideoFrame() : width(0), height(0), frameType(0), pts(0), pixel_fmt(0), buf(nullptr), len(0) {}

        /**
         * @brief Destructor.
         */
        ~CNeedDecodeVideoFrame() {
            buf = nullptr;
            delayData.clear();
        }
    };

    /**
     * @brief Structure representing transfer delay information.
     */
    struct TransDelayInfo {
        uint64_t id; /**< Identifier. */
        uint64_t value; /**< Value. */
    };

    /**
     * @brief Structure representing a list of transfer delay information.
     */
    struct TransDelayInfoList {
        TransDelayInfo* timeInfos; /**< Array of transfer delay information. */
        uint32_t size; /**< Size of the array. */

        /**
         * @brief Constructor.
         */
        TransDelayInfoList() : timeInfos(nullptr), size(0) {}
    };

    /**
     * @brief Media engine structure.
     */
    MEDIATRANSFER_EXTERN struct media_engine;

    /**
     * @brief Callback for receiving video data.
     */
    MEDIATRANSFER_EXTERN typedef void (*video_data_cb)(uint8_t* buf, int32_t len, int32_t width,
                                                       int32_t height, int pixel_fmt, void* context);

    /**
    * @brief Callback for receiving video data.
    */
    MEDIATRANSFER_EXTERN typedef void (*video_cb_with_pts)(uint8_t* buf, int32_t len, int32_t width,
                                                       int32_t height, int pixel_fmt, uint32_t pts, void* context);

    /**
     * @brief Callback for receiving audio data.
     */
    MEDIATRANSFER_EXTERN typedef bool (*audio_data_cb)(void *audioData, int size, uint64_t pts,
                                                       int sampleRate, int channelCont, void* context);

    /**
     * @brief Callback for receiving video data with delay information.
     */
    MEDIATRANSFER_EXTERN typedef void (*video_cb_with_delay)(uint8_t* buf, int32_t len, int32_t width,
                                                             int32_t height, int pixel_fmt, std::map<uint64_t, uint64_t> delayData, void* context);

    /**
     * @brief Callback for receiving undecoded video frames.
     */
    MEDIATRANSFER_EXTERN typedef void (*video_undecode_cb)(CNeedDecodeVideoFrame videoFrame, void* context);

    /**
     * @brief Callback for receiving extended audio data.
     */
    MEDIATRANSFER_EXTERN typedef void (*audio_data_ex_cb)(void *audioData, int size, uint64_t pts,
                                                          int sampleRate, int channelCont, uint8_t *channelId, int channelIdLen, uint64_t uid, void* context);

    /**
     * @brief Callback for receiving extended video data.
     */
    MEDIATRANSFER_EXTERN typedef void (*video_data_ex_cb)(uint8_t* buf, int32_t len, int32_t width,
                                                          int32_t height, int pixel_fmt, uint8_t *channelId, int channelIdLen, uint64_t uid, uint64_t localUid, void* context);

    /**
     * @brief Callback for receiving extended events.
     */
    MEDIATRANSFER_EXTERN typedef void (*event_ex_cb)(int type, const char *buf, int size,
                                                     uint8_t *channelId, int channelIdLen, uint64_t localUid, void* context);

    /**
     * @brief Callback for receiving events.
     */
    MEDIATRANSFER_EXTERN typedef void (*event_cb)(int type, const char *buf, int size, void* context);

    /**
     * @brief Callback for receiving video delay information.
     */
    MEDIATRANSFER_EXTERN typedef void (*video_delay_callback)(int totalDelay, int decodeDelay, int encodeDelay,
                                                              int reciveDelay, int transDelay, int cacheCount, int fps, void * context);

    /**
     * @brief Set whether to use the debug environment for XMTP.
     * @param debug Flag indicating whether to use the debug environment.
     */
    MEDIATRANSFER_EXTERN void media_engine_set_debug_env(bool debug);

    /**
     * @brief Create a media engine.
     * @param buf Buffer containing configuration data.
     * @param size Size of the buffer.
     * @return Pointer to the created media engine.
     */
    MEDIATRANSFER_EXTERN struct media_engine* media_engine_create(const char* buf, int size);
    /**
     * @brief 销毁RTC engine
     */
    MEDIATRANSFER_EXTERN void media_engine_destroy(struct media_engine*);

    /**
     * @brief 推送采集数据
     * @param engine RTC引擎指针
     * @param buf 数据缓冲区指针
     * @param size 数据大小
     * @param msg CaptureVideoFrame的序列号
     * @param msgSize msg数据大小
     * @param pixel_fmt 像素格式
     */
    MEDIATRANSFER_EXTERN void media_engine_push_video(struct media_engine* engine, const char *buf,
                                                      int size, const char *msg, int msgSize, int pixel_fmt);

    MEDIATRANSFER_EXTERN void media_engine_push_raw_video(struct media_engine* engine, const char *buf,
            int size, int width, int height, int rotation, uint64_t timeStamp, int pixel_fmt);

    /**
     * @brief 获取相机列表
     * @param out_buf 输出缓冲区指针
     * @param len 缓冲区长度
     * @return 返回相机列表的数量
     */
    MEDIATRANSFER_EXTERN int media_engine_camera_list(char* out_buf, int len);

    /**
     * @brief 打开相机
     * @param engine RTC引擎指针
     * @param deviceName 设备名称
     * @param width 视频宽度
     * @param height 视频高度
     * @param fps 视频帧率
     */
    MEDIATRANSFER_EXTERN void media_engine_start_camera_capture(struct media_engine* engine,
                                                                char* deviceName, int width, int height, int fps);

    /**
     * @brief 使用配置打开相机
     * @param engine RTC引擎指针
     * @param deviceName 设备名称
     * @param config 配置数据指针
     * @param length 配置数据长度
     */
    MEDIATRANSFER_EXTERN void media_engine_start_camera_capture_with_config(struct media_engine* engine,
                                                                            char* deviceName, const char* config, int length);

    /**
     * @brief 停止相机
     * @param engine RTC引擎指针
     */
    MEDIATRANSFER_EXTERN void media_engine_stop_camera_capture(struct media_engine* engine);

    /**
     * @brief 订阅采集的视频
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_capture_video(struct media_engine* engine, video_data_cb cb, void* context);

    /**
     * @brief 发送pcm音频
     * @param engine RTC引擎指针
     * @param pcm 音频数据指针
     * @param frame_num 每个channel的音频采样数
     * @param sampleRate 采样率
     * @param channelCount 声道数
     * @param bytePerSample 每个采样的字节数
     */
    MEDIATRANSFER_EXTERN void media_engine_push_audio(struct media_engine* engine, const int8_t *pcm,
                                                      int frame_num, int sampleRate, int channelCount, int bytePerSample);

    MEDIATRANSFER_EXTERN void media_engine_push_audio_with_timestamp(struct media_engine* engine, const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample, uint64_t timestamp);

    /**
     * @brief 订阅视频解码数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_video(struct media_engine* engine, video_data_cb cb, void* context);

    MEDIATRANSFER_EXTERN void media_engine_subscribe_video_with_pts(struct media_engine* engine, video_cb_with_pts cb, void* context);

    /**
     * @brief 订阅视频解码数据（带延迟）
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_video_with_delay(struct media_engine* engine, video_cb_with_delay cb, void* context);

    /**
     * @brief 订阅H264裸流数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param uid 用户ID
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_H264_video(struct media_engine* engine, video_data_cb cb, uint64_t uid, void* context);

    /**
     * @brief 发送H264裸流数据
     * @param engine RTC引擎指针
     * @param buf 数据缓冲区指针
     * @param len 数据大小
     */
    MEDIATRANSFER_EXTERN void media_engine_push_H264_video(struct media_engine* engine, uint8_t* buf, int32_t len);

    /**
     * @brief 发送编码后的裸流数据
     * @param engine RTC引擎指针
     * @param width 视频宽度
     * @param height 视频高度
     * @param frameType 帧类型
     * @param pts 时间戳
     * @param codecType 编码类型
     * @param buf 数据缓冲区指针
     * @param len 数据大小
     * @param iTsInfos 延时统计数据
     */
    MEDIATRANSFER_EXTERN void media_engine_push_encode_video(struct media_engine* engine, int width, int height, int frameType,
                                                             int pts, int  codecType, uint8_t* buf, int32_t len, TransDelayInfoList* iTsInfos);

    /**
     * @brief 订阅未解码视频数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_undecode_video(struct media_engine* engine, video_undecode_cb cb, void* context);

    /**
     * @brief 订阅视频解码延时数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_video_delay_info(struct media_engine* engine, video_delay_callback cb, void* context);

    /**
     * @brief 订阅解码音频
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_audio(struct media_engine* engine, audio_data_cb cb, void* context);
    /**
     * @brief 订阅采集音频
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_capture_audio(struct media_engine* engine, audio_data_cb cb, void* context);

    /**
     * @brief 订阅麦克风数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_mic_audio(struct media_engine* engine, audio_data_cb cb, void* context);

    /**
     * @brief 订阅内录音频
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_submix_audio(struct media_engine* engine, audio_data_cb cb, void* context);

    /**
     * @brief 注销订阅的回调
     * @param engine RTC引擎指针
     * @param type 类型
     */
    MEDIATRANSFER_EXTERN void media_engine_unsubscribe_callback(struct media_engine* engine, int type);

    /**
     * @brief 发送远端的可用带宽
     * @param engine RTC引擎指针
     * @param bm 带宽
     */
    MEDIATRANSFER_EXTERN void media_engine_send_remote_bm(struct media_engine* engine, int bm);

    /**
     * @brief 发送远端请求I帧的命令
     * @param engine RTC引擎指针
     */
    MEDIATRANSFER_EXTERN void media_engine_request_remote_I_frame(struct media_engine* engine);

    /**
     * @brief 订阅编码后的视频数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_encoded_video(struct media_engine* engine, video_data_cb cb, void* context);

    /**
     * @brief 发送控制事件
     * @param engine RTC引擎指针
     * @param event_type 事件类型
     * @param mediaData 媒体数据
     * @param len 数据长度
     * @return 返回执行结果
     */
    MEDIATRANSFER_EXTERN int media_engine_send_event(struct media_engine* engine, int event_type, char* mediaData, int len);

    /**
     * @brief 注册native事件回调
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_register_event_listener(struct media_engine* engine, event_cb cb, void* context);

    /**
     * @brief 获取native数据
     * @param engine RTC引擎指针
     * @param event_type 事件类型
     * @param mediaData 媒体数据
     * @param len 数据长度
     * @param retLen 返回长度指针
     * @return 返回数据指针
     */
    MEDIATRANSFER_EXTERN char* media_engine_get_event(struct media_engine* engine, int event_type, char* mediaData, int len, int * retLen);

    /**
     * @brief 订阅解码视频数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_video_ex(struct media_engine* engine, video_data_ex_cb cb, void* context);

    /**
     * @brief 订阅解码音频
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_audio_ex(struct media_engine* engine, audio_data_ex_cb cb, void* context);

    /**
     * @brief 注册native事件回调
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_register_event_ex_listener(struct media_engine* engine, event_ex_cb cb, void* context);

    /**
     * @brief 发送音频
     * @param engine RTC引擎指针
     * @param pcm PCM音频数据指针
     * @param frame_num 帧数
     * @param sampleRate 采样率
     * @param channelCount 通道数
     * @param bytePerSample 每样本字节数
     * @param key 键值
     * @return 返回执行结果
     */
    MEDIATRANSFER_EXTERN int media_engine_push_audio_ex(struct media_engine* engine, const int8_t *pcm,
                                                        int frame_num, int sampleRate, int channelCount, int bytePerSample, const char *key);

    /**
     * @brief 发送视频
     * @param engine RTC引擎指针
     * @param buf 视频数据缓冲区指针
     * @param size 数据大小
     * @param msg 附加消息
     * @param msgSize 消息大小
     * @param pixel_fmt 像素格式
     * @param key 键值
     * @return 返回执行结果
     */
    MEDIATRANSFER_EXTERN int media_engine_push_video_ex(struct media_engine* engine, const char *buf,
                                                        int size, const char *msg, int msgSize, int pixel_fmt, const char *key);

    /**
     * @brief 订阅录屏视频数据
     * @param engine RTC引擎指针
     * @param cb 回调函数指针
     * @param context 上下文指针
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_screen_capture_video(struct media_engine* engine, video_data_cb cb, void* context);

    /**
     * @brief 开始内录
     * @param engine RTC引擎指针
     * @param enableAudio 是否启用音频
     * @param enableVideo 是否启用视频
     * @param width 视频宽度
     * @param height 视频高度
     * @param fps 帧率
     * @param bitrate 比特率
     */
    MEDIATRANSFER_EXTERN void media_engine_start_screen_capture(struct media_engine* engine,
                                                                int enableAudio, int enableVideo, int width, int height, int fps, int bitrate);

    /**
     * @brief 停止内录
     * @param engine RTC引擎指针
     */
    MEDIATRANSFER_EXTERN void media_engine_stop_screen_capture(struct media_engine* engine);

    /**
     * @brief 设置解码配置
     * @param engine RTC引擎指针
     * @param decodeType 解码类型
     * @param isLowLatency 是否低延迟
     */
    MEDIATRANSFER_EXTERN void media_engine_set_decode_config(struct media_engine* engine, int decodeType, int isLowLatency);

    MEDIATRANSFER_EXTERN uint64_t media_engine_get_media_base_time();

    MEDIATRANSFER_EXTERN void request_local_I_Frame(struct media_engine* engine);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_MEDIA_ENGINE_H
/*@}*/

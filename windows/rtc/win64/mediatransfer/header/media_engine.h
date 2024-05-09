//
// Created by Administrator on 2023/1/9.
//

#ifndef LJSDK_MEDIA_ENGINE_H
#define LJSDK_MEDIA_ENGINE_H

#include <stdint.h>
#include <map>
#include "transfer_common.h"

#define PIXEL_FMT_RGBA 		1
#define PIXEL_FMT_YUV_I420  2
#define PIXEL_FMT_NV21 		3
#define PIXEL_FMT_RGB24 	4
#define PIXEL_FMT_NV12 		5
#define PIXEL_FMT_YUY2 		6
#define PIXEL_FMT_H264 		7
#define PIXEL_FMT_H265 		8
#define AUDIO_CALLBACK_MIC 		1
#define AUDIO_CALLBACK_SUBMIX 	2
#define AUDIO_CALLBACK_MIXED 	3
#define AUDIO_CALLBACK_DECODE 	4
#define AUDIO_CALLBACK_DECODE_EX 	5
#define VIDEO_CALLBACK_ENCODE 50
#define VIDEO_CALLBACK_DECODE 51
#define VIDEO_CALLBACK_CAPTURE 52
#define VIDEO_CALLBACK_SCREEN_CAPTURE 53

#define VIDEO_CALLBACK_DECODE_EX 54

#ifdef __cplusplus
extern "C" {
#endif
	struct CNeedDecodeVideoFrame {
		uint8_t* buf;
		int32_t len;
		int32_t width;
		int32_t height;
		int32_t frameType; // VideoFrameType
		uint64_t pts;
		int32_t pixel_fmt; // PIXEL_FMT_H264 or PIXEL_FMT_H265
		std::map<uint64_t, uint64_t> delayData;
		CNeedDecodeVideoFrame() :width(0), height(0), frameType(0), pts(0), pixel_fmt(0), buf(nullptr), len(0) {

		}
		~CNeedDecodeVideoFrame() {
			buf = nullptr;
			delayData.clear();
		}
	};

struct TransDelayInfo {
	uint64_t id;
	uint64_t value;
};

struct TransDelayInfoList {
	TransDelayInfo* timeInfos;
	uint32_t size;
	TransDelayInfoList() : timeInfos(NULL), size(0) {}
};

	MEDIATRANSFER_EXTERN struct media_engine;

	MEDIATRANSFER_EXTERN typedef void (*video_data_cb)(uint8_t* buf, int32_t len, int32_t width,
			int32_t height, int pixel_fmt, void* context);

	MEDIATRANSFER_EXTERN typedef bool (*audio_data_cb)(void *audioData, int size, uint64_t pts,
			int sampleRate, int channelCont, void* context);
	MEDIATRANSFER_EXTERN typedef void (*video_cb_with_delay)(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, std::map<uint64_t, uint64_t> delayData, void* context);

	MEDIATRANSFER_EXTERN typedef void (*video_undecode_cb)(CNeedDecodeVideoFrame videoFrame, void* context);

    MEDIATRANSFER_EXTERN typedef void (*audio_data_ex_cb)(void *audioData, int size, uint64_t pts,
    		int sampleRate, int channelCont,uint8_t *channelId, int channelIdLen, uint64_t uid, void* context);

    MEDIATRANSFER_EXTERN typedef void (*video_data_ex_cb)(uint8_t* buf, int32_t len, int32_t width,
            int32_t height, int pixel_fmt, uint8_t *channelId, int channelIdLen, uint64_t uid, uint64_t localUid, void* context);

	MEDIATRANSFER_EXTERN typedef void (*event_ex_cb)(int type, const char *buf, int size,
			uint8_t *channelId, int channelIdLen, uint64_t localUid, void* context);

	MEDIATRANSFER_EXTERN typedef void (*event_cb)(int type, const char *buf, int size, void* context);

	MEDIATRANSFER_EXTERN typedef void (*video_delay_callback)(int totalDelay, int decodeDelay, int encodeDelay, int reciveDelay, int transDelay, int fps, void * context);
	/**
	 * 设置xmtp是正式环境还是测试环境
	 * @param debug
	 */
	MEDIATRANSFER_EXTERN void media_engine_set_debug_env(bool debug);

    /**
     * 创建RTC engine
     */
	MEDIATRANSFER_EXTERN struct media_engine* media_engine_create(const char* buf, int size);
    /**
     * 销毁RTC engine
     */
	MEDIATRANSFER_EXTERN void media_engine_destroy(struct media_engine*);
    /**
     * 推送采集数据
     * msg 是CaptureVideoFrame的序列号
     * CaptureVideoFrame frame;
     * frame.stride = 640;
     * frame.width = 640;
     * frame.height = 480;
     * frame.rotation = 0;
     * frame.type = VIDEO_BUFFER_RAW_DATA;
     * frame.format = VIDEO_PIXEL_I420; // 数据格式
     * frame.timestamp = LJ::DateTime::currentTimeMillis();
     * frame.mirror = 0;
     * std::string data;
     * ljtransfer::mediaSox::PacketToString(frame, data);
     */
	MEDIATRANSFER_EXTERN void media_engine_push_video(struct media_engine* engine, const char *buf,
			int size, const char *msg, int msgSize, int pixel_fmt);
    /**
    * 获取相机列表
    */
	MEDIATRANSFER_EXTERN int media_engine_camera_list(char* out_buf, int len);
    /**
    * 打开相机
    */
	MEDIATRANSFER_EXTERN void media_engine_start_camera_capture(struct media_engine* engine,
			char* deviceName, int width, int height, int fps);
	MEDIATRANSFER_EXTERN void media_engine_start_camera_capture_with_config(struct media_engine* engine,
			char* deviceName, const char* config, int length);
    /**
    * 停止相机
    */
	MEDIATRANSFER_EXTERN void media_engine_stop_camera_capture(struct media_engine* engine);
    /**
    * 订阅采集的视频
    */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_capture_video(struct media_engine* engine, video_data_cb cb, void* context);
    /**
     * 发送pcm音频
     * @param engine
     * @param pcm
     * @param frame_num 每个channel的音频采样数
     * @param sampleRate 采样率
     * @param channelCount 声道数
     * @param bytePerSample Int16 2 int8 1 int32 4
     */
	MEDIATRANSFER_EXTERN void media_engine_push_audio(struct media_engine* engine, const int8_t *pcm,
			int frame_num, int sampleRate, int channelCount, int bytePerSample);
    /**
     * 订阅视频解码数据
     * @param engine
     * @param cb
     * @param context
     */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_video(struct media_engine* engine, video_data_cb cb, void* context);

	/**
	* 订阅视频解码数据
	* @param engine
	* @param cb
	* @param context
	*/
	MEDIATRANSFER_EXTERN void media_engine_subscribe_video_with_delay(struct media_engine* engine, video_cb_with_delay cb, void* context);
    /**
     * 订阅H264裸流数据
     */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_H264_video(struct media_engine* engine, video_data_cb cb, uint64_t uid, void* context);
	/**
	 * 发送H264裸流数据
	 * @param engine
	 * @param buf
	 * @param len
	 */
	MEDIATRANSFER_EXTERN void media_engine_push_H264_video(struct media_engine* engine, uint8_t* buf, int32_t len);

	/**
	* 发送编码后的裸流数据
	* @param engine
	* @param frameType @see VideoFrameType
	* @param codecType @see ENTYPE_H264 or ENTYPE_H265
	* @param iTsInfos 延时统计数据，应该包含采集开始 采集结束 编码开始 编码结束事件 @see DelayConstants
	*/
	MEDIATRANSFER_EXTERN void media_engine_push_encode_video(struct media_engine* engine, int width, int height, int frameType,
			int pts, int  codecType, uint8_t* buf, int32_t len, TransDelayInfoList* iTsInfos);

	/**
	* 订阅未解码视频数据
	* @param engine
	* @param cb
	* @param context
	*/
	MEDIATRANSFER_EXTERN void media_engine_subscribe_undecode_video(struct media_engine* engine, video_undecode_cb cb, void* context);

	/**
	* 订阅视频解码延时数据
	* @param engine
	* @param cb
	* @param context
	*/
	MEDIATRANSFER_EXTERN void media_engine_subscribe_video_delay_info(struct media_engine* engine, video_delay_callback cb, void* context);
	/**
	 * 订阅解码音频
	 * @param engine
	 * @param cb
	 * @param context
	 */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_audio(struct media_engine* engine, audio_data_cb cb, void* context);
    /**
     * 订阅采集音频
     * @param engine
     * @param cb
     * @param context
     */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_capture_audio(struct media_engine* engine, audio_data_cb cb, void* context);
    /**
    * 订阅麦克风数据
    * @param engine
    * @param cb
    * @param context
    */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_mic_audio(struct media_engine* engine, audio_data_cb cb, void* context);
	/**
	 * 订阅内录音频
	 * @param engine
	 * @param cb
	 * @param context
	 */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_submix_audio(struct media_engine* engine, audio_data_cb cb, void* context);
	/**
	 * 注销订阅的回调
	 * @param engine
	 * @param type
	 */
	MEDIATRANSFER_EXTERN void media_engine_unsubscribe_callback(struct media_engine* engine, int type);

	/**
	 * 发送远端的可用带宽
	 * @param engine
	 * @param type
	 */
    MEDIATRANSFER_EXTERN void media_engine_send_remote_bm(struct media_engine* engine, int bm);
    /**
    * 发送远端的可用带宽
    * @param engine
    * @param type
    */
    MEDIATRANSFER_EXTERN void media_engine_request_remote_I_frame(struct media_engine* engine);
    /**
     * 订阅编码后的视频数据
     * @param engine
     * @param cb
     * @param context
     */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_encoded_video(struct media_engine* engine, video_data_cb cb, void* context);
    /**
     * 发送控制事件
     * @param engine
     * @param event_type
     * @param mediaData
     * @param len
     * @return
     */
	MEDIATRANSFER_EXTERN int media_engine_send_event(struct media_engine* engine, int event_type, char* mediaData, int len);

	/**
	 * 注册native事件回调
	 * @param engine
	 * @param cb
	 * @param context
	 */
	MEDIATRANSFER_EXTERN void media_engine_register_event_listener(struct media_engine* engine, event_cb cb, void* context);

	/**
	 * 获取native数据
	 * @param engine
	 * @param event_type
	 * @param mediaData
	 * @param len
	 * @param retLen
	 * @return
	 */
	MEDIATRANSFER_EXTERN char* media_engine_get_event(struct media_engine* engine, int event_type, char* mediaData, int len, int * retLen);
    /**
     * 订阅解码视频数据
     * @param engine
     * @param cb
     * @param context
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_video_ex(struct media_engine* engine, video_data_ex_cb cb, void* context);

    /**
     * 订阅解码音频
     * @param engine
     * @param cb
     * @param context
     */
    MEDIATRANSFER_EXTERN void media_engine_subscribe_audio_ex(struct media_engine* engine, audio_data_ex_cb cb, void* context);

	/**
	 * 注册native事件回调
	 * @param engine
	 * @param cb
	 * @param context
	 */
	MEDIATRANSFER_EXTERN void media_engine_register_event_ex_listener(struct media_engine* engine, event_ex_cb cb, void* context);
    /**
     * 发送音频
     * @param engine
     * @param pcm
     * @param frame_num
     * @param sampleRate
     * @param channelCount
     * @param bytePerSample
     * @param key
     * @return
     */
    MEDIATRANSFER_EXTERN int media_engine_push_audio_ex(struct media_engine* engine, const int8_t *pcm,
    		int frame_num, int sampleRate, int channelCount, int bytePerSample, const char *key);

	/**
	* 发送视频
	* @return
	*/
	MEDIATRANSFER_EXTERN int media_engine_push_video_ex(struct media_engine* engine, const char *buf,
			int size, const char *msg, int msgSize, int pixel_fmt, const char *key);

    /**
     * 订阅录屏视频数据
     * @param engine
     * @param cb
     * @param context
     */
	MEDIATRANSFER_EXTERN void media_engine_subscribe_screen_capture_video(struct media_engine* engine, video_data_cb cb, void* context);
	/**
	 * 开始内录
	 * @param engine
	 * @param enableAudio
	 * @param enableVideo
	 * @param width
	 * @param height
	 * @param fps
	 * @param bitrate
	 */
	MEDIATRANSFER_EXTERN void media_engine_start_screen_capture(struct media_engine* engine,
			int enableAudio, int enableVideo, int width, int height, int fps, int bitrate);
	/**
	 * 停止内录
	 * @param engine
	 */
	MEDIATRANSFER_EXTERN void media_engine_stop_screen_capture(struct media_engine* engine);

	/**
	 * 设置解码配置
	 * @param engine
	 */
	MEDIATRANSFER_EXTERN void media_engine_set_decode_config(struct media_engine* engine, int decodeType, int isLowLatency);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_MEDIA_ENGINE_H

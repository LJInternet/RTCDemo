//
// Created by Administrator on 2023/1/9.
//

#ifndef LJSDK_MEDIA_ENGINE_H
#define LJSDK_MEDIA_ENGINE_H

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(BUILDING_MEDIATRANSFER_SHARED)
    /* Building shared library. */
#   define MEDIATRANSFER_EXTERN __declspec(dllexport)
# elif defined(USING_MEDIATRANSFER_SHARED)
    /* Using shared library. */
#   define MEDIATRANSFER_EXTERN __declspec(dllimport)
# else
    /* Building static library. */
#   define MEDIATRANSFER_EXTERN /* nothing */
# endif
#elif __GNUC__ >= 4
# define MEDIATRANSFER_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define MEDIATRANSFER_EXTERN __global
#else
# define MEDIATRANSFER_EXTERN /* nothing */
#endif

#include <stdint.h>

#define PIXEL_FMT_RGBA 		1
#define PIXEL_FMT_YUV_I420  2
#define PIXEL_FMT_NV21 		3
#define PIXEL_FMT_RGB24 	4
#define PIXEL_FMT_NV12 		5

#ifdef __cplusplus
extern "C" {
#endif

	MEDIATRANSFER_EXTERN struct media_engine;

	MEDIATRANSFER_EXTERN typedef void (*video_data_cb)(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, void* context);

	MEDIATRANSFER_EXTERN typedef void (*audio_data_cb)(void *audioData, int size, long pts, int sampleRate, int channelCont, void* context);

	MEDIATRANSFER_EXTERN typedef void (*event_cb)(int type, const char *buf, int size, void* context);

	MEDIATRANSFER_EXTERN struct media_engine* media_engine_create(const char* buf, int size);

	MEDIATRANSFER_EXTERN void media_engine_destroy(struct media_engine*);

	//pixel_fmt: RGBA=1 YUV420=2 NV21=3
	MEDIATRANSFER_EXTERN void media_engine_push_video(struct media_engine* engine, const char *buf, int size, const char *msg, int msgSize, int pixel_fmt);
	
	MEDIATRANSFER_EXTERN int media_engine_camera_list(char* out_buf, int len);

	MEDIATRANSFER_EXTERN void media_engine_start_camera_capture(struct media_engine* engine, char* deviceName, int width, int height, int fps);

	MEDIATRANSFER_EXTERN void media_engine_stop_camera_capture(struct media_engine* engine);

	MEDIATRANSFER_EXTERN void media_engine_subscribe_capture_video(struct media_engine* engine, video_data_cb cb, void* context);

	MEDIATRANSFER_EXTERN void media_engine_push_audio(struct media_engine* engine, const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample);

	MEDIATRANSFER_EXTERN void media_engine_subscribe_video(struct media_engine* engine, video_data_cb cb, void* context);

	MEDIATRANSFER_EXTERN void media_engine_subscribe_audio(struct media_engine* engine, audio_data_cb cb, void* context);

	MEDIATRANSFER_EXTERN void media_engine_subscribe_capture_audio(struct media_engine* engine, audio_data_cb cb, void* context);

	MEDIATRANSFER_EXTERN void media_engine_subscribe_encoded_video(struct media_engine* engine, video_data_cb cb, void* context);

	MEDIATRANSFER_EXTERN int media_engine_send_event(struct media_engine* engine, int event_type, char* mediaData, int len);

	MEDIATRANSFER_EXTERN void media_engine_register_event_listener(struct media_engine* engine, event_cb cb, void* context);

	MEDIATRANSFER_EXTERN char* media_engine_get_event(struct media_engine* engine, int event_type, char* mediaData, int len, int& retLen);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_MEDIA_ENGINE_H

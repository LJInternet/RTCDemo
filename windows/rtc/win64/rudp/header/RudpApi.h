//
// Created by Administrator on 2023/5/11.
//

#ifndef LJSDK_RUDPAPI_H
#define LJSDK_RUDPAPI_H
#include "ClientConstants.h"



#ifdef __cplusplus
extern "C" {
#endif

#ifndef RUDP_EXPORT_API
#define RUDP_EXPORT_API
#endif

RUDP_EXPORT_API class RUDPEngineExApi;
RUDP_EXPORT_API class RUDPEngineApi;
RUDP_EXPORT_API RUDPEngineApi* ios_rudp_engine_create(struct RUDPConfig* config);
RUDP_EXPORT_API int ios_rudp_engine_destroy(struct RUDPEngineApi* engine);
RUDP_EXPORT_API int ios_rudp_engine_join_channel(struct RUDPEngineApi* engine, uint64_t uid, const char* channelId);
RUDP_EXPORT_API int ios_rudp_engine_leave_channel(struct RUDPEngineApi* engine);
RUDP_EXPORT_API int ios_rudp_engine_send(struct RUDPEngineApi* engine, const char* data, int len);
RUDP_EXPORT_API int ios_rudp_engine_register_msg_callback(struct RUDPEngineApi* engine, rudp_msg_callback callback, void* content);
RUDP_EXPORT_API int ios_rudp_engine_register_event_callback(struct RUDPEngineApi* engine, rudp_evnt_callback callback, void* content);

RUDP_EXPORT_API RUDPEngineExApi* ios_rudp_engine_create_ex(struct RUDPConfig* config);
RUDP_EXPORT_API int ios_rudp_engine_destroy_ex(struct RUDPEngineExApi* engine);
RUDP_EXPORT_API int ios_rudp_engine_join_channel_ex(struct RUDPEngineExApi* engine, uint64_t uid, const char* channelId);
RUDP_EXPORT_API int ios_rudp_engine_join_channel_with_token_ex(struct RUDPEngineExApi* engine, const char* token, uint64_t uid, const char* channelId);

RUDP_EXPORT_API int ios_rudp_engine_leave_channel_ex(struct RUDPEngineExApi* engine);
RUDP_EXPORT_API int ios_rudp_engine_send_ex(struct RUDPEngineExApi* engine, const char* data, int len);
RUDP_EXPORT_API int ios_rudp_engine_register_msg_callback_ex(struct RUDPEngineExApi* engine, rudp_msg_callback_ex callback, void* content);
RUDP_EXPORT_API int ios_rudp_engine_register_event_callback_ex(struct RUDPEngineExApi* engine, rudp_evnt_callback_ex callback, void* content);
RUDP_EXPORT_API void ios_set_xmtp_debug(bool debug);
/**
 * @brief RUDP引擎扩展类，提供更多功能的RUDP引擎。
 */
RUDP_EXPORT_API class RUDPEngineEx;

/**
 * @brief RUDP配置结构体，用于配置RUDP引擎的参数。
 */
RUDP_EXPORT_API struct RUDPConfig;

/**
 * @brief RUDP引擎类，提供基本的RUDP功能。
 */
RUDP_EXPORT_API class RUDPEngine;

/**
 * @brief 创建一个RUDP引擎对象。
 *
 * @param config RUDP配置结构体指针。
 * @return 返回创建的RUDP引擎对象指针。
 */
RUDP_EXPORT_API RUDPEngine* rudp_engine_create(struct RUDPConfig* config);

/**
 * @brief 销毁RUDP引擎对象。
 *
 * @param engine 要销毁的RUDP引擎对象指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_destroy(struct RUDPEngine* engine);

/**
 * @brief 加入指定频道。
 *
 * @param engine RUDP引擎对象指针。
 * @param uid 用户ID。
 * @param channelId 频道ID。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_join_channel(struct RUDPEngine* engine, uint64_t uid, const char* channelId);

/**
 * @brief 离开当前频道。
 *
 * @param engine RUDP引擎对象指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_leave_channel(struct RUDPEngine* engine);

/**
 * @brief 发送数据。
 *
 * @param engine RUDP引擎对象指针。
 * @param data 要发送的数据指针。
 * @param len 数据长度。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_send(struct RUDPEngine* engine, const char* data, int len);

/**
 * @brief 注册消息回调函数。
 *
 * @param engine RUDP引擎对象指针。
 * @param callback 消息回调函数指针。
 * @param content 回调函数的上下文内容指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_register_msg_callback(struct RUDPEngine* engine, rudp_msg_callback callback, void* content);

/**
 * @brief 注册事件回调函数。
 *
 * @param engine RUDP引擎对象指针。
 * @param callback 事件回调函数指针。
 * @param content 回调函数的上下文内容指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_register_event_callback(struct RUDPEngine* engine, rudp_evnt_callback callback, void* content);

/**
 * @brief 创建一个RUDP引擎扩展对象。
 *
 * @param config RUDP配置结构体指针。
 * @return 返回创建的RUDP引擎扩展对象指针。
 */
RUDP_EXPORT_API RUDPEngineEx* rudp_engine_create_ex(struct RUDPConfig* config);

/**
 * @brief 销毁RUDP引擎扩展对象。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_destroy_ex(struct RUDPEngineEx* engine);

/**
 * @brief 加入指定频道（扩展版）。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @param uid 用户ID。
 * @param channelId 频道ID。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_join_channel_ex(struct RUDPEngineEx* engine, uint64_t uid, const char* channelId);

/**
 * @brief 使用令牌加入指定频道（扩展版）。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @param token 频道令牌。
 * @param uid 用户ID。
 * @param channelId 频道ID。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_join_channel_with_token_ex(struct RUDPEngineEx* engine, const char* token, uint64_t uid, const char* channelId);

/**
 * @brief 离开当前频道（扩展版）。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_leave_channel_ex(struct RUDPEngineEx* engine);

/**
 * @brief 发送数据（扩展版）。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @param data 要发送的数据指针。
 * @param len 数据长度。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_send_ex(struct RUDPEngineEx* engine, const char* data, int len);

/**
 * @brief 注册消息回调函数（扩展版）。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @param callback 消息回调函数指针。
 * @param content 回调函数的上下文内容指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_register_msg_callback_ex(struct RUDPEngineEx* engine, rudp_msg_callback_ex callback, void* content);

/**
 * @brief 注册事件回调函数（扩展版）。
 *
 * @param engine RUDP引擎扩展对象指针。
 * @param callback 事件回调函数指针。
 * @param content 回调函数的上下文内容指针。
 * @return 返回操作结果，成功返回0，失败返回负值。
 */
RUDP_EXPORT_API int rudp_engine_register_event_callback_ex(struct RUDPEngineEx* engine, rudp_evnt_callback_ex callback, void* content);

/**
 * @brief 设置XMTP测试模式。
 */
RUDP_EXPORT_API void set_xmtp_debug(bool debug);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_RUDPAPI_H

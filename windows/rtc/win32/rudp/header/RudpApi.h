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

RUDP_EXPORT_API class RUDPEngineEx;
RUDP_EXPORT_API struct RUDPConfig;

RUDP_EXPORT_API class RUDPEngine;
RUDP_EXPORT_API RUDPEngine* rudp_engine_create(struct RUDPConfig* config);
RUDP_EXPORT_API int rudp_engine_destroy(struct RUDPEngine* engine);
RUDP_EXPORT_API int rudp_engine_join_channel(struct RUDPEngine* engine, uint64_t uid, const char* channelId);
RUDP_EXPORT_API int rudp_engine_leave_channel(struct RUDPEngine* engine);
RUDP_EXPORT_API int rudp_engine_send(struct RUDPEngine* engine, const char* data, int len);
RUDP_EXPORT_API int rudp_engine_register_msg_callback(struct RUDPEngine* engine, rudp_msg_callback callback, void* content);
RUDP_EXPORT_API int rudp_engine_register_event_callback(struct RUDPEngine* engine, rudp_evnt_callback callback, void* content);

RUDP_EXPORT_API RUDPEngineEx* rudp_engine_create_ex(struct RUDPConfig* config);
RUDP_EXPORT_API int rudp_engine_destroy_ex(struct RUDPEngineEx* engine);
RUDP_EXPORT_API int rudp_engine_join_channel_ex(struct RUDPEngineEx* engine, uint64_t uid, const char* channelId);
RUDP_EXPORT_API int rudp_engine_join_channel_with_token_ex(struct RUDPEngineEx* engine, const char* token, uint64_t uid, const char* channelId);

RUDP_EXPORT_API int rudp_engine_leave_channel_ex(struct RUDPEngineEx* engine);
RUDP_EXPORT_API int rudp_engine_send_ex(struct RUDPEngineEx* engine, const char* data, int len);
RUDP_EXPORT_API int rudp_engine_register_msg_callback_ex(struct RUDPEngineEx* engine, rudp_msg_callback_ex callback, void* content);
RUDP_EXPORT_API int rudp_engine_register_event_callback_ex(struct RUDPEngineEx* engine, rudp_evnt_callback_ex callback, void* content);
RUDP_EXPORT_API void set_xmtp_debug(bool debug);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_RUDPAPI_H

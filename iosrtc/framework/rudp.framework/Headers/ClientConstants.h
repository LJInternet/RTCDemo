//
// Created by Administrator on 2023/5/22.
//

#ifndef LJSDK_CLIENTCONSTANTS_H
#define LJSDK_CLIENTCONSTANTS_H
#define RUDP_EXPORT

#include <stdio.h>
#ifdef _WIN32
#ifdef RUDP_EXPORT
#define RUDP_EXPORT_API __declspec(dllexport)
#else
#define RUDP_EXPORT_API __declspec(dllimport)
#endif
#else
#include <stdlib.h>

#ifdef RUDP_EXPORT
#define RUDP_EXPORT_API __attribute__((visibility ("default")))
#else
#endif

#endif

RUDP_EXPORT_API typedef void (*rudp_msg_callback)(const char* msg, uint32_t len, uint64_t uid, void* content);
RUDP_EXPORT_API typedef void (*rudp_evnt_callback)(int type, const char* msg, uint32_t len, int result, void* content);
RUDP_EXPORT_API typedef void (*rudp_msg_callback_ex)(int type, const char* msg, uint32_t len, uint64_t uid, void* content);
RUDP_EXPORT_API typedef void (*rudp_evnt_callback_ex)(int type, const char* msg, uint32_t len, int result, void* content);
#define API_SAFE_DELETE(ptr) {if (ptr) {delete ptr; ptr = NULL;} }

#define  API_FAILURE -1
#define  API_ERROR_ILLEGAL_PARAMETER -2
#define  API_SUCCESS  0

#define API_STATUS_CONNECTED 	1
#define API_STATUS_DISCONNECTED 2
#define API_STATUS_LOST 		3
#define API_STATUS_CLOSE		4

enum DataWorkMode
{
    SEND_AND_RECV = 0,
    SEND_ONLY = 1,
    RECV_ONLY = 2,
    LOCK_STEP_SEND_RECV = 3,
};
enum SdkMode {
    RTM,
    RTC
};
typedef struct RUDPConfig {
    const char* token;//正式环境不能为空，测试环境使用默认的token
    uint64_t appId;
    int mode;// RUDPMode 0 RTM 1 RTC
    int role;// RUDPRole 0 normal 1 controller
    bool isDebug;// 测试环境还是正式环境
    int dataWorkMode;// DataWorkMode
    uint32_t localIp;  // locally bound IP addr, in network-byte-order

    RUDPConfig() {localIp = 0;}
}RUDPConfig;

typedef struct ApiRelayInfo {
    char* relayIP;
    int relayPort;
    int relayId;
    int sessionId;
    bool bgp;
} ApiRelayInfo;

enum MsgType {
    MsgData,
    JoinChannel,
    LeaveChannel,
    LinkStatus,
    RemoteUserJoin,
    RemoteUserLeave
};
#endif //LJSDK_CLIENTCONSTANTS_H

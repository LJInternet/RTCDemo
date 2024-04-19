//
// Created by Administrator on 2023/5/12.
//

#ifndef LJSDK_RUDPENGINE_H
#define LJSDK_RUDPENGINE_H
#include "rudp_wrapper.h"
#include "XmtpManager.h"
#include <map>
#include <atomic>
#include <set>
#include "ConnectionResponse.h"
#include "ClientConstants.h"
#include "bundle.h"
#include "rudp_mgr.h"


class RUDP_EXTERN RUDPEngine {
private:
    rudp_msg_callback mMsgCallback = nullptr;
    void* mMsgContext = nullptr;
    rudp_evnt_callback mEventCallback = nullptr;
    void* mEventContext = nullptr;
    RudpProxy* mRudpProxy = nullptr;
    std::mutex _lock;
    RUDPConfig mConfig;
    std::string _token;
    int64_t last_disconnect_ts = 0;
    int64_t last_lost_ts = 0;
    std::string _channelId;
    RudpGroupInfo _rudpGroupInfo;
public:
    RUDPEngine(RUDPConfig *pConfig);
    ~RUDPEngine();

    int joinChannel(uint64_t uid, const char* channelId);
    int leaveChannel();

    int send(const char* data, int len);

    int registerMsgCallback(rudp_msg_callback callback, void *pVoid);

    rudp_mode getRudpMode(int mode) {
        return (RUDP_MODE)mode;
    }

    static int client_callback(RUDP_CB_TYPE type, const char* buf, int len, void* aux_param);

    int registerEventCallback(rudp_evnt_callback callback, void *context);
};

class XMTPClient;
class RUDP_EXTERN RUDPEngineEx {
private:
    void doRudpJoin(uint64_t uid, std::string& responseStr);
    void destroyXmtpClient();
    void doJoinChannelEx(uint64_t uid, std::string& token, std::string &channelIdStr);

    std::string _debugHost = "testws.fancyjing.com";
    std::string _releaseHost = "rtc.fancyjing.com";
    rudp_msg_callback_ex mExtMsgCallback = nullptr;
    bool isFirstConnected = false;
    void* mMsgContext = nullptr;
    RudpWrapper* mRudpWrapper = nullptr;
    std::mutex _callbackLock;
    std::mutex _rudpLock;
    std::mutex _xmtpLock;
    RUDPConfig mConfig;
    XMTPClient * mXMTPClient = nullptr;

    std::atomic<bool> mHaseJoinChannel = { false };
    JoinResponse joinResponse;
    RUDP_WRAP_CALLBACK _rudpCallback = nullptr;
    void * callbackContext = nullptr;
    std::string _channelId;
    uint64_t _localUid = 0;
    int _port = 18001;
    std::mutex _eventCallbackLock;
    rudp_evnt_callback_ex mExtEventCallback = nullptr;
    void * eventCallbackContext = nullptr;
    std::string _token;
    int64_t last_disconnect_ts = 0;
    int64_t last_lost_ts = 0;
    std::mutex mUserSetLock;
    std::set<uint64_t> mUserSets;
public:
    RUDPEngineEx(RUDPConfig *pConfig);
    ~RUDPEngineEx();
    rudp_mode getRudpMode(int mode) {
        return (RUDP_MODE)mode;
    }
    static int rudp_wrapper_callback(RUDP_CB_TYPE type, int64_t src_uid, std::string& channel_id, const char* buf, int buf_len, void* aux_param);

    int joinChannelEx(uint64_t uid, const char* channelId);

    int registerExtMsgCallback(rudp_msg_callback_ex callback, void *pVoid);

    int leaveChannel();

    int send(const char* data, int len);


    int registerRudpCallback(RUDP_WRAP_CALLBACK cb, void *context);

    int registerExtEventCallback(rudp_evnt_callback_ex eventCallback, void* context);

    int joinChannelExWithToken(uint64_t uid, const char *token, const char *channelId);

    void handleAllUserListEvent(LJ::Bundle bundle);

    void callbackRemoteUserStatus(int type, uint64_t user);
};


#endif //LJSDK_RUDPENGINE_H

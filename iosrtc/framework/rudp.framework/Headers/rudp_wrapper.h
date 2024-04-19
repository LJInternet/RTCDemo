//
// Created by Administrator on 2023/5/9.
//

#ifndef LJSDK_RUDP_WRAPPER_H
#define LJSDK_RUDP_WRAPPER_H

#include "rudp_proxy.h"

typedef int (*RUDP_WRAP_CALLBACK)(RUDP_CB_TYPE type, int64_t src_uid, std::string& channel_id, const char* buf, int buf_len, void* aux_param);

class RUDP_EXTERN RudpWrapper {

public:
    RudpWrapper(int64_t uid, bool isController, RUDP_WRAP_CALLBACK cb, void* cb_ctx, RUDP_MODE realtime_mode);
    RudpWrapper(int64_t uid, bool isController, struct sockaddr_in local_addr, RUDP_WRAP_CALLBACK cb, void* cb_ctx, RUDP_MODE realtime_mode);
    ~RudpWrapper();
    int send(const char* buf, int len);

    void updateRemotePeer(PeerInfo& info);
    void updateRelay(std::vector<RelayInfo>& relays);

    bool setMinBw(int bw);
    void setChannelId(uint64_t appId, std::string& channelId);
    bool setLinkCallback(RUDP_LINK_CALLBACK cb);
    bool setRcvLatencyTolerate(uint32_t rcv_latency_tolerate);
private:
    static int onRudpEvent(RUDP_CB_TYPE type, const char* buf, int buf_len, void* aux_param);

    int64_t _uid;
    std::string _channelId;
    RudpProxy* _rudpProxy;

    RUDP_WRAP_CALLBACK _cb;
    void* _cb_ctx;
};


#endif //LJSDK_RUDP_WRAPPER_H

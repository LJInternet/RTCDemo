//
// Created by Administrator on 2022/11/7.
//

#ifndef LJSDK_RUDP_MGR_H
#define LJSDK_RUDP_MGR_H

#include <atomic>
#include <map>
#include "rudp_proxy.h"

class XMTPClient;

// each RUDP-Group has at most 2 instances, i.e., one RTC and one RTM; they both
// have the same channel-id, but with different realtime-mode.
class RudpGroupInfo {
public:
    RudpGroupInfo() {
        _appId = 0;
        _uid = "";
        _channelId = "";
        generateGroupId();
    }

    void setGroupInfo(uint64_t appId, std::string uid, std::string channelId) {
        _appId = appId;
        _uid = uid;
        _channelId = channelId;
        generateGroupId();
    }

    void getGroupInfo(uint64_t& appId, std::string& uid, std::string& channelId) {
        appId = _appId;
        uid = _uid;
        channelId = _channelId;
    }

    std::string& getGroupId() {
        return _groupId;
    }

private:
    uint64_t _appId;
    std::string _uid;
    std::string _channelId;
    std::string _groupId;

    void generateGroupId() {
        _groupId = std::to_string(_appId) + "_" + _uid + "_" + _channelId;
    }
};

class RudpChannelInfo {
public:
    std::string _channel_id;
    RUDP_MODE _mode;
    std::string _token;
    uint32_t boundLocalIp = 0;  // the local IP that this rudp binds to. in network-byte-order
    bool _auth_failed = false;
};

class SignalConnectionInfo {
public:
    int64_t _appid;
    std::string _uid;
    bool _disconnected;
    std::vector<RudpChannelInfo> _channels;
};

class RUDP_EXTERN RudpManager {
public:
    static RudpManager &instance();

    RudpProxy* createRudp(bool isController, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
    RudpProxy* createRudp(bool isController, uint32_t local_ip, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
    RudpProxy* createRudp(bool isController, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param);
    RudpProxy* createRudp(bool isController, uint32_t local_ip, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param, RudpGroupInfo groupInfo);
    RudpProxy* createRudp(bool isController, struct sockaddr_in local_addr, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param);
    RudpProxy* createRudp(bool isController, struct sockaddr_in local_addr, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param, RudpGroupInfo groupInfo);

    void destroyRudp(RudpProxy* rudp);

    // the 'boundLocalIp' is required to be in network-byte-order
    void joinChannel(int64_t appID, std::string uid, std::string channelID, RUDP_MODE mode, std::string token, uint32_t boundLocalIp = 0);

    void leaveChannel(RudpGroupInfo& group_info, RUDP_MODE mode);

    // rtmReuse: whether rtm uses the same relay(s) as rtc, for backward compatibility
    void updateRelay(SignalConnectionInfo* connInfo, std::string channelId, RUDP_MODE mode, std::vector<RelayInfo>& relays, bool rtmReuse = false);

    void startNatTraversal(SignalConnectionInfo* connInfo, std::string channelId, RUDP_MODE mode,
        struct sockaddr_in signal_addr, uint64_t nat_traversal_id, int is_same_isp, int is_mobile_net_allowed);
    void stopNatTraversal();

    void updateRemotePeer(SignalConnectionInfo* connInfo, std::string channelId, RUDP_MODE mode, PeerInfo& info);

    void processSDKMessage(const std::string& cmd, const std::string& msg);

    // IP must be network-byte-order
    //void setLocalIp(uint32_t ipAddr) {_localIp = ipAddr;}
    //uint32_t getLocalIp() {return _localIp;}
private:
    RudpManager() = default;
    ~RudpManager() = default;

    void appendRudp(RudpProxy* rudp);
    void doJoin(XMTPClient* xmtpClient, SignalConnectionInfo& connInfo, RudpChannelInfo& channelInfo);
    void tryTransferTransportAddr(XMTPClient* client, SignalConnectionInfo* connInfo, std::string channelId, RUDP_MODE mode);
    void getRtmRelaybyRtc(const std::vector<RelayInfo>& input, std::vector<RelayInfo>& output);
    void updateRudpRelay(std::vector<RelayInfo>& relays, std::string& rudpGroupId, RUDP_MODE mode);

    std::recursive_mutex _lock;
    //std::vector<RudpProxy*> _rudps;
    //std::map<std::string, std::vector<RudpProxy*>> _rudpMap;  // key: groupId_mode
    std::map<std::string, RudpProxy*> _rudpMap;               // key: groupId_mode
    std::map<std::string, std::vector<RelayInfo>> _relayMap;  // key: groupId_mode
    std::map<XMTPClient*, SignalConnectionInfo*> _connMap;

    //XMTPClient* _xmtpClient = nullptr;
    //std::string _channelID;
    //int64_t _appID;
    //std::string _uid;
    //std::string _token;
    //std::atomic<bool> _joining = {false};
    //std::atomic<bool> _join_auth_failed = {false};
    //std::atomic<int> _disconnected = {false};
    //std::atomic<int> _using_rtc_relay = {false};

    //std::vector<RelayInfo> _relayList;
    //uint32_t _localIp = 0;

    static RudpManager s_theObj;
};

#endif //LJSDK_RUDP_MGR_H

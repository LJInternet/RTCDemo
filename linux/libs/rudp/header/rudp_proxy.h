//
// Created by Administrator on 2023/7/13.
//

#ifndef LJSDK_RUDP_PROXY_H
#define LJSDK_RUDP_PROXY_H

#include <string>
#include <vector>
#include <mutex>

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(BUILDING_RUDP_SHARED)
    /* Building shared library. */
#   define RUDP_EXTERN __declspec(dllexport)
# elif defined(USING_RUDP_SHARED)
    /* Using shared library. */
#   define RUDP_EXTERN __declspec(dllimport)
# else
    /* Building static library. */
#   define RUDP_EXTERN /* nothing */
# endif
#elif __GNUC__ >= 4
# define RUDP_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define RUDP_EXTERN __global
#else
# define RUDP_EXTERN /* nothing */
#endif

#define NO_CONGESTION                    0
#define LOW_CONGESTION                   1
#define MEDIUM_CONGESTION                2
#define SEVERE_CONGESTION                3

typedef struct RelayInfo {
    std::string relayIP;
    uint16_t relayPort;
    uint64_t relayId;
    uint64_t sessionId;
    bool bgp;
}RelayInfo;

struct udp_link_info{
    std::string remoteIP;
    uint16_t remotePort;

    uint64_t relay_id;
    uint64_t session_id;
    uint32_t my_node_id;
    uint8_t same_isp_flag;

    int link_type;
    int client_net_type;
    int rtt;
    uint32_t max_bw;
    int score;
};

typedef struct rudp_net_info{
    int rtt;
    int lost;
    int congest_level;
}rudp_net_info;

typedef struct PeerInfo{
//    std::string ip;
    std::vector<std::string> ips;
    uint16_t port;
    std::string channel_id;
    std::string user_id;

    uint64_t channel_int_id;
    uint64_t peer_id;
}PeerInfo;

typedef enum rudp_cb_type{
    RUDP_CB_DATA = 1,
    RUDP_CB_AVAILABLE_BW,
    RUDP_CB_PACKET_DROPPED,
    RUDP_CB_LINK_OK,
    RUDP_CB_LINK_FAILURE,
    RUDP_CB_NET_REPORT = 7,
    RUDP_CB_LINK_CLOSED = 8,
}RUDP_CB_TYPE;

typedef enum rudp_mode{
    RUDP_REALTIME_ULTRA,
    RUDP_REALTIME_NORMAL,
    RUDP_INVALID_MODE = 255
}RUDP_MODE;

typedef int (*RUDP_CALLBACK)(RUDP_CB_TYPE type, const char* buf, int buf_len);
typedef int (*RUDP_CALLBACK_V2)(RUDP_CB_TYPE type, const char* buf, int buf_len, void* aux_param);
typedef void (*RUDP_LINK_CALLBACK)(const udp_link_info* info_array, int count);
typedef void (*RUDP_LINK_CALLBACK_V2)(const udp_link_info* info_array, int count, void* aux_param);

struct rudp_instance;

class RUDP_EXTERN RudpProxy {
public:
    RudpProxy(bool isController, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
    RudpProxy(bool isController, uint32_t local_ip, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
    RudpProxy(bool isController, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param);
    RudpProxy(bool isController, uint32_t local_ip, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param);
    RudpProxy(bool isController, struct sockaddr_in local_addr, RUDP_CALLBACK_V2 cb, RUDP_MODE realtime_mode, void* aux_param);
    ~RudpProxy();

    void updateRelay(std::vector<RelayInfo>& relays);
    void updateRemotePeer(PeerInfo& info);

    int send(const char* buf, int len);
    int send(const char* header, int header_len, const char* buf, int len);

    void startNatTraversal(struct sockaddr_in signal_addr, uint64_t nat_traversal_id, int is_same_isp, int is_mobile_net_allowed);
    void stopNatTraversal();

    bool setMinBw(int bw);
    bool setLinkCallback(RUDP_LINK_CALLBACK cb);
    bool setLinkCallback(RUDP_LINK_CALLBACK_V2 cb, void* aux_param);
    bool setRcvLatencyTolerate(uint32_t rcv_latency_tolerate);
    void setSndLatencyTolerate(uint32_t snd_latency_tolerate);
    bool setRudpID(std::string rudpID);
    void setGroupId(std::string& groupId);

    RUDP_MODE mode();
    uint32_t localIp();
    int localPort();

    RUDP_CALLBACK _usr_callback;
    RUDP_CALLBACK_V2 _usr_callback_v2;
    void* _usr_aux_param;

    RUDP_LINK_CALLBACK _link_callback;
    RUDP_LINK_CALLBACK_V2 _link_callback_v2;
    void* _link_aux_param;

    std::string _groupId;
    bool _rtmReuseRtcRelay = false;
private:
    RudpProxy() {}
    //struct rudp_instance* create(bool isController, RUDP_MODE realtime_mode);
    struct rudp_instance* create_with_port_range(bool isController, uint32_t local_ip, RUDP_MODE realtime_mode);
    struct rudp_instance* create(bool isController, struct sockaddr_in local_addr, RUDP_MODE realtime_mode);

    struct rudp_instance* _instance = nullptr;
};


#endif //LJSDK_RUDP_PROXY_H

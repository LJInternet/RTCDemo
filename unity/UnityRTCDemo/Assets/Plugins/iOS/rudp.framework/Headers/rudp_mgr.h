//
// Created by Administrator on 2022/11/7.
//

#ifndef LJSDK_RUDP_MGR_H
#define LJSDK_RUDP_MGR_H

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

typedef enum rudp_cb_type{
    RUDP_CB_DATA = 1,
    RUDP_CB_AVAILABLE_BW,
    RUDP_CB_PACKET_DROPPED,
    RUDP_CB_LINK_OK,
    RUDP_CB_LINK_FAILURE,
    RUDP_CB_NET_REPORT = 7,
}RUDP_CB_TYPE;

typedef enum rudp_mode{
    RUDP_REALTIME_ULTRA,
    RUDP_REALTIME_NORMAL
}RUDP_MODE;

typedef int (*RUDP_CALLBACK)(RUDP_CB_TYPE type, const char* buf, int buf_len);
typedef void (*RUDP_LINK_CALLBACK)(const udp_link_info* info_array, int count);

struct rudp_instance;

class RUDP_EXTERN RudpProxy {
public:
    RudpProxy(bool isController, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
    ~RudpProxy();

    void updateRelay(std::vector<RelayInfo>& relays);

    int send(const char* buf, int len);

    void startNatTraversal(struct sockaddr_in signal_addr, uint64_t nat_traversal_id, int is_same_isp, int is_mobile_net_allowed);
    void stopNatTraversal();

    bool setMinBw(int bw);
    bool setLinkCallback(RUDP_LINK_CALLBACK cb);
    bool setRcvLatencyTolerate(uint32_t rcv_latency_tolerate);
private:
    struct rudp_instance* _instance;
};

class RUDP_EXTERN RudpManager {
public:
    static RudpManager &instance();

    RudpProxy* createRudp(bool isController, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);

    void destroyRudp(RudpProxy* rudp);

    void updateRelay(std::vector<RelayInfo>& relays);

    void startNatTraversal(struct sockaddr_in signal_addr, uint64_t nat_traversal_id, int is_same_isp, int is_mobile_net_allowed);

    void stopNatTraversal();

    void processSDKMessage(const std::string& cmd, const std::string& msg);
private:
    RudpManager() = default;
    ~RudpManager() = default;

    std::mutex _lock;
    std::vector<RudpProxy*> _rudps;

    static RudpManager s_theObj;
};

#endif //LJSDK_RUDP_MGR_H

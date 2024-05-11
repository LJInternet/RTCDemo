//=============================================================================
// RUDP - Robust UDP
//  
// Features:
// + Throughput-based Bandwidth Estimation & Congestion Control.
// + RS FEC integrated, with automatic FEC on/off policy.
// + Rely on KCP (with minor modification) for ARQ & packet-segmentation
// + Supports at least 25Mbytes per packet, when mss = 1024bytes
// + Packet order & message boundary are guaranteed.
// + Supports a simple relay protocol
// + Ablility to customize the low-layer output function.
//
// Change Log(s):
// 1) 2022/05, linzhengxian: 
//       Features ready.
//=============================================================================

#ifndef __ROBUST_UDP_H__
#define __ROBUST_UDP_H__

#include "thread.h"
#include "rudp_helper.h"
#include "rudp_mgr.h"
#include "ikcp.h"


#define RUDP_SEND_WINDOW              512    // in packet(s), not byte(s)
#define RUDP_RECV_WINDOW              1024   // in packet(s), not byte(s)
#define RUDP_MTU                      1200
#define RUDP_MAX_OVERHEAD             64     // Note: keep it multiple of 8, for memory alignment

#define RUDP_STOPPED                  0
#define RUDP_PENDING                  1
#define RUDP_READY                    2

#define RELAY_PACKET_DATA             0
#define RELAY_KEEP_ALIVE              1
#define RELAY_PACKET_DATA_V2          2
#define RELAY_KEEP_ALIVE_V2           3

#define LINK_BGP_REALY                1
#define LINK_REALY                    2
#define LINK_P2P                      3
#define LINK_C2S                      4

#define NET_MOBILE                    0
#define NET_WIFI                      1
#define NET_UNKNOW                    2

#define FEC_RX_QUE_NUM                128

#ifdef __cplusplus
extern "C" {
#endif

	struct uv_udp_s;
	struct uv_loop_s;
	struct uv_timer_s;
	struct uv_async_s;

typedef struct udp_link udp_link;
typedef struct udp_link_info udp_link_info;
typedef struct udp_link_mgr udp_link_mgr;
typedef struct link_event link_event;

typedef struct rudp_instance RUDP_INSTANCE;
typedef struct queued_segment QUEUED_SEGMENT;
typedef struct queued_packet QUEUED_PACKET;


typedef int (*RUDP_LOW_LAYER_OUTPUT)(char* buf, int len, udp_link* link, void* param);

struct notification_callback {
    uint32_t sub_type;
    uint32_t len;  // length of the subsequent buffer
    char buf[1];
};

struct relay_info {
    struct sockaddr_in addr;
    uint64_t relay_id;
    uint64_t session_id;
    int bgp;
};

struct packet_has_fec {
    uint8_t hdr_len;
    uint8_t version;
    uint8_t packet_type;
    uint8_t epoch;
    uint16_t payload_size;
    uint32_t fec_group_id;
    uint16_t fec_row;
};

struct fec_data_queue {
    struct IQUEUEHEAD head;
    uint16_t que_size;
    uint8_t fec_k;         // original-block number of FEC scheme
    uint8_t fec_m;         // recovery-block number of FEC scheme
    uint32_t fec_group_id;
    uint16_t rx_origin;    // number of received original-blocks
    uint16_t rx_recovery;  // number of received recovery-blocks
    uint16_t block_size;
    uint8_t done;          // done with fec
};

// for RUDP segments
struct queued_segment {
	struct IQUEUEHEAD node;
	uint32_t len;
    uint32_t offset;
    uint16_t fec_row;
	char data[RUDP_MTU + RUDP_MAX_OVERHEAD];
};

struct app_fragment_queue {
    struct IQUEUEHEAD head;
    uint32_t size;
};

// for packets from upper layer
struct queued_packet {
	struct IQUEUEHEAD node;
	uint32_t len;
	char data[1];
};

struct udp_link {
	udp_link_info info;

	struct uv_udp_s* transport;
    struct sockaddr_in local_addr;
    struct sockaddr_in remote_addr;

    uint16_t congest_count;
    uint32_t congest_ts;

    uint16_t switch_fail_count;
    uint32_t switch_fail_ts;

    uint32_t create_ts;
    uint32_t last_pong_ts;

    uint8_t reported;
};

typedef enum link_mgr_state {
    LS_BLOCK,
    LS_STABLE,
    LS_SWITCHING
}link_mgr_state;

struct udp_link_mgr{
    udp_link* links[32];
    int link_count;
	int connected_link_count;

	struct uv_udp_s* primary_transport;
	struct uv_udp_s* p2p_transport;

    link_mgr_state state;
    uint16_t switch_percentage;

    udp_link* main_link;
    udp_link* backup_link;

    uint32_t max_bw;
    uint32_t last_congest_level;
    uint32_t last_switch_fail_ts;
};

struct link_event{
    int type;
    void* param;
    int param_size;
};

// rdup: robust udp
struct rudp_instance {
    int work_as_server;  // indicates whether this is a client or server
    uint32_t conn_id;
    RUDP_MODE mode;
    int link_status;
    int fsm_status;
    LJ::Thread* thread;
    uint32_t jiffies_10ms;
    uint32_t jiffies_1s;

    RUDP_CALLBACK callback;
    RUDP_LINK_CALLBACK link_cb;
    RUDP_LOW_LAYER_OUTPUT low_layer_output;
    void* output_param;
    ikcpcb* kcp;
    int normal_fec_enabled;
    int probe_fec_enabled;

    char* packet_buf;
    uint32_t buf_len;
    RUDP_FIFO_Q app_packet_que;
    RUDP_FIFO_Q link_event_que;
    struct app_fragment_queue app_fragment_que;
    struct IQUEUEHEAD fec_tx_queue;
	struct fec_data_queue fec_rx_queue[FEC_RX_QUE_NUM];
    struct IQUEUEHEAD free_packet_que;
    uint32_t fec_tx_que_size;
    uint32_t fec_group_seq;
	char* fec_in_buf;
	char* fec_out_buf;
    uint32_t fec_k;  // original block number
    uint32_t fec_m;  // recovery block number
    uint32_t fec_quarantine_time;

	udp_link_mgr* link_mgr;

    void* nt_holder;
	struct uv_loop_s* loop;
	struct uv_timer_s* every_1s_timer;
	struct uv_timer_s* every_10ms_timer;
	struct uv_async_s* link_handle;
	struct uv_async_s* app_packet_handle;
	struct uv_async_s* exit_handle;

	uint32_t tx_goodput;  // in Byte-per-second
    uint32_t congest_threshold;
    uint8_t network_saturated;
    uint32_t resend_percentage;
    uint64_t total_que_bytes;
    uint32_t que_size_samples;
    uint32_t avg_que_bytes;
    int probing;
    uint32_t next_probe_time;
    uint32_t probe_times_left;
    uint32_t probe_traffic;       // in bytes
    uint32_t remain_probe_quota;  // in bytes
    uint32_t probe_frequency;     // send probe traffic in every x packets
    int congest_count_30s;

    int congest_level;
    uint32_t consecutive_non_congest;
    uint32_t consecutive_congest;
    int initial_fsm_duration;
    uint32_t kcp_1s_budget;           // kcp total traffic budget in current second, in bytes
    uint32_t rudp_1s_budget;          // rudp total traffic budget in current second, in bytes
    uint32_t app_1s_budget;           // application traffic budget in current second, in bytes
    uint32_t rudp_remain_1s_quota;    // rudp remaining traffic quota in current second, in bytes
    uint32_t rudp_remain_10ms_quota;  // app remaining traffic quota in current 10ms period, in bytes
    uint16_t ref_link_loss_rate;
    uint16_t ref_resend_rate;
    //uint32_t last_increase_bw_time;

	uint32_t min_app_budget;
	uint32_t min_rudp_budget;
	uint32_t min_kcp_budget;
    
    RUDP_KPI_HISTORY srtt_history;
    RUDP_KPI_HISTORY resend_percentage_history;
    RUDP_KPI_HISTORY link_lossrate_history;
    RUDP_KPI_HISTORY goodput_history;
    RUDP_KPI_HISTORY que_bytes_history;
    RUDP_KPI_HISTORY max_100ms_goodput_history;

    RUDP_TX_STAT tx_stat;
    RUDP_RX_STAT rx_stat;
    struct peer_feedback_info peer_feedback;
    OCCUR_COUNTER network_congest_counter;
    OCCUR_COUNTER bad_fec_counter;
    uint32_t tx_packet_dropped;
    KCP_METRICS_RECORDER kcp_10ms_recorder;
    KCP_METRICS_RECORDER kcp_100ms_recorder;
    KCP_METRICS_RECORDER kcp_1s_recorder;
    uint16_t que_overload_times;       // queue overload times in current 1s period
    uint8_t resend_rate_abnormal;      // whether the resend-rate is abnormal in current second
    uint8_t resend_percentage_100ms;   // the kcp resend percentage in last 100ms period
    uint32_t max_100ms_goodput;        // maximum 100ms goodput in current 1s period
    uint64_t sum_100ms_que_bytes;
    uint32_t que_bytes_100ms_samples;
    uint64_t output_pkt_count;

    uint64_t total_link_rx;
    uint64_t last_link_rx;
    uint32_t rx_bandwidth;
    uint32_t peer_rx_bandwidth;

    uint32_t last_rx_hello;

    uint32_t tx_packet_id;
    RUDP_DEFRAG_BUF defrag_buf;
};

RUDP_INSTANCE* rudp_client_create(struct sockaddr_in remote_addr, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
RUDP_INSTANCE* rudp_server_create(struct sockaddr_in local_addr, RUDP_CALLBACK cb, RUDP_MODE realtime_mode);
int rudp_destroy(RUDP_INSTANCE* rudp);

int update_relay(RUDP_INSTANCE* rudp, struct relay_info* info_array, int size);

int rudp_send(RUDP_INSTANCE* rudp, const char* buf, int len);

void start_nat_traversal(RUDP_INSTANCE* rudp, struct sockaddr_in signal_addr, int8_t nat_traversal_id[16], int is_same_isp, int is_mobile_net_allowed);
void stop_nat_traversal(RUDP_INSTANCE* rudp);

bool set_low_layer_output(RUDP_INSTANCE* rudp, RUDP_LOW_LAYER_OUTPUT output, void* param);
bool set_rudp_min_bw(RUDP_INSTANCE* rudp, int bw);
bool set_rcv_latency_tolerate(RUDP_INSTANCE* rudp, uint32_t rcv_latency_tolerate);
bool set_rudp_link_cb(RUDP_INSTANCE* rudp, RUDP_LINK_CALLBACK cb);

#ifdef __cplusplus
}
#endif

#endif  // __ROBUST_UDP_H__

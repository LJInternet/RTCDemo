//=============================================================================
// RUDP helper methods
//  
// Features:
// + FIFO queue with mutex lock.
// + packet de-fragmentation
// + metrics history queue.
// + statistics related.
// + counter implementation for event-occurrence-frequency.
//
// Change Log(s):
// 1) 2022/05, linzhengxian: 
//       File created.
// 2) 2022/07/22, linzhengxian:
//       Add mutex to FIFO queue, to support multiple producers.
//=============================================================================

#ifndef __RUDP_HELPER_H__
#define __RUDP_HELPER_H__

#include <stdint.h>
#include <stddef.h>
#include "ikcp.h"
#include "uv.h"

#ifdef __cplusplus
extern "C" {
#endif


#define RUDP_FIFOQ_SIZE                1024    // note: MUST be power of 2
#define MAX_KPI_HISTORY                8
#define MAX_EPOCH_SIZE                 256     // epoch is defined as uint8_t

#define RUDP_CHUNK_PAYLOAD_SIZE        (100 * 1024)
#define RUDP_CHUNK_HDR_SIZE            6

typedef struct rudp_fifo_q {
    //_Atomic(uint32_t) head;
    //_Atomic(uint32_t) tail;
    uint32_t head;
    uint32_t tail;
    uv_mutex_t mutex;
    void* arr[RUDP_FIFOQ_SIZE];
}RUDP_FIFO_Q;

typedef struct rudp_kpi_item {
	struct IQUEUEHEAD node;
	int32_t value;
}RUDP_KPI_ITEM;

typedef struct rudp_kpi_history {
    struct IQUEUEHEAD head;
    uint16_t que_size;
    int32_t max_value;
    int32_t min_value;
    int32_t total_value;
    int32_t avg_value;
}RUDP_KPI_HISTORY;

typedef struct rudp_defrag_buf {
    uint32_t packet_id;
    char* buf;               // the buffer to store chunks (fragments)
    int len;                 // the size of the buffer
    uint16_t chunk_count;    // the expected total chunk numbers for the packet
    uint16_t last_chunk_id;  // the chunk-id received last time
    char* packet;            // the final packet after de-fragmentation, may not point to 'buf'
    int packet_len;          // length of the packet, after de-fragmentation
}RUDP_DEFRAG_BUF;

typedef struct kcp_metrics_recorder {
    struct kcp_metrics last_metrics;
    struct kcp_metrics delta;
}KCP_METRICS_RECORDER;

typedef struct occur_counter {
    uint64_t num;
}OCCUR_COUNTER;

typedef struct rudp_tx_stat {
    uint8_t curr_epoch;
    uint32_t tx_count[MAX_EPOCH_SIZE];       // epoch 1~255, 0 is invalid epoch
    uint32_t peer_rx_count[MAX_EPOCH_SIZE];  // if value is not zero, means peer has reported
    uint32_t tx_bytes;                       // number of tx bytes during specific period
    uint32_t last_tx_bytes;                  // number of tx bytes in the last period
    uint32_t sum_tx;                         // sum of tx packets during specific period
    uint32_t sum_peer_rx;                    // sum of packets received by peer during specific period
    uint8_t loss_percentage;
}RUDP_TX_STAT;

typedef struct rudp_rx_stat {
    uint32_t rx_count[MAX_EPOCH_SIZE];      // rx packets in each epoch
    uint32_t last_updated[MAX_EPOCH_SIZE];  // the last time when rx_count was updated, in 10ms_jiffies
    uint32_t rx_fec_groups;
    uint32_t fec_recovered_times;
}RUDP_RX_STAT;

typedef struct peer_feedback_info {
    uint32_t rx_feedback_time;
    uint32_t last_fec_groups;
    uint32_t last_recovered_number;
    uint32_t fec_group_increment;
    uint32_t fec_recovered_increment;
    uint32_t rx_buf_backlogs;
}PEER_FEEDBACK_INFO;

typedef struct recent_max_value {
    uint32_t round;
    uint32_t index;
    uint32_t max[2];
}RECENT_MAX_VALUE;

void init_rudp_fifoq(RUDP_FIFO_Q* que);
void destroy_rudp_fifoq(RUDP_FIFO_Q* que);
int rudp_fifoq_put(RUDP_FIFO_Q* que, void* data);
void* rudp_fifoq_get(RUDP_FIFO_Q* que);

void init_defrag_buf(RUDP_DEFRAG_BUF* buf);
void free_defrag_buf(RUDP_DEFRAG_BUF* buf);
int defrag_rudp_chunk(RUDP_DEFRAG_BUF* buf, char* chunk, int chunk_size);

void record_kpi_history(RUDP_KPI_HISTORY* history, int value);
void init_kpi_history(RUDP_KPI_HISTORY* history);
void reset_kpi_history(RUDP_KPI_HISTORY* history);

void init_tx_stat(RUDP_TX_STAT* stat);
void init_rx_stat(RUDP_RX_STAT* stat);
uint8_t calc_tx_loss_percentage(RUDP_TX_STAT* stat);
void reset_tx_stat(RUDP_TX_STAT* stat);
void update_rudp_epoch(RUDP_TX_STAT* stat, uint32_t curr_10ms_jiffies);
void inc_rx_packet_count(RUDP_RX_STAT* stat, uint8_t epoch, uint32_t curr_10ms_jiffies);
void reset_rx_stat(RUDP_RX_STAT* stat);

void init_kcp_metrics_recorder(KCP_METRICS_RECORDER* recorder);
void update_kcp_metrics(KCP_METRICS_RECORDER* recorder, KCP_METRICS* cur_metrics);
uint32_t get_resend_percentage(KCP_METRICS_RECORDER* recorder);

void init_recent_max(RECENT_MAX_VALUE* maxv);
void update_max_value(RECENT_MAX_VALUE* maxv, uint32_t value);
uint32_t get_max_value(RECENT_MAX_VALUE* maxv);

void init_occurrence(OCCUR_COUNTER* counter);
int add_occurrence(OCCUR_COUNTER* counter, int occur_or_not);
int occurrence_in_60_samples(OCCUR_COUNTER* counter);
int occurrence_in_30_samples(OCCUR_COUNTER* counter);

int add_fd_to_epoll(int epfd, int fd, uint32_t events);
int remove_fd_from_epoll(int epfd, int fd);

void reset_peer_feedback(PEER_FEEDBACK_INFO* info);

#ifdef __cplusplus
}
#endif

#endif  // __RUDP_HELPER_H__
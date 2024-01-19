//
// Created by Administrator on 2023/2/2.
//

#ifndef LJSDK_REPORT_C_H
#define LJSDK_REPORT_C_H

#include "basestone.h"

#ifdef __cplusplus
extern "C" {
#endif

BASESTONE_EXTERN typedef void (*do_upload_func)(char* msg, int len);

BASESTONE_EXTERN void reporter_init(int cd, do_upload_func f);

BASESTONE_EXTERN void reporter_set_common_attr(char* attr, int len);

BASESTONE_EXTERN void* reporter_register_slot(char* key, int key_len, int level);

BASESTONE_EXTERN void reporter_do_report(void* slot_ptr, char* attr, int len);

BASESTONE_EXTERN void reporter_release();

BASESTONE_EXTERN void reporter_enable_performance(bool enabled);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_REPORT_C_H
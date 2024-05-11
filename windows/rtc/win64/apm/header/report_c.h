//
// Created by Administrator on 2023/2/2.
//

#ifndef LJSDK_REPORT_C_H
#define LJSDK_REPORT_C_H

#pragma once

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(BUILDING_REPORT_SHARED)
    /* Building shared library. */
#   define REPORT_EXTERN __declspec(dllexport)
# elif defined(USING_REPORT_SHARED)
    /* Using shared library. */
#   define REPORT_EXTERN __declspec(dllimport)
# else
    /* Building static library. */
#   define REPORT_EXTERN /* nothing */
# endif
#elif __GNUC__ >= 4
# define REPORT_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define REPORT_EXTERN __global
#else
# define REPORT_EXTERN /* nothing */
#endif

#ifdef __cplusplus
extern "C" {
#endif

REPORT_EXTERN typedef void (*do_upload_func)(char* msg, int len);

REPORT_EXTERN typedef void (*report_callback)(int result, char* msg, int len);

REPORT_EXTERN void reporter_init(int cd, do_upload_func f);

REPORT_EXTERN void reporter_init_ex(bool isDebug, int cd, int appId, report_callback callback);

REPORT_EXTERN void reporter_set_userinfo(char* token, long uid);

REPORT_EXTERN void reporter_set_common_attr(char* attr, int len);

REPORT_EXTERN void* reporter_register_slot(char* key, int key_len, int level);

REPORT_EXTERN void* reporter_register_event_slot(char* key, int key_len, int level);

REPORT_EXTERN void reporter_do_report(void* slot_ptr, char* attr, int len);

REPORT_EXTERN void reporter_release();

REPORT_EXTERN void reporter_enable_performance(bool enabled);
#ifdef __cplusplus
}
#endif

#endif //LJSDK_REPORT_C_H
//
// Created by Administrator on 2023/4/6.
//

#ifndef LJSDK_FEEDBACKCONSTANT_H
#define LJSDK_FEEDBACKCONSTANT_H
#define EXPORT

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(EXPORT)
    /* Building shared library. */
#   define FEEDBACK_EXTERN __declspec(dllexport)
# else
    /* Using shared library. */
#   define FEEDBACK_EXTERN __declspec(dllimport)
# endif
#elif __GNUC__ >= 4
# define FEEDBACK_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define FEEDBACK_EXTERN __global
#else
# define FEEDBACK_EXTERN /* nothing */
#endif

#define WRITEBUFFERSIZE (16384)
#include "log.h"
#define MODULE_NAME  "Feedback"
#define FLOGE(...) LJ::LOG(MODULE_NAME, LJ::LOG_ERROR, __VA_ARGS__)
#define FLOGD(...) LJ::LOG(MODULE_NAME, LJ::LOG_DEBUG, __VA_ARGS__)
#define FLOGI(...) LJ::LOG(MODULE_NAME, LJ::LOG_INFO, __VA_ARGS__)
#define FEEDBACK_HOST "feedback.fancyjing.com"
#define FEEDBACK_DEBUG_HOST "testfeedback.fancyjing.com"
#ifdef __cplusplus
extern "C" {
#endif
FEEDBACK_EXTERN typedef void (*feedback_callback)(int result, const char *buf, int32_t len);
#ifdef __cplusplus
}
#endif
#endif //LJSDK_FEEDBACKCONSTANT_H

//
// Created by Administrator on 2023/6/7.
//

#ifndef LJSDK_LJ_RTMP_COMMON_H
#define LJSDK_TRANSFER_COMMON_H

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(BUILDING_MEDIATRANSFER_SHARED)
    /* Building shared library. */
#   define MEDIATRANSFER_EXTERN __declspec(dllexport)
# elif defined(USING_MEDIATRANSFER_SHARED)
    /* Using shared library. */
#   define MEDIATRANSFER_EXTERN __declspec(dllimport)
# else
    /* Building static library. */
#   define MEDIATRANSFER_EXTERN /* nothing */
# endif
#elif __GNUC__ >= 4
# define MEDIATRANSFER_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define MEDIATRANSFER_EXTERN __global
#else
# define MEDIATRANSFER_EXTERN /* nothing */
#endif

#endif //LJSDK_LJ_RTMP_COMMON_H

#pragma once

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(BUILDING_BASESTONE_SHARED)
    /* Building shared library. */
#   define BASESTONE_EXTERN __declspec(dllexport)
# elif defined(USING_BASESTONE_SHARED)
    /* Using shared library. */
#   define BASESTONE_EXTERN __declspec(dllimport)
# else
    /* Building static library. */
#   define BASESTONE_EXTERN /* nothing */
# endif
#elif __GNUC__ >= 4
# define BASESTONE_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define BASESTONE_EXTERN __global
#else
# define BASESTONE_EXTERN /* nothing */
#endif
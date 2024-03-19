#ifndef RTC_OBJC_BASE_RTCMACROS_H_
#define RTC_OBJC_BASE_RTCMACROS_H_

#define RTC_OBJC_EXPORT __attribute__((visibility("default")))

#ifdef WIN32
#define RTC_EXTERN _declspec(dllexport)
#else
#define RTC_EXTERN 
#endif

#ifdef __OBJC__
#if defined(__cplusplus)
#define RTC_EXTERN extern "C" RTC_OBJC_EXPORT
#else
#define RTC_EXTERN extern RTC_OBJC_EXPORT
#endif

#define RTC_FWD_DECL_OBJC_CLASS(classname) @class classname
#else
#define RTC_FWD_DECL_OBJC_CLASS(classname) typedef struct objc_object classname
#endif

#endif  // RTC_OBJC_BASE_RTCMACROS_H_

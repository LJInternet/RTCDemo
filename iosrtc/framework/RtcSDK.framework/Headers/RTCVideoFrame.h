//
//  RTCVideoFrame.h
//  OpenRTC
//
//  Created by zhoujr on 2019/7/28.
//

#import <AVFoundation/AVFoundation.h>
#import<RtcSDK/RTCMacros.h>
NS_ASSUME_NONNULL_BEGIN

/**
 视频旋转方向
 */
typedef NS_ENUM(NSInteger, RTCVideoRotation) {
    /// 视频旋转0度
    RTCVideoRotation_0 = 0,
    /// 视频旋转90度
    RTCVideoRotation_90 = 90,
    /// 视频旋转180度
    RTCVideoRotation_180 = 180,
    /// 视频旋转270度
    RTCVideoRotation_270 = 270
};

/**
 视频帧
 */
RTC_OBJC_EXPORT
@interface RTCVideoFrame : NSObject

/** Width without rotation applied. */
@property (nonatomic, readonly) int width;

/** Height without rotation applied. */
@property (nonatomic, readonly) int height;

/** 图像旋转角度 */
@property (nonatomic, readonly) RTCVideoRotation rotation;

/** Timestamp in nanoseconds. */
@property (nonatomic, readonly) int64_t timeStampNs;

/** 图像数据 */
@property(nonatomic, readonly) char *buffer;

/// 请使用 -[RTCVideoFrame initWithBuffer:rotation:timeStampNs:]
- (instancetype)init NS_UNAVAILABLE;
/// 请使用 -[RTCVideoFrame initWithBuffer:rotation:timeStampNs:]
- (instancetype) new NS_UNAVAILABLE;

/** Initialize an RTCVideoFrame from a frame buffer, rotation, and timestamp.
 */
- (instancetype)initWithBuffer:(char *)frameBuffer
                        width:(int)width
                        height:(int)height
                          size:(int)size
                      rotation:(RTCVideoRotation)rotation
                   timeStampNs:(int64_t)timeStampNs;

- (void)destroy;

@end

NS_ASSUME_NONNULL_END

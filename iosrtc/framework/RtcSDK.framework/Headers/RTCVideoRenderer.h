//
//  RTCVideoRenderer.h
//  OpenRTC
//
//  Created by zhoujr on 2019/7/28.
//

#import <UIKit/UIKit.h>
#import "RTCMacros.h"

NS_ASSUME_NONNULL_BEGIN

/**
 视频渲染模式枚举
 */
typedef NS_ENUM(NSInteger, RTCRenderMode) {
    RTCRenderModeHidden = 1,   // 保留模式
    RTCRenderModeFit = 2,      // 保持宽高比可能留黑边
    RTCRenderModeAdaptive = 3, // 填充容器可能裁剪
    RTCRenderModeFill = 4      // 拉伸填满容器
};

/**
 视频渲染器
 @note 如需自定义视频输出，请实现RTCVideoRenderer协议，并调用如下接口进行设置
 @code [RTCEngine sharedInstance].localVideoRenderer @endcode
 @code [[RTCEngine sharedInstance] addVideoRenderer:forSourceId] @endcode
 */
RTC_OBJC_EXPORT
@protocol RTCVideoRenderer <NSObject>

/**
 渲染视频帧
 @param frame 视频帧
 @note 图像宽度、高度、旋转角度等信息请参考RTCVideoFrame属性
 @note 如果图像大小发生变化，会触发-[RTCVideoRendererDelegate videoView:didChangeVideoSize:]通知
 */
- (void)displayYUV420pData:(void *)data size:(NSInteger)size width:(NSInteger)w height:(NSInteger)h;

- (void)setLowLatency:(BOOL)lowLatency;
/**
 设置渲染模式
 @param renderMode 渲染模式
 @note RTCRenderModeHidden: 1 保留
 @note RTCRenderModeFit: 2 填充，根据视频以及容器的宽高比进行缩放，保证一边铺满高或者铺满宽，可能会出现黑边
 @note RTCRenderModeAdaptive: 3 保留 填充，将视频填满容器，可能会被裁剪
 @note RTCRenderModeFill: 3 铺满，将视频填满容器，可能拉伸
 @note 默认为RTCRenderModeFit
 */
- (void)setRenderMode:(int)renderMode;
/**
 视频渲染器是否启用
 @note 如果enabled=NO，则不会调用 -renderFrame:
 */
@property (nonatomic, getter=isEnabled) BOOL enabled;

@end

/**
 视频渲染代理
 */
RTC_OBJC_EXPORT
@protocol RTCVideoRendererDelegate

/**
 视频大小变化通知
 @param videoRender 视频渲染器
 @param size 视频大小
 */
- (void)videoRender:(id<RTCVideoRenderer>)videoRender didChangeVideoSize:(CGSize)size;

@end

NS_ASSUME_NONNULL_END

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import<RtcSDK/RTCMacros.h>
#import<RtcSDK/RTCVideoRenderer.h>
#import<RtcSDK/RTCVideoViewShading.h>


NS_ASSUME_NONNULL_BEGIN

@class RTCEAGLVideoView;

/**
 * RTCEAGLVideoView is an FISHVideoRenderer which renders video frames in its
 * bounds using OpenGLES 2.0 or OpenGLES 3.0.
 */
RTC_OBJC_EXPORT
NS_EXTENSION_UNAVAILABLE_IOS("Rendering not available in app extensions.")
@interface RTCEAGLVideoView : UIView <RTCVideoRenderer>

//@property(nonatomic, weak) id<RTCVideoViewDelegate> delegate;

- (instancetype)initWithFrame:(CGRect)frame
                       shader:(id<RTCVideoViewShading>)shader NS_DESIGNATED_INITIALIZER;

- (instancetype)initWithCoder:(NSCoder *)aDecoder
                       shader:(id<RTCVideoViewShading>)shader NS_DESIGNATED_INITIALIZER;

- (void)dealloc;
@end

NS_ASSUME_NONNULL_END

## IOS RTC 使用说明

#### 1.拷贝demo中的famework RTC相关依赖到项目中

#### 2.在项目工程中添加framework依赖
![frame_image.jpg](image%2Fframe_image.jpg)

#### 3.初始化RTCSDK
```swift
        let folderName = "rtclog" // 自定义文件夹名称
        let documentsURL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first!
        let folderURL = documentsURL.appendingPathComponent(folderName)
        
        let rtcConfig = RtcEngineConfig()
        rtcConfig.enableNativeLog = true
        rtcConfig.logPath = folderURL.path
        LJRtcEngine rtcEngine = LJRtcEngine.sharedEngine(c : rtcConfig)
        // 设置RTC是否运行在debug模式
        engine.setDebug(debug: 1)
```
#### 4.开启或关闭视频模块
```swift
    // 开启
    rtcEngine.enableVideo()
    // 关闭
    rtcEngine.disableVideo()
```

#### 5.开启或关闭音频模块
```swift
    // 开启
    rtcEngine.enableAudio()
    // 关闭
    rtcEngine.disableAudio()
```

#### 6.设置编码参数
```swift
    public class VideoEncoderConfiguration: NSObject {
        public var dimensions = CGSize(width : 640, height : 480) //分辨率
        public var codecType = LJVideoCodecType.h264 //编码方式
        public var frameRate : Int32 = 0 //编码帧率
        public var bitrate : Int32 = 0 // 编码码率
        public var minBitrate : Int32 = 0 // 最小码率
        public var orientationMode = LJVideoMirrorMode.auto //
        public var mirrorMode = LJVideoMirrorMode.disable //是否镜像
}

    rtcEngine.setVideoEncoderConfiguration(config: VideoEncoderConfiguration)
```

#### 7.设置本地预览
```swift
     @IBOutlet weak var previewView: LJPreviewView!
     self.view.insertSubview(previewView)
     previewView.autorotate = true;
     engine.setupLocalVideo(view: previewView)
```

#### 8.设置远端用户预览
```swift
    @IBOutlet weak var remoteView : UIView!
    self.view.insertSubview(remoteView)
    engine.setupRemoteVideo(view: remoteView)
```
#### 9.开始和关闭预览
```swift
    engine.startPreivew()
    
    engine.stopPreivew()
```

#### 10.禁止本地音频采集
```swift
    rtcEngine.disableAudio()
```

#### 11.禁止本地音频采集发布
```swift
    rtcEngine.muteLocalAudioStream(mute: localMuted)
```

#### 12.禁止远端音频采集播放
```swift
    rtcEngine.muteRemoteAudioStream(mute: remoteMuted)
```

#### 13.设置RTC工作模式 在不同设备中，rtc工作模式必须两端不同，例如云游戏的云端为push则客户端为pull
```swift
    @objc public enum RTCWorkMode : Int32 {
        case pull = 0,
             push = 1
    }
    rtcEngine.setWorkMode(mode: mode)
```

#### 14.加入频道
```swift
    let config = ChannelConfig()
    config.userID = ***
    config.token = "***"
    config.appID = *
    config.channelID = "*"
    let udpConfig = UdpInitConfig()
    config.configs.append(udpConfig)
    _ = engine.joinChannel(channelConfig: config)
```

#### 15.退出频道
```swift
    rtcEngine?.leaveChannel()
```

#### 16.注册解码回调
```swift
    func onVideoDecodeFrame(buf: UnsafeMutableRawPointer, size: Int, width: Int, height: Int, pixelFmt: Int) {
        <#code#>
    }
    engine.registerDecodeVideoFrameObserver(delegate:self)
```

#### 17.注册视频延时信息
```swift
    func onVideoDecodeFrame(delayInfo: RtcSDK.VideoDelayInfo) {
        <#code#>
    }
    engine.registerVideoDelayInfoObserver(delegate:self)
```
#### 18.销毁RTCEngine
```swift
    engine.destroy()
```
destroy
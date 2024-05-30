<h2 id="1">1V1 IOS RTC 使用说明</h2>

## IOS RTC 使用说明

### 1V1 RTC使用[示例代码ViewController.swift](iosrtc/ViewController.swift)

### 1.拷贝demo中的famework RTC相关依赖到项目中

### 2.在项目工程中添加framework依赖
![frame_image.jpg](image%2Fframe_image.jpg)

### 3.初始化RTCSDK

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

### 4.开启或关闭视频模块

```swift
    // 开启
    rtcEngine.enableVideo()
    // 关闭
    rtcEngine.disableVideo()
```

### 5.开启或关闭音频模块

```swift
    // 开启
    rtcEngine.enableAudio()
    // 关闭
    rtcEngine.disableAudio()
```

### 6.设置编码参数

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

### 7.设置本地预览

```swift
     @IBOutlet weak var previewView: LJPreviewView!
     self.view.insertSubview(previewView)
     previewView.autorotate = true;
     engine.setupLocalVideo(view: previewView)
```

### 8.设置远端用户预览

```swift
    @IBOutlet weak var remoteView : UIView!
    self.view.insertSubview(remoteView)
    engine.setupRemoteVideo(view: remoteView)
```

### 9.开始和关闭预览

```swift
    engine.startPreivew()
    
    engine.stopPreivew()
```

### 10.禁止本地音频采集

```swift
    rtcEngine.disableAudio()
```

### 11.禁止本地音频采集发布

```swift
    rtcEngine.muteLocalAudioStream(mute: localMuted)
```

### 12.禁止远端音频播放

```swift
    rtcEngine.muteRemoteAudioStream(mute: remoteMuted)
```

### 13.设置RTC工作模式 在不同设备中，rtc工作模式必须两端不同，例如云游戏的云端为push则客户端为pull

```swift
    @objc public enum RTCWorkMode : Int32 {
        case pull = 0,
             push = 1
    }
    rtcEngine.setWorkMode(mode: mode)
```

### 14.加入频道

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

### 15.退出频道

```swift
    rtcEngine?.leaveChannel()
```

### 16.注册解码回调

```swift
    func onVideoDecodeFrame(buf: UnsafeMutableRawPointer, size: Int, width: Int, height: Int, pixelFmt: Int) {
        <#code#>
    }
    engine.registerDecodeVideoFrameObserver(delegate:self)
```

### 17.注册视频延时信息

```swift
    func onVideoDecodeFrame(delayInfo: RtcSDK.VideoDelayInfo) {
        <#code#>
    }
    engine.registerVideoDelayInfoObserver(delegate:self)
```
### 18.销毁RTCEngine

```swift
    engine.destroy()
```

## IOS RTM 使用说明
#### 1.拷贝demo中的famework RTM相关依赖到项目中

#### 2.在项目工程中添加framework依赖
![rtm_framework.jpg](image/rtm_framwork.jpg)


<h2 id="2"> 1V1 RTM使用</h2>

## 1V1 RTM使用 ([示例代码RTMViewController.swift](iosrtc/RtmViewController.swift)) ，1V1设置模式，双方使用的模式(RTMConfig.role)必须不一样，一端为0 另外一端必须为1

#### 1.初始化

```swift
    import RtmSdk

    let config = RTMConfig()
    config.appId = GlobalConstants.appId // 服务端分配的appId
    config.localIp = 0
    config.dataWorkMode = 0 //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    config.isDebug = 1
    config.role = push ? RUDPMode.PUSH.rawValue : RUDPMode.PULL.rawValue // 1V1设置模式，双方使用的模式必须不一样，一端为0 另外一端必须为1
    config.token = GlobalConstants.token // 服务端分配的token
    rtmEngine = RTMEngine(config: config)

    rtmEngine?.setDebugEvn(isTestEvn: true) // 设置是正式环境还是测试环境，默认是正式环境
```

#### 2.注册链路状态回调以及消息回调实现IRtmMsgDelegate和IRtmEventDelegate

```swift
        class RTMViewController : UIViewController, IRtmMsgDelegate, IRtmEventDelegate {
            func onMsgcallbck(buf: UnsafePointer<CChar>?, size: UInt32, uid: UInt64) {
                let data = Data(bytes: buf!, count: Int(size))
                let str = String(data: data, encoding: .utf8)!
                let tempStr = "RTM onMsgcallbck uid \(uid) msg \(str)"
                showStr(tempStr: tempStr)
            }
            //LinkStatus时，status表示：API_STATUS_CONNECTED 1，API_STATUS_DISCONNECTED 2，API_STATUS_LOST 3
            // 当status == STATUS_CONNECTED时，表示与对端是连通的，可以互发消息
            func onLinkStatus(status: Int32) {
                showStr(tempStr: "RTM onLinkStatus status \(status)")
            }
        }

        rtmEngine?.eventDelegate = self
        rtmEngine?.subcribeMsgCallback(msgcallback : self)
```

#### 3.加入频道（指定uid以及channelId）

```swift
       rtmEngine?.joinChannel(uid: GlobalConstants.uid, channelId: channelTextField.text!)
```

#### 4.发送消息（支持发送String以及UnsafePointer<CChar>）

```swift
       rtmEngine?.sendMsg(msg: "test msg")
       rtmEngine?.sendMsg(msg : UnsafePointer<CChar>, len : Int32)
```

#### 5.退出频道
```swift
        rtmEngine?.leveChannel()
        rtmEngine = nil
```

<h2 id="3"> 多人 RTM使用</h2>

## 多人 RTM使用 [示例代码MultiRTMViewController.swift](iosrtc/MultiRTMViewController.swift) 无论是那一端RTMExConfig的role写死1

### 1.初始化

```swift
    import RtmSdk

    let config = RTMExConfig()
    config.appId = GlobalConstants.appId // 服务端分配的appId
    config.localIp = 0
    config.dataWorkMode = 0 //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    config.isDebug = 1
    config.role // 可以不用填，默认为1
    config.token = GlobalConstants.token // 服务端分配的token
    rtmEngine = RTMEngineEx(config: config)

    rtmEngine?.setDebugEvn(isTestEvn: true) // 设置是正式环境还是测试环境，默认是正式环境
```

### 2.注册链路状态回调以及消息回调实现IRtmMsgDelegate和IRtmExEventDelegate

```swift
        class MultiRTMViewController : UIViewController, IRtmMsgDelegate,IRtmExEventDelegate {
            public func onJoinChannelFail(result: Int32, msg: String) {
                showStr(tempStr: "RTMEx onJoinChannelFail result \(result) msg \(msg)")
            }
            
            public func onLeveChannelFail(result: Int32, msg: String) {
                showStr(tempStr: "RTMEx onLeveChannelFail result \(result) msg \(msg)")
            }
            
            public func onUserJoined(userId: UInt64) {
                showStr(tempStr: "RTMEx onUserJoined userId \(userId)")
            }
            
            public func onUserOffLine(userId: UInt64) {
                showStr(tempStr: "RTMEx onUserOffLine userId \(userId)")
            }
            
            public func onJoinChannelSuccess() {
                showStr(tempStr: "RTMEx onJoinChannelSuccess")
            }
            
            public func onLeveChannelSuccess() {
                showStr(tempStr: "RTMEx onLeveChannelSuccess")
            }
            
            //LinkStatus时，status表示：API_STATUS_CONNECTED 1，API_STATUS_DISCONNECTED 2，API_STATUS_LOST 3
            //当status == STATUS_CONNECTED时，表示与服务端是连通的，可以互发消息
            func onLinkStatus(status: Int32) {
            public func onLinkStatus(status : Int32) {
                showStr(tempStr: "RTMEx onLinkStatus status \(status)")
            }
        }

        rtmEngine?.eventDelegate = self
        rtmEngine?.subcribeMsgCallback(msgcallback : self)
```

### 3.加入频道（指定uid以及channelId）

```swift
       rtmEngine?.joinChannel(uid: GlobalConstants.uid, channelId: channelTextField.text!)
```

### 4.发送消息（支持发送String以及UnsafePointer<CChar>）

```swift
       rtmEngine?.sendMsg(msg: "test msg")
       rtmEngine?.sendMsg(msg : UnsafePointer<CChar>, len : Int32)
```

### 5.退出频道

```swift
        rtmEngine?.leveChannel()
        rtmEngine = nil
```

<h2 id="4"> 日志库使用说明</h2>

### 目前RTC打包，自动带上了日志库，主要依赖（basestone.framework 和 ljlog.framework）

```swift
      //初始化日志库，目前默认保存日志大小为5M，保存3天，超过5M则分文件，超过3天则删除
      // 日志等级
      // 日志保存的文件路径
      // 日志的文件名前缀
      // 是否打开控制台输出
      FLog.initLog(level: LogLeve.LOG_INFO.rawValue, logPath: "xxx", logPreName: "tag", openConsole: true)

      // 初始化后即可使用info等方法打印日志，并保存到文件
      FLog.info("xxxxxxxxxxxx")

      // 销毁日志
      FLogFlush(1)
      FlogDestroy()
```

<h2 id="5"> 多人RTC使用说明</h2>

## 因为多人RTC是基于1V1 RTC开发，因此1V1 RTC的接口，在多人频道中基本都可以使用，请参考[1V1 RTC 使用说明](#ios-rtc-使用说明)

## 多人RTC使用[示例代码MultiRTCViewController.swift](iosrtc/MultiRTCViewController.swift)

### sdk的初始化与1V1一致([拷贝demo中的famework RTC相关依赖到项目中](#1拷贝demo中的famework-rtc相关依赖到项目中)和[在项目工程中添加framework依赖](#2在项目工程中添加framework依赖))

### 1.初始化RTCSDK

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

### 2.开启或关闭视频模块

```swift
    // 开启
    rtcEngine.enableVideo()
    // 关闭
    rtcEngine.disableVideo()
```

### 3.开启或关闭音频模块

```swift
    // 开启
    rtcEngine.enableAudio()
    // 关闭
    rtcEngine.disableAudio()
```

### 4.设置编码参数

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

### 5.设置本地预览

```swift
     @IBOutlet weak var previewView: LJPreviewView!
     self.view.insertSubview(previewView)
     previewView.autorotate = true;
     engine.setupLocalVideo(view: previewView)
```

### 6.初始化多人频道以及加入多人频道(参数定义可参考([ChannelMediaOptions](app/src/main/java/com/linjing/rtc/demo/multichannel/ChannelMediaOptions_.text)))

```swift
    func join(){
        if joined {
            print("----already joined, ignore action")
            return
        }

        _channel = rtcEngine?.createChannel(channelId: channelTextField.text!, uid: Int64(GlobalConstants.uid))// 创建Channel
        let channelMediaOptions : ChannelMediaOptions = ChannelMediaOptions() // 创建ChannelMediaOptions
        // 参数定义可参考（[ChannelMediaOptions](app/src/main/java/com/linjing/rtc/demo/multichannel/ChannelMediaOptions_.text)）
        channelMediaOptions.publishMicrophoneTrack = true;
        channelMediaOptions.publishCameraTrack = true;
        /**
         *
         * @param token 服务器分配的token
         * @param appId 服务器分配的appid
         * @param uid 用户id
         * @param options 音视频配置
         */
        _channel?.joinChannel(token: GlobalConstants.token, appId: Int64(GlobalConstants.appId),
                              uid: Int64(GlobalConstants.uid), options: channelMediaOptions);
        _channel?.setRtcChannelEventHandler(eventHandler: self);
        joined = true
    }
```

### 7.设置多人频道事件回调

```swift
    func join(){
       ////////////////////////////////////////////
        _channel?.setRtcChannelEventHandler(eventHandler: self);
        joined = true
    }

    /////////////ILJChannelEventHandler
    ILJChannelEventHandler{
        public static Int QUALITY_GOOD = 1; /**< 网络质量好 */
        public static Int QUALITY_COMMON = 2; /**< 网络质量一般 */
        public static Int QUALITY_BAD = 3; /**< 勉强能沟通 */
        public static Int QUALITY_VBAD = 4; /**< 网络质量非常差，基本不能沟通。 */
        public static Int QUALITY_BLOCK = 5; /**< 链路不通 */
        /**
         *
         * @param ljChannel
         * @param uid 用户的网络状态
         * @param mLocalQuality 用户本地的网络状态
         * @param mRemoteQuality  这个值，在多人频道中，无用
         */
        func onNetQuality(_ ljChannel:LJChannel, uid: Int64, mLocalQuality: Int32, mRemoteQuality: Int32) {
            //FLog.info("MultiRtc onNetQuality uid=\(uid) mLocalQuality=\(mLocalQuality) mRemoteQuality=\(mRemoteQuality)")
        }
        /**
         * 加入频道，只是表示执行加入频道方法成功，并不表示连接连通可用，连接状态请参考onLinkStatus回调
         * @param channelId
         * @param uid
         * @param elapsed
         */
        func onJoinChannelSuccess(_ channelId: String, uid: Int64, elapsed: Int64) {
            FLog.info("MultiRtc onJoinChannelSuccess channelId=\(channelId) uid=\(uid) elapsed=\(elapsed)")
        }
        /**
         * 退出频道，只是表示执行退出频道方法成功，并不表示连接连通状态
         * @param ljChannel
         */
        func onLeaveChannelSuccess(_ ljChannel: LJChannel) {
            FLog.info("MultiRtc onLeaveChannelSuccess ljChannel=\(ljChannel.getChannelId())")
        }
        /**
         * channel的连接状态回调，这个才是链接是否可用的状态
         * @param ljChannel
         * @param result STATUS_CONNECTED 1 STATUS_DISCONNECTED 2 STATUS_LOST 3
         */
        func onLinkStatus(_ ljChannel: LJChannel, result: Int32) {
            FLog.info("MultiRtc onLinkStatus ljChannel=\(ljChannel.getChannelId()) result=\(result)")
        }
        /**
         * 频道中有新用户加入
         * @param ljChannel
         * @param uid 新用户的Uid
         * @param elapsed
         */
        func onUserJoined(_ ljChannel: LJChannel, uid: Int64, elapsed: Int64) {
           
        }
        /**
         * 频道中有用户退出
         * @param ljChannel
         * @param uid 退出的用户Uid
         */
        func onUserOffLine(_ ljChannel: LJChannel, uid: Int64) {
           
        }
        /**
         * 频道中，某个用户，第一帧视频数据被解码
         * @param ljChannel
         * @param uid 频道内某个用户uid
         * @param width 解码宽度
         * @param height 解码高度
         * @param joinTime
         */
        func onFirstRemoteVideoFrameDecode(_ ljChannel: LJChannel, uid: Int64, width: Int32, height: Int32, joinTime: Int64) {
            FLog.info("MultiRtc onFirstRemoteVideoFrameDecode ljChannel=\(ljChannel.getChannelId()) uid=\(uid) w=\(width) h=\(height)")
        }
        /**
         * 解码视频宽高变化
         * @param ljChannel
         * @param uid 视频宽高变化Uid
         * @param width 新的解码宽度
         * @param height 新的解码高度
         */
        func onVideoSizeChange(_ ljChannel: LJChannel, uid: Int64, width: Int32, height: Int32) {
            FLog.info("MultiRtc onVideoSizeChange ljChannel=\(ljChannel.getChannelId()) uid=\(uid) w=\(width) h=\(height)")
        }
    }
```

### 8.频道内新用户加入或者退出时，更新UI(在ILJChannelEventHandler中的onUserJoined和onUserOffLine)

```swift
    func onUserJoined(_ ljChannel: LJChannel, uid: Int64, elapsed: Int64) {
        FLog.info("MultiRtc onUserJoined ljChannel=\(ljChannel.getChannelId()) uid=\(uid)")
        DispatchQueue.main.async {
            // 在主线程中更新UI
            if (self.remoteViews.keys.contains(uid)) {
                return
            }
            let counts = self.remoteViews.count
            if (counts >= 3) {
                return
            }
            let rect = self.remoteViewsInfo[counts]
            let remoteView = LJRemoteView()
            // 禁用AutoresizingMask，以便使用Auto Layout
            remoteView.translatesAutoresizingMaskIntoConstraints = false

            // 将新视图添加到父视图中
            self.videoSeatViews.addSubview(remoteView)
            // 添加约束
            NSLayoutConstraint.activate([
                remoteView.topAnchor.constraint(equalTo: self.videoSeatViews.topAnchor, constant: rect!.minY),
                remoteView.leadingAnchor.constraint(equalTo: self.videoSeatViews.leadingAnchor, constant: rect!.minX),
                remoteView.widthAnchor.constraint(equalTo: self.view.widthAnchor, multiplier: 0.5), // 宽度为父视图宽度的一半
                remoteView.heightAnchor.constraint(equalToConstant: 160)
            ])
            self.remoteViews[uid] = remoteView
            // 设置远端显示的UI到多人频道中
            ljChannel.setForMultiChannelUser(videoView: remoteView, uid: uid, fps: 60)
        }
    }
    
    func onUserOffLine(_ ljChannel: LJChannel, uid: Int64) {
        FLog.info("MultiRtc onUserOffLine ljChannel=\(ljChannel.getChannelId()) uid=\(uid)")
        DispatchQueue.main.async {
            // 在主线程中更新UI
            if (!self.remoteViews.keys.contains(uid)) {
                return
            }
            // 销毁远端显示UI
            let remoteView = self.remoteViews.removeValue(forKey: uid)
            remoteView?.removeFromSuperview()
        }
    }
```

### 9.销毁Engine以及退出频道

```swift
    if (_channel != nil) {
        _channel!.leaveChannel();
        _channel!.releaseChannel();
        _channel = nil;
    }
    rtcEngine?.leaveChannel()
    LJRtcEngine.destroy()
    rtcEngine = nil
```

### 10.禁用远端音视频

```swift

    //若不需要音频则可以调用一下方法
    _channel.MuteRemoteAudioStream(频道内其他人的uid, mute);
    //若不需要视频则可以调用一下方法
    _channel.MuteRemoteVideoStream(频道内其他人的uid, mute);
```
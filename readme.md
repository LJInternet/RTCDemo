
## 一.Android 使用说明

### [1V1 RTC使用说明](android/readme.md#1)

### [1V1 RTM使用说明](android/readme.md#一1v1-rtm使用说明1v1-rtm使用需要注意joinchannel-role的设置一般两端必须保持不一致一端为值为0时另外一端必须为1-1v1示例p2prtmactivity)

### [多人RTM使用说明](android/readme.md#二多人rtm使用-示例multirtmactivity)

## 二.IOS 使用说明

### [1V1 RTC使用说明](iosrtc/readme.md#ios-rtc-使用说明)

### [1V1 RTM使用说明](iosrtc/readme.md#1v1-rtm使用-rtmviewcontrollerswift)

### [多人RTM使用说明](iosrtc/readme.md#多人-rtm使用multirtmviewcontrollerswift)

## 三.C++ Windows使用说明

### [1V1 RTC使用说明](windows/readme.md#windows-c层接入说明-接口定义在接口定义在media_engineh)

### [1V1 RTM使用说明](windows/readme.md#1v1-rtm使用p2prtmtestcpp-clientconstantsh)

### [多人RTM使用说明](windows/readme.md#多人-rtm使用multirtmtestcpp-clientconstantsh)

## 四.C++ Linux使用说明

### [1V1 RTC使用说明](linux/readme.md#linux-rtc-c层接入说明-接口定义在media_engineh)

### [1V1 RTM使用说明](linux/readme.md#1v1-rtm使用p2prtmtestcpp-clientconstantsh)

### [多人RTM使用说明](linux/readme.md#多人-rtm使用multirtmtestcpp-clientconstantsh)

## RTC 使用说明

## 一.Android RTC使用说明

#### 1.初始LJSDK，设置日志、统计上报等基础信息
```java
    /**
     * 初始化SDK需要填写的配置
     */
    public class LJSDKConfig {

        /**
         * 为每个业务分配的AppId
         */
        public String mAppId = "";
        /**
         * 测试环境
         */
        public boolean isTestEv = false;
        /**
         * 测试模式, 主要用于打印更多日志, 和其他用于测试的功能
         */
        public boolean isDebugMode = false;

        /**
         * 应用UA
         */
        public String mAppUa = "";

        /**
         * 打印日志实现
         */
        public IJLog mJLog;

        public IReportApi mIReportApi;
    }
    public static void initLJSDK(Application context) {
    LJSDKConfig config = new LJSDKConfig.Builder()
                    .setAppId("ssss")
                    .setDebugMode(true)
                    .setAppUa("test&1.1.0&offical")
                    .setTestEv(true)
                    .setJLog(new IJLog(){})
                    .setReportApi(new IReportApi() {}).build();

            LJSDK.instance().init(context, config, null);
    }
```

#### 2.创建RTCEngine
```java
// 暂时无用
    public class RtcEngineConfig {
        public IRtcEngineEventHandler mEventHandler = null;
    }

    @Retention(SOURCE)
    @interface ClientRole {
        /**
         * 推流模式
         */
        int CLIENT_ROLE_PUSH = 1;
        /**
         * 拉流模式
         */
        int CLIENT_ROLE_PULL = 2;
    }

    @Retention(SOURCE)
    @interface AudioProfile {
        /**
         0：默认设置。
        通信场景下，该选项代表指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
        直播场景下，该选项代表指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
        */
        int  AUDIO_PROFILE_DEFAULT = 0;
        /**
         1：指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
        */
        int AUDIO_PROFILE_SPEECH_STANDARD = 1;
        /**
         2：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
        */
        int AUDIO_PROFILE_MUSIC_STANDARD = 2;
        /**
         3：指定 48 kHz采样率，音乐编码，双声道，编码码率最大值为 80 Kbps。
        */
        int AUDIO_PROFILE_MUSIC_STANDARD_STEREO = 3;

        /**
         * 4：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 96 Kbps。
         */
        int AUDIO_PROFILE_MUSIC_HIGH_QUALITY = 4;

        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 128 Kbps。
         */
        int AUDIO_PROFILE_MUSIC_HIGH_QUALITY_STEREO = 5;
        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 80 Kbps,拿解码数据，不播放
         */
        int AUDIO_PROFILE_CALLBACK_DATA_NORENDER = 6;
    }

    @Retention(SOURCE)
    public @interface OrientationMode {
        int ORIENTATION_MODE_ADAPTIVE = 0;  // 自适应
        int ORIENTATION_MODE_FIXED_LANDSCAPE = 1; // 横屏
        int ORIENTATION_MODE_FIXED_PORTRAIT = 2; // 竖屏
    }

    // 创建RTCEngine
    RtcEngineConfig config = new RtcEngineConfig();
    IRtcEngine mRtcEngine = IRtcEngine.create(config);
    // 设置RTC工作模式  @see ClientRole，在不同设备中，rtc工作模式必须两端不同，例如云游戏的云端为push则客户端为pull
    mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
    // 设置音频工作场景
    mRtcEngine.setAudioProfile(RTCEngineConstants.AudioProfile.AUDIO_PROFILE_DEFAULT, 0);
    // 设置推流的编解码参数

    /**
     *
     * @param width 编码宽高
     * @param height 编码宽高
     * @param frameRate 编码帧率
     * @param bitrate 编码码率
     * @param orientationMode OrientationMode
     */
    VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration(1920, 1080,
                    30, 3000, VideoEncoderConfiguration.OrientationMode.ORIENTATION_MODE_FIXED_LANDSCAPE);
    // 启用视频模块，在加入频道成功后，调用StartPreview，则会自动编码推流
    mRtcEngine.enableVideo();
    // 启动音频模块，在加入频道成功后，则会自动编码推流
    mRtcEngine.enableAudio();
```
#### 3.设置本地预览
```java

    public void setupLocalVideo(Context context, FrameLayout group) {
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        surfaceView.setKeepScreenOn(true);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupLocalVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        group.addView(surfaceView);
    }
```

#### 4.设置远端用户预览
```java

    public void setupRemoteUi(Context context, FrameLayout group) {
        JLog.info("MediaPlayer", " setupRemoteUi ");
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupRemoteVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        surfaceView.setKeepScreenOn(true);
        group.addView(surfaceView);
    }
```

#### 5.加入频道
```java
    public class ChannelConfig {
        public List<UdpInitConfig> configs = new ArrayList<UdpInitConfig>();
        public String p2pSignalServer = "61.155.136.209:9988";

        /**
         * 服务器分配的Appid， 测试环境，可以自己定义
         */
        public long appID;
        /**
         * 用户Id
         */
        public long userID;
        /**
         * 频道Id
         */
        public String channelID;
        /**
         * 临时令牌，由服务器分配
         */
        public String token;
    }

    ChannelConfig channelConfig = new ChannelConfig();
    channelConfig.appID = 1;
    channelConfig.userID = uid;
    channelConfig.channelID = BuildConfig.sessionId + "";
    channelConfig.token = "token";
    mRtcEngine.joinChannel(channelConfig);
```

#### 6.退出频道
```java
     mRtcEngine.leaveChannel();
```

#### 7.销毁RTCEngine
```java
     mRtcEngine.destroy();
```


## 二.PC C++层接入说明 （接口定义在media_engine.h）
### Linux层接入说明 （接口定义在media_engine.h） 接入方法与PC相似，只是没有音视频的采集模块，需使用自定义的音视频采集，进行推送
#### 1.初始化RTCEngine
```cpp
    RTCEngineConfig rtc_config;
    rtc_config.enableLog = false;
    std::string rtccfgstr;
    ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
    media_engine* nginx = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
```

#### 2.初始化加入频道参数以及编码参数
```cpp
    MIEUploadConfig c;
    MIETransferConfig config;
    config.appID = 1; // 服务器分配的Appid， 测试环境，可以自己定义
    config.channelID = "95452311111"; // 频道Id
    config.userID = 3443434; //用户Id
    config.token = "token"; // 临时令牌，由服务器分配
    config.transferMode = PUSH_MODE; // 在不同设备中，rtc工作模式必须两端不同，例如云游戏的云端为push则客户端为pull
    // 这个配置的宽高应该跟采集宽高一致，否则在编码时，会根据编码帧的实际宽高编码，不会根据这个配置的框进行缩放和裁剪
    MIEVideoUploadConfig videoUploadConfig; 
    videoUploadConfig.encodeWidth = 640; // 视频编码宽
    videoUploadConfig.encodeHeight = 480; // 视频编码高
    videoUploadConfig.maxVideoBitrateInbps = 800000; // 编码最大码率
    videoUploadConfig.minVideoBitrateInbps = 700000; // 编码最小码率
    videoUploadConfig.realVideoBitrateInbps = 800000; // 实际编码码率
    videoUploadConfig.fps = 30; // 视频编码帧率
    c.transferConfig = config;
    c.videoUploadConfig = videoUploadConfig;

    // 加入频道
    std::string cfgstr;
    ljtransfer::mediaSox::PacketToString(c, cfgstr);
    media_engine_send_event(nginx, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());
```

#### 3.配置相机采集参数以及启动相机采集
```cpp
    char devices[1000];
    media_engine_camera_list(devices, 1000); // 获取相机列表，返回结果以 “;”分割，可以split “;”，获取相机列表
    // 配置采集参数
    CaptureConfig captureConfig; 
    captureConfig.width = 640; // 期望宽高
    captureConfig.height = 480; 
    captureConfig.fps = 30; // 采集帧率
    // FIT_XY 有效
    captureConfig.oriMode = ORIENTATION_MODE_FIXED_LANDSCAPE; // 横屏采集 640X480 // ORIENTATION_MODE_FIXED_LANDSCAPE; // 竖屏 480X640 // ORIENTATION_MODE_FIXED_PORTRAIT; // 按设置的宽高
    captureConfig.fillMode = FIT_XY;// COR_CENTER;相机出来的画面跟采集宽高不一致，则居中裁剪四周 // FIT_XY; 相机出来的画面跟采集宽高不一致，则画面缩放到采集期望宽高，按比例上下左右补黑边
    std::string cfgstr11;
    ljtransfer::mediaSox::PacketToString(captureConfig, cfgstr11);
    media_engine_start_camera_capture_with_config(nginx, "相机名称", cfgstr11.c_str(), cfgstr11.length());
    media_engine_subscribe_capture_video(nginx, on_capture_video, nullptr);

```

#### 4.配置音频模块
```cpp
    // 创建音频模块
    AudioEnableEvent createAudioEvent;
    createAudioEvent.evtType = AUDIO_CREATE;
    createAudioEvent.enabled = true;
    std::string audio_create_data;
    ljtransfer::mediaSox::PacketToString(createAudioEvent, audio_create_data);
    media_engine_send_event(nginx, createAudioEvent.evtType, (char*)audio_create_data.c_str(), audio_create_data.length());
    // 开启音频模块
    AudioEnableEvent captureEvent;
    captureEvent.evtType = AUDIO_ENABLE_EVENT;
    captureEvent.enabled = true;
    std::string audio_enable_data;
    ljtransfer::mediaSox::PacketToString(captureEvent, audio_enable_data);
    media_engine_send_event(nginx, captureEvent.evtType, (char*)audio_enable_data.c_str(), audio_enable_data.length());

```
#### 5.停止音频采集采集
```cpp
    // 停止音频模块
    AudioEnableEvent captureEvent;
    captureEvent.evtType = AUDIO_ENABLE_EVENT;
    captureEvent.enabled = false;
    std::string audio_enable_data;
    ljtransfer::mediaSox::PacketToString(captureEvent, audio_enable_data);
    media_engine_send_event(nginx, captureEvent.evtType, (char*)audio_enable_data.c_str(), audio_enable_data.length());
    // 销毁音频模块
    AudioEnableEvent createAudioEvent;
    createAudioEvent.evtType = AUDIO_DESTROY;
    createAudioEvent.enabled = true;
    std::string audio_create_data;
    ljtransfer::mediaSox::PacketToString(createAudioEvent, audio_create_data);
    media_engine_send_event(nginx, createAudioEvent.evtType, (char*)audio_create_data.c_str(), audio_create_data.length());

```

#### 6.订阅解码视频（在加入频道前设置）
```cpp
    static void OnDecodeVideoWithDelayCallback(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt, std::map<uint64_t, uint64_t> delayMap, void* context) {
        printf("OnDecodeVideoWithDelayCallback %d X %d pixel_fmt %d %lu \n", width, height, pixel_fmt, delayMap[KEY_CAPTURE_TIME]);

    }
    // 订阅解码视频
    media_engine_subscribe_video_with_delay(nginx, OnDecodeVideoWithDelayCallback, nginx);
```

#### 7.停止视频采集
```cpp
    media_engine_stop_camera_capture(nginx);
```

#### 8.自定义视频采集推送视频数据
```cpp
    CaptureVideoFrame frame;
    frame.stride = 640;
    frame.width = 640;
    frame.height = 480;
    frame.rotation = 0;
    frame.type = VIDEO_BUFFER_RAW_DATA;
    frame.format = VIDEO_PIXEL_I420; // 数据格式
    frame.timestamp = LJ::DateTime::currentTimeMillis();
    frame.mirror = 0;
    std::string data;
    ljtransfer::mediaSox::PacketToString(frame, data);

     media_engine_push_video(mMediaEngine, (const char*)yuvData, size, data.c_str(), data.length(), 2);
```

#### 9.自定义音频采集推送音频数据
```cpp
    /**
     * 发送pcm音频
     * @param engine
     * @param pcm
     * @param frame_num 每个channel的音频采样数
     * @param sampleRate 采样率
     * @param channelCount 声道数
     * @param bytePerSample Int16 2 int8 1 int32 4
     */
	MEDIATRANSFER_EXTERN void media_engine_push_audio(struct media_engine* engine, const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample);
```

#### 10.退出频道
```cpp
    media_engine_send_event(nginx, LEAVE_CHANNEL, nullptr, 0);
```

#### 11.销毁RTC engine
```cpp
    /**
     * 销毁RTC engine
     */
	MEDIATRANSFER_EXTERN void media_engine_destroy(struct media_engine*);
```

## 三.Linux层接入说明 （接口定义在media_engine.h） 接入方法与PC相似，只是没有音视频的采集模块，需使用自定义的音视频采集，进行推送
```cpp
static void testLinuxPull() {
        // 创建RTC实例
        RTCEngineConfig rtc_config;
        rtc_config.enableLog = false;
        std::string rtccfgstr;
        ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
        mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
        media_engine_set_debug_env(true);

        // 订阅解码视频
        media_engine_subscribe_video(mMediaEngine, OnDecodeVideoCallback, mMediaEngine);

        // 加入频道
        MIEUploadConfig c;
        MIETransferConfig config;
        config.appID = 1;
        config.channelID = "954523111";
        config.userID = 34434341;
        config.token = "token";
        config.transferMode = 0; // 0设置为server模式
        c.transferConfig = config;
        std::string cfgstr;
        ljtransfer::mediaSox::PacketToString(c, cfgstr);
        media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

        while (true) {
            LJ::SystemUtil::sleep(500);
            std::string msgStr = "test pull";
            rudp_engine_send(engine, msgStr.c_str(), msgStr.size());
        }

        LJ::SystemUtil::sleep(1000 * 60 * 500);

        media_engine_destroy(mMediaEngine);
    }

    static void testLinuxPush() {
        // 创建RTC实例
        RTCEngineConfig rtc_config;
        rtc_config.enableLog = false;
        std::string rtccfgstr;
        ljtransfer::mediaSox::PacketToString(rtc_config, rtccfgstr);
        mMediaEngine = media_engine_create(rtccfgstr.c_str(), rtccfgstr.length());
        media_engine_set_debug_env(true);

        // 加入频道
        MIEUploadConfig c;
        MIETransferConfig config;
        config.appID = 1;
        config.channelID = "954523111";
        config.userID = 3443434;
        config.token = "token";
        config.transferMode = 1; // 1设置为client模式
        c.transferConfig = config;
        std::string cfgstr;
        ljtransfer::mediaSox::PacketToString(c, cfgstr);
        media_engine_send_event(mMediaEngine, JOIN_CHANNEL, (char*)cfgstr.c_str(), cfgstr.length());

        while (true) {
            LJ::SystemUtil::sleep(100);
            std::string msgStr = "test push";
            rudp_engine_send(engine, msgStr.c_str(), msgStr.size());
        }

        LJ::SystemUtil::sleep(1000 * 60 * 500);

        media_engine_destroy(mMediaEngine);
    }
```

## 四.Unity层接入说明

###### 1.初始化
```csharp
public class RtcLogImpl : ILog
    {
        public void Debug(string msg)
        {
            FLog.Debug(msg);
        }

        public void Error(string msg)
        {
            FLog.Debug(msg);
        }

        public void Info(string msg)
        {
            FLog.Debug(msg);
        }
    }

    RtcEngineConfig config = new RtcEngineConfig();
    config.mReport = new ReportImpl();
    config.mAppId = 111;
    config.isTestEv = true;
    config.mJLog = new RtcLogImpl(); // 日志实现
    config.mReport = new ReportImpl();
    IRtcEngine mRtcEngine = IRtcEngine.CreateRtcEngine(config);
    // 启动音频模块,joinChannel成功后，会自动启动音频采集编码和推流
    mRtcEngine.EnableAudio(true);
    // 启动视频模块，joinChannel成功后，会自动启动视频采集编码和推流
    mRtcEngine.EnableAudio(true);
 ```

###### 2.设置RTC工作模式
```csharp
    public enum CLIENT_ROLE_TYPE
    {
        ///
        /// <summary>
        /// 1: Host. A host can both send and receive streams.
        /// </summary>
        ///
        CLIENT_ROLE_BROADCASTER = 1, // PUSH

        ///
        /// <summary>
        /// 2: (Default) Audience. An audience member can only receive streams.
        /// </summary>
        ///
        CLIENT_ROLE_AUDIENCE = 2, // PULL
    };
    /// 在不同设备中，rtc工作模式必须两端不同，例如云游戏的云端为CLIENT_ROLE_BROADCASTER则客户端为CLIENT_ROLE_AUDIENCE
    mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
 ```

###### 3.设置编码参数
```csharp

     public enum FILL_MODE {
        COR_CENTER = 0, // 居中裁剪
        FIT_XY = 1 //按宽高视频填满，宽或者高不满，则补充黑边
    }; 
    public static VideoEncoderConfiguration CreateVideoEncodeConfiguration() {
        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(ENCODE_WIDTH, ENCODE_HEIGHT);
        encoderConfiguration.frameRate = 15;
        encoderConfiguration.orientationMode = OriMode;
        encoderConfiguration.fillMode = FILL_MODE.FIT_XY;
        return encoderConfiguration;
    }

     mRtcEngine.SetVideoEncoderConfiguration(CreateVideoEncodeConfiguration());
 ```

###### 4.设置本地预览控件
```csharp
    private static LJVideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        // to be renderered onto
        RawImage rawImage = go.AddComponent<RawImage>();
        //rawImage.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, 1f);
        //rawImage.rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, 1f);
        // make the object draggable
        go.AddComponent<UIElementDrag>();
        GameObject canvas = GameObject.Find("VideoCanvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
            Debug.Log("add video view");
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }
        // set up transform

        go.transform.localPosition = Vector3.zero;
        // configure videoSurface，添加预览控件
        LJVideoSurface videoSurface = go.AddComponent<LJVideoSurface>();
        int width = InitHelper.GetEncodeWidth();
        int height = InitHelper.GetEncodeHeight();
        float temp = 1.0f * height / width;
        width = Math.Min(InitHelper.GetEncodeWidth(), 300);
        height = (int)(width * temp);
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        return videoSurface;
    }

    // 启动预览
    if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
    {
        yield return  Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }
    if (mRtcEngine != null)
    {
        mRtcEngine.StartPreview();
    }
 ```

###### 5.设置远端显示控件
```csharp
    // JoinChannel后调用
    mRtcEngine.SetRemoteRender(RawImage);
 ```

###### 6.加入频道
```csharp
    // JoinChannel后调用
    ChannelConfig channelConfig = new ChannelConfig();
    channelConfig.appID = 1; // 服务器分配的Appid， 测试环境，可以自己定义
    channelConfig.channelID = "95452311111"; // 频道Id
    channelConfig.userID = 3443434; //用户Id
    channelConfig.token = "token"; // 临时令牌，由服务器分配
    mRtcEngine.JoinChannel(channelConfig);
 ```

###### 7.自定义推送视频和音频（指定要推送视频数据时，需要停止sdk内部的相机采集，调用StopPreview() 方法）(指定要推送音频数据时，需要停止sdk内部的音频采集，调用EnableAudio(false)方法)
```csharp
    public class CaptureVideoFrame
    {

        ///
        /// <summary>
        /// The video type. See VIDEO_BUFFER_TYPE .
        /// </summary>
        ///
        public VIDEO_BUFFER_TYPE type;

        ///
        /// <summary>
        /// The pixel format. See VIDEO_PIXEL_FORMAT .
        /// </summary>
        ///
        public VIDEO_PIXEL_FORMAT format;

        ///
        /// <summary>
        /// Video frame buffer.
        /// </summary>
        ///
        public byte[] buffer;

        ///
        /// <summary>
        /// Line spacing of the incoming video frame, which must be in pixels instead of bytes. For textures, it is the width of the texture.
        /// </summary>
        ///
        public int stride;

        public int width;
        ///
        /// <summary>
        /// Height of the incoming video frame.
        /// </summary>
        ///
        public int height;

        ///
        /// <summary>
        /// Raw data related parameter 0~1. The number of pixels trimmed from the left. The default value is 0.
        /// </summary>
        ///
        public float cropLeft;

        ///
        /// <summary>
        /// Raw data related parameter0~1. The number of pixels trimmed from the top. The default value is 0.
        /// </summary>
        ///
        public float cropTop;

        ///
        /// <summary>
        /// Raw data related parameter0~1. The number of pixels trimmed from the right. The default value is 0.
        /// </summary>
        ///
        public float cropRight;

        ///
        /// <summary>
        /// Raw data related parameter0~1. The number of pixels trimmed from the bottom. The default value is 0.
        /// </summary>
        ///
        public float cropBottom;

        ///
        /// <summary>
        /// Raw data related parameter. The clockwise rotation of the video frame. You can set the rotation angle as 0, 90, 180, or 270. The default value is 0.
        /// </summary>
        ///
        public int rotation;

        ///
        /// <summary>
        /// Timestamp (ms) of the incoming video frame. An incorrect timestamp results in frame loss or unsynchronized audio and video.
        /// </summary>
        ///
        public long timestamp;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format.When using the OpenGL interface (javax.microedition.khronos.egl.*) defined by Khronos, set eglContext to this field.When using the OpenGL interface (android.opengl.*) defined by Android, set eglContext to this field.
        /// </summary>
        ///
        public byte[] eglContext;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. Texture ID of the frame.
        /// </summary>
        ///
        public EGL_CONTEXT_TYPE eglType;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. Incoming 4 x 4 transformational matrix. The typical value is a unit matrix.
        /// </summary>
        ///
        public int textureId;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. The MetaData buffer. The default value is NULL.
        /// </summary>
        ///
        public byte[] metadata_buffer;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. The MetaData size. The default value is 0.
        /// </summary>
        ///
        public int metadata_size;

        public int mirror = (int)MIRROR_TYPE.MIRROR_NONE;
        public int programType = (int)ProgramType.TEXTURE_2D_SRGB;
        public int corpType = (int)CropType.TOP_LEFT;
    }

    // JoinChannel后调用
    // 指定要推送视频数据时，需要停止sdk内部的相机采集，调用StopPreview() 方法
    mRtcEngine.PushVideoCaptureFrame(CaptureVideoFrame videoFrame);

    // 指定要推送音频数据时，需要停止sdk内部的音频采集，调用EnableAudio(false)方法
     /**
     * 发送pcm音频
     * @param engine
     * @param pcm
     * @param frame_num 每个channel的音频采样数
     * @param sampleRate 采样率
     * @param channelCount 声道数
     * @param bytePerSample Int16 2 int8 1 int32 4
     */
	MEDIATR
    mRtcEngine.PushAudioFrame(byte[] pcm, int sampleRate, int channelCount, int bytePerSample);
 ```

###### 8.退出频道
```csharp
    // JoinChannel后调用
    mRtcEngine.LeaveChannel();
 ```

###### 9.销毁RTC
```csharp
    // JoinChannel后调用
    mRtcEngine.OnDestroy();
 ```
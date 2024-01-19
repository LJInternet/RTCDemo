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
```[readme.md](..%2Freadme.md)

#### 6.退出频道
```java[readme.md](..%2Freadme.md)
     mRtcEngine.leaveChannel();
```

#### 7.销毁RTCEngine
```java
     mRtcEngine.destroy();
```

#### 8.增加权限，在AndroidManifest.xml增新增权限
```java
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.RECORD_AUDIO" />
    <uses-permission android:name="android.permission.READ_PHONE_STATE" />
    <uses-permission android:name="android.permission.INTERNET"/>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE"/>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.CHANGE_NETWORK_STATE" />
```

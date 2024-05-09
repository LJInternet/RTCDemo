## RTC 使用说明

## 一. 1V1 RTC使用说明（1V1 RTC使用需要注意ClientRole的设置，一般两端必须保持不一致，一端为值为0时，另外一端必须为1）

#### 1.增加权限，在AndroidManifest.xml增新增权限
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

#### 2.初始LJSDK，设置日志、统计上报等基础信息
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

#### 3.创建RTCEngine
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
#### 4.设置本地预览
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

#### 5.设置远端用户预览
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

#### 6.加入频道
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

#### 7.退出频道
```java
     mRtcEngine.leaveChannel();
```

#### 8.销毁RTCEngine
```java
     mRtcEngine.destroy();
```

## RTM 使用说明

###### 一.1V1 RTM使用说明：（1V1 RTM使用需要注意joinChannel role的设置，一般两端必须保持不一致，一端为值为0时，另外一端必须为1）[1V1示例](app/src/main/java/com/linjing/rtc/demo/rtm/P2PRTMActivity.java)

###### 创建RTM实例：
```java
    private RudpEngineJni mRudpEngine;

    mRudpEngine = new RudpEngineJni();
    // 创建的同时设置数据回调以及连接状态相关回调
    mRudpEngine.create(new RUDPCallback() {
    @Override
    public void onDataCallback(long uid, String channelId, byte[] data) {

    }

    /**
    *
    * @param uid
    * @param channelId
    * @param type RudpEngineConstants，当type == LinkStatus时，result表示：LinkStatusConnected，LinkStatusDisconnected，LinkStatusLost
    * @param result 0 成功，否则失败
    * @param msg
    */
    @Override
    public void onEventCallback(long uid, String channelId, int type, int length, String msg) {

    }
    });
```
###### 加入RTM频道：
```java
    /**
     * 当前1V1 RTM与RTC使用相同的ChannelId，因此需要同时使用RTC，才会生效
     * @param token
     * @param role 是否是控制端，表示一个是控制端  1，一个是被控制端 0，在两个不同设备时，需要两个端的role不一样
     * @param isDebug 是否是测试环境
     * @param dataWorkMode RTM 的工作模式 @see RudpEngineConstants
     * @param uid 用户ID
     * @param appId 用户Appid
     * @param channelId 频道ID
     */
     mRudpEngine.joinChannel("token", 1/0, true, 0, 2, 1111, channelId);
```

###### 发送消息：
```java
    mRudpEngine.sendMessage(byte);
```

###### 退出频道：
```java
    mRudpEngine.leaveChannel();
```

###### 销毁：
```java
    mRudpEngine.destroy();
```

###### 二.多人RTM使用[示例](app/src/main/java/com/linjing/rtc/demo/rtm/MultiRTMActivity.java)
###### 创建RTM实例：
```java
    private RudpEngineWrapperJni mRudpEngine;

    mRudpEngine = new RudpEngineWrapperJni();
    // 创建的同时设置数据回调以及连接状态相关回调
    mRudpEngine.create(new RUDPCallback() {
    @Override
    public void onDataCallback(long uid, String channelId, byte[] data) {

    }

    /**
    *
    * @param uid
    * @param channelId
    * @param type RudpEngineConstants，当type == LinkStatus时，result表示：LinkStatusConnected，LinkStatusDisconnected，LinkStatusLost
    * @param result 0 成功，否则失败
    * @param msg
    */
    @Override
    public void onEventCallback(long uid, String channelId, int type, int length, String msg) {

    }
    });
```
###### 加入RTM频道：
```java
    /**
     * 当前1V1 RTM与RTM使用相同的ChannelId，因此需要同时使用RTC，才会生效
     * @param token
     * @param isDebug 是否是测试环境
     * @param dataWorkMode RTM 的工作模式 @see RudpEngineConstants
     * @param uid 用户ID
     * @param appId 用户Appid
     * @param channelId 频道ID
     */
     mRudpEngine.joinChannel("token", true, 0, 2, 1111, channelId);
```

###### 发送消息：
```java
    mRudpEngine.sendMessage(uid, chanelId, byte);
```
###### 退出频道：
```java
    mRudpEngine.leaveChannel();
```

###### 销毁：
```java
    mRudpEngine.destroy();
```
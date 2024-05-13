## RTC 使用说明
<h2 id="1">1V1 RTC使用说明</h2>

#### （1V1 RTC使用需要注意ClientRole的设置，一般两端必须保持不一致，一端为值为0时，另外一端必须为1）

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

<h2 id="2">多人RTC使用：</h2>

## 因为多人RTC是基于1V1 RTC开发，因此1V1 RTC的接口，在RTCEngineEx中基本都可以使用，请参考[1V1 RTC 使用说明](#rtc-使用说明)

## RTC使用demo示例代码[MultiChannelPresenter.java](app/src/main/java/com/linjing/rtc/demo/multichannel/MultiChannelPresenter.java)和[MultiChannelActivity.java](app/src/main/java/com/linjing/rtc/demo/multichannel/MultiChannelActivity.java)中

### sdk的初始化与1V1一致([增加权限，在AndroidManifest.xml增新增权限](#1增加权限在androidmanifestxml增新增权限)和[初始LJSDK，设置日志、统计上报等基础信息](#2初始ljsdk设置日志统计上报等基础信息))

### 1.初始化多人RTCEngine

```java
     RtcEngineConfig config = new RtcEngineConfig();
    mRtcEngine = IRtcEngineEx.CreateRtcEngineEx(config);
    // 设置音频场景，这里会初始化音频的采样率和声道数
    mRtcEngine.setAudioProfile(RTCEngineConstants.AudioProfile.AUDIO_PROFILE_DEFAULT, 0);
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
    // 启用视频模块，在加入频道成功后，调用StartPreview，则会自动编码推流，在多人频道不用调用rtcengine的joinchannel
    mRtcEngine.enableVideo();
    // 启动音频模块，在加入频道成功后，则会自动编码推流，在多人频道不用调用rtcengine的joinchannel
    mRtcEngine.enableAudio();
```

### 2.设置本地预览

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

### 3.创建多人频道加入频道（[ChannelMediaOptions](app/src/main/java/com/linjing/rtc/demo/multichannel/ChannelMediaOptions_.text)）

```java
    public void startUpload(String channelId) {
        LJChannel _channel = mRtcEngine.CreateChannel(channelId, _userId);// 创建Channel
        ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions();// 创建ChannelMediaOptions
        channelMediaOptions.publishMicrophoneTrack = true;//发送麦克风采集的音频,默认是false
        channelMediaOptions.publishCameraTrack = true;// 发送相机采集视频，默认false
        // 其他参数请参考[ChannelMediaOptions](app/src/main/java/com/linjing/rtc/demo/multichannel/ChannelMediaOptions_.text)，该文件只是为了注释，正常使用时，不要使用该类
        /**
         *
         * @param token 服务器分配的token
         * @param appId 服务器分配的appid
         * @param uid 用户id
         * @param options 音视频配置
         */
        _channel.JoinChannel(BuildConfig.token, BuildConfig.appId, _userId, channelMediaOptions);
    }
```

### 4.设置多人频道事件回调

```java
    public class ILJChannelEventHandler {

        public static int QUALITY_GOOD = 1; /**< 网络质量好 */
        public static int QUALITY_COMMON = 2; /**< 网络质量一般 */
        public static int QUALITY_BAD = 3; /**< 勉强能沟通 */
        public static int QUALITY_VBAD = 4; /**< 网络质量非常差，基本不能沟通。 */
        public static int QUALITY_BLOCK = 5; /**< 链路不通 */
        /**
         *
         * @param ljChannel
         * @param uid 用户的网络状态
         * @param mLocalQuality 用户本地的网络状态
         * @param mRemoteQuality  这个值，在多人频道中，无用
         */
        public void onNetQuality(LJChannel ljChannel, long uid, int mLocalQuality, int mRemoteQuality) {

        }

        /**
         * 加入频道，只是表示执行加入频道方法成功，并不表示连接连通可用，连接状态请参考onLinkStatus回调
         * @param channelId
         * @param uid
         * @param elapsed
         */
        public void onJoinChannelSuccess(String channelId, long uid, long elapsed) {

        }

        /**
         * 退出频道，只是表示执行退出频道方法成功，并不表示连接连通状态
         * @param ljChannel
         */
        public void onLeaveChannelSuccess(LJChannel ljChannel) {

        }

        /**
         * channel的连接状态回调，这个才是链接是否可用的状态
         * @param ljChannel
         * @param result STATUS_CONNECTED 1 STATUS_DISCONNECTED 2 STATUS_LOST 3
         */
        public void onLinkStatus(LJChannel ljChannel, int result) {

        }

        /**
         * 频道中有新用户加入
         * @param ljChannel
         * @param uid 新用户的Uid
         * @param elapsed
         */
        public void onUserJoined(LJChannel ljChannel, long uid, int elapsed) {

        }

        /**
         * 频道中有用户退出
         * @param ljChannel
         * @param uid 退出的用户Uid
         */
        public void onUserOffLine(LJChannel ljChannel, long uid) {

        }

        /**
         * 频道中，某个用户，第一帧视频数据被解码
         * @param ljChannel
         * @param uid 频道内某个用户uid
         * @param width 解码宽度
         * @param height 解码高度
         * @param joinTime
         */
        public void onFirstRemoteVideoFrameDecode(LJChannel ljChannel, long uid, int width, int height, int joinTime) {

        }

        /**
         * 解码视频宽高变化
         * @param ljChannel
         * @param uid 视频宽高变化Uid
         * @param width 新的解码宽度
         * @param height 新的解码高度
         */
        public void onVideoSizeChange(LJChannel ljChannel, long uid, int width, int height) {

        }
    }
    // 设置多人频道事件回调
    _channel.setRtcChannelEventHandler(ILJChannelEventHandler);
```

### 5.频道内新用户加入或者退出时，更新UI(在ILJChannelEventHandler中的onUserJoined和onUserOffLine)

```java
    // 有用户新加入频道
    public void onUserJoined(IRtcEngineEx mRtcEngine, LJChannel ljChannel, long uid, int fps) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // _remoteViews 是一个FrameLayout的一个map，实际应用中，可以使用recycleView或者ListView
                if (_remoteViews.containsKey(uid)) {
                    JLog.info("multiChannel", "onUserJoined _remoteViews containsKey " + uid);
                    return;
                }
                FrameLayout frameLayout = _remoteViewsSet.pop();
                if (frameLayout == null) {
                    JLog.info("multiChannel", "onUserJoined frameLayout is null ");
                    return;
                }
                // 创建新的渲染UI
                SurfaceView surfaceView = mRtcEngine.CreateRendererView(MultiChannelActivity.this);
                surfaceView.setKeepScreenOn(true);
                VideoViews views = new VideoViews(surfaceView);
                surfaceView.setZOrderMediaOverlay(true);
                // 设置UI给多人频道
                ljChannel.SetForMultiChannelUser(views, uid, fps);
                frameLayout.addView(surfaceView);
                _remoteViews.put(uid, frameLayout);
            }
        });
    }
    // 有人退出频道，销毁UI
    public void onUserOffLine(IRtcEngineEx mRtcEngine, LJChannel ljChannel, long uid) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                FrameLayout frameLayout = _remoteViews.remove(uid);
                if (frameLayout != null) {
                    frameLayout.removeAllViews();
                    _remoteViewsSet.push(frameLayout);
                }
            }
        });
    }
```

### 6.销毁Engine以及退出频道

```java
    if (_channel != null) {
        _channel.LeaveChannel();
        _channel.ReleaseChannel();
        _channel = null;
    }
    if (mRtcEngine != null) {
        mRtcEngine.leaveChannel();
        mRtcEngine.destroy();
        mRtcEngine = null;
    }
```

### 7.禁用远端音视频

```java

    //若不需要音频则可以调用一下方法
    _channel.MuteRemoteAudioStream(频道内其他人的uid, mute);
    //若不需要视频则可以调用一下方法
    _channel.MuteRemoteVideoStream(频道内其他人的uid, mute);
```


## RTM 使用说明

<h2 id="3">1V1 RTM使用说明：</h2>

## （1V1 RTM使用需要注意joinChannel role的设置，一般两端必须保持不一致，一端为值为0时，另外一端必须为1） [1V1示例P2PRTMActivity](app/src/main/java/com/linjing/rtc/demo/rtm/P2PRTMActivity.java)

### 创建1v1 RTM实例：

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
    * @param type RudpEngineConstants，当type == LinkStatus时，result表示：LinkStatusConnected，LinkStatusDisconnected，LinkStatusLost，当status == STATUS_CONNECTED时，表示与对端是连通的，可以互发消息
    * @param result 0 成功，否则失败
    * @param msg
    */
    @Override
    public void onEventCallback(long uid, String channelId, int type, int length, String msg) {

    }
    });
```

### 加入RTM频道：

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

### 发送消息：

```java
    mRudpEngine.sendMessage(byte);
```

### 退出频道：

```java
    mRudpEngine.leaveChannel();
```

### 销毁：

```java
    mRudpEngine.destroy();
```

<h2 id="4">多人RTM使用：</h2>

### [示例MultiRTMActivity](app/src/main/java/com/linjing/rtc/demo/rtm/MultiRTMActivity.java)

### 创建多人RTM实例：

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
    * @param type RudpEngineConstants，当type == LinkStatus时，result表示：LinkStatusConnected，LinkStatusDisconnected，LinkStatusLost，当status == STATUS_CONNECTED时，表示与服务端是连通的，可以互发消息
    * @param result 0 成功，否则失败
    * @param msg
    */
    @Override
    public void onEventCallback(long uid, String channelId, int type, int length, String msg) {

    }
    });
```

### 加入多人RTM频道：

```java
    /**
     * @param token
     * @param isDebug 是否是测试环境
     * @param dataWorkMode RTM 的工作模式 @see RudpEngineConstants
     * @param uid 用户ID
     * @param appId 用户Appid
     * @param channelId 频道ID
     */
     mRudpEngine.joinChannel("token", true, 0, 2, 1111, channelId);
```

### 发送消息：

```java
    mRudpEngine.sendMessage(uid, chanelId, byte);
```

### 退出多人RTM频道：

```java
    mRudpEngine.leaveChannel();
```

### 销毁多人RTM：

```java
    mRudpEngine.destroy();
```

<h2 id="5">日志库使用说明</h2>

### 日志写文件（基本实现代码是从mars的xlog移植过来，进行了一些定制化的修改）

### a.当应用启动时候进行初始化

```java
    FLog xlog = new FLog();
    // 设置实例到Log中，方便静态方法打印日志
    Log.setLogImp(xlog);
    /**
     设置是否同时在控制台输出日志，android 是logcat 
    */
    Xlog.setConsoleLogOpen(true);
    mLogPath = logPath;
    /**
	 *
	 * @param level 日志等级
	 * @param mode 同步或者异步模式
	 * @param cacheDir 缓存文件夹路径，一般传”“即可
	 * @param logDir 日志文件夹路径
	 * @param nameprefix 日志文件的前缀
	 * @param cacheDays 日志缓存天数
	 * @param pubkey 传”“即可
	 */
    Xlog.appenderOpen(Xlog.LEVEL_DEBUG, Xlog.AppednerModeAsync, "", logPath, "LJLog", 2, "");
    // 设置每个日志文件的大小
    Xlog.setMaxFileSize(10 * 1024 * 1024); // 10M
    // 设置日志缓存时间，秒为单位，以下是3天，每次启动换删除超过该时间的日志文件
    Xlog.setMaxAliveTime(3 * 24 * 60 * 60);
    
    /////////////////////////////////////////////////////////////////
    
    /**
	 *
	 * @param level 日志等级
	 * @param mode 同步或者异步模式
	 * @param cacheDir 缓存文件夹路径，一般传”“即可
	 * @param logDir 日志文件夹路径
	 * @param nameprefix 日志文件的前缀
	 * @param cacheDays 日志缓存天数
	 * @param pubkey 传”“即可
	 */
	public static void open(int level, int mode, String cacheDir, String logDir, String nameprefix, int cacheDays, String pubkey);

	/**
	 * 销毁日志实例
	 */
	public  void appenderClose();
	/**
	 * 把缓存的日志刷新到文件中
	 */
	public native void appenderFlush(boolean isSync);
	/**
	 * 设置是否需要在控制台打印日志
	 */
	public void setLogConsoleOpen(boolean isOpen);
	/**
	 *  设置每个文件的大小，单位是B， 例如：10 * 1024 * 1024 1M
	 * @param size
	 */
	public void setLogMaxFileSize(long size);

	/**
	 * 设置日志最大的保存时间，内部会定期清理，单位是秒 例如：24 * 60 * 60 既一天
	 * @param aliveSeconds
	 */
	public void seLogMaxAliveTime(long aliveSeconds);

```

### b.当应用退出时候，进行注销

````java
    Log.appenderClose();
````

### c.进行日志打印时，可以直接调用等等

````java
    Log.d();
    Log.e();
    Log.i();
````

<h2 id="6">反馈使用说明</h2>

```java
    
    /**
     *
     * @param token 用户登录成功的token，用于请求上传信息时，做校验
     * @param host 反馈请求的域名SDK内部设置生产环境为"app.fancyjing.com” 测试环境为"testapp.fancyjing.com"
     * @param port 端口，默认-1，没有则填-1
     * @param isDebug 是否是测试，用于旋转host
     */
     public void init(String token, String host, int port, boolean isDebug);
     
     /**
     * 销毁反馈实例
     */
     public void destroy();
    
     /**
     * 反馈日志
     * @param title 反馈标题
     * @param content 反馈内容
     * @param filePath 日志文件夹路径
     * @param liveId liveId
     */
     public void sendFeedback(String title, String content, String filePath, String liveId);
     
     /**
     * 设置公共字段以下三个字段为必须填写
     * system 系统 adr ios or windows
     * appver 应用版本号
     * userId 用户Id
     */
    public void setCommonAttrs(Map<String,Object> map);
    
    //////////////////////////
    //初始化
    FeedBackManager.getInstance().init("", "",  -1, true);
    FeedBackManager.getInstance().setCommonAttrs(new HashMap<String, Object>());
    
    //调用反馈接口，即可完成文件打包和文件上传：
    FeedBackManager.getInstance().sendFeedback("test", "ssss", JLog.getLogPath(), "");
    
    //若需要监听上传结果,则增加监听回调
    FeedBackManager.getInstance().setFeedbackResultCallback(OnFeedbackResult callback);

    public interface OnFeedbackResult {
        /**
         *
         * @param result 上传结果： 0 成功， 非0 失败
         * @param msg 描述
         */
        void onFeedbackResult(int result, String msg);
    }
```

<h2 id="7">统计上报使用说明</h2>

```java
    private void initReport() {
        long uid = UserInfo.userId;
        ReportCenterConfig config = new ReportCenterConfig();
        config.isTestEv = true;
        config.collectDuration = 10000;
        config.appId = config.isTestEv ? 1001 : 1000;
        config.eventCallback = (result, msg) -> {
            JLog.info("report : result "+ result + ", msg :" + msg);
        };
        boolean result = ReportCenter.instance().initEx(config);
        ReportCenter.instance().setUserInfo(BuildConfig.token, uid); // 必须设置token，token 是统计上报长连接的校验token
        // 以下公共字段必须填写
        Map<String,Object> attrs = new HashMap<>();
        attrs.put("appid",config.appId);
        attrs.put("ua","ljsdkdemo&0.0.1&test");
        attrs.put("userId", String.valueOf(uid));
        attrs.put("platform", "android");
        attrs.put("platfromVer",  String.valueOf(Build.VERSION.SDK_INT));
        attrs.put("monitorVer",  "0.0.1");
        attrs.put("product",  Build.PRODUCT);
        attrs.put("rtcMode",  1);
        attrs.put("liveid", nextID());
        ReportCenter.instance().setCommonAttrs(attrs);
    }
```
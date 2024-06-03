
<h2 id="1"> Unity 1V1 RTC 使用说明</h2>

### 采集推流示例代码在[CameraCaptureTest.cs](UnityRTCDemo/Assets/demo/Video/capture/CameraCaptureTest.cs)中

### 示例场景在[SampleScene.unity](UnityRTCDemo/Assets/Scenes/SampleScene.unity)中

### 拉流示例代码在[PullStreamDemo.cs](UnityRTCDemo/Assets/demo/RTC/PullStreamDemo.cs)中

### 示例场景在[PullRender.unity](UnityRTCDemo/Assets/Scenes/PullRender.unity)中

#### RTC SDK 的接口代码主要封装在[IRtcEngine.cs](UnityRTCDemo/Assets/RTC/IRtcEngine.cs)中

#### 使用是需要有channelId、appId、uid、token等有效参数,假如写死token测试可修改demo中[InitHelper.cs](UnityRTCDemo/Assets/demo/InitHelper.cs)的token

#### 1.初始化RTC SDK

#### （可选）实现RTC的日志打印接口[ILog.cs](UnityRTCDemo/Assets/RTC/Common/Log/ILog.cs),此时C#层的代码会通过接口打印，但是C++层的代码不会打印出来，可以初始化SDK内部配套使用的FLog

 ```csharp
        public static void InitLog(string logDir, string tag, int cacheDays) {
            FXLog fxlog = new FXLog();
            fxlog.Init((int)FLogLevel.LEVEL_DEBUG, (int)LogMode.ASYNC, logDir, tag, cacheDays, true);
            fxlog.SetMaxLogFileSize(10 * 1024 * 1024);
            fxlog.SetConsoleOpen(true);
            FLog.Init(fxlog);
        }

        public class RtcLogImpl : ILog {
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
```

#### （可选）实现RTC的统计上报接口[IReprot.cs](UnityRTCDemo/Assets/RTC/Common/Log/ILog.cs)，此时SDK会通过内部的长连接进行上报SDK相关的数据到服务器

 ```csharp
        public static void InitReport(ReportCenter.upload_event_func cb) {
            LJ.Report.ReportCenterConfig cfg = new LJ.Report.ReportCenterConfig();
            cfg.eventCb = cb;
            cfg.collectDuration = 10000;
            cfg.isTestEvn = true;
            cfg.appId = cfg.isTestEvn ? 1003 : 1002;
            LJ.Report.ReportCenter.InitEx(cfg);
            LJ.Report.ReportCenter.SetUserInfo(InitHelper._token, 123);
            Dictionary<string, System.Object> info = new Dictionary<string, System.Object>();
            info.Add("appid", cfg.appId);
            info.Add("ua", "unityRTCdemo&0.0.1&test");
            info.Add("userId", 123456);
#if UNITY_ANDROID
            info.Add("platform", "android");
#elif UNITY_IOS
           info.Add("platform", "apple");
#else
            info.Add("platform", "windows");
#endif
            info.Add("monitorVer", "0.0.1");
            info.Add("rtcMode", 1);
            info.Add("liveid", nextID());
            LJ.Report.ReportCenter.SetCommonAttrs(info);
            LJ.Report.ReportCenter.EnablePerformance(true);
        }

        class ReportImpl : IReprot
        {
            public void DoReport(string key, string info)
            {
                ReportCenter.Report(key, info);
            }
        }
```

#### 初始化RTCEngine

```csharp
    RtcEngineConfig config = new RtcEngineConfig();
    config.mAppId = 111;
    config.isTestEv = true; // 是否是测试环境，目前这个值暂未使用可以使用LJRtcEngine.SetDebugEnv(true); 方法设置测试环境
    config.mJLog = new RtcLogImpl(); // 可选，不需要则设置为null
    config.mReport = new ReportImpl(); // 可选，不需要则设置为null
    IRtcEngine mRtcEngine = IRtcEngine.CreateRtcEngine(config);
    // 启动音频模块,joinChannel成功后，会自动启动音频采集编码和推流
    mRtcEngine.EnableAudio(true);
    // 启动视频模块，joinChannel成功后，会自动启动视频采集编码和推流
    mRtcEngine.EnableAudio(true);

    LJRtcEngine.SetDebugEnv(true); // 设置为测试环境，上线则设置为正式环境
 ```

#### 2.设置RTC工作模式

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

#### 3.设置编码参数

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

#### 4.设置本地预览控件

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

#### 5.设置远端显示控件

```csharp
    // JoinChannel后调用
    mRtcEngine.SetRemoteRender(RawImage);
 ```

#### 6.加入频道

```csharp
    // JoinChannel后调用
    ChannelConfig channelConfig = new ChannelConfig();
    channelConfig.appID = 1; // 服务器分配的Appid， 测试环境，可以自己定义
    channelConfig.channelID = "95452311111"; // 频道Id
    channelConfig.userID = 3443434; //用户Id
    channelConfig.token = "token"; // 临时令牌，由服务器分配
    mRtcEngine.JoinChannel(channelConfig);
 ```

#### 7.自定义推送视频和音频（指定要推送视频数据时，需要停止sdk内部的相机采集，调用StopPreview() 方法）(指定要推送音频数据时，需要停止sdk内部的音频采集，调用EnableAudio(false)方法)

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

    mRtcEngine.PushAudioFrame(byte[] pcm, int sampleRate, int channelCount, int bytePerSample);
 ```

#### 8.退出频道

```csharp
    // JoinChannel后调用
    mRtcEngine.LeaveChannel();
 ```

#### 9.销毁RTC

```csharp
    // JoinChannel后调用
    mRtcEngine.OnDestroy();
 ```

#### 10.切换原生摄像头（目前SDK内部摄像头采集有两种方式：1.unity 自带的相机组件CameraTexture 2.在C++层自己启动相机设备采集），因此存在切换原生摄像头的方法，在调用该方法前，需要先StopPreview，停止相机采集，设置后，再StartPreview启动相机

```csharp
        userNativeCamera = !userNativeCamera;
        mRtcEngine.StopPreview();
        mRtcEngine.SetUseNativeCamera(userNativeCamera);
        mRtcEngine.StartPreview();
 ```

#### 11.获取相机设备列表

```csharp
    IVideoDeviceManager manager = mRtcEngine.GetVideoDeviceManager();
    DeviceInfo[] infos = manager.EnumerateVideoDevices();
    //or
    //string[] deviceNames = mRtcEngine.GetCameraDeviceNames();

    // 指定使用相机名，设置相机ID后，需要调用RTC的StopPreview方法，先关闭当前设备，然后调用StartPreview，重新打开设备
    mRtcEngine.StartCameraDevice(string cameraDeviceName);

     manager = mRtcEngine.GetVideoDeviceManager();
    FLog.Info("onValueChanged:" + data.text);
    if (manager.SetDevice(data.text) == 0)
    {
        mRtcEngine.StopPreview();
        mRtcEngine.StartPreview();
    }


 ```

#### 12.切换横竖屏（需要先StopPreview停止相机采集，然后重新设置编码参数，再调用StartPreview启动采集，因为切换横竖屏会使相机旋转角度发生变化，需要重新初始化）

```csharp
    public void SetPortrait()
    {

        mCanvasScaler.referenceResolution = new Vector2(1080, 1920);
        mCanvasScaler.matchWidthOrHeight = 0;
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.Portrait;
        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(640, 480);
        encoderConfiguration.frameRate = 30;
        encoderConfiguration.mScreenOrientation = (int)ScreenOrientation.Portrait;
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT;
        mRtcEngine.StopPreview();
        mRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        mRtcEngine.StartPreview();
    }

    public void SetLandscape()
    {
        mCanvasScaler.referenceResolution = new Vector2(1920, 1080);
        mCanvasScaler.matchWidthOrHeight = 1;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(640, 480);
        encoderConfiguration.frameRate = 30;
        encoderConfiguration.mScreenOrientation = (int)ScreenOrientation.LandscapeLeft;
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_LANDSCAPE;
        mRtcEngine.StopPreview();
        mRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        mRtcEngine.StartPreview();
    }
 ```

#### 13.切换编码分辨率（需要先StopPreview停止相机采集，重新设置编码参数，再调用StartPreview启动采集，因为编码分辨率变化，相机分辨率需要随着编码分辨率发生变化）

```csharp
    public void setEncodeWH(int width， int height)
    {
        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(width，, height);
        encoderConfiguration.frameRate = 30;
        encoderConfiguration.mScreenOrientation = (int)ScreenOrientation.Portrait;
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT;
        mRtcEngine.StopPreview();
        mRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        mRtcEngine.StartPreview();
    }
 ```

#### 14.注册相机采集数据回调(startPreview前调用,OnCaptureVideoFrame回调在相机采集，在使用Unity相机组件时，是Unity主线程，因此不要在该回调做耗时处理)

```csharp
    mRtcEngine.RegisterCaptureFrame(OnCaptureVideoFrame capatureVideoFrame);

    void OnCaptureVideoFrame(CaptureVideoFrame videoFrame) {

    }
 ```

#### 15.开启内录功能（目前内录功能主要是android以及PC，IOS的内录需要使用系统的内录插件，需要额外实现代码）

```csharp
    mRtcEngine.EnableSubMix(bool enable);
 ```

#### 16.注册对端的视频解码数据

```csharp
    mRtcEngine.RegisterDecodedFrame(OnDecodedVideoFrame);
    public void OnDecodedVideoFrame(VideoFrame videoFrame) {
        JLog.Debug("OnDecodedVideoFrame videoFrame " + videoFrame.frameType);
    }
 ```

#### 17.注册音频采集数据

```csharp
    /// <summary>
    /// 当开了内录后，回调的是麦克风和内录的混音
    /// </summary>
    /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
    /// <returns>true 拦截当前采集流程，不进行编码推流，需要额外调用PushAudioFrame进行编码推流， false 不拦截当前流程</returns>
    mRtcEngine.RegisterCaptureAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
    /// <returns>true 当前麦克风声音将被拦截，麦克风声音将静音，false 不拦截当前流程</returns>
    mRtcEngine.RegisterMicAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
    /// <returns>true 拦截当前内录流程，内录音频将不会被编码推流到远端， false 不拦截当前流程</returns>
        mRtcEngine.RegisterSubMixAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type);
 ```

#### 18.注册对端的音频解码数据（回调在解码线程，不要做耗时操作）

```csharp
    mRtcEngine.RegisterDecodedAudioFrame(OnDecodedVideoFrame);
 ```

#### 19.开启音量提示

```csharp
    /**
     interval	指定音量提示的时间间隔：
     ≤ 0：禁用音量提示功能。
     > 0：返回音量提示的间隔，单位为毫秒。建议设置到大于 200 毫秒。最小不得少于 10 毫秒，否则会收不到 onAudioVolumeIndication 回调。
     smooth	平滑系数，指定音量提示的灵敏度。取值范围为 [0, 10]，建议值为 3，数字越大，波动越灵敏；数字越小，波动越平滑。
     report_vad	是否开启人声检测
     true: 开启本地人声检测功能。开启后，onAudioVolumeIndication 回调的 vad 参数会报告是否在本地检测到人声。
     false: （默认）关闭本地人声检测功能。除引擎自动进行本地人声检测的场景外，onAudioVolumeIndication 回调的 vad 参数不会报告是否在本地检测到人声。
     */
    mRtcEngine.enableAudioVolumeIndication(int interval, int smooth, bool report_vad);
    // 音量回调在IRtcEngineEventHandler的回调中void onAudioVolumeIndication(AudioVolumeEvent info)通过InitEventHandler设置
    mRtcEngine.InitEventHandler(IRtcEngineEventHandler handler);
```

#### 20.IRtcEngineEventHandler全局回调

```csharp
    public abstract class IRtcEngineEventHandler
    {
        // 网络质量回调，localQuality是本地的网络质量，remoteQuality是对端的网络质量
        /**
        * @brief 网络质量级别枚举。
        */
        enum NetQualityLevel {
            QUALITY_GOOD = 1, /**< 网络质量好 */
            QUALITY_COMMON = 2, /**< 网络质量一般 */
            QUALITY_BAD = 3, /**< 勉强能沟通 */
            QUALITY_VBAD = 4, /**< 网络质量非常差，基本不能沟通。 */
            QUALITY_BLOCK = 5, /**< 链路不通 */
        };
        public virtual void onNetworkQuality(int uid, int localQuality, int remoteQuality)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"> 枚举值如下</param>
        /// #define API_STATUS_CONNECTED 	1
        /// #define API_STATUS_DISCONNECTED 2
        /// #define API_STATUS_LOST 		3
        /// #define API_STATUS_CLOSE		4 
        public virtual void onLinkStatus(int status)
        {

        }
        /// <summary>
        /// 采集音量提示回调
        /// </summary>
        /// <param name="info"></param>
        public virtual void onAudioVolumeIndication(AudioVolumeEvent info)
        {

        }

        public virtual void onJoinChannelSuccess(string channelId, long uid, string msg)
        {

        }

        public virtual void onJoinChannelFail(string channelId, long uid, string msg)
        {

        }

        public virtual void onLeavehannelSuccess(string channelId, long uid, string msg)
        {

        }

        public virtual void onLeaveChannelFail(string channelId, long uid, string msg)
        {
        }

        /// <summary>
        /// 相机操作结果回调，startPreview 时，重新启动相机会有该回调
        /// </summary>
        /// <param name="action">CAMERA_ACTION</param>
        /// <param name="result">CAMERA_CAPTURE_ERROR</param>
        /// <param name="msg">result 是CAMERA_BUSY时是占用进程名</param>
        /*
        public enum CAMERA_CAPTURE_ERROR
        { 
            SUCCESS = 0,
            DEVICE_NOT_FOUND = -1,
            PERMISSION_DENICE = -2,
            CAMERA_PLAY_ERROR = -3,
            NO_CAMERA_DEVICE = -4,
            PARAM_ERROR = -5,
            CAMERA_BUSY = -6,
        }
        public enum CAMERA_ACTION
        {
            ACTION_START = 0,
            ACTION_STOP = 1,
        }*/
        public virtual void OnCameraActionResult(int action, int result, string msg) { 
        
        }
    };
```csharp

#### 21.获取音频的采集和播放设备列表

```csharp
    IAudioDeviceManager manager = mRtcEngine.GetAudioDeviceManager();
    if (manager != null) {
        captureDevices = manager.EnumerateRecordingDevices();
    }
    // 设置采集设备,设置设备ID后，需要先mRtcEngine.EnableAudio(false)关闭当前设备，然后调用mRtcEngine.EnableAudio(true)再次打开设备
    String deviceId = "";
    foreach (DeviceInfo deviceInfo in captureDevices)
    {
        if (deviceInfo.deviceName == data.text)
        {
            deviceId = deviceInfo.deviceId;
            break;
        }
    }
    if (manager.SetRecordingDevice(deviceId) == 0)
    {
        mRtcEngine.EnableAudio(false);
        mRtcEngine.EnableAudio(true);
    }

     IAudioDeviceManager manager = mRtcEngine.GetAudioDeviceManager();
    if (manager != null)
    {
        renderDevices = manager.EnumeratePlaybackDevices();
    }
    // 设置播放设备，设置设备ID后，需要先mRtcEngine.EnableAudio(false)关闭当前设备，然后调用mRtcEngine.EnableAudio(true)再次打开设备
    foreach (DeviceInfo deviceInfo in renderDevices)
    {
        if (deviceInfo.deviceName == data.text)
        {
            deviceId = deviceInfo.deviceId;
            break;
        }
    }
    if (manager.SetPlaybackDevice(deviceId) == 0)
    {
        mRtcEngine.EnableAudio(false);
        mRtcEngine.EnableAudio(true);
    }

`````

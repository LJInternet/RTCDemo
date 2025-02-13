<h2 id="1"> 多人RTC使用说明</h2>


## 因为多人RTC是基于1V1 RTC开发，因此1V1 RTC的接口，在RTCEngineEx中基本都可以使用，请参考[Unity 1V1 RTC 使用说明](p2prtc.md#1)

### 示例代码在[MultiChannelRTCExTest2.cs](UnityRTCDemo/Assets/demo/RTC/MultiChannelRTCExTest2.cs)中

### 示例场景在[MultiChannelTest1.unity](UnityRTCDemo/Assets/Scenes/MultiChannelTest1.unity)中

#### RTC SDK 的接口代码主要封装在[IRtcEngine.cs](UnityRTCDemo/Assets/RTC/IRtcEngine.cs)中的IRtcEngineEx

#### 1.初始化RTCEngineEx

```csharp
RtcEngineConfig config = new RtcEngineConfig();
config.mAppId = 0x70FFFFFF; // 服务器下发的APPID
config.isTestEv = true; // 是否是测试环境
config.mJLog = ILog; // 实现LJ.RTC.Common.ILog
IRtcEngineEx mRtcEngine = IRtcEngine.CreateRtcEngineEx(config); // 创建RTCEngine
mRtcEngine.EnableAudio(true); // 开启音频模块
mRtcEngine.EnableVideo(true); // 开启视频模块,不需要视频，则设置为false
GameObject canvas = GameObject.Find("Canvas");
canvas.AddComponent<LJVideoSurface>(); // 使用非原生相机时，需要添加该控件驱动相机采集
``````

#### 2.创建LJChannel

```csharp
LJChannel channel = mRtcEngine.CreateChannel("channelId", uid); // 创建Channel
ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions(); // 创建ChannelMediaOptions
channelMediaOptions.publishMicrophoneTrack = true; //发送麦克风采集的音频
channelMediaOptions.publishCameraTrack = true; // 发送相机采集视频，不需要发送相机视频则设置为false

channel.ChannelOnUserJoined = Channel1OnUserJoinedHandler; // 用户加入频道回调， SetForMultiChannelUser在该回调时调用
channel.ChannelOnUserOffLine = Channel1OnUserLeavedHandler; // 用户退出频道回调
// todo 增加其他需要的回调
//LJChannel ChannelOnJoinChannelSuccessHandler ChannelOnJoinChannelSuccess;
//LJChannel ChannelOnErrorHandler ChannelOnError;
//LJChannel ChannelOnLeaveChannelHandler ChannelOnLeaveChannelSuccess;
//LJChannel ChannelOnLeaveChannelErrorHandler ChannelOnLeaveChannelError;
//LJChannel ChannelOnUserJoinedHandler ChannelOnUserJoined;
//LJChannel ChannelOnUserOffLineHandler ChannelOnUserOffLine;
//LJChannel ChannelOnNetworkQualityHandler ChannelOnNetworkQuality;
//LJChannel ChannelOnVideoSizeChangedHandler ChannelOnVideoSizeChanged;
//LJChannel ChannelOnConnectionStateChangedHandler ChannelOnConnectionStateChanged;
//LJChannel ChannelOnFirstRemoteVideoFrameHandler ChannelOnFirstRemoteVideoFrame;
void Channel1OnUserJoinedHandler(string channelId, UInt64 uid, int elapsed) {
    Debug.Log($"Channel1OnUserJoinedHandler {channelId} {uid} {elapsed}");
}

void Channel1OnUserLeavedHandler(string channelId, UInt64 uid)
{
    Debug.Log($"Channel1OnUserJoinedHandler {channelId} {uid}");
}

``````

#### 3.加入频道

```csharp
channel.JoinChannel("token",appid, uid, channelMediaOptions);
``````

#### 4.设置对端视频预览的视图

```csharp
    void Channel1OnUserJoinedHandler(string channelId, UInt64 uid, int elapsed) {
        Debug.Log($"Channel1OnUserJoinedHandler {channelId} {uid} {elapsed}");
        lock (_lock)
        {
            if (_remoteViews.ContainsKey(uid)) {
                return;
            }
            RawImage view = _views.Pop();
            if (view == null) {
                return;
            }
            _channel1.SetForMultiChannelUser(view, (long)uid, 30);
            _remoteViews.TryAdd(uid, view);
        }
        
    }

    void Channel1OnUserLeavedHandler(string channelId, UInt64 uid)
    {
        Debug.Log($"Channel1OnUserJoinedHandler {channelId} {uid}");
        lock (_lock)
        {
            RawImage view;
            _remoteViews.TryRemove(uid, out view);
            if (view != null)
            {
                _views.Push(view);
            }
           
        }
    }

    //调用该方法会默认定义该用户的音频和视频
    channel.SetForMultiChannelUser(rawImage, 频道内其他人的uid, 帧率);// joinchannel后，在其他人加入到频道中时，需要显示视频调用该方法增加远端视频的显示
    //若不需要音频则可以调用一下方法
    channel.MuteRemoteAudioStream(频道内其他人的uid, mute);
    //若不需要视频则可以调用一下方法
    channel.MuteRemoteVideoStream(频道内其他人的uid, mute);
``````

#### 5.离开和释放LJChannel

```csharp
channel.LeaveChannel();
channel.ReleaseChannel();
channel = null;
``````

#### 6.获取音频的采集和播放设备列表

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

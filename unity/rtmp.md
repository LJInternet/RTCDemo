# Unity RTMP推流库

## 接口定义在[RTMPEngine.cs](UnityRTCDemo/Assets/RTMP/RTMPEngine.cs)中

## 示例代码[RTMPEngineTest.cs](UnityRTCDemo/Assets/demo/RTMP/RTMPEngineTest.cs),代码中是结合了RTC SDK中的音视频采集功能，用于采集音视频并进行推流，同时结合了android平台推送纹理的方式在进行推流

## 示例场景[RTMPPush.unity](UnityRTCDemo/Assets/Scenes/RTMPPush.unity)

### 1.初始化RTMEngine

```csharp
rtmpEngine = RTMPEngine.getInstance();
```

### 2.建立RTMP连接

```csharp
    /// <summary>
    /// 建立连接
    /// </summary>
    /// <param name="url">rtmp url</param>
    /// <param name="width">rtmp 推流的宽</param>
    /// <param name="height">rtmp 推流的高</param>
    /// <param name="fps">rtmp 推流的帧率</param>
    /// <param name="bitrate">rtmp 推流的码率</param>
    rtmpEngine.open("rtmp://103.215.36.233/live/test11111", 720, 1280, 30, 1800000);
```

### 3.设置rtmp状态回调（回调执行在sendVideo或者sendAudio的线程，需要注意多线程的处理）

```csharp
    rtmpEngine.SetStatusCallback(OnRtmpStatusCallback);
    private void OnRtmpStatusCallback(RTMPStatus status) {
        if (status == RTMPStatus.CONNECT_LOST) {
            rtmpEngine = RTMPEngine.getInstance();
            rtmpEngine.close();
            StartTimer();
        }
    }
    // 启动timer是为了尝试重连RTMP，因为sdk内部没有重连机制
    public void StartTimer()
    {
        if (mTimer != null)
        {
            return;
        }
        mRunning = true;
        mTimer = new Timer(DoTimerCallback, 1, TIMER_TIME, Timeout.Infinite);
    }

    public virtual void StopTimer()
    {
        if (mTimer == null)
        {
            return;
        }
        mRunning = false;
    }

    public void DoTimerCallback(object state)
    {
        if (!mRunning)
        {
            mTimer.Change(-1, Timeout.Infinite);
            mTimer.Dispose();
            mTimer = null;
            return;
        }
        rtmpEngine = RTMPEngine.getInstance();
        int ret = rtmpEngine.open("rtmp://103.215.36.233/live/test11111", 720, 1280, 30, 1800000);
        if (ret < 0) {
            rtmpEngine.close();
            TIMER_TIME = TIMER_TIME * 2;
            mTimer.Change(TIMER_TIME, Timeout.Infinite);
        }
        JLog.Info("DoTimerCallback");
    }
```

### 4.发送视频 CaptureVideoFrame定义在[VideoMedia.cs](UnityRTCDemo/Assets/RTC/Video//VideoMedia.cs)中

```csharp
        /// <summary>
        /// 发送视频数据
        /// </summary>
        /// <param name="buf">除android平台使用纹理编码外，其他平台这个值 不能为null，而且是有效的视频数据</param>
        /// <param name="size">视频数据长度</param>
        /// <param name="msg">CaptureVideoFrame序列化后的数据</param>
        /// <param name="msgSize">CaptureVideoFrame序列化后的数据长度</param>
        /// <param name="pixel_fmt">视频数据的格式,buf的数据格式请在CaptureVideoFrame中的format指定，目前这个参数暂时不生效</param>
        /// <returns></returns>
        rtmpEngine.WriteVideo(byte[] buf, int size, byte[] msg, int msgSize, int pixel_fmt)
```

### 5.发送音频

```csharp
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf">PCM 数据</param>
        /// <param name="frame_num">PCM数据样本数，一般是buf.Length / (bytePerSample * channelCount) </param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="bytePerSample">每个音频数据暂多少个字节，例如：int16 2 int8 1 int32 4</param>
        /// <returns></returns>
        rtmpEngine.WriteAudio(byte[] buf, int frame_num, int sampleRate, int channelCount, int bytePerSample)
```

### 6.销毁RTMPEngine

```csharp
        rtmpEngine.close();
        rtmpEngine.SetStatusCallback(null);
        rtmpEngine = null;
```


<h2 id="1">Linux RTC C++层接入说明</h2>

## Linux RTC C++层接入说明 （接口定义在[media_engine.h](libs/mediatransfer/header/media_engine.h)）
### Linux层接入说明 （接口定义在[media_engine.h](libs/mediatransfer/header/media_engine.h)） 接入方法与PC相似，只是没有音视频的采集模块，需使用自定义的音视频采集，进行推送
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
    config.channelID = "channelId"; // 频道Id
    config.userID = 3443434; //用户Id
    config.token = "token"; // 临时令牌，由服务器分配
    config.transferMode = PUSH_MODE; // 在不同设备中，rtc工作模式必须两端不同，例如云游戏的云端为push（1）则客户端为pull（0）
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

#### 4.配置音频模块（若自定义音频采集则不需要）
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
     * @param bytePerSample Int16 2 int8 1 int32 4（目前只支持int16）
     */
	MEDIATRANSFER_EXTERN void media_engine_push_audio(struct media_engine* engine, const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample);
```

#### 9.自己编码，发送自己编码后数据（需要注册RTC事件回调media_engine_register_event_listener 处理type == RUDP_CB_TYPE_REQUEST_I_FRAME || type == RUDP_CB_TYPE_LINK_OK I帧请求以及RUDP_CB_TYPE_AVAILABLE_BW == type 可用带宽处理）
```cpp
       /**
    * 发送编码后的裸流数据
    * @param engine
    * @param frameType @see VideoFrameType
    * @param codecType @see ENTYPE_H264 or ENTYPE_H265
    * @param iTsInfos 延时统计数据，应该包含采集开始 采集结束 编码开始 编码结束事件 @see DelayConstants
    */
    //media_engine_push_encode_video(struct media_engine* engine, int width, int height, int frameType,
    //    int pts, int  codecType, uint8_t * buf, int32_t len, std::map<uint64_t, uint64_t> iTsInfos);
```

#### 10.RTC 事件回调
```cpp
   static void onEventCallback(int type, const char* buf, int size, void* context) {
        //首次建连成功或者中途断开重连成功，这个时候需要发一个I帧
        if (type == RUDP_CB_TYPE_REQUEST_I_FRAME || type == RUDP_CB_TYPE_LINK_OK) {
    
        }
        else if (RUDP_CB_TYPE_AVAILABLE_BW == type) {
        // 可用带宽
            AvailableBands bands;
            std::string bandStr(buf, size);
            ljtransfer::mediaSox::Unpack up(bandStr.data(), size);
            bands.unmarshal(up);
            int videoBands = bands.m_availableBands[VIDEO_DATA];
        }
        else if (CB_LINK_STATUS == type) {
        // RTC 连接状态
            LinkStatusEvent linkStatus;
            std::string linkStatusStr(buf, size);
            ljtransfer::mediaSox::Unpack up(linkStatusStr.data(), size);
            linkStatus.unmarshal(up);
            // 1 connected 2 disconnectd 3 lost 4 close by peer
        }
    }
    
        /**
     * 注册native事件回调
     * @param engine
     * @param cb
     * @param context
     */
    media_engine_register_event_listener(nginx, onEventCallback, nullptr);
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

## Linux RTM C++层接入说明 (接口定义在 [RUDPApi.h](libs/rudp/header/RudpApi.h))

<h2 id="2">1V1 RTM使用</h2>

## 1V1 RTM使用([示例代码P2PRTMTest.cpp](src/P2PRTMTest.cpp) [参数说明ClientConstants.h](libs/rudp/header/ClientConstants.h))


#### 1V1编译产物可通过channelId=xxx role=xxx token=xxx uid=xxx appId=xxx 指定参数，RTM只依赖basestone.so和rudp.so两个库，1V1设置模式，双方使用的模式（RUDPConfig.role）必须不一样，一端为0 另外一端必须为1


###### 创建RTM实例:

```cpp
    enum SdkMode {
        RTM,
        RTC
    };
    typedef struct RUDPConfig {
        const char* token;//正式环境不能为空，测试环境使用默认的token
        int appId;
        int mode;// RUDPMode 0 RTM
        int role;// RUDPRole 0 normal 1 controller
        bool isDebug;// 测试环境还是正式环境
        int dataWorkMode;// DataWorkMode //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    }RUDPConfig;
    /**
     * 当前1V1 RTM与RTM使用相同的ChannelId，因此需要同时使用RTC，才会生效
     * @param token
     * @param isDebug 是否是测试环境
     * @param dataWorkMode RTM 的工作模式 DataWorkMode //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
     * @param uid 用户ID
     * @param appId 用户Appid
     * @param channelId 频道ID
     */
    RUDPConfig config;
    config.dataWorkMode = dataWorkMode; //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    config.token = token.c_str();
    config.role = role;
    config.isDebug = isDebug;
    config.appId = (int)appid;
    config.mode = 0; //SdkMode
    m_rudp = rudp_engine_create(&config);
    // 通过该方法设置RTM运行的环境，true为测试环境，false为正式环境，正式上线时，应该设置为false
    set_xmtp_debug(true); // 设置为测试环境，必须在joinchannel前调用，否则会崩溃

    rudp_engine_join_channel(m_rudp, (uint64_t)uid, channelId.c_str());
```
###### 设置回调：
```cpp

    // content为传递的this
    static void onRudpMsgCallback(const char* msg, uint32_t len, uint32_t uid, void* content) {
        if (content == nullptr) {
            return;
        }

    }
    /**
    * @see ClientConstants.h
    * @param uid
    * @param channelId
    * @param type MsgType，当type == LinkStatus时，result表示：API_STATUS_CONNECTED，API_STATUS_DISCONNECTED，API_STATUS_LOST
    * @param result 0 成功，否则失败
    * @param msg
    */
    static void onRudpEventCallback(int type, const char* msg, uint32_t len, int result, void* content) {
        if (content == nullptr) {
            return;
        }
 
    }

    rudp_engine_register_msg_callback(m_rudp, onRudpMsgCallback, this);
    rudp_engine_register_event_callback(m_rudp, onRudpEventCallback, this);
```

###### 发送消息：
```cpp
    rudp_engine_send(m_rudp, char* msg, uint32_t len);
```

###### 退出频道：
```cpp
    rudp_engine_leave_channel(m_rudp);
```

###### 销毁：
```cpp
    rudp_engine_destroy(m_rudp);
```

<h2 id="3">多人 RTM使用</h2>

## 多人 RTM使用([MultiRTMTest.cpp](src/MultiRTMTest.cpp)  [ClientConstants.h](libs/rudp/header/ClientConstants.h))
#### 编译产物可通过channelId=xxx token=xxx uid=xxx appId=xxx 指定参数，，无论是那一端RUDPConfig的role写死1，RTM只依赖basestone.so和rudp.so两个库
###### 创建RTM实例：
```cpp
    enum SdkMode {
        RTM,
        RTC
    };
    typedef struct RUDPConfig {
        const char* token;//正式环境不能为空，测试环境使用默认的token
        int appId;
        int mode;// RUDPMode 0 RTM 写死0
        int role;// RUDPRole 1 controller 写死1
        bool isDebug;// 测试环境还是正式环境
        int dataWorkMode;// DataWorkMode 写死0 //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    }RUDPConfig;
    /**
     * @param token
     * @param isDebug 是否是测试环境
     * @param dataWorkMode RTM 的工作模式 DataWorkMode //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
     * @param uid 用户ID
     * @param appId 用户Appid
     * @param channelId 频道ID
     */
    RUDPConfig config;
    config.dataWorkMode = dataWorkMode; //0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    config.token = token.c_str();
    config.role = role;
    config.isDebug = isDebug;
    config.appId = (int)appid;
    config.mode = 0; //SdkMode
    m_rudp = rudp_engine_create_ex(&config);

    // 通过该方法设置RTM运行的环境，true为测试环境，false为正式环境，正式上线时，应该设置为false
    set_xmtp_debug(true); // 设置为测试环境，必须在joinchannel前调用，否则会崩溃

    rudp_engine_join_channel_ex(m_rudp, (uint64_t)uid, channelId.c_str());
```
###### 设置回调：
```cpp

    // content为传递的this
    static void onRudpMsgCallback(int type, const char* msg, uint32_t len, uint64_t uid, void* content) {
        if (content == nullptr) {
            return;
        }

    }
    /**
    * @see ClientConstants.h
    * @param uid
    * @param channelId
    * @param type MsgType，当type == LinkStatus时，result表示：API_STATUS_CONNECTED，API_STATUS_DISCONNECTED，API_STATUS_LOST
    * @param result 0 成功，否则失败
    * @param msg
    */
    static void onRudpEventCallback(int type, const char* msg, uint32_t len, int result, void* content) {
        if (content == nullptr) {
            return;
        }
 
    }

    rudp_engine_register_msg_callback_ex(m_rudp, onRudpMsgCallback, this);
    rudp_engine_register_event_callback_ex(m_rudp, onRudpEventCallback, this);
```

###### 发送消息：
```cpp
    rudp_engine_send_ex(m_rudp, char* msg, uint32_t len);
```

###### 退出频道：
```cpp
    rudp_engine_leave_channel_ex(m_rudp);
```

###### 销毁：
```cpp
    rudp_engine_destroy_ex(m_rudp);
```
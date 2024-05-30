import ctypes
import sys
import RTCEngineBean
import MediaInvokeEvent
import os


class RTCEngineLib:
    _instance = None
    _lib = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            if sys.platform == 'win32':
                cls._lib = ctypes.CDLL(os.path.abspath('./windows/bin/win64/mediatransfer.dll'))
                print("This is Windows " + os.path.abspath('./windows/bin/win64/mediatransfer.dll'))
            elif sys.platform == 'linux':
                # Linux平台下的代码
                cls._lib = ctypes.CDLL('libmediatransfer.so')
                print("This is linux " + os.path.abspath('./bin/libmediatransfer.so')) 
            else:
                print("Unknown platform")

            return cls._instance

    def lib(self):
        return self._lib

# 获取 MyLibSingleton 的实例
rtcEngineLib = RTCEngineLib()

media_engine_create = rtcEngineLib.lib().media_engine_create
media_engine_create.restype = ctypes.c_void_p

media_engine_send_event = rtcEngineLib.lib().media_engine_send_event
media_engine_send_event.argtypes = [ctypes.c_void_p, ctypes.c_int, ctypes.c_char_p, ctypes.c_int]
media_engine_send_event.restype = ctypes.c_int

media_engine_destroy = rtcEngineLib.lib().media_engine_destroy
media_engine_destroy.argtypes = [ctypes.c_void_p]

media_engine_set_debug_env = rtcEngineLib.lib().media_engine_set_debug_env

#/**
#* @brief 推送采集数据
#* @param engine RTC引擎指针
#* @param buf 数据缓冲区指针
#* @param size 数据大小
#* @param msg CaptureVideoFrame的序列号
#* @param msgSize msg数据大小
#* @param pixel_fmt 像素格式
#*/
media_engine_push_video = rtcEngineLib.lib().media_engine_push_video
media_engine_push_video.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_int, ctypes.c_char_p, ctypes.c_int, ctypes.c_int]
media_engine_push_video.restype = None

#/**
#* @brief 推送采集数据
#* @param engine RTC引擎指针
#* @param buf 数据缓冲区指针
#* @param size 数据大小
#* @param width 视频宽度
#* @param height 视频高度
#* @param rotation 旋转角度 0 90 180 270 360
#* @param timeStamp 时间戳
#* @param pixel_fmt 像素格式 PixelFormat
#*/
media_engine_push_raw_video = rtcEngineLib.lib().media_engine_push_raw_video
media_engine_push_raw_video.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_int, ctypes.c_int, ctypes.c_int, ctypes.c_int, ctypes.c_uint64, ctypes.c_int]
media_engine_push_raw_video.restype = None

#/**
#* @brief 订阅视频解码数据
#* @param engine RTC引擎指针
#* @param cb 回调函数指针
#* @param context 上下文指针
#*/
VideoDataCallback = ctypes.CFUNCTYPE(None, ctypes.POINTER(ctypes.c_uint8), ctypes.c_int32, ctypes.c_int32, ctypes.c_int32, ctypes.c_int32, ctypes.c_void_p)
media_engine_subscribe_video = rtcEngineLib.lib().media_engine_subscribe_video
media_engine_subscribe_video.argtypes = [ctypes.c_void_p, VideoDataCallback, ctypes.c_void_p]
media_engine_subscribe_video.restype = None

#/**
# * @brief 发送pcm音频
# * @param engine RTC引擎指针
# * @param pcm 音频数据指针
# * @param frame_num 每个channel的音频采样数
# * @param sampleRate 采样率
# * @param channelCount 声道数
# * @param bytePerSample 每个采样的字节数
# */
media_engine_push_audio = rtcEngineLib.lib().media_engine_push_audio
media_engine_push_audio.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_int8), ctypes.c_int, ctypes.c_int, ctypes.c_int, ctypes.c_int]
media_engine_push_audio.restype = None

# 转换函数指针类型
#bool (*audio_data_cb)(void *audioData, int size, uint64_t pts,int sampleRate, int channelCont, void* context);
audio_data_cb = ctypes.CFUNCTYPE(ctypes.c_bool, ctypes.POINTER(ctypes.c_void_p), ctypes.c_int, ctypes.c_uint64, ctypes.c_int, ctypes.c_int, ctypes.c_void_p)

media_engine_subscribe_audio = rtcEngineLib.lib().media_engine_subscribe_audio
media_engine_subscribe_audio.argtypes = [ctypes.c_void_p, audio_data_cb, ctypes.c_void_p]
media_engine_subscribe_audio.restype = None

# 定义 media_engine_send_remote_bm 函数
media_engine_send_remote_bm = rtcEngineLib.lib().media_engine_send_remote_bm
media_engine_send_remote_bm.argtypes = [ctypes.c_void_p, ctypes.c_int]
media_engine_send_remote_bm.restype = None

# 定义 media_engine_request_remote_I_frame 函数
media_engine_request_remote_I_frame = rtcEngineLib.lib().media_engine_request_remote_I_frame
media_engine_request_remote_I_frame.argtypes = [ctypes.c_void_p]
media_engine_request_remote_I_frame.restype = None

# 定义回调函数类型
EventCallback = ctypes.CFUNCTYPE(None, ctypes.c_int, ctypes.POINTER(ctypes.c_char), ctypes.c_int, ctypes.c_void_p)
# 调用 media_engine_register_event_listener 函数，传入结构体指针和回调函数
media_engine_register_event_listener = rtcEngineLib.lib().media_engine_register_event_listener
media_engine_register_event_listener.argtypes = [ctypes.c_void_p, EventCallback, ctypes.c_void_p]
media_engine_register_event_listener.restype = None

# 将MyObject转换成void*指针
def py_object_to_void_ptr(obj):
    return ctypes.cast(id(obj), ctypes.c_void_p)

# 将void*指针转换回MyObject对象
def void_ptr_to_py_object(void_ptr):
    return ctypes.cast(void_ptr, ctypes.py_object).value

def marshall_to_char_ptr(value):
    byte_array = value.marshall()
    length = len(byte_array)
    return (ctypes.c_char * length).from_buffer(byte_array), length

def rtc_event_callback(type, buf, size, context):
    # print(context)
    # 处理事件回调
    buf_bytes = bytearray(buf[:size])
    obj = void_ptr_to_py_object(context)
    # print(obj)
    obj.handle_rtc_event(type, buf_bytes, size)


    

class IRTCEventHandler:
    # status 1 connected 2 disconnectd 3 lost 4 close by peer
    def onLinkStatus(self, status):
        print("onLInkStatus {0}".format(status))
    #QUALITY_GOOD = 1, /**< 网络质量好 */
    #QUALITY_COMMON = 2, /**< 网络质量一般 */
    #QUALITY_BAD = 3, /**< 勉强能沟通 */
    #QUALITY_VBAD = 4, /**< 网络质量非常差，基本不能沟通。 */
    #QUALITY_BLOCK = 5, /**< 链路不通 */
    def onNetworkQuality(self, uid, localQuality, remoteQuality):
        print("onNetworkQuality uid {0} localQuality {1} remoteQuality {2}".format(uid, localQuality, remoteQuality))
    #链接可用
    def onLinkOk(self):
        print("onLinkOk")
    #业务自己编码的话，这个回调需要编码前马上输出一个关键帧
    def onRequestIFrame(self):
        print("onRequestIFrame")
    #业务自己编码的话，这个回调需要视频编码器按照videoBan输出视频的编码码率，音频按照audioBan输出音频的编码码率
    def onAvailableBandWidth(self, videoBan, audioBan):
        print("onAvailableBandWidth")



class LJRTCEngine:
    def __init__(self):
        self.mMediaEngine = None
        self.uploadConfig = RTCEngineBean.MIEUploadConfig()

    def create(self):
        if self.mMediaEngine:
            return
        config = RTCEngineBean.RTCEngineConfig()
        config.enableLog = False
        configStr, length = marshall_to_char_ptr(config)
        self.mMediaEngine = media_engine_create(configStr, length)

    #设置是否是测试环境
    def set_debug_env(self, enable):
        media_engine_set_debug_env(enable)

    #订阅音频回调
    def subscribe_audio_callback(self, callback, userData):
        if self.mMediaEngine:
            self.audioCallback = audio_data_cb(callback)
            ptr = py_object_to_void_ptr(userData)
            print("subscribe_audio_callback")
            print(ptr)
            media_engine_subscribe_audio(self.mMediaEngine, self.audioCallback, ptr)
        else:
            print("subscribe_audio self.mMediaEngine is null")

    #订阅远端解码视频回调
    def subscribe_video_callback(self, callback, userData):
        if self.mMediaEngine:
            self.remoteVideoCallback = VideoDataCallback(callback)
            media_engine_subscribe_video(self.mMediaEngine, self.remoteVideoCallback, py_object_to_void_ptr(userData))
        else:
            print("subscribe_video self.mMediaEngine is null")

    def handle_rtc_event(self, type, msg, size):
        print(f"Type: {type}, Buffer: {msg}, Size: {size}")
        if self.eventCallback and isinstance(self.eventCallback, IRTCEventHandler):
            if type == RTCEngineBean.UDPCallbackType.CB_LINK_STATUS.value:
                linstatus = RTCEngineBean.LinkStatusEvent()
                linstatus.unmarshall(msg)
                self.eventCallback.onLinkStatus(linstatus.reslut)
            elif type == RTCEngineBean.UDPCallbackType.RUDP_CB_TYPE_AVAILABLE_BW.value:
                bands = RTCEngineBean.AvailableBands()
                bands.unmarshall(msg)
                self.eventCallback.onAvailableBandWidth(bands.out[1],bands.out[2])
            elif type == RTCEngineBean.UDPCallbackType.RUDP_CB_TYPE_LINK_OK.value:
                self.eventCallback.onLinkOk()
            elif type == RTCEngineBean.UDPCallbackType.RUDP_CB_TYPE_REQUEST_I_FRAME.value:
                self.eventCallback.onRequestIFrame()
            elif type == RTCEngineBean.UDPCallbackType.RUDP_CB_TYPE_NET_REPORT.value:
                netquality = RTCEngineBean.NetQuality()
                netquality.unmarshall(msg)
                self.eventCallback.onNetworkQuality(0, netquality.mLocalQuality, netquality.mRemoteQuality)
    #订阅RTC状态回调
    def subscribe_rtc_event_callback(self, callback):
        self.eventCallback = callback
        self.c_eventCallback = EventCallback(rtc_event_callback)
        ptr = py_object_to_void_ptr(self)
        media_engine_register_event_listener(self.mMediaEngine, self.c_eventCallback, ptr)

    def handle_audio_callback(self,audioData, size, pts, sampleRate, channelCount):
        print(f"Received audio data: size={size}, pts={pts}, sampleRate={sampleRate}, channelCount={channelCount}")

    #发送的可用带宽到远端
    def send_remote_bm(self, value):
        if self.mMediaEngine:
            media_engine_send_remote_bm(value)
        else:
            print("subscribe_video self.mMediaEngine is null")

        #请求远端发送一个关键帧
    def request_remote_I_frame(self):
        if self.mMediaEngine:
            media_engine_request_remote_I_frame()
        else:
            print("request_remote_I_frame self.mMediaEngine is null")

    #/**
    # * @brief 发送pcm音频
    # * @param engine RTC引擎指针
    # * @param pcm 音频数据指针
    # * @param pcm 数据长度
    # * @param sampleRate 采样率
    # * @param channelCount 声道数
    # * @param bytePerSample 每个采样的字节数 int16 传2 int8 传1 int32 传4 等于是bit数除以8
    # */
    def push_audio_pcm_frame(self, audioByteArray, sample_rate, channel_count, bytePerSample):
        if self.mMediaEngine:
            length = len(audioByteArray)
            frame_num = int(length / (channel_count * bytePerSample))
            if isinstance(audioByteArray, bytes):
                int_ptr = ctypes.cast(audioByteArray, ctypes.POINTER(ctypes.c_int8))
                media_engine_push_audio(self.mMediaEngine, int_ptr, frame_num, sample_rate, channel_count, bytePerSample)
            elif isinstance(audioByteArray, bytearray):
                audioPtr = (ctypes.c_int8 * length).from_buffer(audioByteArray)
                media_engine_push_audio(self.mMediaEngine, audioPtr, frame_num, sample_rate, channel_count, bytePerSample)
        else:
            print("push_audio_pcm_frame self.mMediaEngine is null")

    #/**
    #* @brief 推送采集数据
    #* @param engine RTC引擎指针
    #* @param buf 数据缓冲区指针
    #* @param size 数据大小
    #* @param width 视频宽度
    #* @param height 视频高度
    #* @param rotation 旋转角度 0 90 180 270 360
    #* @param timeStamp 时间戳
    #* @param pixel_fmt 像素格式 PixelFormat PIXEL_FMT_RGBA 1 PIXEL_FMT_YUV_I420 2 PIXEL_FMT_NV21 3 PIXEL_FMT_RGB24 4 PIXEL_FMT_NV12 5
    #*/
    def push_raw_video_frame(self, video_frame, width, height, rotation, timeStamp, pixel_fmt):
        if self.mMediaEngine:
            length = len(video_frame)
            if isinstance(video_frame, bytes):
                media_engine_push_raw_video(self.mMediaEngine, video_frame, length, width, height, rotation, timeStamp,  pixel_fmt)
            elif isinstance(video_frame, bytearray):
                videoPtr = (ctypes.c_char * length).from_buffer(video_frame)
                media_engine_push_raw_video(self.mMediaEngine, videoPtr, length, width, height, rotation, timeStamp,  pixel_fmt)
            
        else:
            print("push_raw_video_frame self.mMediaEngine is null")

    #设音频回调模式 
    #self.callbackDecodeData: bool = False #是否需要回调音频数据
    #self.renderAudioData: bool = False #数据是否需要播放，false 则直接静音
    #self.directDecode: bool = False #收到远端音频直接解码，不需要经过JitterBuffer
    #self.directCallback: bool = False #rudp收到数据包啥也不做，直接返回未解码数据，回调IAudioProcessor
    def set_audio_play_event(self, event):
        if self.mMediaEngine:
            eventStr, length = marshall_to_char_ptr(event)
            media_engine_send_event(self.mMediaEngine, MediaInvokeEvent.MediaInvokeEventType.AUDIO_PLAYER_EVENT, eventStr, length)
        else:
            print("set_audio_play_event self.mMediaEngine is null")

    #channels 频道号 uid 用户Id token 加入频道的token mode RTC的模式，0 是server 1是client
    def join_channel(self, channels, uid, token, mode):
        if self.mMediaEngine:
            # self.uploadConfig.transferConfig.appID = 1
            # self.uploadConfig.transferConfig.channelID = channels
            # self.uploadConfig.transferConfig.userID = uid
            # self.uploadConfig.transferConfig.token = token
            # self.uploadConfig.transferConfig.transferMode = mode
            # configStr, length = marshall_to_char_ptr(self.uploadConfig)
            # media_engine_send_event(self.mMediaEngine, MediaInvokeEvent.MediaInvokeEventType.JOIN_CHANNEL, configStr, length)
            c = RTCEngineBean.MIEUploadConfig()
            config = RTCEngineBean.MIETransferConfig()
            config.appID = 1
            config.channelID = channels
            config.userID = uid
            config.token = token
            config.transferMode = mode
            c.transferConfig = config
            configStr, length = marshall_to_char_ptr(c)
            media_engine_send_event(self.mMediaEngine, MediaInvokeEvent.MediaInvokeEventType.JOIN_CHANNEL, configStr, length)
        else:
            print("join_channel self.mMediaEngine is null")

    def update_video_config(self, width, height, biteRate, minBiteRate, fps):
        self.uploadConfig.videoUploadConfig.encodeHeight = height
        self.uploadConfig.videoUploadConfig.encodeWidth = width
        self.uploadConfig.videoUploadConfig.maxVideoBitrateInbps = biteRate
        self.uploadConfig.videoUploadConfig.minVideoBitrateInbps = minBiteRate
        self.uploadConfig.videoUploadConfig.realVideoBitrateInbps = biteRate
        self.uploadConfig.videoUploadConfig.fps = fps
        self.uploadConfig.videoUploadConfig.keyFrameInterval = 3
        configStr, length = marshall_to_char_ptr(self.uploadConfig)
        media_engine_send_event(self.mMediaEngine, MediaInvokeEvent.MediaInvokeEventType.MIET_UPDATE_UPLOAD_CONFIG, configStr, length)

    def update_audio_Config(self, sample_rate, channels, biteRate):
        self.uploadConfig.audioUploadConfig.audioBitrateInbps = biteRate
        self.uploadConfig.audioUploadConfig.sampleRate = sample_rate
        self.uploadConfig.audioUploadConfig.channels = channels
        configStr, length = marshall_to_char_ptr(self.uploadConfig)
        media_engine_send_event(self.mMediaEngine, MediaInvokeEvent.MediaInvokeEventType.MIET_UPDATE_UPLOAD_CONFIG, configStr, length) 

    #离开频道
    def leave_channel(self):
        if self.mMediaEngine:
            c = RTCEngineBean.TransferMsg()
            configStr, length = marshall_to_char_ptr(c)
            media_engine_send_event(self.mMediaEngine, MediaInvokeEvent.MediaInvokeEventType.LEAVE_CHANNEL, configStr, length)
        else:
            print("leave_channel self.mMediaEngine is null")

    #销毁RTC对象
    def destroy(self):
        if self.mMediaEngine:
            self.leave_channel()
            media_engine_destroy(self.mMediaEngine)
            self.mMediaEngine = None
        else:
            print("destroy self.mMediaEngine is null")
import ctypes
import sys
import os
from enum import Enum
import struct
class RTMEngineLib:
    _instance = None
    _lib = None

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            if sys.platform == 'win32':
                cls._lib = ctypes.CDLL(os.path.abspath('./windows/bin/win64/rudp.dll'))
                print("This is Windows " + os.path.abspath('./windows/bin/win64/rudp.dll'))
            elif sys.platform == 'linux':
                # Linux平台下的代码
                cls._lib = ctypes.CDLL('librudp.so')
                print("This is linux " + os.path.abspath('./lbin/librudp.so')) 
            else:
                print("Unknown platform")

            return cls._instance

    def lib(self):
        return self._lib

# 获取 MyLibSingleton 的实例
librudp = RTMEngineLib()

class RUDPConfig(ctypes.Structure):
    _fields_ = [
        ("token", ctypes.c_char_p),
        ("appId", ctypes.c_uint64),
        ("mode", ctypes.c_int),
        ("role", ctypes.c_int),
        ("isDebug", ctypes.c_bool),
        ("dataWorkMode", ctypes.c_int),
        ("localIp", ctypes.c_uint32)
    ]

class RTMCallbackType(Enum):
    MsgData = 0
    JoinChannel = 1
    LeaveChannel = 2
    LinkStatus = 3
    RemoteUserJoin = 4
    RemoteUserLeave = 5


# 定义函数原型
rudp_engine_create = librudp.lib().rudp_engine_create
rudp_engine_create.argtypes = [ctypes.POINTER(RUDPConfig)]
rudp_engine_create.restype = restype = ctypes.c_void_p

rudp_engine_destroy = librudp.lib().rudp_engine_destroy
rudp_engine_destroy.argtypes = [ctypes.c_void_p]
rudp_engine_destroy.restype = ctypes.c_int

rudp_engine_join_channel = librudp.lib().rudp_engine_join_channel
rudp_engine_join_channel.argtypes = [ctypes.c_void_p, ctypes.c_uint64, ctypes.c_char_p]
rudp_engine_join_channel.restype = ctypes.c_int

rudp_engine_leave_channel = librudp.lib().rudp_engine_leave_channel
rudp_engine_leave_channel.argtypes = [ctypes.c_void_p]
rudp_engine_leave_channel.restype = ctypes.c_int

rudp_engine_send = librudp.lib().rudp_engine_send
rudp_engine_send.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_int]
rudp_engine_send.restype = ctypes.c_int

RudpMsgCallback = ctypes.CFUNCTYPE(None, ctypes.POINTER(ctypes.c_char), ctypes.c_uint32, ctypes.c_uint64, ctypes.c_void_p)
rudp_engine_register_msg_callback = librudp.lib().rudp_engine_register_msg_callback
rudp_engine_register_msg_callback.argtypes = [ctypes.c_void_p, RudpMsgCallback, ctypes.c_void_p]
rudp_engine_register_msg_callback.restype = ctypes.c_int

RudpEvntCallback = ctypes.CFUNCTYPE(None, ctypes.c_int, ctypes.POINTER(ctypes.c_char), ctypes.c_uint32, ctypes.c_int, ctypes.c_void_p)
rudp_engine_register_event_callback = librudp.lib().rudp_engine_register_event_callback
rudp_engine_register_event_callback.argtypes = [ctypes.c_void_p, RudpEvntCallback, ctypes.c_void_p]
rudp_engine_register_event_callback.restype = ctypes.c_int

set_xmtp_debug = librudp.lib().set_xmtp_debug
set_xmtp_debug.argtypes = [ctypes.c_bool]
set_xmtp_debug.restype = None

class IRTMEventHandler:
    # status 1 connected 2 disconnectd 3 lost 4 close by peer
    def onLinkStatus(self, status):
        print("onLInkStatus {0}".format(status))
    def onJoinChannelSuccess(self):
        print("onJoinChannelSuccess")
    def onJoinChannelFail(self):
        print("onJoinChannelFail")
    def onLeaveChannelSuccess(self):
        print("onLeaveChannelSuccess")
    def onLeaveChannelFail(self):
        print("onLeaveChannelFail")
    def onUserJoin(self, userId):
        print("onUserJoin")
    def onUserLeave(self, userId):
        print("onUserLeave")

def rtm_event_callback(type, buf, size, result, context):
    #print(f"context {context} size {size} result {result}")
    # 处理事件回调
    buf_bytes = bytearray(buf[:size])
    obj = ctypes.cast(context, ctypes.py_object).value
    #print(obj)
    obj.handle_rtm_event_callback(type, buf_bytes, result)

class LJRTMEngine:
    def __init__(self):
        self.mRudpEngine = None
    #创建RTM实例 config RUDPConfig
        #token 服务端分配的令牌
        #appId 服务端分配的appId
        #mode 请写死0
        #role 0 是server端 1是client端
        #isDebug 测试环境为true
        #dataWorkMode数据发送模式，0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    def create(self, config):
        self.mRudpEngine = rudp_engine_create(ctypes.byref(config))
    #销毁RTM实例
    def destroy(self):
        if self.mRudpEngine:
            rudp_engine_destroy(self.mRudpEngine)
            self.mRudpEngine = None
    #加入RTM实例 uid 用户ID， channelId 频道号
    def joinChannel(self, uid, channnelId):
        if self.mRudpEngine:
            char_p_value = ctypes.c_char_p(channnelId.encode('utf-8'))
            uint64uid = ctypes.c_uint64(uid)
            rudp_engine_join_channel(self.mRudpEngine, uint64uid, char_p_value)

    def leaveChannel(self):
        if self.mRudpEngine:
            rudp_engine_leave_channel(self.mRudpEngine)

    def sendMsg(self, msg):
        if self.mRudpEngine:
            if isinstance(msg, bytes):
                rudp_engine_send(self.mRudpEngine, msg, len(msg))
            elif isinstance(msg, bytearray):
                msgPtr = (ctypes.c_char * len(msg)).from_buffer(msg)
                rudp_engine_send(self.mRudpEngine, msgPtr, len(msg))

    def subscribe_msg_callback(self, callback, userData):
        if self.mRudpEngine:
            self.msgCallback = RudpMsgCallback(callback)
            ptr = ctypes.cast(id(userData), ctypes.c_void_p)
            rudp_engine_register_msg_callback(self.mRudpEngine, self.msgCallback, ptr)

    def subscribe_event_callback(self, callback, userData):
        if self.mRudpEngine:
            self.eventCallback = callback
            self.c_eventCallback = RudpEvntCallback(rtm_event_callback)
            ptr = ctypes.cast(id(userData), ctypes.c_void_p)
            rudp_engine_register_event_callback(self.mRudpEngine, self.c_eventCallback, ptr)

    def handle_rtm_event_callback(self, type, msg, result):
        #print(f"type {type} buf_bytes {msg}")
        if self.eventCallback and isinstance(self.eventCallback, IRTMEventHandler):
            if type == RTMCallbackType.LinkStatus.value:
                self.eventCallback.onLinkStatus(result)


    def set_debug(self, debug):
        set_xmtp_debug(debug)


########################### 多人RTM###################################
# 定义函数原型
rudp_engine_create_ex = librudp.lib().rudp_engine_create_ex
rudp_engine_create_ex.argtypes = [ctypes.POINTER(RUDPConfig)]
rudp_engine_create_ex.restype = restype = ctypes.c_void_p

rudp_engine_destroy_ex = librudp.lib().rudp_engine_destroy_ex
rudp_engine_destroy_ex.argtypes = [ctypes.c_void_p]
rudp_engine_destroy_ex.restype = ctypes.c_int

rudp_engine_join_channel_ex = librudp.lib().rudp_engine_join_channel_ex
rudp_engine_join_channel_ex.argtypes = [ctypes.c_void_p, ctypes.c_uint64, ctypes.c_char_p]
rudp_engine_join_channel_ex.restype = ctypes.c_int

rudp_engine_join_channel_with_token_ex = librudp.lib().rudp_engine_join_channel_with_token_ex
rudp_engine_join_channel_with_token_ex.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_uint64, ctypes.c_char_p]
rudp_engine_join_channel_with_token_ex.restype = ctypes.c_int


rudp_engine_leave_channel_ex = librudp.lib().rudp_engine_leave_channel_ex
rudp_engine_leave_channel_ex.argtypes = [ctypes.c_void_p]
rudp_engine_leave_channel_ex.restype = ctypes.c_int

rudp_engine_send_ex = librudp.lib().rudp_engine_send_ex
rudp_engine_send_ex.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_int]
rudp_engine_send_ex.restype = ctypes.c_int

RudpMsgExCallback = ctypes.CFUNCTYPE(None, ctypes.c_int, ctypes.POINTER(ctypes.c_char), ctypes.c_uint32, ctypes.c_uint64, ctypes.c_void_p)
rudp_engine_register_msg_callback_ex = librudp.lib().rudp_engine_register_msg_callback_ex
rudp_engine_register_msg_callback_ex.argtypes = [ctypes.c_void_p, RudpMsgExCallback, ctypes.c_void_p]
rudp_engine_register_msg_callback_ex.restype = ctypes.c_int

RudpEvntExCallback = ctypes.CFUNCTYPE(None, ctypes.c_int, ctypes.POINTER(ctypes.c_char), ctypes.c_uint32, ctypes.c_int, ctypes.c_void_p)
rudp_engine_register_event_callback_ex = librudp.lib().rudp_engine_register_event_callback_ex
rudp_engine_register_event_callback_ex.argtypes = [ctypes.c_void_p, RudpEvntExCallback, ctypes.c_void_p]
rudp_engine_register_event_callback_ex.restype = ctypes.c_int
def rtm_ex_event_callback(type, buf, size, result, context):
    #print(f"context {context} size {size} result {result}")
    # 处理事件回调
    buf_bytes = bytearray(buf[:size])
    obj = ctypes.cast(context, ctypes.py_object).value
    #print(obj)
    obj.handle_rtm_ex_event_callback(type, buf_bytes, result)
class LJRTMEngineEx:
    def __init__(self):
        self.mRudpEngine = None
    #创建RTM实例 config RUDPConfig
        #token 服务端分配的令牌
        #appId 服务端分配的appId
        #mode 请写死0
        #role 0 是server端 1是client端, 请写死1
        #isDebug 测试环境为true
        #dataWorkMode数据发送模式，0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    def create(self, config):
        config.role = ctypes.c_int(1)
        config.mode = ctypes.c_int(0)
        self.mRudpEngine = rudp_engine_create_ex(ctypes.byref(config))
    #销毁RTM实例
    def destroy(self):
        if self.mRudpEngine:
            rudp_engine_destroy_ex(self.mRudpEngine)
            self.mRudpEngine = None
    #加入RTM实例 uid 用户ID， channelId 频道号
    def joinChannel(self, uid, channnelId):
        if self.mRudpEngine:
            char_p_value = ctypes.c_char_p(channnelId.encode('utf-8'))
            uint64uid = ctypes.c_uint64(uid)
            rudp_engine_join_channel_ex(self.mRudpEngine, uint64uid, char_p_value)

    def leaveChannel(self):
        if self.mRudpEngine:
            rudp_engine_leave_channel_ex(self.mRudpEngine)

    def sendMsg(self, msg):
        if self.mRudpEngine:
            if isinstance(msg, bytes):
                rudp_engine_send_ex(self.mRudpEngine, msg, len(msg))
            elif isinstance(msg, bytearray):
                msgPtr = (ctypes.c_char * len(msg)).from_buffer(msg)
                rudp_engine_send_ex(self.mRudpEngine, msgPtr, len(msg))

    def subscribe_msg_callback(self, callback, userData):
        if self.mRudpEngine:
            self.msgCallback = RudpMsgExCallback(callback)
            ptr = ctypes.cast(id(userData), ctypes.c_void_p)
            rudp_engine_register_msg_callback_ex(self.mRudpEngine, self.msgCallback, ptr)

    def subscribe_event_callback(self, callback, userData):
        if self.mRudpEngine:
            self.eventCallback = callback
            self.c_eventCallback = RudpEvntExCallback(rtm_ex_event_callback)
            ptr = ctypes.cast(id(userData), ctypes.c_void_p)
            rudp_engine_register_event_callback_ex(self.mRudpEngine, self.c_eventCallback, ptr)

    def handle_rtm_ex_event_callback(self, type, msg, result):
        #print(f"type {type} buf_bytes {msg}")
        if self.eventCallback and isinstance(self.eventCallback, IRTMEventHandler):
            if self.eventCallback and isinstance(self.eventCallback, IRTMEventHandler):
                if type == RTMCallbackType.LinkStatus.value:
                    self.eventCallback.onLinkStatus(result)
                elif type == RTMCallbackType.JoinChannel.value:
                    if result == 0:
                        self.eventCallback.onJoinChannelSuccess()
                    else:
                        self.eventCallback.onJoinChannelFail()
                elif type == RTMCallbackType.LeaveChannel.value:
                    if result == 0:
                        self.eventCallback.onLeaveChannelSuccess()
                    else:
                        self.eventCallback.onLeaveChannelFail()
                elif type == RTMCallbackType.RemoteUserJoin.value:
                    #print(f"RemoteUserJoin {}")
                    #uid = struct.unpack('Q', msg)[0]
                    self.eventCallback.onUserJoin(int(msg.decode('utf-8')))
                elif type == RTMCallbackType.RemoteUserLeave.value:
                    self.eventCallback.onUserLeave(int(msg.decode('utf-8')))
                    
    def set_debug(self, debug):
        set_xmtp_debug(debug)
########################### 多人RTM###################################
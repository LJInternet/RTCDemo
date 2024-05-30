import RTMEngine
import ctypes
import time
import RTCEngineBean
import RTCEngine
import os
import sys
class RTCEventHandler(RTCEngine.IRTCEventHandler):
    def onLinkStatus(self, status):
        print("onLInkStatus {0}".format(status))

    def onNetworkQuality(self, uid, localQuality, remoteQuality):
        print("onNetworkQuality uid {0} localQuality {1} remoteQuality {2}".format(uid, localQuality, remoteQuality))

    def onLinkOk(self):
        print("onLinkOk")

    def onRequestIFrame(self):
        print("onRequestIFrame")

    def onAvailableBandWidth(self, videoBan, audioBan):
        print(f"onAvailableBandWidth videoBan {videoBan} audioBan {audioBan}")

def rudp_msg_callback(msg, len, uid, content):
    print("Received message: Length:", len, "UID:", uid)
    buf_bytes = bytearray(msg[:len])
    obj = ctypes.cast(content, ctypes.py_object).value
    print(buf_bytes.decode("utf-8"))

class RTMEventHandler(RTMEngine.IRTMEventHandler):
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

def setup_rtc():
    #channels 频道号 uid 用户Id token 加入频道的token mode RTC的模式，0 是server 1是client
    rtcEngine = RTCEngine.LJRTCEngine()
    # 释放把日志保存到文件中
    rtcEngine.create()
    #设置是否是测试环境
    rtcEngine.set_debug_env(True)
    #订阅RTC回调事件
    rtcEventHandler = RTCEventHandler()
    rtcEngine.subscribe_rtc_event_callback(rtcEventHandler)
    #channels 频道号 uid 用户Id token 加入频道的token mode RTC的模式，0 是server 1是client
    rtcEngine.join_channel("954523133", 31212121, "token", 0)
    return rtcEngine

def setup_rtm():
    #创建实例
    rtmEngine = RTMEngine.LJRTMEngine()
    #设置发测试环境，默认正式环境
    rtmEngine.set_debug(True)
    #RTM初始化参数
    config = RTMEngine.RUDPConfig()
    #token
    config.token = ctypes.c_char_p(b"token")  # 使用b前缀表示字节字符串,请替换为自己的 token
    #服务端分配的appId
    config.appId = ctypes.c_uint64(1)
    #RTM还是RTC，RTM默认都是0
    config.mode = ctypes.c_int(0)
    #是server端还是client端 1是client端 0是server端
    config.role = ctypes.c_int(0)
    #暂时没有用，测试环境可用先设置为true，正式则设置为false
    config.isDebug = ctypes.c_bool(True)
    #0 即收又发 1是只发送 2 只接收消息 3 既收又发同时会收到自己的同步消息
    config.dataWorkMode = ctypes.c_int(0)
    config.localIp = ctypes.c_uint32(0)
    rtmEngine.create(config)
    #要注册RTM的事件回调可用通过继承RTMEngine.IRTMEventHandler方法，重写父类的默认回调
    handler = RTMEventHandler()
    rtmEngine.subscribe_event_callback(handler, rtmEngine)
    #注册RTM的消息回调
    rtmEngine.subscribe_msg_callback(rudp_msg_callback, rtmEngine)
    #加入频道12362 是用户ID， 121212是频道号
    rtmEngine.joinChannel(31212121, "954523133")
    return rtmEngine

def main():
    rtmEngine = setup_rtm()
    rtcEngine = setup_rtc()
   
    time.sleep(2000)
    #离开RTM频道
    rtmEngine.leaveChannel()
    #销毁RTM实例
    rtmEngine.destroy()

    rtcEngine.leave_channel()
    rtcEngine.destroy()
    rtcEngine = None
    

if __name__ == "__main__":
    main()
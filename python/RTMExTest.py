import RTMEngine
import ctypes
import time

def rudp_ex_msg_callback(type, msg, len, uid, content):
    print("Received message: Length:", len, "UID:", uid, " type:", type)
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
    def onUserLeave(self, userId):
        print(f"onUserLeave {userId}")
    def onUserJoin(self, userId):
        print(f"onUserJoin {userId}")

def main():
    #创建实例
    rtmEngine = RTMEngine.LJRTMEngineEx()
    #设置发测试环境，默认正式环境
    rtmEngine.set_debug(True)
    #RTM初始化参数
    config = RTMEngine.RUDPConfig()
    #token
    config.token = ctypes.c_char_p(b"token")  # 使用b前缀表示字节字符串,请替换为自己的 token
    #服务端分配的appId
    config.appId = ctypes.c_uint64(1)
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
    rtmEngine.subscribe_msg_callback(rudp_ex_msg_callback, rtmEngine)
    #加入频道12362 是用户ID， 121212是频道号
    rtmEngine.joinChannel(123621, "1212121")
    bytes_data = b'hello bytes data'
    bytes_data1 = b'hello bytes array data'
    bytes_array = bytearray(bytes_data1)
    index = 0
    while index < 10000:
        index = index + 1
        if index % 2 == 0:
            #发送RTM消息，直接发送bytes类型
            rtmEngine.sendMsg(bytes_data)
        else:
            #发送RTM消息，直接发送bytearray类型
            rtmEngine.sendMsg(bytes_array)
        time.sleep(1)
        

    time.sleep(20)
    #离开RTM频道
    rtmEngine.leaveChannel()
    #销毁RTM实例
    rtmEngine.destroy()
    

if __name__ == "__main__":
    main()
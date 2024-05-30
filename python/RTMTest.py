import RTMEngine
import ctypes
import time

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

def main():
    rtmEngine = RTMEngine.LJRTMEngine()
    rtmEngine.set_debug(True)
    config = RTMEngine.RUDPConfig()
    config.token = ctypes.c_char_p(b"linjing@2023")  # 使用b前缀表示字节字符串
    config.appId = ctypes.c_uint64(1)
    config.mode = ctypes.c_int(0)
    config.role = ctypes.c_int(0)
    config.isDebug = ctypes.c_bool(True)
    config.dataWorkMode = ctypes.c_int(0)
    config.localIp = ctypes.c_uint32(0)
    rtmEngine.create(config)
    handler = RTMEventHandler()
    rtmEngine.subscribe_event_callback(handler, rtmEngine)
    rtmEngine.subscribe_msg_callback(rudp_msg_callback, rtmEngine)

    rtmEngine.joinChannel(12362, "121212")
    bytes_data = b'hello bytes data'
    bytes_data1 = b'hello bytes array data'
    bytes_array = bytearray(bytes_data1)
    index = 0
    while index < 10000:
        index = index + 1
        if index % 2 == 0:
            rtmEngine.sendMsg(bytes_data)
        else:
            rtmEngine.sendMsg(bytes_array)
        time.sleep(1)
        

    time.sleep(20)

    rtmEngine.leaveChannel()
    rtmEngine.destroy()
    

if __name__ == "__main__":
    main()
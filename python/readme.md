# windows 打包注意事项

## 目前的python脚本运行在3.10版本

### 打包顺序

#### 1.安装pyinstaller pip install pyinstaller

#### 2.生成项目打包的spec  pyi-makespec RTMTest.py

```csharp
# -*- mode: python ; coding: utf-8 -*-

block_cipher = None

PROJECT_DIR='E:\\RTCDemo\\' // 指定项目路径

a = Analysis(['RTMTest.py', 'RTMEngine.py'], // 指定需要打包的python脚本
             pathex=[PROJECT_DIR + 'python', PROJECT_DIR+'windows\\bin\\win64\\rudp.dll'], // 设置依赖库的搜索路径
             binaries=[],
             datas=[],
             hiddenimports=[],
             hookspath=[],
             runtime_hooks=[],
             excludes=[],
             win_no_prefer_redirects=False,
             win_private_assemblies=False,
             cipher=block_cipher,
             noarchive=False)
pyz = PYZ(a.pure, a.zipped_data,
             cipher=block_cipher)
exe = EXE(pyz,
          a.scripts,
          [],
          exclude_binaries=True,
          name='RTMTest',
          debug=False,
          bootloader_ignore_signals=False,
          strip=False,
          upx=True,
          console=True,)
coll = COLLECT(exe,
               a.binaries,
               a.zipfiles,
               a.datas,
               strip=False,
               upx=True,
               upx_exclude=[],
               name='RTMTest')

// linux RTMTest.spec
# -*- mode: python ; coding: utf-8 -*-


a = Analysis(
    ['RTMTest.py', 'RTMEngine.py'],
    pathex=[],
    binaries=[],
    datas=[],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    [],
    exclude_binaries=True,
    name='RTMTest',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    console=True,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
coll = COLLECT(
    exe,
    a.binaries,
    a.datas,
    strip=False,
    upx=True,
    upx_exclude=[],
    name='RTMTest',
)
```

#### 3. linux 运行需要设置依赖so到系统路径下 export LD_LIBRARY_PATH=xxxx/python/bin

#### 4.python使用1V1 RTM

```python
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
```

#### 4.python使用多人 RTM

```python
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
```

#### 5.1V1 RTC使用

```python
import RTCEngineBean
import RTCEngine
import time
import os
import traceback
# 定义与C回调函数原型相匹配的函数
def audio_data_callback(audioData, size, pts, sampleRate, channelCount, context):
    # 将void*指针转换回MyObject对象
        obj = RTCEngine.void_ptr_to_py_object(context)
        obj.handle_audio_callback(audioData, size, pts, sampleRate, channelCount)
   

def video_data_callback(data, len, width, height, pixel_fmt, context):
    obj = RTCEngine.void_ptr_to_py_object(context)
    print(f"len {len} width {width} height {height} pixel_fmt {pixel_fmt} obj {obj}")

class RTCEventHandler(RTCEngine.IRTCEventHandler):
    def __init__(self) -> None:
        super().__init__()
        self.isLinkOk = False

    def onLinkStatus(self, status):
        self.isLinkOk = True
        print("onLInkStatus {0}".format(status))

    def onNetworkQuality(self, uid, localQuality, remoteQuality):
        print("onNetworkQuality uid {0} localQuality {1} remoteQuality {2}".format(uid, localQuality, remoteQuality))

    def onLinkOk(self):
        print("onLinkOk")

    def onRequestIFrame(self):
        print("onRequestIFrame")

    def onAvailableBandWidth(self, videoBan, audioBan):
        print(f"onAvailableBandWidth videoBan {videoBan} audioBan {audioBan}")


def main():
    #channels 频道号 uid 用户Id token 加入频道的token mode RTC的模式，0 是server 1是client
    rtcEngine = RTCEngine.LJRTCEngine()
    # 释放把日志保存到文件中
    rtcEngine.create()
    #设置是否是测试环境
    rtcEngine.set_debug_env(True)
    #订阅RTC回调事件
    rtcEventHandler = RTCEventHandler()
    rtcEngine.subscribe_rtc_event_callback(rtcEventHandler)
    #订阅音频回调
    rtcEngine.subscribe_audio_callback(audio_data_callback, rtcEngine)
    #订阅远端视频解码回调
    rtcEngine.subscribe_video_callback(video_data_callback, rtcEngine)
    #设音频回调模式 
    #self.callbackDecodeData: bool = False #是否需要回调音频数据
    #self.renderAudioData: bool = False #数据是否需要播放，false 则直接静音
    #self.directDecode: bool = False #收到远端音频直接解码，不需要经过JitterBuffer
    #self.directCallback: bool = False #rudp收到数据包啥也不做，直接返回未解码数据，回调IAudioProcessor
    event = RTCEngineBean.AudioPlayerEvent()
    event.callbackDecodeData = True
    event.directDecode = True
    # 设置视频编码才是 640 宽 480 高 1000000 最大码率和目标码率 800000最小码率 30 编码帧率
    rtcEngine.update_video_config(640, 480, 1000000, 800000, 30)
    # 设置音频编码参数 48000 采样率 1 声道数（单声道） 80000 （编码码率）
    rtcEngine.update_audio_Config(48000, 1, 80000)
    rtcEngine.set_audio_play_event(event)
    #channels 频道号 uid 用户Id token 加入频道的token mode RTC的模式，0 是server 1是client
    rtcEngine.join_channel("954523133", 31212121, "token", 0)

    yuvFilePath = os.path.abspath('./windows/bin/win64/640X480.yuv')
    yuvFile = open(yuvFilePath, 'rb')
    yuvData = yuvFile.read(int(640*480*3/2))
    yuvByteArray =  bytearray()
    yuvByteArray.extend(yuvData)
    yuvFile.close()
    pcmfilePath = os.path.abspath('./windows/bin/win64/capture_in_debug.pcm')
    sampleCount = 480
    pcm_file = open(pcmfilePath, 'rb')
    index = 0
    while  True:
        if rtcEventHandler.isLinkOk:
            break

    try:
        while True:
            index = index + 1
            if index % 3 == 0:
                if index % 6 == 0:
                    #发送yuv数据到RTC中编码640 宽 480 高 0 旋转方向 0 时间戳 2 数据格式 2 是YUVI420
                    rtcEngine.push_raw_video_frame(yuvByteArray, 640, 480, 0, 0, 2)
                else:
                    rtcEngine.push_raw_video_frame(yuvData, 640, 480, 0, 0, 2)
            chunk = pcm_file.read(sampleCount * 2)
            if not chunk:
                break
            # 发送音频数据 48000采样率 1 声道数（单声道） 2 位深（每个sample包含多少个byte例如：int16 则为2 int8 1 int32 4）
            rtcEngine.push_audio_pcm_frame(chunk, 48000, 1, 2)
            # if index % 2 == 0:
            #     pcm_bytearray = bytearray()
            #     pcm_bytearray.extend(chunk)
            #     rtcEngine.push_audio_pcm_frame(pcm_bytearray, 48000, 1, 2)
            # else:
            #     rtcEngine.push_audio_pcm_frame(chunk, 48000, 1, 2)
            
            
            time.sleep(0.01)
    finally:
        pcm_file.close()

    time.sleep(200)

    rtcEngine.leave_channel()
    rtcEngine.destroy()
    rtcEngine = None




if __name__ == "__main__":
    main()

```

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
    rtcEngine.update_video_config(640, 480, 1000000, 800000, 30)
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
    try:
        while True:
            index = index + 1
            chunk = pcm_file.read(sampleCount * 2)
            if not chunk:
                break
            rtcEngine.push_audio_pcm_frame(chunk, 48000, 1, 2)
            # if index % 2 == 0:
            #     pcm_bytearray = bytearray()
            #     pcm_bytearray.extend(chunk)
            #     rtcEngine.push_audio_pcm_frame(pcm_bytearray, 48000, 1, 2)
            # else:
            #     rtcEngine.push_audio_pcm_frame(chunk, 48000, 1, 2)
            
            if index % 3 == 0:
                if index % 6 == 0:
                    rtcEngine.push_raw_video_frame(yuvByteArray, 640, 480, 0, 0, 2)
                else:
                    rtcEngine.push_raw_video_frame(yuvData, 640, 480, 0, 0, 2)
            time.sleep(0.01)
    finally:
        pcm_file.close()

    time.sleep(200)

    rtcEngine.leave_channel()
    rtcEngine.destroy()
    rtcEngine = None




if __name__ == "__main__":
    main()

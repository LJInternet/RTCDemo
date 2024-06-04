import struct
from enum import Enum

class PixelFormat(Enum):
    PIXEL_FMT_RGBA = 1
    PIXEL_FMT_YUV_I420 = 2
    PIXEL_FMT_NV21 = 3
    PIXEL_FMT_RGB24 = 4
    PIXEL_FMT_NV12 = 5
    PIXEL_FMT_YUY2 = 6
    PIXEL_FMT_H264 = 7
    PIXEL_FMT_H265 = 8

class UDPCallbackType(Enum):
    RUDP_CB_TYPE_DECODE_DATA = 1
    RUDP_CB_TYPE_AVAILABLE_BW = 2
    RUDP_CB_TYPE_REQUEST_I_FRAME = 3
    RUDP_CB_TYPE_LINK_OK = 4
    RUDP_CB_TYPE_LINK_FAILURE = 5
    RUDP_CB_TYPE_LINK_REPORT = 6
    RUDP_CB_TYPE_NET_REPORT = 7
    RUDP_CB_VIDEO_FRAME_RATE_CONTROL = 8
    CB_AUDIO_CAPTURE_VOLUME = 9
    CB_JOIN_CHANNEL = 10
    CB_LEAVE_CHANNEL = 11
    CB_LINK_STATUS = 12
    CB_TRANS_STOP = 13
    MUTI_CHANNEL_REMOTE_JOIN = 1000
    MUTI_CHANNEL_REMOTE_LEAVE = 1001

class ELenType(Enum):
    E_SHORT = 1
    E_INT = 2
    E_LONG = 3
    E_NONE = 4

class Marshallable:
    def __init__(self):
        self.buf = bytearray()

    def marshall(self):
        return self.buf
    
    def marshallBuffer(self, buffer):
        self.buf = buffer

    def unmarshall(self, buffer):
        if isinstance(buffer, memoryview):
            self.buf = bytearray(buffer.tobytes())
            self.mv = buffer
        else:
            self.buf = buffer
            self.mv = memoryview(self.buf)
    def clear(self):
        self.buf.clear()

    def pushBool(self, val):
        packed_value = struct.pack('?', val)  # 将布尔值打包成一个字节
        self.buf.extend(packed_value)  # 将打包后的字节写入到 bytearray 中

    def popBool(self):
        if self.mv and len(self.mv) > 0:
            result = bool(self.mv[0])
            self.mv = self.mv[1:]
            return result
        else:
            print("popBool false buffer len is 0")
            return False

    def pushByte(self, val):
        self.buf.append(val)  # 将传入的字节值添加到 bytearray 中

    def popByte(self):
        if self.mv and len(self.mv) > 0:
            result = self.mv[0]
            self.mv = self.mv[1:]
            return result
        else:
            print("popByte false buffer len is 0")
            return False

    def pushBytes(self, bytes):
        if bytes:
            length = len(bytes)  # 获取字节数组的长度
            self.pushShort(length)
            self.buf.extend(bytes)  # 写入内容
        else:
            self.pushShort(0)

    def popBytes(self):
        if self.mv and len(self.mv) >= 2:
            len = int.from_bytes(self.mv[:2], byteorder='little')
            if len == 0:
                self.mv = self.mv[2:]
                return None
            else:
                result = self.mv[2:len]
                self.mv = self.mv[2+len:]
                return result
        else:
            print("popBytes false buffer len is < 2")
            return None

    def pushBytes32(self, bytes):
        if bytes:
            length = len(bytes)  # 获取字节数组的长度
            self.pushInt(length)
            self.buf.extend(bytes)  # 写入内容
        else:
            self.pushInt(0)

    def popBytes32(self):
        if self.mv and len(self.mv) >= 4:
            len = int.from_bytes(self.mv[:4], byteorder='little')
            if len == 0:
                self.mv = self.mv[4:]
                return None
            else:
                result = self.mv[4:len]
                self.mv = self.mv[4+len:]
                return result
        else:
            print("popBytes32 false buffer len is < 4")
            return None

    def pushShort(self, val):
        self.buf.extend(val.to_bytes(2, byteorder='little', signed=False))  # 将转换为 2 个字节并写入

    def popShort(self):
        if self.mv and len(self.mv) >= 2:
            result = int.from_bytes(self.mv[:2], byteorder='little')
            self.mv = self.mv[2:]
            return result
        else:
            print("popShort false buffer len is < 2")
            return 0

    def pushInt(self, val):
        self.buf.extend(val.to_bytes(4, byteorder='little', signed=False))  # 将转换为 4 个字节并写入

    def popInt(self):
        if self.mv and len(self.mv) >= 4:
            result = int.from_bytes(self.mv[:4], byteorder='little')
            self.mv = self.mv[4:]
            return result
        else:
            print("popInt false buffer len is < 4")
            return 0

    def pushInt64(self, val):
        self.buf.extend(val.to_bytes(8, byteorder='little', signed=False))  # 将转换为 8 个字节并写入

    def popInt64(self):
        if self.mv and len(self.mv) >= 8:
            result = int.from_bytes(self.mv[:8], byteorder='little')
            self.mv = self.mv[8:]
            return result
        else:
            print("popShort false buffer len is < 8")
            return 0

    def pushString16(self, val):
        if val:
            encoded_str = val.encode('utf-8')  # 将字符串编码为字节序列
            length = len(encoded_str)
            self.pushShort(length)
            if length > 0:
                self.buf.extend(encoded_str)  # 写入编码后的字符串
        else:
            self.pushShort(0)

    def popString16(self):
        if self.mv and len(self.mv) > 2:
            len = self.popShort()
            if len == 0 :
                return ""
            else:
                result = (self.mv[:len]).decode('utf-8')
                self.mv = self.mv[len:]
                return result
        else:
            print("popString16 false buffer len is < 2")
            return ""

    def pushString32(self, val):
        if val:
            encoded_str = val.encode('utf-8')  # 将字符串编码为字节序列
            length = len(encoded_str)
            self.pushInt(length)
            self.buf.extend(encoded_str)  # 写入编码后的字符串
        else:
            self.pushInt(0)

    def popString32(self):
        if self.mv and len(self.mv) > 4:
            len = self.popInt()
            if len == 0 :
                return ""
            else:
                result = (self.mv[:len]).decode('utf-8')
                self.mv = self.mv[len:]
                return result
        else:
            print("popString32 false buffer len is < 4")
            return ""

    def pushMarshallable(self, val):
        if val:
            val.marshallBuffer(self.buf)

    def popMarshallable(self, marshal_class):
        val = None
        try:
            val = marshal_class()
        except Exception as e:
            print(e)
        
        val.unmarshall(self.mv)
        return val

    def pushCollection(self, data, elemClass, lenType):
        if data is None or len(data) == 0:
            self.pushInt(0)
        else:
            self.pushInt(len(data))
            for elem in data:
                self.pushElem(elem, elemClass, lenType)

    def pushElem(self, elem, elem_class, len_type):
        if elem_class == int:
            if len_type == ELenType.E_SHORT:
                self.pushShort(elem)
            elif len_type == ELenType.E_INT:
                self.pushInt(elem)
            elif len_type == ELenType.E_LONG:
                self.pushInt64(elem)
            else:
                self.pushInt(elem)
        elif elem_class == bytes:
            self.pushBytes32(elem)
        elif elem_class == str:
            if len_type == ELenType.E_SHORT:
                self.pushString16(elem)
            elif len_type == ELenType.E_INT:
                self.pushString32(elem)
            else:
                print("invalid len_type={} for push_string".format(len_type))
        elif isinstance(elem, Marshallable):
            self.pushMarshallable(self.buf, elem)
        else:
            raise RuntimeError("unable to marshal element of class " + str(elem_class))

    def popCollection(self, elemClass, lenType):
        size = self.popInt(0)
        if size == 0:
            return []
        else:
            result = []
            for i in range(size):
                result.append(self.popElem(elemClass, lenType))

            return result

    def popElem(self, elem_class, len_type):
        if elem_class == int:
            if len_type == ELenType.E_SHORT:
                return self.popShort()
            elif len_type == ELenType.E_INT:
                return self.popInt()
            elif len_type == ELenType.E_LONG:
                return self.popInt64()
            else:
                return self.popInt()
        elif elem_class == bytes:
            return self.popBytes32()
        elif elem_class == str:
            if len_type == ELenType.E_SHORT:
                return self.popString16()
            elif len_type == ELenType.E_INT:
                return self.popString32()
            else:
                print("invalid len_type={} for pop_string".format(len_type))
                # 这里需要根据您的逻辑返回合适的值，这里只是示例
                return ""
        elif issubclass(elem_class, Marshallable):
            return self.popMarshallable(elem_class)
        else:
            raise RuntimeError("unable to unmarshal element of class " + str(elem_class))
        
    def popMap(self, key_class, key_len_type, value_calss, value_len_type):
        size = self.popInt()
        out = {}
        for i in range(size):
            key = self.popElem(key_class, key_len_type)
            elem = self.popElem(value_calss, value_len_type)
            out[key] = elem
        return out

class MIEAudioUploadConfig(Marshallable):
    def __init__(self):
        super().__init__()
        self.sampleRate: int = 0
        self.channels: int = 0
        self.bitsPerSample: int = 0
        self.audioBitrateInbps: int = 0
        self.audioType: int = 0

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushInt(self.sampleRate)
        self.pushInt(self.channels)
        self.pushInt(self.bitsPerSample)
        self.pushInt(self.audioBitrateInbps)
        self.pushInt(self.audioType)
    
    def marshall(self):
        self.pushInt(self.sampleRate)
        self.pushInt(self.channels)
        self.pushInt(self.bitsPerSample)
        self.pushInt(self.audioBitrateInbps)
        self.pushInt(self.audioType)
        return super().marshall()

class MIEVideoUploadConfig(Marshallable):
    def __init__(self):
        super().__init__()
        self.encodeWidth: int = 0
        self.encodeHeight: int = 0
        self.minVideoBitrateInbps: int = 0
        self.maxVideoBitrateInbps: int = 0
        self.realVideoBitrateInbps: int = 0
        self.codecType: int = 0
        self.fps: int = 0
        self.mirror: int = 0
        self.keyFrameInterval: int = 0
        self.mode: int = 0

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushInt(self.encodeWidth)
        self.pushInt(self.encodeHeight)
        self.pushInt(self.minVideoBitrateInbps)
        self.pushInt(self.maxVideoBitrateInbps)
        self.pushInt(self.realVideoBitrateInbps)
        self.pushInt(self.codecType)
        self.pushInt(self.fps)
        self.pushInt(self.mirror)
        self.pushInt(self.keyFrameInterval)
        self.pushInt(self.mode)
    
    def marshall(self):
        self.pushInt(self.encodeWidth)
        self.pushInt(self.encodeHeight)
        self.pushInt(self.minVideoBitrateInbps)
        self.pushInt(self.maxVideoBitrateInbps)
        self.pushInt(self.realVideoBitrateInbps)
        self.pushInt(self.codecType)
        self.pushInt(self.fps)
        self.pushInt(self.mirror)
        self.pushInt(self.keyFrameInterval)
        self.pushInt(self.mode)
        return super().marshall()

class UdpInitConfig(Marshallable):
    def __init__(self):
        super().__init__()
        self.relayId:int = 12222
        self.netType:int = 1
        self.remoteIP:str = "61.155.136.209"
        self.remotePort:int = 30001
        self.remoteSessionId:int = 10083

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushInt64(self.relayId)
        self.pushString16(self.remoteIP)
        self.pushInt(self.remotePort)
        self.pushInt(self.remoteSessionId)
        self.pushInt(self.netType)
    
    def marshall(self):
        self.pushInt64(self.relayId)
        self.pushString16(self.remoteIP)
        self.pushInt(self.remotePort)
        self.pushInt(self.remoteSessionId)
        self.pushInt(self.netType)
        return super().marshall()

class MIETransferConfig(Marshallable):
    def __init__(self):
        super().__init__()
        self.configs = []
        self.p2pSignalServer:str = ""
        self.transferMode:int = 0
        self.appID:int = 0
        self.userID:int = 0
        self.channelID:int = ""
        self.token:str = ""
        self.localIp:int = 0

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushString16(self.p2pSignalServer)
        self.pushInt(self.transferMode)
        self.pushCollection(self.configs, UdpInitConfig, ELenType.E_NONE)
        self.pushInt64(self.appID)
        self.pushInt64(self.userID)
        self.pushString16(self.channelID)
        self.pushString16(self.token)

    def marshall(self):
        self.pushString16(self.p2pSignalServer)
        self.pushInt(self.transferMode)
        self.pushCollection(self.configs, UdpInitConfig, ELenType.E_NONE)
        self.pushInt64(self.appID)
        self.pushInt64(self.userID)
        self.pushString16(self.channelID)
        self.pushString16(self.token)
        return super().marshall()

class MIEUploadConfig(Marshallable):
    def __init__(self):
        super().__init__()
        self.enableAudio:int = 1
        self.enableVideo:int = 1
        self.audioUploadConfig:MIEAudioUploadConfig = MIEAudioUploadConfig()
        self.videoUploadConfig:MIEVideoUploadConfig = MIEVideoUploadConfig()
        self.transferConfig:MIETransferConfig = MIETransferConfig()

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushInt(self.enableAudio)
        self.pushInt(self.enableVideo)
        self.pushMarshallable(self.audioUploadConfig)
        self.pushMarshallable(self.videoUploadConfig)
        self.pushMarshallable(self.transferConfig)
    
    def marshall(self):
        self.pushInt(self.enableAudio)
        self.pushInt(self.enableVideo)
        self.pushMarshallable(self.audioUploadConfig)
        self.pushMarshallable(self.videoUploadConfig)
        self.pushMarshallable(self.transferConfig)
        return super().marshall()

class RTCEngineConfig(Marshallable):
    def __init__(self):
        super().__init__()
        self.enableLog: bool = True

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushBool(self.enableLog)
        
    
    def marshall(self):
        self.pushBool(self.enableLog)
        return super().marshall()
    
class AudioPlayerEvent(Marshallable):
    def __init__(self):
        super().__init__()
        self.callbackDecodeData: bool = False #是否需要回调音频数据
        self.renderAudioData: bool = False #数据是否需要播放，false 则直接静音
        self.directDecode: bool = False #收到远端音频直接解码，不需要经过JitterBuffer
        self.directCallback: bool = False #rudp收到数据包啥也不做，直接返回未解码数据，回调IAudioProcessor

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushBool(self.callbackDecodeData)
        self.pushBool(self.renderAudioData)
        self.pushBool(self.directDecode)
        self.pushBool(self.directCallback)
        
    
    def marshall(self):
        self.pushBool(self.callbackDecodeData)
        self.pushBool(self.renderAudioData)
        self.pushBool(self.directDecode)
        self.pushBool(self.directCallback)
        return super().marshall()

class TransferMsg(Marshallable):
    def __init__(self):
        super().__init__()
        self.value:int = 0

    def marshallBuffer(self, buffer):
        super().marshallBuffer(buffer)
        self.pushInt(self.value)

    def marshall(self):
        self.pushInt(self.value)
        return super().marshall()
    
class AvailableBands(Marshallable):
    def __init__(self):
        self.out= {}

    def unmarshall(self, buffer):
        super().unmarshall(buffer)
        self.out = self.popMap(int, ELenType.E_INT, int, ELenType.E_INT)

class LinkStatusEvent(Marshallable):
    def __init__(self):
        self.reslut = 0

    def unmarshall(self, buffer):
        super().unmarshall(buffer)
        self.reslut = self.popInt()

class NetQuality(Marshallable):
    def __init__(self):
        self.mLocalQuality = 0
        self.mRemoteQuality = 0

    def unmarshall(self, buffer):
        super().unmarshall(buffer)
        self.mLocalQuality = self.popByte()
        self.mRemoteQuality = self.popByte()
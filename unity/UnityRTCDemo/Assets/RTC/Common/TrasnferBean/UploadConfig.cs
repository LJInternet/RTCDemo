using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJ.RTC.Common
{
    public enum UploadType
    {
        UDP = 0,
        RTMP = 1,
        RTSP = 2,
    }

    public enum AudioType {
        AAC = 0,
        OPUS = 1,
    }
    /**
     * 推流配置参数
     */
    public class UploadConfig : HPMarshaller
    {
        /**
         * 推流类型，rtmp、rtsp udp
         */
        public int uploadType = (int)UploadType.UDP;

        public long baseTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        public int enableAudio = 1;
        public int enableVideo = 1;

        public AudioUploadConfig audioUploadConfig = new AudioUploadConfig();

        public VideoUploadConfig videoUploadConfig = new VideoUploadConfig();

        public TransferConfig transferConfig = new TransferConfig();

        public override byte[] marshall()
        {
            pushInt(enableAudio);
            pushInt(enableVideo);
            pushMarshallable(audioUploadConfig);
            pushMarshallable(videoUploadConfig);
            pushMarshallable(transferConfig);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            enableAudio = popInt();
            enableVideo = popInt();
            audioUploadConfig = popMarshallable<AudioUploadConfig>();
            videoUploadConfig = popMarshallable<VideoUploadConfig>();
            transferConfig = popMarshallable<TransferConfig>();
        }
    }

    public class AudioUploadConfig : HPMarshaller
    {

        /** 音频 */
        // 采样率，一般为44100
        public int sampleRate;
        // 通道数
        public int channels;
        // 每个采样占多少位
        public int bitsPerSample;
        // 音频码率，高音质一般为192kbps，低音质一般为24kbps
        public int audioBitrateInbps;

        public int audioType = (int)AudioType.AAC;


        public override byte[] marshall()
        {
            pushInt(sampleRate);
            pushInt(channels);
            pushInt(bitsPerSample);
            pushInt(audioBitrateInbps);
            pushInt(audioType);
            return base.marshall();
        }

        public override void marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.marshall(stream, writer, reader);
            pushInt(sampleRate);
            pushInt(channels);
            pushInt(bitsPerSample);
            pushInt(audioBitrateInbps);
            pushInt(audioType);
        }

        public override void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.unmarshall(stream, writer, reader);
            sampleRate = popInt();
            channels = popInt();
            bitsPerSample = popInt();
            audioBitrateInbps = popInt();
            audioType = popInt();
        }


        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            sampleRate = popInt();
            channels = popInt();
            bitsPerSample = popInt();
            audioBitrateInbps = popInt();
            audioType = popInt();
        }
    }

    public class VideoUploadConfig : HPMarshaller
    {

        /** 视频 */
        // 视频编码后的宽高
        public int encodeWidth;
        public int encodeHeight;
        // 视频帧率
        public int fps;

        // 视频码率，包括最大码率、最小码率和实际码率，单位都为bps
        public int minVideoBitrateInbps;
        public int maxVideoBitrateInbps;
        public int realVideoBitrateInbps;

        // 编码方式，h265：1，h264：0
        public int codecType;

        public int mirror = 0;

        public int keyFrameInterval = 3;

        public int bitrateMode = 2;

        public bool isH265()
        {
            return codecType == 1;
        }

        // 是否是硬编码
        public bool isHardEncode = true;

        /**
         * 视频编码封装类型
         */
        public String videoMuxType;

        public override byte[] marshall()
        {
            pushInt(encodeWidth);
            pushInt(encodeHeight);
            pushInt(minVideoBitrateInbps);
            pushInt(maxVideoBitrateInbps);
            pushInt(realVideoBitrateInbps);
            pushInt(codecType);
            pushInt(fps);
            pushInt(mirror);
            pushInt(keyFrameInterval);
            pushInt(bitrateMode);
            return base.marshall();
        }
        public override void marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.marshall(stream, writer, reader);
            pushInt(encodeWidth);
            pushInt(encodeHeight);
            pushInt(minVideoBitrateInbps);
            pushInt(maxVideoBitrateInbps);
            pushInt(realVideoBitrateInbps);
            pushInt(codecType);
            pushInt(fps);
            pushInt(mirror);
            pushInt(keyFrameInterval);
            pushInt(bitrateMode);
        }

        public override void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.unmarshall(stream, writer, reader);
            encodeWidth = popInt();
            encodeHeight = popInt();
            minVideoBitrateInbps = popInt();
            maxVideoBitrateInbps = popInt();
            realVideoBitrateInbps = popInt();
            codecType = popInt();
            fps = popInt();
            mirror = popInt();
            keyFrameInterval = popInt();
            bitrateMode = popInt();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            encodeWidth = popInt();
            encodeHeight = popInt();
            minVideoBitrateInbps = popInt();
            maxVideoBitrateInbps = popInt();
            realVideoBitrateInbps = popInt();
            codecType = popInt();
            fps = popInt();
            keyFrameInterval = popInt();
        }
    }

    public class TransferConfig : HPMarshaller
    {

        public List<UdpInitConfig> configs = new List<UdpInitConfig>();
        public int transferMode = 1;
        public string p2pSignalServer = "61.155.136.209:9988";

        public Int64 appID;
        public Int64 userID;
        public string channelID;
        public string token;

        public TransferConfig()
        {
        }

        public byte[] marshall()
        {
            pushString16(p2pSignalServer);
            pushInt(transferMode);
            pushCollection<UdpInitConfig>(configs, ELenType.E_NONE);
            pushInt64(appID);
            pushInt64(userID);
            pushString16(channelID);
            pushString16(token);
            return base.marshall();
        }

        public override void marshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.marshall(stream, writer, reader);
            pushString16(p2pSignalServer);
            pushInt(transferMode);
            pushCollection<UdpInitConfig>(configs, ELenType.E_NONE);
            pushInt64(appID);
            pushInt64(userID);
            pushString16(channelID);
            pushString16(token);
        }

        public override void unmarshall(MemoryStream stream, BinaryWriter writer, BinaryReader reader)
        {
            base.unmarshall(stream, writer, reader);
            p2pSignalServer = popString16();
            transferMode = popInt();
            configs = popCollection<UdpInitConfig>(ELenType.E_NONE);
            appID = popInt64();
            userID = popInt64();
            channelID = popString16();
            token = popString16();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            p2pSignalServer = popString16();
            transferMode = popInt();
            configs = popCollection<UdpInitConfig>(ELenType.E_NONE);
            appID = popInt64();
            userID = popInt64();
            channelID = popString16();
            token = popString16();
        }

    }
}

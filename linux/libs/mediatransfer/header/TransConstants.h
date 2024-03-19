//
// Created by Administrator on 2022/6/14.
//

#ifndef LJSDK_TRANSCONSTANTS_H
#define LJSDK_TRANSCONSTANTS_H

#define VIDEO_REPORT_DURATION 2 // 1S

#include <sstream>
#include "MediaConstants.h"
#include "LJPacket.h"

namespace LJMediaLibrary {

    enum UDPCallbackType {
        RUDP_CB_TYPE_DECODE_DATA = 1,  // 解码数据
        RUDP_CB_TYPE_AVAILABLE_BW = 2, // 有效带宽
        RUDP_CB_TYPE_REQUEST_I_FRAME = 3, // 等同于需要一个I帧
        RUDP_CB_TYPE_LINK_OK = 4, // 首次建连成功或者中途断开重连成功，这个时候需要发一个I帧
        RUDP_CB_TYPE_LINK_FAILURE = 5, // 丢包非常严重，可能需要切流什么的
        RUDP_CB_TYPE_LINK_REPORT = 6, // 链路上报
        RUDP_CB_TYPE_NET_REPORT = 7, //网络质量回调
        RUDP_CB_VIDEO_FRAME_RATE_CONTROL = 8, //视频帧率控制
        CB_AUDIO_CAPTURE_VOLUME = 9, //音频采集音量回调
        CB_JOIN_CHANNEL = 10, //加入频道结果回调
        CB_LEAVE_CHANNEL = 11, //离开频道结果回调
        CB_LINK_STATUS = 12, //RUDP 连接状态
        CB_TRANS_STOP = 13, //调用了C++层的LeaveChannel
        MUTI_CHANNEL_REMOTE_JOIN = 1000, //多人RTC远端有人加入
        MUTI_CHANNEL_REMOTE_LEAVE = 1001, //多人RTC远端有人退出
    };

    enum NetQualityLevel {
        QUALITY_GOOD = 1, //网络质量好
        QUALITY_COMMON = 2, //网络质量一般
        QUALITY_BAD = 3, //勉强能沟通
        QUALITY_VBAD = 4, //网络质量非常差，基本不能沟通。
        QUALITY_BLOCK = 5, //链路不通
    };

    enum DelayConstants {
        /**
         * 采集时间戳，一般是相机的的纹理回调回来的时间戳
         */
        KEY_CAPTURE_TIME = 1,
        /**
         * 结束前处理时间戳
         */
        KEY_END_PREPROCESS_TIME = 2,
        /**
         * 结束编码时间戳
         */
        KEY_END_ENCODE_TIME = 3,

        /**
         * JNI开始的时间
         */
        KEY_JNI_START_TIME = 4,
        /**
         * 发送帧数据的时间
         */
        KEY_SEND_UDP_TIME = 5,
        /**
         * 接收到数据的时间
         */
        KEY_REV_DATA_TIME = 6,
        /**
        * 开始解码码的时间
        */
        KEY_START_DECODE_TIME = 7,

        KEY_END_DECODE_TIME = 8,
        /**
        * java收到数据包的时间
        */
        KEY_JVM_REV_DATA = 9,
        /**
        * 开始渲染时间
        */
        KEY_START_RENDER = 10,
        /**
        * 结束渲染时间
        */
        KEY_END_RENDER = 11,

        /**
        * 帧类型
        */
        KEY_FRAME_TYPE = 12,
        /**
        * 帧类型
        */
        KEY_FRAME_SIZE = 13,

        /**
         * 传输延时，这里是上次包的传输延时
         */
        KEY_TRANS_DELAY = 14,

        /**
         * 传输延时对应的帧Id
         */
        KEY_TRANS_DELAY_FRAME_ID = 15,

        KEY_START_ENCODE_TIME = 16,

        /**
         * 解码切换线程队列，开始回调和结束回调的时间
         */
        KEY_DECODE_CACHE_START = 100,
        KEY_DECODE_CACHE_END = 101,
    };

    /**
     * 在调用UDP send 的方法时，数据的前4为，是传输数据类型
     */
    enum TransDataType {
        VIDEO_DATA = 1,
        AUDIO_DATA = 2,
        BACK_HEAD_DATA = 3,
        NET_STATIC = 4,
        COMMON_STATIC = 5,
        AVAILABLE_BM = 6,
        REQUEST_I_FRAME = 7,
        RUDP_JOIN = 8,
        RUDP_LEAVE = 9,
    };


    enum CommonStatisticKey {
        /**
         * 解码性能统计，统计解码缓存格式以及解码帧率
         */
        KEY_DECODE_CACHE_COUNT = 1,
        KEY_DECODE_FPS = 2,
        KEY_DECODE_COAST = 3,
    };

    class TransPack {
    public:
        TransPack() : m_pb(), m_pk(m_pb) {

        }

    public:
        bool isPackError() const {
            return m_pk.isPackError();
        }

        size_t bodySize() {
            return m_pk.size();
        }

        const char *body() {
            return m_pk.data();
        }

        void marshall(const ljtransfer::mediaSox::Marshallable &m) {
            m.marshal(m_pk);
        }

        void clear() {
            m_pb.resize(0);
            m_pk.setPackError(false);
        }

    protected:
        ljtransfer::mediaSox::PackBuffer m_pb;
        ljtransfer::mediaSox::Pack m_pk;
    };

    struct MIEPushEncodedVideoData : ljtransfer::mediaSox::Marshallable {
        uint32_t iFrameType;
        uint32_t iEncodeType;
        uint32_t iPts;
        uint32_t iDts;
        uint64_t iStreamId;
        uint32_t width;
        uint32_t height;
        std::string iData;
        std::string iMetaDta;
        std::string iExtraData;
        std::map<uint64_t, uint64_t> iTsInfos;

        MIEPushEncodedVideoData()
                : iFrameType(0), iEncodeType(0), iPts(0), iDts(0), iStreamId(0), width(0),
                  height(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << iFrameType << iEncodeType << iPts << iDts << iStreamId << width << height;
//            pak.push_varstr32(iData.data(), iData.length());
            pak.push_varstr32(iMetaDta.data(), iMetaDta.length());
            pak.push_varstr32(iExtraData.data(), iExtraData.length());
            marshal_container(pak, iTsInfos);
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> iFrameType >> iEncodeType >> iPts >> iDts >> iStreamId >> width >> height;
//            iData = pak.pop_varstr32();
            iMetaDta = pak.pop_varstr32();
            iExtraData = pak.pop_varstr32();
            unmarshal_container(pak, inserter(iTsInfos, iTsInfos.begin()));
        }
    };

    struct MIEPushVideoRawData : ljtransfer::mediaSox::Marshallable {
        uint32_t width;
        uint32_t height;
        uint32_t pixelFormat;
        uint32_t rotation;
        uint64_t timestamp;
//        std::string iData;

        MIEPushVideoRawData()
                : width(0), height(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << width << height<<pixelFormat<<rotation<<timestamp;
//            pak.push_varstr32(iData.data(), iData.length());
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> width >> height>>pixelFormat>>rotation>>timestamp;
//            iData = pak.pop_varstr32();
        }
    };

    struct CaptureVideoFrame : ljtransfer::mediaSox::Marshallable {
        uint32_t type;
        uint32_t format;
        uint32_t stride;
        uint32_t width;
        uint32_t height;
        uint32_t rotation;
        uint32_t eglType;
        uint32_t textureId;
        uint64_t timestamp;
        uint32_t metadata_size;
        std::string metadata_buffer;
        uint32_t mirror;
        uint32_t corpLeft;
        uint32_t cropRight;
        uint32_t cropTop;
        uint32_t cropBottom;
        uint32_t ProgramType;
        uint32_t corpType;

        CaptureVideoFrame() : type(0),
                              format(0),
                              stride(0),
                              width(0),
                              height(0),
                              rotation(0),
                              eglType(0),
                              textureId(0),
                              timestamp(0),
                              mirror(0),
                              corpLeft(0),
                              cropRight(0),
                              cropTop(0),
                              cropBottom(0),
                              ProgramType(0),
                              corpType(0),
                              metadata_size(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type << format << stride << width << height << rotation << eglType << textureId
                << timestamp << metadata_size;
            pak.push_varstr32(metadata_buffer.data(), metadata_buffer.length());
            pak << mirror << corpLeft << cropRight << cropTop << cropBottom << ProgramType
                << corpType;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type >> format >> stride >> width >> height >> rotation >> eglType >> textureId
                >> timestamp >> metadata_size;
            metadata_buffer = pak.pop_varstr32();
            pak >> mirror;
            if (pak.size() > 4u) {
                pak >> corpLeft >> cropRight >> cropTop >> cropBottom >> ProgramType >> corpType;
            }
        }
    };

    struct VideoDecodedData : ljtransfer::mediaSox::Marshallable {
        uint32_t width;
        uint32_t height;
        uint32_t widthY;
        uint32_t heightY;
        uint32_t widthUV;
        uint32_t heightUV;
        uint32_t offsetY;
        uint32_t offsetU;
        uint32_t offsetV;
        uint32_t len;
        uint32_t frameId;
        uint8_t *data[8];
        std::string iExtraData;
        std::map<uint64_t, uint64_t> delayData;

        VideoDecodedData()
                : width(0), height(0), widthY(0), heightY(0), widthUV(0), heightUV(0), offsetY(0),
                  offsetU(0), offsetV(0), frameId(0), len(0) {
            for (int i = 0; i < 8; i++) {
                data[i] = NULL;
            }
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << width << height << widthY << heightY << widthUV << heightUV << offsetY << offsetU
                << offsetV << len << frameId;
            pak.push_varstr32(iExtraData.data(), iExtraData.length());
            marshal_container(pak, delayData);
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> width >> height >> widthY >> heightY >> widthUV >> heightUV >> offsetY >> offsetU
                >> offsetV >> len >> frameId;
            iExtraData = pak.pop_varstr32();
            unmarshal_container(pak, inserter(delayData, delayData.begin()));
        }

        void reset() {
            widthY = 0;
            heightY = 0;
            heightUV = 0;
            widthUV = 0;
            offsetU = 0;
            offsetV = 0;
            offsetY = 0;
            frameId = 0;
            len = 0;
            for (int i = 0; i < 8; i++) {
                data[i] = NULL;
            }
            delayData.clear();
            iExtraData.clear();
        }
    };

    struct MIEVideoCaptureConfig : ljtransfer::mediaSox::Marshallable {
        uint32_t width;
        uint32_t height;
        uint32_t fps;
        std::string deviceCode;

        MIEVideoCaptureConfig()
                : width(0), height(0), fps(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << width << height << fps;
            pak.push_varstr32(deviceCode.data(), deviceCode.length());
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> width >> height >> fps;
            deviceCode = pak.pop_varstr32();
        }
    };

    struct CacheVideoDecodedData : VideoDecodedData {
        DelayTimeInfoList *mDelayList;
        char *extraData;
        uint32_t iExtraDataSize;

        CacheVideoDecodedData() :
                mDelayList(nullptr), extraData(nullptr), iExtraDataSize(0) {
        }
    };

    struct MIEPushEncodedAudioData : ljtransfer::mediaSox::Marshallable {
        uint64_t iPts;
        uint32_t isHead;
        uint32_t audioType;
        std::string iData;

        MIEPushEncodedAudioData()
                : iPts(0), isHead(0), audioType(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << iPts << isHead << audioType;
            pak.push_varstr32(iData.data(), iData.length());
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> iPts >> isHead >> audioType;
            iData = pak.pop_varstr32();
        }
    };

    struct AudioHead : ljtransfer::mediaSox::Marshallable {
        uint64_t iPts;
        std::string iHead;
        uint32_t audioType;
        uint32_t frameIndex;

        AudioHead() : iPts(0), audioType(0), frameIndex(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << iPts << audioType << frameIndex;
            pak.push_varstr32(iHead.data(), iHead.length());
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> iPts >> audioType >> frameIndex;
            iHead = pak.pop_varstr32();
        }
    };

    struct VideoHead : ljtransfer::mediaSox::Marshallable {
        uint32_t iFrameType;
        uint32_t iEncodeType;
        uint32_t iPts;
        uint32_t iDts;
        uint32_t iFrameId;
        uint32_t iMetaDataLen;
        std::string iMetaDta;
        uint32_t iExtraDataLen;
        std::string iExtraData;
        std::string header;
        std::map<uint64_t, uint64_t> iTsInfos;
        uint16_t width;
        uint16_t height;

        VideoHead()
                : iFrameType(0), iEncodeType(0), iPts(0), iDts(0), iFrameId(0), iMetaDataLen(0),
                  iExtraDataLen(0), width(720), height(1280) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << iFrameType << iEncodeType << iPts << iDts << iFrameId << iMetaDataLen
                << iExtraDataLen;
            pak.push_varstr32(iMetaDta.data(), iMetaDta.length());
            pak.push_varstr32(iExtraData.data(), iExtraData.length());
            marshal_container(pak, iTsInfos);
            pak << width << height;
            pak.push_varstr(header.data(), header.length());
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> iFrameType >> iEncodeType >> iPts >> iDts >> iFrameId >> iMetaDataLen
                >> iExtraDataLen;
            iMetaDta = pak.pop_varstr32();
            iExtraData = pak.pop_varstr32();
            unmarshal_container(pak, inserter(iTsInfos, iTsInfos.begin()));
            if (!pak.empty()) {
                pak >> width >> height;
            }
            if (!pak.empty()) {
                header = pak.pop_varstr();
            }
        }
    };

    struct UDPConfig : ljtransfer::mediaSox::Marshallable {
        uint64_t relayId;
        std::string remoteIP;
        uint32_t remotePort;
        uint32_t remoteSessionId;
        uint32_t netType;

        UDPConfig()
                : remoteIP("61.155.136.210"),
                  relayId(122222),
                  remotePort(30001),
                  remoteSessionId(10089),
                  netType(2) {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << relayId << remoteIP << remotePort << remoteSessionId << netType;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> relayId >> remoteIP >> remotePort >> remoteSessionId >> netType;
        }
    };

    struct MIETransferConfig : ljtransfer::mediaSox::Marshallable {
        std::vector<UDPConfig> configs;
        std::string p2pSignalServer;
        uint32_t transferMode;
        uint64_t appID;
        uint64_t userID;
        std::string channelID;
        std::string token;
        uint32_t localIp;  // in network-byte-order
        MIETransferConfig()
                : transferMode(1),
                  p2pSignalServer("61.155.136.209:9988"),
                  appID(0),
                  userID(0),
                  channelID(""),
                  token(""),
                  localIp(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << p2pSignalServer << transferMode;
            marshal_container(pk, configs);
            pk << appID << userID << channelID << token;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> p2pSignalServer >> transferMode;
            unmarshal_container(up, inserter(configs, configs.begin()));
            up >> appID >> userID >> channelID >> token;
        }
    };

    struct MIEMuteMediaEvent : ljtransfer::mediaSox::Marshallable {
        uint32_t mediaType;
        bool mute;

        MIEMuteMediaEvent()
                : mediaType(0), mute(false) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << mediaType << mute;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> mediaType >> mute;
        }
    };

    struct MIEAudioUploadConfig : ljtransfer::mediaSox::Marshallable {
        uint32_t sampleRate;
        uint32_t channels;
        uint32_t bitsPerSample;
        uint32_t audioBitrateInbps;
        uint32_t audioType;

        MIEAudioUploadConfig()
                : sampleRate(0), channels(0), bitsPerSample(0), audioBitrateInbps(0), audioType(0) {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << sampleRate << channels << bitsPerSample << audioBitrateInbps << audioType;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> sampleRate >> channels >> bitsPerSample >> audioBitrateInbps >> audioType;
        }
    };

    struct MIEVideoUploadConfig : ljtransfer::mediaSox::Marshallable {
        uint32_t encodeWidth;
        uint32_t encodeHeight;
        uint32_t minVideoBitrateInbps;
        uint32_t maxVideoBitrateInbps;
        uint32_t realVideoBitrateInbps;
        uint32_t codecType;
        uint32_t fps;
        uint32_t mirror;
        uint32_t keyFrameInterval;
        uint32_t mode; // 0:BITRATE_MODE_CQ, 1:BITRATE_MODE_VBR, 2:BITRATE_MODE_CBR

        MIEVideoUploadConfig()
                : encodeWidth(0), encodeHeight(0), minVideoBitrateInbps(0), maxVideoBitrateInbps(0),
                  realVideoBitrateInbps(0), codecType(0), fps(0), mirror(0), keyFrameInterval(3), mode(2) {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << encodeWidth << encodeHeight << minVideoBitrateInbps << maxVideoBitrateInbps
               << realVideoBitrateInbps << codecType << fps << mirror << keyFrameInterval << mode;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> encodeWidth >> encodeHeight >> minVideoBitrateInbps >> maxVideoBitrateInbps
               >> realVideoBitrateInbps >> codecType >> fps >> mirror >> keyFrameInterval >> mode;
        }

        std::string toString() {
            std::stringstream stream;
            stream << "VideoUploadConfig encodeWidth " << encodeWidth<< " encodeHeight " << encodeHeight
                    << " minVideoBitrateInbps " << minVideoBitrateInbps << " maxVideoBitrateInbps " << maxVideoBitrateInbps
                    << " realVideoBitrateInbps " << realVideoBitrateInbps << " codecType " << codecType
                    << " fps " << fps << " mirror " << mirror << " keyFrameInterval " << keyFrameInterval << " mode " << mode
            << std::endl;
            return stream.str();
        }
    };

    struct RTCEngineConfig
            : public ljtransfer::mediaSox::Marshallable {
        bool enableLog;

        RTCEngineConfig()
                : enableLog(true) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << enableLog;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> enableLog;
        }
    };

    struct MIEUploadConfig : ljtransfer::mediaSox::Marshallable {
        uint32_t enableAudio;
        uint32_t enableVideo;
        MIEAudioUploadConfig audioUploadConfig;
        MIEVideoUploadConfig videoUploadConfig;
        MIETransferConfig transferConfig;

        MIEUploadConfig() : enableAudio(1), enableVideo(1) {

        }

        void updateConfig(MIEUploadConfig config) {
            enableAudio = config.enableAudio;
            enableVideo = config.enableVideo;
            if (config.audioUploadConfig.sampleRate != 0) {
                audioUploadConfig.audioType = config.audioUploadConfig.audioType;
                audioUploadConfig.sampleRate = config.audioUploadConfig.sampleRate;
                audioUploadConfig.channels = config.audioUploadConfig.channels;
                audioUploadConfig.bitsPerSample = config.audioUploadConfig.bitsPerSample;
                audioUploadConfig.audioBitrateInbps = config.audioUploadConfig.audioBitrateInbps;
            }
            if (config.videoUploadConfig.encodeHeight != 0) {
                videoUploadConfig.encodeHeight = config.videoUploadConfig.encodeHeight;
                videoUploadConfig.encodeWidth = config.videoUploadConfig.encodeWidth;
                videoUploadConfig.codecType = config.videoUploadConfig.codecType;
                videoUploadConfig.maxVideoBitrateInbps = config.videoUploadConfig.maxVideoBitrateInbps;
                videoUploadConfig.realVideoBitrateInbps = config.videoUploadConfig.realVideoBitrateInbps;
                videoUploadConfig.minVideoBitrateInbps = config.videoUploadConfig.minVideoBitrateInbps;
                videoUploadConfig.fps = config.videoUploadConfig.fps;
                videoUploadConfig.mirror = config.videoUploadConfig.mirror;
                videoUploadConfig.keyFrameInterval = config.videoUploadConfig.keyFrameInterval;
                videoUploadConfig.mode = config.videoUploadConfig.mode;
            }

            transferConfig.configs = config.transferConfig.configs;
            transferConfig.transferMode = config.transferConfig.transferMode;
        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << enableAudio << enableVideo << audioUploadConfig << videoUploadConfig
               << transferConfig;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> enableAudio >> enableVideo >> audioUploadConfig >> videoUploadConfig
               >> transferConfig;
        }

    };

    struct WebSocketMessage : ljtransfer::mediaSox::Marshallable {
        std::string cmd;
        std::string msg;

        WebSocketMessage() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << cmd << msg;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> cmd >> msg;
        }
    };

    struct AudioPlayerEvent : ljtransfer::mediaSox::Marshallable {
        bool callbackDecodeData = false; // 是否需要回调音频数据
        bool renderAudioData = false; // 数据是否需要播放，false 则直接静音
        bool directDecode = false; // 收到远端音频直接解码，不需要经过JitterBuffer
        bool directCallback = false; // rudp收到数据包啥也不做，直接返回未解码数据，回调IAudioProcessor
        AudioPlayerEvent() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << callbackDecodeData << renderAudioData << directDecode;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> callbackDecodeData >> renderAudioData >> directDecode;
        }
    };

    enum SoftDecodeInvokeEventType {
        START_SOFT_DECODE_EVENT = 0,
        STOP_SOFT_DECODE_EVENT = 1,
        ON_SOFT_DECODE_SURFACE_CHANGE = 2,
    };


    struct MIESoftDecodeEvent : ljtransfer::mediaSox::Marshallable {
        uint32_t evtType;
        uint32_t subEvtType;

        MIESoftDecodeEvent()
                : evtType(100), subEvtType(START_SOFT_DECODE_EVENT) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << evtType << subEvtType;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> evtType >> subEvtType;
        }
    };

    struct MIESurfaceChange : ljtransfer::mediaSox::Marshallable {
        uint32_t evtType;
        uint32_t subEvtType;
        uint32_t width;
        uint32_t height;
        bool remove;

        MIESurfaceChange()
                : evtType(100), subEvtType(ON_SOFT_DECODE_SURFACE_CHANGE), width(16), height(16),
                  remove(false) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << evtType << subEvtType << width << height << remove;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> evtType >> subEvtType >> width >> height >> remove;
        }
    };


    struct TestSaveVideoFileEvent : ljtransfer::mediaSox::Marshallable {
        bool enable;

        TestSaveVideoFileEvent() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << enable;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> enable;
        }
    };

    struct AudioConfig
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t sampleRate = 48000;

        uint32_t channels = 1;

        uint32_t framePerBuffer = 480;

        uint32_t bitrateInbps = 64000;

        uint32_t audioType = 1;
        uint32_t micVolume = 100;
        uint32_t sourceType = 1;

        uint32_t aacProfile = 2;

        bool needAdts = false;
        bool isHardEncode = false;
        bool callbackDecodeData = false;
        bool renderAudioData = true;
        bool directDecode = false;

        uint32_t captureType = 3;

        AudioConfig() {
        }

        void assign(AudioConfig config) {
            sampleRate = config.sampleRate;
            channels = config.channels;
            framePerBuffer = config.framePerBuffer;
            bitrateInbps = config.bitrateInbps;
            audioType = config.audioType;
            micVolume = config.micVolume;
            sourceType = config.sourceType;
            aacProfile = config.aacProfile;
            needAdts = config.needAdts;
            isHardEncode = config.isHardEncode;
            callbackDecodeData = config.callbackDecodeData;
            renderAudioData = config.renderAudioData;
            directDecode = config.directDecode;
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << sampleRate << channels << framePerBuffer << bitrateInbps << audioType
                << micVolume << sourceType
                << aacProfile << needAdts << isHardEncode << callbackDecodeData << renderAudioData
                << directDecode << captureType;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> sampleRate >> channels >> framePerBuffer >> bitrateInbps >> audioType
                >> micVolume >> sourceType
                >> aacProfile >> needAdts >> isHardEncode >> callbackDecodeData >> renderAudioData
                >> directDecode >> captureType;
        }
    };

    struct CommonStatistic : ljtransfer::mediaSox::Marshallable {
        std::map<uint16_t, uint16_t> delayData;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            marshal_container(pak, delayData);
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            unmarshal_container(pak, inserter(delayData, delayData.begin()));
        }
    };

    struct VideoFrameRateControl : ljtransfer::mediaSox::Marshallable {
        uint32_t frameRate;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << frameRate;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> frameRate;
        }
    };

    struct LinkStatusEvent : ljtransfer::mediaSox::Marshallable {
        uint32_t result;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << result;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> result;
        }
    };

    struct AvailableBands : ljtransfer::mediaSox::Marshallable {
        std::map<uint32_t, uint32_t> m_availableBands;

        AvailableBands() {

        }

        ~AvailableBands() {
            m_availableBands.clear();
        }

        void marshal(ljtransfer::mediaSox::Pack &pak) const {
            marshal_container(pak, m_availableBands);
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            unmarshal_container(pak, inserter(m_availableBands, m_availableBands.begin()));
        }
    };

    struct NetworkQuality : ljtransfer::mediaSox::Marshallable {
        uint8_t m_localQuality;
        uint8_t m_remoteQuality;

        NetworkQuality() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << m_localQuality << m_remoteQuality;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> m_localQuality >> m_remoteQuality;
        }
    };

    struct LeaveChannelEvent : ljtransfer::mediaSox::Marshallable {
        uint8_t leaveChannel = 0;

        LeaveChannelEvent() {

        }

        ~LeaveChannelEvent() {
        }

        void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << leaveChannel;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> leaveChannel;
        }
    };
    enum CaptureType {
        AudioRecorder = 3,
        AAudio = 2,
        openSLES = 1,
    };

    enum AudioProfile {
        /**
         0：默认设置。
         通信场景下，该选项代表指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
         直播场景下，该选项代表指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
         */
        AUDIO_PROFILE_DEFAULT = 0,
        /**
         1：指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
         */
        AUDIO_PROFILE_SPEECH_STANDARD = 1,
        /**
         2：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
         */
        AUDIO_PROFILE_MUSIC_STANDARD = 2,
        /**
         3：指定 48 kHz采样率，音乐编码，双声道，编码码率最大值为 80 Kbps。
         */
        AUDIO_PROFILE_MUSIC_STANDARD_STEREO = 3,

        /**
         * 4：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 96 Kbps。
         */
        AUDIO_PROFILE_MUSIC_HIGH_QUALITY = 4,

        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 128 Kbps。
         */
        AUDIO_PROFILE_MUSIC_HIGH_QUALITY_STEREO = 5,
        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 80 Kbps,拿解码数据，不播放
         */
        AUDIO_PROFILE_CALLBACK_DATA_NORENDER = 6,

        AUDIO_PROFILE_NORENDER_NO_CALLBACK = 7,
    };

    enum AudioScenario {
        /**
         * 0：默认的音频应用场景。
         */
        AUDIO_SCENARIO_DEFAULT = 0,
        /**
         * 1：娱乐场景，适用于用户需要频繁上下麦的场景。
         */
        AUDIO_SCENARIO_CHATROOM_ENTERTAINMENT = 1,
        /**
         * 2：教育场景，适用于需要高流畅度和稳定性的场景。
         */
        AUDIO_SCENARIO_EDUCATION = 2,
        /**
         * 3：高音质语聊房场景，适用于音乐为主的场景。
         */
        AUDIO_SCENARIO_GAME_STREAMING = 3,
        /**
         * 4：秀场场景，适用于需要高音质的单主播场景。
         */
        AUDIO_SCENARIO_SHOWROOM = 4,
        /**
         * 5：游戏开黑场景，适用于只有人声的场景。
         */
        AUDIO_SCENARIO_CHATROOM_GAMING = 5,
        /**
         * 6: IoT（物联网）场景，适用于使用低功耗 IoT 设备的场景。
         */
        AUDIO_SCENARIO_IOT = 6,
        /**
         * 8: 会议场景，适用于人声为主的多人会议。
         */
        AUDIO_SCENARIO_MEETING = 8,
        AUDIO_SCENARIO_NUM = 9,
    };
    enum MixingAction {
        ACTION_START = 0,
        ACTION_STOP = 1,
        ACTION_PAUSE = 2,
        ACTION_RESUME = 3,
        ACTION_SET_POST = 4
        // 0 start 1 stop 2 pause 3 resume 4 set position
    };

    struct AudioEnableEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t evtType;
        bool enabled;


        AudioEnableEvent()
                : evtType((uint32_t)1000), enabled(false) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << evtType << enabled;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> evtType >> enabled;
        }
    };

    struct AudioAdjustEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t evtType;
        uint32_t val;


        AudioAdjustEvent()
                : evtType((uint32_t)1000), val(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << evtType << val;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> evtType >> val;
        }
    };

    struct AudioProfileEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t profile;
        uint32_t scenario;


        AudioProfileEvent()
                : profile(AUDIO_PROFILE_DEFAULT), scenario(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << profile << scenario;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> profile >> scenario;
        }
    };

    struct AudioVolumeIndicationEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t interval;
        uint32_t smooth;
        bool reportVad;

        void assign(AudioVolumeIndicationEvent event) {
            interval = event.interval;
            smooth = event.smooth;
            reportVad = event.reportVad;
        }

        AudioVolumeIndicationEvent()
                : interval(0), smooth(0), reportVad(false) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << interval << smooth << reportVad;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> interval >> smooth >> reportVad;
        }
    };

    class RTCAudioConfiguration {
    public:
        int audioProfile = AUDIO_PROFILE_DEFAULT;
        int audioScenario;

        AudioConfig *createAudioConfig() {
            AudioConfig *audioConfig = new AudioConfig();
            audioConfig->bitrateInbps = 64000;
            audioConfig->framePerBuffer = 480; //
            audioConfig->sampleRate = 48000;
            audioConfig->channels = 1;
            audioConfig->audioType = 1;
            audioConfig->isHardEncode = false;
            audioConfig->needAdts = false;
            if (audioProfile == AUDIO_PROFILE_DEFAULT) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 1;
                audioConfig->bitrateInbps = 80000;
            } else if (audioProfile == AUDIO_PROFILE_MUSIC_STANDARD_STEREO) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 2;
                audioConfig->bitrateInbps = 80000;
            } else if (audioProfile == AUDIO_PROFILE_CALLBACK_DATA_NORENDER) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 2;
                audioConfig->bitrateInbps = 80000;
                audioConfig->callbackDecodeData = true;
                audioConfig->renderAudioData = false;
                audioConfig->directDecode = true;
            }else if (audioProfile == AUDIO_PROFILE_NORENDER_NO_CALLBACK) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 2;
                audioConfig->bitrateInbps = 80000;
                audioConfig->callbackDecodeData = false;
                audioConfig->renderAudioData = false;
                audioConfig->directDecode = false;
            }
            return audioConfig;
        }
    };

    struct EnumerateAudioDevicesEvent : public ljtransfer::mediaSox::Marshallable {
        uint32_t type = 0; // 0 Record devices 1 play devices

        EnumerateAudioDevicesEvent() : type(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type;
        }
    };

    struct AudioDevice : public ljtransfer::mediaSox::Marshallable {
        std::string name;
        uint32_t id;

        AudioDevice() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << name << id;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> name >> id;
        }
    };

    struct AudioDevicesEvent : public ljtransfer::mediaSox::Marshallable {
        std::vector<AudioDevice> devices;

        AudioDevicesEvent() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            marshal_container(pak, devices);

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            unmarshal_container(pak, inserter(devices, devices.begin()));
        }
    };

    struct SetDeviceInfoEvent : public ljtransfer::mediaSox::Marshallable {
        AudioDevice audioDevice;
        uint32_t type;

        SetDeviceInfoEvent() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type << audioDevice;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type >> audioDevice;
        }
    };

    struct MuteDeviceEvent : public ljtransfer::mediaSox::Marshallable {
        bool isMute;
        uint32_t type;

        MuteDeviceEvent() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type << isMute;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type >> isMute;
        }
    };

    struct AudioMixingEvent : public ljtransfer::mediaSox::Marshallable {
        std::string filePath;
        bool loopback;
        uint32_t cycle;
        uint32_t startPos;
        uint32_t acton; // 0 start 1 stop 2 pause 3 resume 4 set position

        AudioMixingEvent() : filePath(""), loopback(false), startPos(0), cycle(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << filePath << loopback << cycle << startPos << acton;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> filePath >> loopback>> cycle >> startPos >> acton;
        }
    };

    struct AudioVolumeEvent : public ljtransfer::mediaSox::Marshallable {
        uint32_t uid;
        uint32_t volume;
        std::string channelId;


        AudioVolumeEvent() : uid(0), volume(0), channelId("") {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << uid << volume << channelId;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> uid >> volume >> channelId;
        }
    };
}
#endif //LJSDK_TRANSCONSTANTS_H

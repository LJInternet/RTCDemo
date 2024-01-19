#include "transfer_common.h"
#include "IEncoder.h"
#ifndef LJSDK_ITRANSPROCESSOR_H
#define LJSDK_ITRANSPROCESSOR_H
namespace LJMediaLibrary {

    struct CommonStatistic;
    struct VideoDecodedData;
    struct VideoHead;
    struct AudioPlayerEvent;
    struct AudioConfig;
    class IAVMgrController {
    public:
        virtual ~IAVMgrController() {};

        virtual void adjustVideoBitrate(int bitrate) = 0;
        virtual void adjustAudioBitrate(int bitrate) = 0;
        virtual void requestKeyFrame(int reason) = 0;
        virtual void onTransConnected() = 0;
        virtual void onCommonStatistics(CommonStatistic statistic) = 0;
    };

    class IMediaAudioController {
    public:
        virtual ~IMediaAudioController() {};

        virtual void updateTransStatus(bool isConnected) = 0;

        virtual void create() = 0;

        virtual void destroy() = 0;

        virtual int HandleMediaEvent(int event_type, const std::string& strData, int len) = 0;

        virtual void adjustEncodeBitrate(int bitrateInbps) = 0;

        virtual char *HandleAudioMediaGetEvent(int eventType, char *mediaData, int len, int * retLen) = 0;

        virtual void initAudioJNIEnv(void *context, bool enable) = 0;
    };


    class MEDIATRANSFER_EXTERN IVideoProcessor {
    public:
        virtual ~IVideoProcessor() {};

        virtual void onDecodedVideoData(VideoDecodedData* data) = 0;

        virtual void onDecodedVideoData(VideoDecodedData* data, uint64_t uid, uint64_t localUid, std::string& channelId) { };

        virtual void onEncodedVideoData(MIEPushEncodedVideoData* data) = 0;

        virtual void relCallDecodeDataCallback(const char* realBuffer, int len, int frameType, const VideoHead& videoHead) = 0;

        virtual void relCallDecodeDataCallback(uint64_t uid, std::string& channelId, const char* realBuffer, int len, int frameType, const VideoHead& videoHead) {};

        virtual void doUploadCallback(int type, const char* buffer, int len) = 0;

        virtual void doUploadCallbackEx(int type, const char* buffer, int len, uint64_t localUid, std::string& channelId){};

        virtual void handleReceiveMetaData(const VideoHead& head) = 0;

        virtual void onCaptureVideo(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt) = 0;

        virtual void onScreenCaptureVideo(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt) {};
    };

    class MEDIATRANSFER_EXTERN IAudioProcessor {
    public:
        virtual ~IAudioProcessor() {};

        virtual int32_t onDecodeData(void* audioData, int size, uint64_t pts, int sampleRate, int channelCont, uint64_t uid) { return 0; };

        virtual int32_t onDecodeDataEx(void* audioData, int size, uint64_t pts, int sampleRate, int channelCont, uint64_t uid, std::string channelId) { return 0; };

        virtual void onDecodeError(int errorCode) {};

        virtual int32_t onEncodedData(void* audioData, int size, uint64_t pts, int sampleRate, int channelCont, bool isHead, int audioType) { return 0; };

        virtual void onEncodeError(int errorCode) {};

        virtual int32_t onCaptureData(void* audioData, int32_t numFrames, int32_t channelCount, int32_t byrePerSample, int32_t sampleRate) { return 0; };

        virtual void onCaptureError(int errorCode) {};

        virtual void onCaptureVolume(int volume) {};

        virtual void onRevAudioData(uint64_t uid, const char *audioData, int len) {};

        virtual int32_t onCaptureMicData(void* audioData, int32_t numFrames, int32_t channelCount, int32_t byrePerSample, int32_t sampleRate) { return 0; };

        virtual int32_t onCaptureSubMixData(void* audioData, int32_t numFrames, int32_t channelCount, int32_t byrePerSample, int32_t sampleRate) { return 0; };
    };

    class MEDIATRANSFER_EXTERN ITransProcessor {
    public:
        virtual ~ITransProcessor() {};

        virtual int pushEncodedAudioData(MIEPushEncodedAudioData data) = 0;

        virtual void handleAudioPlayerEvent(const AudioPlayerEvent& event) = 0;

        virtual void updateAudioConfig(AudioConfig config) = 0;
    };

    class MEDIATRANSFER_EXTERN IChannelExProcessor {
    public:
        virtual ~IChannelExProcessor() {};
        virtual int pushEncodedAudioData(MIEPushEncodedAudioData& data) { return 0; };
        virtual int pushEncodedVideoData(MIEPushEncodedVideoData& encodedVideoData) { return 0; };

    };
}
#endif //LJSDK_ITRANSPROCESSOR_H
//
// Created by Administrator on 2022/6/14.
//

#ifndef LJSDK_AVTRANSMGR_H
#define LJSDK_AVTRANSMGR_H

#include <cstdio>
#include <atomic>
#include <list>
#include "TransConstants.h"

#include "CameraCapture.h"

#include "IProcessor.h"
#include "IMP4Muxer.h"

namespace LJMediaLibrary {

    class ITransApi;
    class MTransEngine;
    class VideoEncodeEngine;
    class AudioEncodeEngine;
    class MultiChannelMgr;
    /*
    struct MIETransferConfig;
    struct MIEUploadConfig;
    struct MIEMuteMediaEvent;
    struct WebSocketMessage;*/

    enum TransferMode {
        PUSH_MODE = 1, // 推流
        PULL_MODE = 2, // 拉流
    };


    class MEDIATRANSFER_EXTERN AVTransMgr :public IAVMgrController, public ITransProcessor, public IVideoCaptureCallback {

    public:
        AVTransMgr(IVideoProcessor* videoProcessor, IAudioProcessor* audioProcessor);

        ~AVTransMgr();

        bool Start(MIETransferConfig config);
        bool Stop();

        //pixel_fmt: RGBA=1 YUV420=2 NV21=3 RGB = 4
        int pushYUVData(const char *buf, int size, int width, int height, int rotation, uint64_t timeStamp, int pixel_fmt);

        int pushYUVData(const char *buf, int size, const char *msg, int msgSize, int pixel_fmt);

        int pushPCMData(const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample);

        int pushEncodedVideoData(MIEPushEncodedVideoData encodedVideoData);

        int pushEncodedH264Data(uint8_t* buf, int32_t len);

        int handlerSofDecodeEvent(int eventType, const std::string& dataStr, void* pJobject);

        int pushEncodedAudioData(MIEPushEncodedAudioData data) override;

        void setUploadConfig(const MIEUploadConfig& config);

        void muteMediaEvent(const MIEMuteMediaEvent& event);

        void handleWebMessage(const WebSocketMessage& message);

        void handleAudioPlayerEvent(const AudioPlayerEvent& event) override;

        int HandleAudioMediaEvent(int event_type, std::string strData, int len);

        void updateAudioConfig(AudioConfig config) override;

        void startVideoCapture(char* deviceCode, int width, int height, int fps);
        void startVideoCaptureWithConfig(char* deviceCode, const char* captureConfig, int length);
        void stopVideoCapture();

        void enableSaveEncodeH264(bool saveH264);
        void saveH264File(MIEPushEncodedVideoData* data);

        char *HandleAudioMediaGetEvent(int eventType, char *mediaData, int len, int * retLen);

        void handlerDecodeStatistics(std::map<uint16_t, uint16_t> delayData);
#if defined(ANDROID) || defined(WIN32)|| defined(__APPLE__)
        void createAudioManager();

        void destroyAudioManager();

        void initAudioJNIEnv(void* context, bool enable);
#endif

        void sendVideoTexture(int32_t textureId, int32_t width, int32_t height, int32_t rotation,
                              int32_t format);

        void requestKeyFrame(int reason) override;

        void sendRemoteBm(int bm);

        void requestRemoteKeyFrame();

        void
        pushAudioFrameEx(const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample, const char *key);

        int HandleMultiChannelEvent(int event_type, std::string& mediaData, int len);

        void subscribeAVStream(std::string& channelId, uint64_t localUid, uint64_t subscriberUid, bool isVideo);
        void unsubscribeAVStream(std::string& channelId, uint64_t localUid, uint64_t subscriberUid, bool isVideo);

        void
        pushVideoFrameEx(const char *buf, int size, const char *msg, int msgSize, int pixel_fmt,
                         const char *key);

        void pushEncodedData(int width, int height, int frameType, int pts, int  codecType,
                             uint8_t* buf, int32_t len, std::map<uint64_t, uint64_t> iTsInfos);

        void pushEVD2MultiChannel(MIEPushEncodedVideoData& data);

    private:

        MIEUploadConfig* m_uploadConfig = nullptr;
        ITransApi * transApi = nullptr;
        MTransEngine* m_transEngine = nullptr;

        CameraCapture* m_videoCapture = nullptr;
        IVideoProcessor* m_videoProcessor = nullptr;
        IAudioProcessor* m_audioProcessor = nullptr;

        VideoEncodeEngine* m_videoEncodeEngine = nullptr;
        AudioEncodeEngine* m_audioEncodeEngine = nullptr;

        IMediaAudioController* audioManager = nullptr;
        MultiChannelMgr* m_multiChannelMgr = nullptr;

        std::atomic<bool> m_connected = { false };
        std::atomic<bool> m_setRudpConnected = { false };

        int m_lastTargetFrameRate = 0;

        IMp4Muxer *m_mp4Muxer = nullptr;

        void adjustVideoBitrate(int bitrate) override;
        void adjustAudioBitrate(int bitrate) override;

        void onTransConnected() override;

        void onCommonStatistics(CommonStatistic statistic) override;

        void onNewFrame(unsigned char* data, int len, int width, int height, int rotation, uint64_t timeStamp, int pixel_fmt) override;
    };
}

#endif //LJSDK_AVTRANSMGR_H

/**
 * @file AVTransMgr.h
 * @brief Header file for the AVTransMgr class.
 * @author Administrator
 * @date 2022/6/14
 */

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
    class FrameRateController;

    /**
     * @brief Enumerates the transfer mode.
     */
    enum TransferMode {
        PUSH_MODE = 1, ///< Push mode for media transfer.
        PULL_MODE = 2  ///< Pull mode for media transfer.
    };

    /**
     * @brief Class for managing audio and video transfer.
     */
    class MEDIATRANSFER_EXTERN AVTransMgr : public IAVMgrController, public ITransProcessor, public IVideoCaptureCallback {

    public:
        /**
         * @brief Constructor for AVTransMgr.
         *
         * @param videoProcessor Pointer to the video processor.
         * @param audioProcessor Pointer to the audio processor.
         */
        AVTransMgr(IVideoProcessor* videoProcessor, IAudioProcessor* audioProcessor);

        /**
         * @brief Destructor for AVTransMgr.
         */
        ~AVTransMgr();

        /**
         * @brief Starts the media transfer.
         *
         * @param config Configuration for the media transfer.
         * @return True if the operation is successful, false otherwise.
         */
        bool Start(MIETransferConfig config);

        /**
         * @brief Stops the media transfer.
         *
         * @return True if the operation is successful, false otherwise.
         */
        bool Stop();

        /**
         * @brief Pushes YUV video data for processing.
         *
         * @param buf Buffer containing YUV video data.
         * @param size Size of the YUV video data buffer.
         * @param width Width of the video frame.
         * @param height Height of the video frame.
         * @param rotation Rotation angle of the video frame.
         * @param timeStamp Timestamp of the video frame.
         * @param pixel_fmt Pixel format of the video frame.
         * @return Result code indicating success or failure.
         */
        int pushYUVData(const char* buf, int size, int width, int height, int rotation, uint64_t timeStamp, int pixel_fmt);

        /**
         * @brief Pushes YUV video data for processing with additional message.
         *
         * @param buf Buffer containing YUV video data.
         * @param size Size of the YUV video data buffer.
         * @param msg Additional message to be processed.
         * @param msgSize Size of the additional message buffer.
         * @param pixel_fmt Pixel format of the video frame.
         * @return Result code indicating success or failure.
         */
        int pushYUVData(const char* buf, int size, const char* msg, int msgSize, int pixel_fmt);

        /**
         * @brief Pushes PCM audio data for processing.
         *
         * @param pcm Pointer to the PCM audio data.
         * @param frame_num Number of audio frames.
         * @param sampleRate Sample rate of the audio data.
         * @param channelCount Number of audio channels.
         * @param bytePerSample Number of bytes per audio sample.
         * @return Result code indicating success or failure.
         */
        int pushPCMData(const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample, uint64_t timestamp);

        /**
         * @brief Pushes encoded video data for processing.
         *
         * @param encodedVideoData Encoded video data to be processed.
         * @return Result code indicating success or failure.
         */
        int pushEncodedVideoData(MIEPushEncodedVideoData encodedVideoData);

        /**
         * @brief Pushes encoded H.264 video data for processing.
         *
         * @param buf Buffer containing encoded H.264 video data.
         * @param len Length of the encoded H.264 video data buffer.
         * @return Result code indicating success or failure.
         */
        int pushEncodedH264Data(uint8_t* buf, int32_t len);

        /**
         * @brief Handles events related to software decoding.
         *
         * @param eventType Type of the decode event.
         * @param dataStr Data associated with the decode event.
         * @param pJobject Pointer to the Java object.
         * @return Result code indicating success or failure.
         */
        int handlerSofDecodeEvent(int eventType, const std::string& dataStr, void* pJobject);

        /**
         * @brief Pushes encoded audio data for processing.
         *
         * @param data Encoded audio data to be processed.
         * @return Result code indicating success or failure.
         */
        int pushEncodedAudioData(MIEPushEncodedAudioData data) override;

        /**
         * @brief Sets the upload configuration.
         *
         * @param config Upload configuration to be set.
         */
        void setUploadConfig(const MIEUploadConfig& config);

        /**
         * @brief Mutes media event.
         *
         * @param event Media event to be muted.
         */
        void muteMediaEvent(const MIEMuteMediaEvent& event);

        /**
         * @brief Handles WebSocket messages.
         *
         * @param message WebSocket message to be handled.
         */
        void handleWebMessage(const WebSocketMessage& message);

        /**
         * @brief Handles audio player events.
         *
         * @param event Audio player event to be handled.
         */
        void handleAudioPlayerEvent(const AudioPlayerEvent& event) override;

        /**
         * @brief Handles audio media event.
         *
         * @param event_type Type of the audio media event.
         * @param strData Data associated with the audio media event.
         * @param len Length of the data.
         * @return Result code indicating success or failure.
         */
        int HandleAudioMediaEvent(int event_type, std::string strData, int len);

        /**
         * @brief Updates the audio configuration.
         *
         * @param config New audio configuration to be applied.
         */
        void updateAudioConfig(AudioConfig config) override;

        /**
         * @brief Starts video capture.
         *
         * @param deviceCode Code of the video capture device.
         * @param width Width of the video frame.
         * @param height Height of the video frame.
         * @param fps Frames per second of the video.
         */
        void startVideoCapture(char* deviceCode, int width, int height, int fps);

        /**
         * @brief Starts video capture with configuration.
         *
         * @param deviceCode Code of the video capture device.
         * @param captureConfig Configuration for video capture.
         * @param length Length of the configuration data.
         */
        void startVideoCaptureWithConfig(char* deviceCode, const char* captureConfig, int length);
        /**
         * @brief 停止视频捕获。
         */
        void stopVideoCapture();

        /**
         * @brief 启用保存编码的H.264数据。
         *
         * @param saveH264 是否保存H.264数据。
         */
        void enableSaveEncodeH264(bool saveH264);

        /**
         * @brief 保存H.264文件。
         *
         * @param data 指向MIEPushEncodedVideoData的指针。
         */
        void saveH264File(MIEPushEncodedVideoData* data);

        /**
         * @brief 处理音频媒体获取事件。
         *
         * @param eventType 事件类型。
         * @param mediaData 媒体数据。
         * @param len 数据长度。
         * @param retLen 返回的数据长度。
         * @return 处理结果。
         */
        char *HandleAudioMediaGetEvent(int eventType, char *mediaData, int len, int * retLen);

        /**
         * @brief 处理解码统计信息。
         *
         * @param delayData 延迟数据映射。
         */
        void handlerDecodeStatistics(std::map<uint16_t, uint16_t> delayData);

#if defined(ANDROID) || defined(WIN32)|| defined(__APPLE__)
        /**
         * @brief 创建音频管理器。
         */
        void createAudioManager();

        /**
         * @brief 销毁音频管理器。
         */
        void destroyAudioManager();

        /**
         * @brief 初始化音频JNIEnv。
         *
         * @param context 上下文。
         * @param enable 是否启用。
         */
        void initAudioJNIEnv(void* context, bool enable);
#endif

        /**
         * @brief 发送视频纹理。
         *
         * @param textureId 纹理ID。
         * @param width 视频宽度。
         * @param height 视频高度。
         * @param rotation 旋转角度。
         * @param format 格式。
         */
        void sendVideoTexture(int32_t textureId, int32_t width, int32_t height, int32_t rotation,
                              int32_t format);

        /**
         * @brief 请求关键帧。
         *
         * @param reason 请求关键帧的原因。
         */
        void requestKeyFrame(int reason) override;

        /**
         * @brief 发送远端的可用单宽。
         *
         * @param bm 可用单宽。
         */
        void sendRemoteBm(int bm);

        /**
         * @brief 请求远程关键帧。
         */
        void requestRemoteKeyFrame();

        /**
         * @brief 请求远程关键帧（扩展）。
         *
         * @param uid 用户ID。
         */
        void requestRemoteKeyFrameEx(uint64_t uid);

        /**
         * @brief 推送音频帧（扩展）。
         *
         * @param pcm PCM音频数据。
         * @param frame_num 帧数。
         * @param sampleRate 采样率。
         * @param channelCount 声道数。
         * @param bytePerSample 每个采样的字节数。
         * @param key
         */
        void
        pushAudioFrameEx(const int8_t *pcm, int frame_num, int sampleRate, int channelCount, int bytePerSample, const char *key);

        /**
         * @brief 处理多通道事件。
         *
         * @param event_type 事件类型。
         * @param mediaData 媒体数据。
         * @param len 数据长度。
         * @return 处理结果。
         */
        int HandleMultiChannelEvent(int event_type, std::string& mediaData, int len);

        /**
         * @brief 订阅音视频流。
         *
         * @param channelId 频道ID。
         * @param localUid 本地用户ID。
         * @param subscriberUid 订阅者ID。
         * @param isVideo 是否为视频流。
         */
        void subscribeAVStream(std::string& channelId, uint64_t localUid, uint64_t subscriberUid, bool isVideo);

        /**
         * @brief 取消订阅音视频流。
         *
         * @param channelId 频道ID。
         * @param localUid 本地用户ID。
         * @param subscriberUid 订阅者ID。
         * @param isVideo 是否为视频流。
         */
        void unsubscribeAVStream(std::string& channelId, uint64_t localUid, uint64_t subscriberUid, bool isVideo);

        /**
         * @brief 推送视频帧（扩展）。
         *
         * @param buf 视频帧缓冲区。
         * @param size 缓冲区大小。
         * @param msg 附加消息。
         * @param msgSize 消息大小。
         * @param pixel_fmt 像素格式。
         * @param key
         */
        void pushVideoFrameEx(const char *buf, int size, const char *msg, int msgSize, int pixel_fmt,
                              const char *key);

        /**
         * @brief 推送编码数据。
         *
         * @param width 视频宽度。
         * @param height 视频高度。
         * @param frameType 帧类型。
         * @param pts 时间戳。
         * @param codecType 编解码器类型。
         * @param buf 数据缓冲区。
         * @param len 缓冲区长度。
         * @param iTsInfos 时间戳信息。
         */
        void pushEncodedData(int width, int height, int frameType, int pts, int  codecType,
                             uint8_t* buf, int32_t len, std::map<uint64_t, uint64_t> iTsInfos);

        /**
         * @brief 推送EVD到多通道。
         *
         * @param data MIEPushEncodedVideoData数据。
         */
        void pushEVD2MultiChannel(MIEPushEncodedVideoData& data);

        void cacheDecodeStatistic(std::map<uint16_t, uint16_t> statisticMap);
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

        FrameRateController* m_frameRateController = nullptr;

        IMp4Muxer *m_mp4Muxer = nullptr;

        std::atomic<bool> m_started = { false };

        void adjustVideoBitrate(int bitrate) override;
        void adjustAudioBitrate(int bitrate) override;

        void onTransConnected() override;

        void onCommonStatistics(CommonStatistic statistic) override;

        void onNewFrame(unsigned char* data, int len, int width, int height, int rotation, uint64_t timeStamp, int pixel_fmt) override;
        void startVideoEncoderIfNeed();
    };
}

#endif //LJSDK_AVTRANSMGR_H

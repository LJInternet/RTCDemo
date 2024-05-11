#include "transfer_common.h"
#include "IEncoder.h"

#ifndef LJSDK_ITRANSPROCESSOR_H
#define LJSDK_ITRANSPROCESSOR_H

namespace LJMediaLibrary {

    struct CommonStatistic;
    struct VideoDecodedData;
    struct VideoHead;
    struct AudioHead;
    struct AudioPlayerEvent;
    struct AudioConfig;

    /**
     * @brief Interface class for managing audio and video transmission.
     */
    class IAVMgrController {
    public:
        virtual ~IAVMgrController() {};

        /**
         * @brief Adjust the bitrate for video transmission.
         * 
         * @param bitrate The new bitrate value.
         */
        virtual void adjustVideoBitrate(int bitrate) = 0;

        /**
         * @brief Adjust the bitrate for audio transmission.
         * 
         * @param bitrate The new bitrate value.
         */
        virtual void adjustAudioBitrate(int bitrate) = 0;

        /**
         * @brief Request a key frame for video transmission.
         * 
         * @param reason The reason for requesting the key frame.
         */
        virtual void requestKeyFrame(int reason) = 0;

        /**
         * @brief Callback when the transmission is connected.
         */
        virtual void onTransConnected() = 0;

        /**
         * @brief Callback for receiving common statistics data.
         * 
         * @param statistic The common statistics data.
         */
        virtual void onCommonStatistics(CommonStatistic statistic) = 0;
    };

    /**
     * @brief Interface class for managing audio functionalities.
     */
    class IMediaAudioController {
    public:
        virtual ~IMediaAudioController() {};

        /**
         * @brief Update the transmission status.
         * 
         * @param isConnected Indicates if the connection is established.
         */
        virtual void updateTransStatus(bool isConnected) = 0;

        // Other virtual methods...
    };


	/**
	 * @brief Interface class for processing video data.
	 */
	class MEDIATRANSFER_EXTERN IVideoProcessor {
	public:
		virtual ~IVideoProcessor() {};

		/**
		 * @brief Callback for receiving decoded video data.
		 * 
		 * @param data Pointer to the decoded video data.
		 */
		virtual void onDecodedVideoData(VideoDecodedData* data) = 0;

		/**
		 * @brief Overloaded callback for receiving decoded video data with additional information.
		 * 
		 * @param data Pointer to the decoded video data.
		 * @param uid User ID associated with the video data.
		 * @param localUid Local user ID associated with the video data.
		 * @param channelId ID of the channel associated with the video data.
		 */
		virtual void onDecodedVideoData(VideoDecodedData* data, uint64_t uid, uint64_t localUid, std::string& channelId) { };

		/**
		 * @brief Callback for receiving encoded video data.
		 * 
		 * @param data Pointer to the encoded video data.
		 */
		virtual void onEncodedVideoData(MIEPushEncodedVideoData* data) = 0;

		/**
		 * @brief Callback for handling decoded data.
		 * 
		 * @param realBuffer Pointer to the buffer containing the decoded data.
		 * @param len Length of the buffer.
		 * @param frameType Type of the video frame.
		 * @param videoHead Video header information.
		 */
		virtual void relCallDecodeDataCallback(const char* realBuffer, int len, int frameType, const VideoHead& videoHead) = 0;

		/**
		 * @brief Overloaded callback for handling decoded data with additional information.
		 * 
		 * @param uid User ID associated with the decoded data.
		 * @param localUid Local user ID associated with the decoded data.
		 * @param channelId ID of the channel associated with the decoded data.
		 * @param realBuffer Pointer to the buffer containing the decoded data.
		 * @param len Length of the buffer.
		 * @param frameType Type of the video frame.
		 * @param videoHead Video header information.
		 */
		virtual void relCallDecodeDataCallback(uint64_t uid, uint64_t localUid, std::string& channelId, const char* realBuffer, int len, int frameType, const VideoHead& videoHead) {};

		/**
		 * @brief Callback for uploading data.
		 * 
		 * @param type Type of upload.
		 * @param buffer Pointer to the data buffer.
		 * @param len Length of the data buffer.
		 */
		virtual void doUploadCallback(int type, const char* buffer, int len) = 0;

		/**
		 * @brief Overloaded callback for uploading data with additional information.
		 * 
		 * @param type Type of upload.
		 * @param buffer Pointer to the data buffer.
		 * @param len Length of the data buffer.
		 * @param localUid Local user ID associated with the data.
		 * @param channelId ID of the channel associated with the data.
		 */
		virtual void doUploadCallbackEx(int type, const char* buffer, int len, uint64_t localUid, std::string& channelId){};

		/**
		 * @brief Callback for handling received metadata.
		 * 
		 * @param head Metadata information.
		 */
		virtual void handleReceiveMetaData(const VideoHead& head) = 0;

		/**
		 * @brief Callback for capturing video.
		 * 
		 * @param buf Pointer to the video buffer.
		 * @param len Length of the video buffer.
		 * @param width Width of the video frame.
		 * @param height Height of the video frame.
		 * @param pixel_fmt Pixel format of the video frame.
		 */
		virtual void onCaptureVideo(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt) = 0;

		/**
		 * @brief Callback for capturing screen video.
		 * 
		 * @param buf Pointer to the video buffer.
		 * @param len Length of the video buffer.
		 * @param width Width of the video frame.
		 * @param height Height of the video frame.
		 * @param pixel_fmt Pixel format of the video frame.
		 */
		virtual void onScreenCaptureVideo(uint8_t* buf, int32_t len, int32_t width, int32_t height, int pixel_fmt) {};
	};

/**
 * @brief Interface class for processing audio data.
 */
	class MEDIATRANSFER_EXTERN IAudioProcessor {
	public:
		virtual ~IAudioProcessor() {};

		/**
		 * @brief Callback for receiving decoded audio data.
		 * 
		 * @param audioData Pointer to the decoded audio data.
		 * @param size Size of the audio data.
		 * @param pts Presentation timestamp of the audio data.
		 * @param sampleRate Sample rate of the audio data.
		 * @param channelCont Number of audio channels.
		 * @param uid User ID associated with the audio data.
		 * @return int32_t Return value indicating success or failure.
		 */
		virtual int32_t onDecodeData(void* audioData, int size, uint64_t pts, int sampleRate, int channelCont, uint64_t uid) { return 0; };

		/**
		 * @brief Overloaded callback for receiving decoded audio data with additional information.
		 * 
		 * @param audioData Pointer to the decoded audio data.
		 * @param size Size of the audio data.
		 * @param pts Presentation timestamp of the audio data.
		 * @param sampleRate Sample rate of the audio data.
		 * @param channelCont Number of audio channels.
		 * @param uid User ID associated with the audio data.
		 * @param channelId ID of the channel associated with the audio data.
		 * @return int32_t Return value indicating success or failure.
		 */
		virtual int32_t onDecodeDataEx(void* audioData, int size, uint64_t pts, int sampleRate, int channelCont, uint64_t uid, std::string channelId) { return 0; };

		/**
		 * @brief Callback for handling decode errors.
		 * 
		 * @param errorCode Error code indicating the type of error.
		 */
		virtual void onDecodeError(int errorCode) {};

		/**
		 * @brief Callback for receiving encoded audio data.
		 * 
		 * @param audioData Pointer to the encoded audio data.
		 * @param size Size of the audio data.
		 * @param pts Presentation timestamp of the audio data.
		 * @param sampleRate Sample rate of the audio data.
		 * @param channelCont Number of audio channels.
		 * @param isHead Indicates if the data contains audio head information.
		 * @param audioType Type of the audio data.
		 * @return int32_t Return value indicating success or failure.
		 */
		virtual int32_t onEncodedData(void* audioData, int size, uint64_t pts, int sampleRate, int channelCont, bool isHead, int audioType) { return 0; };

		/**
		 * @brief Callback for handling encode errors.
		 * 
		 * @param errorCode Error code indicating the type of error.
		 */
		virtual void onEncodeError(int errorCode) {};

		/**
		 * @brief Callback for receiving captured audio data.
		 * 
		 * @param audioData Pointer to the captured audio data.
		 * @param numFrames Number of audio frames.
		 * @param channelCount Number of audio channels.
		 * @param byrePerSample Bytes per audio sample.
		 * @param sampleRate Sample rate of the audio data.
		 * @return int32_t Return value indicating success or failure.
		 */
		virtual int32_t onCaptureData(void* audioData, int32_t numFrames, int32_t channelCount, int32_t byrePerSample, int32_t sampleRate) { return 0; };

		/**
		 * @brief Callback for handling capture errors.
		 * 
		 * @param errorCode Error code indicating the type of error.
		 */
		virtual void onCaptureError(int errorCode) {};

		/**
		 * @brief Callback for receiving captured volume information.
		 * 
		 * @param volume Captured volume level.
		 */
		virtual void onCaptureVolume(int volume) {};

		/**
		 * @brief Callback for receiving reversed audio data.
		 * 
		 * @param uid User ID associated with the audio data.
		 * @param audioHead Audio header information.
		 * @param audioData Pointer to the reversed audio data.
		 * @param len Length of the audio data.
		 */
		virtual void onRevAudioData(uint64_t uid, AudioHead& audioHead, const char *audioData, int len) {};

		/**
		 * @brief Callback for receiving captured microphone audio data.
		 * 
		 * @param audioData Pointer to the captured microphone audio data.
		 * @param numFrames Number of audio frames.
		 * @param channelCount Number of audio channels.
		 * @param byrePerSample Bytes per audio sample.
		 * @param sampleRate Sample rate of the audio data.
		 * @return int32_t Return value indicating success or failure.
		 */
		virtual int32_t onCaptureMicData(void* audioData, int32_t numFrames, int32_t channelCount, int32_t byrePerSample, int32_t sampleRate) { return 0; };

		/**
		 * @brief Callback for receiving captured sub-mix audio data.
		 * 
		 * @param audioData Pointer to the captured sub-mix audio data.
		 * @param numFrames Number of audio frames.
		 * @param channelCount Number of audio channels.
		 * @param byrePerSample Bytes per audio sample.
		 * @param sampleRate Sample rate of the audio data.
		 * @return int32_t Return value indicating success or failure.
		 */
		virtual int32_t onCaptureSubMixData(void* audioData, int32_t numFrames, int32_t channelCount, int32_t byrePerSample, int32_t sampleRate) { return 0; };
	};

	/**
	 * @brief Interface class for processing audio data and events related to media transfer.
	 */
	class MEDIATRANSFER_EXTERN ITransProcessor {
	public:
		virtual ~ITransProcessor() {};

		/**
		 * @brief Push encoded audio data for processing.
		 * 
		 * @param data Encoded audio data to be processed.
		 * @return int Return value indicating success or failure.
		 */
		virtual int pushEncodedAudioData(MIEPushEncodedAudioData data) = 0;

		/**
		 * @brief Handle audio player events.
		 * 
		 * @param event Audio player event to be handled.
		 */
		virtual void handleAudioPlayerEvent(const AudioPlayerEvent& event) = 0;

		/**
		 * @brief Update audio configuration.
		 * 
		 * @param config New audio configuration to be applied.
		 */
		virtual void updateAudioConfig(AudioConfig config) = 0;
	};

	/**
	 * @brief Interface class for processing audio and video data for media transfer.
	 */
	class MEDIATRANSFER_EXTERN IChannelExProcessor {
	public:
		virtual ~IChannelExProcessor() {};

		/**
		 * @brief Push encoded audio data for processing.
		 * 
		 * @param data Encoded audio data to be processed.
		 * @return int Return value indicating success or failure.
		 */
		virtual int pushEncodedAudioData(MIEPushEncodedAudioData& data) { return 0; };

		/**
		 * @brief Push encoded video data for processing.
		 * 
		 * @param encodedVideoData Encoded video data to be processed.
		 * @return int Return value indicating success or failure.
		 */
		virtual int pushEncodedVideoData(MIEPushEncodedVideoData& encodedVideoData) { return 0; };
	};
}
#endif //LJSDK_ITRANSPROCESSOR_H
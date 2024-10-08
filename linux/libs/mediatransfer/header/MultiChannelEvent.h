//
// Created by Administrator on 2023/5/17.
//

#ifndef LJSDK_MULTICHANNELEVENT_H
#define LJSDK_MULTICHANNELEVENT_H
#include <MediaConstants.h>
#include <LJPacket.h>
#include <log.h>
namespace LJMediaLibrary {
/// <summary>
/// The channel profile.
/// </summary>
///
    enum CHANNEL_PROFILE_TYPE {
        ///
        /// <summary>
        /// 0: Communication. Use this profile when there are only two users in the channel.
        /// </summary>
        ///
        CHANNEL_PROFILE_COMMUNICATION = 0,

        ///
        /// <summary>
        /// 1: Live streaming. Live streaming. Use this profile when there are more than two users in the channel.
        /// </summary>
        ///
        CHANNEL_PROFILE_LIVE_BROADCASTING = 1,


        ///
        /// <summary>
        /// 2: Gaming. This profile is deprecated.
        /// </summary>
        ///
        CHANNEL_PROFILE_GAME = 2,


        ///
        /// <summary>
        /// Cloud gaming. The scenario is optimized for latency. Use this profile if the use case requires frequent interactions between users.
        /// </summary>
        ///
        CHANNEL_PROFILE_CLOUD_GAMING = 3,


        ///
        /// @ignore
        ///
        CHANNEL_PROFILE_COMMUNICATION_1v1 = 4,
    };

    enum CLIENT_ROLE_TYPE {
        ///
        /// <summary>
        /// 1: Host. A host can both send and receive streams.
        /// </summary>
        ///
        CLIENT_ROLE_BROADCASTER = 1,

        ///
        /// <summary>
        /// 2: (Default) Audience. An audience member can only receive streams.
        /// </summary>
        ///
        CLIENT_ROLE_AUDIENCE = 2,
    };

    typedef struct AudioFrameEx {
        int sampleRate;
        int channelCount;
        int bytePerSample;
        int uid;
        int frame_num;
        const char *channelId;
        const int8_t *pcm;
    } AudioFrameEx;

    struct MultiChannelEvent : ljtransfer::mediaSox::Marshallable {
        std::string channelId;
        uint64_t uid = 0;
        std::string key;

//        MultiChannelEvent(std::string& _channelId, uint64_t _uid) {
//            channelId = _channelId;
//            uid = _uid;
//            key = std::string(channelId.c_str()).append(std::to_string(uid));
//        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> channelId >> uid;
            key = std::string(channelId.c_str()).append(std::to_string(uid));
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << channelId << uid;
        }
    };

    struct MultiChannelEnableEvent : MultiChannelEvent {
        bool _enable = false;

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> _enable;
            MultiChannelEvent::unmarshal(up);
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << _enable;
            MultiChannelEvent::marshal(pk);
        }
    };

    struct SubscriberStreamEvent : MultiChannelEvent {
        uint64_t subscriberUid;
        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> subscriberUid;
            MultiChannelEvent::unmarshal(up);
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << subscriberUid;
            MultiChannelEvent::marshal(pk);
        }
    };

    struct ChannelMediaOptions : ljtransfer::mediaSox::Marshallable {
        bool publishCameraTrack = false;
        bool publishSecondaryCameraTrack = false;
        bool publishMicrophoneTrack = false;
        bool publishScreenCaptureVideo = false;
        bool publishScreenCaptureAudio = false;
        bool publishScreenTrack = false;
        bool publishSecondaryScreenTrack = false;
        bool publishCustomAudioTrack = false;
        uint32_t publishCustomAudioSourceId = -1;
        bool publishCustomAudioTrackEnableAec = false;
        bool publishDirectCustomAudioTrack = false;
        bool publishCustomAudioTrackAec = false;
        bool publishCustomVideoTrack = false;
        bool publishEncodedVideoTrack = false;
        bool publishMediaPlayerAudioTrack = false;
        bool publishMediaPlayerVideoTrack = false;
        bool publishTrancodedVideoTrack = false;
        bool autoSubscribeAudio = false;
        bool autoSubscribeVideo = false;
        bool enableAudioRecordingOrPlayout = false;
        uint32_t publishMediaPlayerId = 0;
        uint32_t clientRoleType = 1;
        uint32_t channelProfile = 0;
        uint32_t dataWorkMode = 0;
        bool enableBuiltInMediaEncryption = false;
        bool publishRhythmPlayerTrack = false;
        bool isInteractiveAudience = false;
        bool disableSoftDecode = false;

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> publishCameraTrack >> publishSecondaryCameraTrack >> publishMicrophoneTrack
               >> publishScreenCaptureVideo
               >> publishScreenCaptureAudio >> publishScreenTrack >> publishSecondaryScreenTrack
               >> publishCustomAudioTrack
               >> enableBuiltInMediaEncryption >> publishRhythmPlayerTrack >> isInteractiveAudience
               >> disableSoftDecode
               >> publishCustomAudioTrackEnableAec >> publishDirectCustomAudioTrack
               >> publishCustomAudioTrackAec
               >> publishCustomVideoTrack >> publishEncodedVideoTrack
               >> publishMediaPlayerAudioTrack >> publishMediaPlayerVideoTrack
               >> publishTrancodedVideoTrack >> autoSubscribeAudio >> autoSubscribeVideo
               >> enableAudioRecordingOrPlayout
               >> publishMediaPlayerId >> clientRoleType >> channelProfile >> dataWorkMode
               >> publishCustomAudioSourceId;
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << publishCameraTrack << publishSecondaryCameraTrack << publishMicrophoneTrack
               << publishScreenCaptureVideo
               << publishScreenCaptureAudio << publishScreenTrack << publishSecondaryScreenTrack
               << publishCustomAudioTrack
               << enableBuiltInMediaEncryption << publishRhythmPlayerTrack << isInteractiveAudience
               << disableSoftDecode
               << publishCustomAudioTrackEnableAec << publishDirectCustomAudioTrack
               << publishCustomAudioTrackAec
               << publishCustomVideoTrack << publishEncodedVideoTrack
               << publishMediaPlayerAudioTrack << publishMediaPlayerVideoTrack
               << publishTrancodedVideoTrack << autoSubscribeAudio << autoSubscribeVideo
               << enableAudioRecordingOrPlayout
               << publishMediaPlayerId << clientRoleType << channelProfile << dataWorkMode
               << publishCustomAudioSourceId;
        }
    };

    struct JoinChannelExConfig : MultiChannelEvent {
        ChannelMediaOptions _option;
        std::string _token;
        uint64_t appId = 0;
        bool isDebug = true;

        JoinChannelExConfig() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << _option << _token << appId << isDebug;
            MultiChannelEvent::marshal(pak);
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> _option >> _token >> appId >> isDebug;
            MultiChannelEvent::unmarshal(up);
        }
    };


    struct MultiChannelEventResult : ljtransfer::mediaSox::Marshallable {
        uint32_t result;
        uint64_t uid;
        std::string channelId;
        std::string msg;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << result << uid << msg << channelId;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> result >> uid >> msg >> channelId;
        }
    };
}
#endif //LJSDK_MULTICHANNELEVENT_H

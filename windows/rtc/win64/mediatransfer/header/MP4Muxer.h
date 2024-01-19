//
// Created by Administrator on 2023/2/27.
//

#ifndef LJSDK_MP4MUXER_H
#define LJSDK_MP4MUXER_H
#include "IMP4Muxer.h"
#include <atomic>
namespace LJMediaLibrary {
    class Mp4Muxer : public IMp4Muxer {
    public:
        Mp4Muxer();

        ~Mp4Muxer();

        int Open(char *fileName) override;

        int EnableVideo() override;

        int EnableAudio() override;

        int WriteVideoPacket(unsigned char *pData, int frameType, int width, int height, int nLen,
                             long long int pts) override;

        int WriteAudioPacket(unsigned char *pData, int nLen, int nSampleRate, int nChannels,
                             long long pts) override;

        void Close() override;

        void SaveMP4(bool save, char *fileName) override;

    private:
        ICMp4Mux *m_pMuxer = nullptr;
        std::atomic<bool> m_saveMP4 = {false};
        std::atomic<bool> m_hasSaveFirstFrame = {false};
        std::atomic<bool> m_enableVideo = {false};
        std::atomic<bool> m_enableAudio = {false};
        int m_VideoTrackHandle = -1;
        uint32_t m_videoLastPts = 0;
        uint32_t m_videoStartPts = 0;
        uint32_t m_audioLastPts = 0;
        uint32_t m_audioStartPts = 0;
        int m_AudioTrackHandle = -1;

        void destroyMuxer();


    };
}

#endif //LJSDK_MP4MUXER_H

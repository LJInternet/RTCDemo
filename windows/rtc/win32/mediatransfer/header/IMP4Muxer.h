//
// Created by Administrator on 2023/2/27.
//

#ifndef LJSDK_IMP4MUXER_H
#define LJSDK_IMP4MUXER_H
namespace LJMediaLibrary {
    class IMp4Muxer {
    public:
        virtual ~IMp4Muxer() {

        }

        virtual int Open(char *fileName) = 0;

        virtual int EnableVideo() = 0;

        virtual int EnableAudio() = 0;

        virtual int
        WriteVideoPacket(unsigned char *pData, int frameType, int width, int height, int nLen,
                         long long int pts) = 0;

        virtual int WriteAudioPacket(unsigned char *pData, int nLen, int nSampleRate, int nChannels,
                                     long long pts) = 0;

        virtual void Close() = 0;

        virtual void SaveMP4(bool save, char *fileName) = 0;;
    };

    class ICMp4Mux {
    public:
        virtual ~ICMp4Mux() {

        }

        virtual int Open(char *fileName) = 0;

        virtual int GetVideoTrack(int nVideoType, int nW, int nH) = 0;

        virtual int GetAudioTrack(int nAudioType, int nSampleRate, int nChannels) = 0;

        virtual int WritePacket(int hTrack, unsigned char *pData, int nLen, long long TimeStart,
                                long long TimeEnd) = 0;

        virtual int ReleaseTrack(int hTrack) = 0;

        virtual void Close() = 0;
    };
}
#endif //LJSDK_IMP4MUXER_H

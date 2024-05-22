//
// Created by Administrator on 2024/5/22.
//

#ifndef LJSDK_WAVPCMHEADER_H
#define LJSDK_WAVPCMHEADER_H

#include <stdint.h>

namespace LJMediaLibrary {
    namespace WAV {
        /**
         * @brief Structure representing RIFF
         */
        struct RIFF {
            const char riff[4] = {'R', 'I', 'F', 'F'};
            uint32_t fileLength;
            const char wave[4] = {'W', 'A', 'V', 'E'};
        };
        /**
         * @brief Structure representing wave header's format
         */
        struct Format {
            const char fmt[4] = {'f', 'm', 't', ' '};
            uint32_t blockSize = 16;
            uint16_t formatTag = 1;
            uint16_t channels;
            uint32_t sampleRate;
            uint32_t avgBytesPerSec;
            uint16_t blockAlign;
            uint16_t bitsPerSample;
        };
        struct DataChunk {
            const char data[4] = {'d', 'a', 't', 'a'};
            uint32_t dataSize;
        };
        //
        class WavPCMHeader {
        public:
            WavPCMHeader(uint16_t channels, uint32_t sampleRate, uint16_t bitsPerSample, uint32_t dataSize) {
                m_riff.fileLength = 36 + dataSize;
                m_format.channels = channels;
                m_format.sampleRate = sampleRate;
                m_format.avgBytesPerSec = sampleRate * channels * (bitsPerSample / 8);
                m_format.bitsPerSample = bitsPerSample;
                m_format.blockAlign = channels * bitsPerSample / 8;
                m_dataChunk.dataSize = dataSize;
            }
            ~WavPCMHeader() {

            };
        private:
            RIFF m_riff;
            Format m_format;
            DataChunk m_dataChunk;
        };
    }
}


#endif //LJSDK_WAVPCMHEADER_H

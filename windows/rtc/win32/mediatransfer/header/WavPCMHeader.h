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
            /**
             * @brief 标识符（4字节）：固定为RIFF，表示这是一个RIFF文件
             */
            const char riff[4] = {'R', 'I', 'F', 'F'};
            /**
             * 文件长度（4字节）：整个文件的大小减去8字节（不包括标识符和长度自身），存储为小端模式。
             */
            uint32_t fileLength;
            /**
             * 文件类型（4字节）：对于WAV文件，该字段应为WAVE
             */
            const char wave[4] = {'W', 'A', 'V', 'E'};
        };
        /**
         * @brief Structure representing wave header's format
         */
        struct Format {
            /**
             * @brief 子块标识符（4字节）：fmt，表示接下来的内容是格式描述
             */
            const char fmt[4] = {'f', 'm', 't', ' '};
            /**
             * @brief 子块大小（4字节）：通常为16字节，表示fmt块的大小
             */
            uint32_t blockSize = 16;
            /**
             * @brief 音频格式（2字节）：常见的值有1（PCM编码）或其他特定编码的ID。
             */
            uint16_t formatTag = 1;
            /**
             * @brief 声道数（2字节）：表示音频是单声道（1）还是立体声（2）等
             */
            uint16_t channels;
            /**
             * @brief 采样率（4字节）：每秒的采样次数，例如44100Hz
             */
            uint32_t sampleRate;
            /**
             * @brief 平均字节数/秒（4字节）：每秒的音频数据量，等于采样率乘以声道数乘以位深度/8
             */
            uint32_t avgBytesPerSec;
            /**
             * @brief 块对齐（2字节）：每次采样占用的字节数，等于声道数乘以位深度/8
             */
            uint16_t blockAlign;
            /**
             * @brief 位深度（2字节）：每个采样点的比特数，常见的有8位、16位。
             */
            uint16_t bitsPerSample;
        };
        struct DataChunk {
            /**
             * @brief 子块标识符（4字节）：data，表明接下来的数据是音频数据
             */
            const char data[4] = {'d', 'a', 't', 'a'};
            /**
             * @brief 数据长度（4字节）：音频数据的总大小，即实际的音频样本数据量
             */
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

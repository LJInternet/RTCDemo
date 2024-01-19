//
// Created by Administrator on 2022/6/20.
//

#include <string>
#include "MediaConstants.h"
#ifndef LJSDK_IENCODER_H
#define LJSDK_IENCODER_H

#define CODEC_INFO(fmt,...) \
    LJ::LOG("LJ_CODEC", LJ::LOG_INFO, fmt, ##__VA_ARGS__)

namespace LJMediaLibrary {

    struct MIEPushEncodedAudioData;
    struct MIEPushEncodedVideoData;
    enum EncodeState {
        E_STOP,
        E_PREPARE,
        E_START,
        E_DECODING,
        E_PAUSE,
        E_FINISH
    };

    enum MIRROR_TYPE {
        MIRROR_NONE = 0,
        MIRROR_Y = 1,
        MIRROR_X = 2,
    };

    struct VideoEncodedData
    {
        VideoFrameType  iFrameType;
        uint32_t   iPts;
        uint32_t    iDts;
        uint32_t    iDataLen;
        uint32_t    iMetaDataLen;
        uint32_t    iExtraDataLen;
        void           *iData;
        void           *iMetaData;
        void           *iExtraData;
        uint32_t             iEncodedType; //EncodeType
        DelayTimeInfo* delayDatas;
        uint32_t timeInfoCount;
        uint32_t frameId;

        VideoEncodedData()
                : iFrameType(kVideoUnknowFrame)
                , iPts(0)
                , iDts(0)
                , iDataLen(0)
                , iData(NULL)
                , iMetaDataLen(0)
                , iMetaData(NULL)
                , iExtraData(NULL)
                , iEncodedType(ENTYPE_H264) //default H264
                , frameId(0)
                , timeInfoCount(0)
        {
        }

        VideoEncodedData(VideoFrameType frameType, uint32_t pts)
                : iFrameType(frameType)
                , iPts(pts)
                , iDts(0)
                , iDataLen(0)
                , iData(NULL)
                , iEncodedType(ENTYPE_H264) //default H264
                , frameId(0)
        {

        }
    };

    struct VideoEncodedList
    {
        int  iSize;
        VideoEncodedData *iPicData; //VideoEncodedData points array

        VideoEncodedList()
                : iSize(0)
                , iPicData(NULL)
        {

        }
    };

    enum RenderCorpType {
        BOTTOM_LEFT,
        TOP_LEFT,
    };

    struct VideoNeedEncodeData
    {

        uint32_t    iDataLen;
        uint8_t     *iData;
        int         stride;
        int         width;
        int         height;
        int         pixel_fmt;
        int         rotation;
        int         textureId;
        int         mirror;
        int*        corpRet;
        int         ProgramType;
        int         corpType;
        DelayTimeInfoList* mDelayMap;

        VideoNeedEncodeData()
                : iDataLen(0)
                , iData(nullptr)
                , width(0)
                , height(0)
                , pixel_fmt(0)
                , mDelayMap(nullptr)
                , rotation(0)
                , mirror(0)
                , corpRet(nullptr)
                , textureId(-1)
                , ProgramType(0)
                , corpType(0)
        {

        }

        ~VideoNeedEncodeData(){
            if (mDelayMap != nullptr) {
                if (mDelayMap->timeInfos != nullptr) {
                    delete mDelayMap->timeInfos;
                    mDelayMap->timeInfos = nullptr;
                }
                delete mDelayMap;
                mDelayMap = nullptr;
            }

            if (iData != nullptr) {
                delete[] iData;
                iData = nullptr;
            }
            if (corpRet != nullptr) {
                delete[] corpRet;
                corpRet = nullptr;
            }
            if (corpRet != NULL) {
                delete[] corpRet;
            }
        }
    };

    struct AudioNeedEncodeData
    {
        uint8_t       *iData;
        uint32_t    iFrameNum;
        uint64_t    iPts;
        DelayTimeInfoList* mDelayMap;

        AudioNeedEncodeData()
                : iFrameNum(0)
                , iData(NULL)
                , iPts(0)
                , mDelayMap(NULL)
        {

        }

        ~AudioNeedEncodeData(){
            if (iData != NULL) {
                delete[] iData;
                iData = NULL;
            }
            if (mDelayMap != NULL) {
                if (mDelayMap->timeInfos != NULL) {
                    delete mDelayMap->timeInfos;
                    mDelayMap->timeInfos = NULL;
                }
                delete mDelayMap;
                mDelayMap = NULL;
            }
            if (mDelayMap != NULL) {
                if (mDelayMap->timeInfos != NULL) {
                    delete mDelayMap->timeInfos;
                }
                delete mDelayMap;
            }
        }
    };

    typedef void (*AudioEncodeCallback)(MIEPushEncodedAudioData * data, void *content);
    struct AudioEncodeContent {
        AudioEncodeCallback callback;
        void *content;
    };

    typedef void (*VideoEncodeCallback)(MIEPushEncodedVideoData * data, void *content);
    struct VideoEncodeContent {
        VideoEncodeCallback callback;
        void *content;
    };
}

#endif //LJSDK_IENCODER_H

//
// Created by Administrator on 2022/6/14.
//

#ifndef LJSDK_TRANSCONSTANTS_H
#define LJSDK_TRANSCONSTANTS_H

#define VIDEO_REPORT_DURATION 2 // 1S

#include <sstream>
#include "MediaConstants.h"
#include "LJPacket.h"

namespace LJMediaLibrary {

    /**
     * @brief UDP回调类型枚举。
     */
    enum UDPCallbackType {
        RUDP_CB_TYPE_DECODE_DATA = 1, /**< 解码数据 */
        RUDP_CB_TYPE_AVAILABLE_BW = 2, /**< 有效带宽 */
        RUDP_CB_TYPE_REQUEST_I_FRAME = 3, /**< 等同于需要一个I帧 */
        RUDP_CB_TYPE_LINK_OK = 4, /**< 首次建连成功或者中途断开重连成功，这个时候需要发一个I帧 */
        RUDP_CB_TYPE_LINK_FAILURE = 5, /**< 丢包非常严重，可能需要切流什么的 */
        RUDP_CB_TYPE_LINK_REPORT = 6, /**< 链路上报 */
        RUDP_CB_TYPE_NET_REPORT = 7, /**< 网络质量回调 */
        RUDP_CB_VIDEO_FRAME_RATE_CONTROL = 8, /**< 视频帧率控制 */
        CB_AUDIO_CAPTURE_VOLUME = 9, /**< 音频采集音量回调 */
        CB_JOIN_CHANNEL = 10, /**< 加入频道结果回调 */
        CB_LEAVE_CHANNEL = 11, /**< 离开频道结果回调 */
        CB_LINK_STATUS = 12, /**< RUDP连接状态 */
        CB_TRANS_STOP = 13, /**< 调用了C++层的LeaveChannel */
        MUTI_CHANNEL_REMOTE_JOIN = 1000, /**< 多人RTC远端有人加入 */
        MUTI_CHANNEL_REMOTE_LEAVE = 1001, /**< 多人RTC远端有人退出 */
    };

    /**
     * @brief 网络质量级别枚举。
     */
    enum NetQualityLevel {
        QUALITY_GOOD = 1, /**< 网络质量好 */
        QUALITY_COMMON = 2, /**< 网络质量一般 */
        QUALITY_BAD = 3, /**< 勉强能沟通 */
        QUALITY_VBAD = 4, /**< 网络质量非常差，基本不能沟通。 */
        QUALITY_BLOCK = 5, /**< 链路不通 */
    };

    /**
     * @brief 延迟常量枚举。
     */
    enum DelayConstants {
        /**
         * @brief 采集时间戳，一般是相机的纹理回调回来的时间戳。
         */
        KEY_CAPTURE_TIME = 1,
        /**
         * @brief 结束前处理时间戳。
         */
        KEY_END_PREPROCESS_TIME = 2,
        /**
         * @brief 结束编码时间戳。
         */
        KEY_END_ENCODE_TIME = 3,

        /**
         * @brief JNI开始的时间。
         */
        KEY_JNI_START_TIME = 4,
        /**
         * @brief 发送帧数据的时间。
         */
        KEY_SEND_UDP_TIME = 5,
        /**
         * @brief 接收到数据的时间。
         */
        KEY_REV_DATA_TIME = 6,
        /**
         * @brief 开始解码的时间。
         */
        KEY_START_DECODE_TIME = 7,

        /**
         * @brief 结束解码的时间。
         */
        KEY_END_DECODE_TIME = 8,
        /**
         * @brief Java收到数据包的时间。
         */
        KEY_JVM_REV_DATA = 9,
        /**
         * @brief 开始渲染时间。
         */
        KEY_START_RENDER = 10,
        /**
         * @brief 结束渲染时间。
         */
        KEY_END_RENDER = 11,

        /**
         * @brief 帧类型。
         */
        KEY_FRAME_TYPE = 12,
        /**
         * @brief 帧大小。
         */
        KEY_FRAME_SIZE = 13,

        /**
         * @brief 传输延迟，这里是上次包的传输延迟。
         */
        KEY_TRANS_DELAY = 14,

        /**
         * @brief 传输延迟对应的帧Id。
         */
        KEY_TRANS_DELAY_FRAME_ID = 15,

        /**
         * @brief 开始编码时间。
         */
        KEY_START_ENCODE_TIME = 16,

        /**
         * @brief 解码切换线程队列，开始回调和结束回调的时间。
         */
        KEY_DECODE_CACHE_START = 100,
        KEY_DECODE_CACHE_END = 101,
    };

    /**
     * @brief 传输数据类型枚举。
     *
     * 在调用UDP send的方法时，数据的前4位是传输数据类型。
     */
    enum TransDataType {
        /**
         * @brief 视频数据。
         */
        VIDEO_DATA = 1,
        /**
         * @brief 音频数据。
         */
        AUDIO_DATA = 2,
        /**
         * @brief 返回头数据。
         */
        BACK_HEAD_DATA = 3,
        /**
         * @brief 网络静态数据。
         */
        NET_STATIC = 4,
        /**
         * @brief 通用统计。
         */
        COMMON_STATIC = 5,
        /**
         * @brief 可用带宽数据。
         */
        AVAILABLE_BM = 6,
        /**
         * @brief 请求I帧数据。
         */
        REQUEST_I_FRAME = 7,
        /**
         * @brief RUDP加入数据。
         */
        RUDP_JOIN = 8,
        /**
         * @brief RUDP离开数据。
         */
        RUDP_LEAVE = 9,
    };

    /**
     * @brief 通用统计键枚举。
     */
    enum CommonStatisticKey {
        /**
         * @brief 解码缓存计数，用于统计解码缓存格式以及解码帧率。
         */
        KEY_DECODE_CACHE_COUNT = 1,
        /**
         * @brief 解码帧率。
         */
        KEY_DECODE_FPS = 2,
        /**
         * @brief 解码耗时。
         */
        KEY_DECODE_COAST = 3,
    };

    class TransPack {
    public:
        TransPack() : m_pb(), m_pk(m_pb) {

        }

    public:
        bool isPackError() const {
            return m_pk.isPackError();
        }

        size_t bodySize() {
            return m_pk.size();
        }

        const char *body() {
            return m_pk.data();
        }

        void marshall(const ljtransfer::mediaSox::Marshallable &m) {
            m.marshal(m_pk);
        }

        void clear() {
            m_pb.resize(0);
            m_pk.setPackError(false);
        }

    protected:
        ljtransfer::mediaSox::PackBuffer m_pb;
        ljtransfer::mediaSox::Pack m_pk;
    };

    /**
     * @brief 用于推送编码视频数据的结构体。
     */
    struct MIEPushEncodedVideoData : ljtransfer::mediaSox::Marshallable {
        uint32_t iFrameType; /**< 帧类型。*/
        uint32_t iEncodeType; /**< 编码类型。*/
        uint32_t iPts; /**< Presentation Time Stamp。*/
        uint32_t iDts; /**< Decoding Time Stamp。*/
        uint64_t iStreamId; /**< 流ID。*/
        uint32_t width; /**< 视频宽度。*/
        uint32_t height; /**< 视频高度。*/
        std::string iData; /**< 视频数据。*/
        std::string iMetaDta; /**< 元数据。*/
        std::string iExtraData; /**< 额外数据。*/
        std::map<uint64_t, uint64_t> iTsInfos; /**< 时间戳信息。*/

        MIEPushEncodedVideoData()
                : iFrameType(0), iEncodeType(0), iPts(0), iDts(0), iStreamId(0), width(0),
                  height(0) {
        }

        /**
         * @brief 将结构体数据序列化为二进制数据。
         * @param pak 序列化器对象。
         */
        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << iFrameType << iEncodeType << iPts << iDts << iStreamId << width << height;
            pak.push_varstr32(iMetaDta.data(), iMetaDta.length());
            pak.push_varstr32(iExtraData.data(), iExtraData.length());
            marshal_container(pak, iTsInfos);
        }

        /**
         * @brief 将二进制数据反序列化为结构体数据。
         * @param pak 反序列化器对象。
         */
        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> iFrameType >> iEncodeType >> iPts >> iDts >> iStreamId >> width >> height;
            iMetaDta = pak.pop_varstr32();
            iExtraData = pak.pop_varstr32();
            unmarshal_container(pak, inserter(iTsInfos, iTsInfos.begin()));
        }
    };

    /**
     * @brief 用于推送原始视频数据的结构体。
     */
    struct MIEPushVideoRawData : ljtransfer::mediaSox::Marshallable {
        uint32_t width; /**< 视频宽度。*/
        uint32_t height; /**< 视频高度。*/
        uint32_t pixelFormat; /**< 像素格式。*/
        uint32_t rotation; /**< 旋转角度。*/
        uint64_t timestamp; /**< 时间戳。*/
//        std::string iData;

        MIEPushVideoRawData()
                : width(0), height(0) {
        }

        /**
         * @brief 将结构体数据序列化为二进制数据。
         * @param pak 序列化器对象。
         */
        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << width << height << pixelFormat << rotation << timestamp;
//            pak.push_varstr32(iData.data(), iData.length());
        }

        /**
         * @brief 将二进制数据反序列化为结构体数据。
         * @param pak 反序列化器对象。
         */
        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> width >> height >> pixelFormat >> rotation >> timestamp;
//            iData = pak.pop_varstr32();
        }
    };

    /**
     * @brief 用于捕获视频帧的结构体。
     */
    struct CaptureVideoFrame : ljtransfer::mediaSox::Marshallable {
        uint32_t type; /**< 帧类型。*/
        uint32_t format; /**< 帧格式。*/
        uint32_t stride; /**< 行跨距。*/
        uint32_t width; /**< 视频宽度。*/
        uint32_t height; /**< 视频高度。*/
        uint32_t rotation; /**< 旋转角度。*/
        uint32_t eglType; /**< EGL类型。*/
        uint32_t textureId; /**< 纹理ID。*/
        uint64_t timestamp; /**< 时间戳。*/
        uint32_t metadata_size; /**< 元数据大小。*/
        std::string metadata_buffer; /**< 元数据缓冲区。*/
        uint32_t mirror; /**< 是否镜像。*/
        uint32_t corpLeft; /**< 裁剪左边距。*/
        uint32_t cropRight; /**< 裁剪右边距。*/
        uint32_t cropTop; /**< 裁剪顶边距。*/
        uint32_t cropBottom; /**< 裁剪底边距。*/
        uint32_t ProgramType; /**< 程序类型。*/
        uint32_t corpType; /**< 裁剪类型。*/

        CaptureVideoFrame() : type(0),
                              format(0),
                              stride(0),
                              width(0),
                              height(0),
                              rotation(0),
                              eglType(0),
                              textureId(0),
                              timestamp(0),
                              mirror(0),
                              corpLeft(0),
                              cropRight(0),
                              cropTop(0),
                              cropBottom(0),
                              ProgramType(0),
                              corpType(0),
                              metadata_size(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type << format << stride << width << height << rotation << eglType << textureId
                << timestamp << metadata_size;
            pak.push_varstr32(metadata_buffer.data(), metadata_buffer.length());
            pak << mirror << corpLeft << cropRight << cropTop << cropBottom << ProgramType
                << corpType;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type >> format >> stride >> width >> height >> rotation >> eglType >> textureId
                >> timestamp >> metadata_size;
            metadata_buffer = pak.pop_varstr32();
            pak >> mirror;
            if (pak.size() > 4u) {
                pak >> corpLeft >> cropRight >> cropTop >> cropBottom >> ProgramType >> corpType;
            }
        }
    };

    /**
     * @brief 用于存储视频解码数据的结构体。
     */
    struct VideoDecodedData : ljtransfer::mediaSox::Marshallable {
        uint32_t width; /**< 视频宽度。*/
        uint32_t height; /**< 视频高度。*/
        uint32_t widthY; /**< Y分量宽度。*/
        uint32_t heightY; /**< Y分量高度。*/
        uint32_t widthUV; /**< UV分量宽度。*/
        uint32_t heightUV; /**< UV分量高度。*/
        uint32_t offsetY; /**< Y分量偏移。*/
        uint32_t offsetU; /**< U分量偏移。*/
        uint32_t offsetV; /**< V分量偏移。*/
        uint32_t len; /**< 数据长度。*/
        uint32_t frameId; /**< 帧ID。*/
        uint8_t *data[8]; /**< 数据数组。*/
        std::string iExtraData; /**< 额外数据。*/
        std::map<uint64_t, uint64_t> delayData; /**< 延迟数据映射。*/

        VideoDecodedData()
                : width(0), height(0), widthY(0), heightY(0), widthUV(0), heightUV(0), offsetY(0),
                  offsetU(0), offsetV(0), frameId(0), len(0) {
            for (int i = 0; i < 8; i++) {
                data[i] = NULL;
            }
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << width << height << widthY << heightY << widthUV << heightUV << offsetY << offsetU
                << offsetV << len << frameId;
            pak.push_varstr32(iExtraData.data(), iExtraData.length());
            marshal_container(pak, delayData);
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> width >> height >> widthY >> heightY >> widthUV >> heightUV >> offsetY >> offsetU
                >> offsetV >> len >> frameId;
            iExtraData = pak.pop_varstr32();
            unmarshal_container(pak, inserter(delayData, delayData.begin()));
        }

        void reset() {
            widthY = 0;
            heightY = 0;
            heightUV = 0;
            widthUV = 0;
            offsetU = 0;
            offsetV = 0;
            offsetY = 0;
            frameId = 0;
            len = 0;
            for (int i = 0; i < 8; i++) {
                data[i] = NULL;
            }
            delayData.clear();
            iExtraData.clear();
        }
    };

	/**
	 * @brief 用于存储MIE视频捕获配置的结构体。
	 */
	struct MIEVideoCaptureConfig : ljtransfer::mediaSox::Marshallable {
		uint32_t width; /**< 视频宽度。*/
		uint32_t height; /**< 视频高度。*/
		uint32_t fps; /**< 视频帧率。*/
		std::string deviceCode; /**< 设备编码。*/

		/**
		 * @brief 默认构造函数。
		 */
		MIEVideoCaptureConfig()
			: width(0), height(0), fps(0) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pak 序列化器对象。
		 */
		virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
			pak << width << height << fps;
			pak.push_varstr32(deviceCode.data(), deviceCode.length());
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param pak 反序列化器对象。
		 */
		virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
			pak >> width >> height >> fps;
			deviceCode = pak.pop_varstr32();
		}
	};

	/**
	 * @brief 用于缓存视频解码数据的结构体。
	 */
	struct CacheVideoDecodedData : VideoDecodedData {
		DelayTimeInfoList *mDelayList; /**< 延迟时间信息列表指针。*/
		char *extraData; /**< 额外数据指针。*/
		uint32_t iExtraDataSize; /**< 额外数据大小。*/

		/**
		 * @brief 默认构造函数。
		 */
		CacheVideoDecodedData() :
			mDelayList(nullptr), extraData(nullptr), iExtraDataSize(0) {
		}
	};

	/**
	 * @brief 用于存储MIE推送编码音频数据的结构体。
	 */
	struct MIEPushEncodedAudioData : ljtransfer::mediaSox::Marshallable {
		uint64_t iPts; /**< 时间戳。*/
		uint32_t isHead; /**< 是否为头部。*/
		uint32_t audioType; /**< 音频类型。*/
		std::string iData; /**< 音频数据。*/

		/**
		 * @brief 默认构造函数。
		 */
		MIEPushEncodedAudioData()
			: iPts(0), isHead(0), audioType(0) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pak 序列化器对象。
		 */
		virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
			pak << iPts << isHead << audioType;
			pak.push_varstr32(iData.data(), iData.length());
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param pak 反序列化器对象。
		 */
		virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
			pak >> iPts >> isHead >> audioType;
			iData = pak.pop_varstr32();
		}
	};

	/**
	 * @brief 用于存储音频头部信息的结构体。
	 */
	struct AudioHead : ljtransfer::mediaSox::Marshallable {
		uint64_t iPts; /**< 时间戳。*/
		std::string iHead; /**< 头部数据。*/
		uint32_t audioType; /**< 音频类型。*/
		uint32_t frameIndex; /**< 帧索引。*/

		/**
		 * @brief 默认构造函数。
		 */
		AudioHead() : iPts(0), audioType(0), frameIndex(0) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pak 序列化器对象。
		 */
		virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
			pak << iPts << audioType << frameIndex;
			pak.push_varstr32(iHead.data(), iHead.length());
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param pak 反序列化器对象。
		 */
		virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
			pak >> iPts >> audioType >> frameIndex;
			iHead = pak.pop_varstr32();
		}
	};

	/**
	 * @brief 用于存储视频头部信息的结构体。
	 */
	struct VideoHead : ljtransfer::mediaSox::Marshallable {
		uint32_t iFrameType; /**< 帧类型。*/
		uint32_t iEncodeType; /**< 编码类型。*/
		uint32_t iPts; /**< 时间戳。*/
		uint32_t iDts; /**< 解码时间戳。*/
		uint32_t iFrameId; /**< 帧ID。*/
		uint32_t iMetaDataLen; /**< 元数据长度。*/
		std::string iMetaDta; /**< 元数据。*/
		uint32_t iExtraDataLen; /**< 额外数据长度。*/
		std::string iExtraData; /**< 额外数据。*/
		std::string header; /**< 头部信息。*/
		std::map<uint64_t, uint64_t> iTsInfos; /**< 时间戳信息。*/
		uint16_t width; /**< 视频宽度。*/
		uint16_t height; /**< 视频高度。*/

		/**
		 * @brief 默认构造函数。
		 */
		VideoHead()
			: iFrameType(0), iEncodeType(0), iPts(0), iDts(0), iFrameId(0), iMetaDataLen(0),
			  iExtraDataLen(0), width(720), height(1280) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pak 序列化器对象。
		 */
		virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
			pak << iFrameType << iEncodeType << iPts << iDts << iFrameId << iMetaDataLen
				<< iExtraDataLen;
			pak.push_varstr32(iMetaDta.data(), iMetaDta.length());
			pak.push_varstr32(iExtraData.data(), iExtraData.length());
			marshal_container(pak, iTsInfos);
			pak << width << height;
			pak.push_varstr(header.data(), header.length());
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param pak 反序列化器对象。
		 */
		virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
			pak >> iFrameType >> iEncodeType >> iPts >> iDts >> iFrameId >> iMetaDataLen
				>> iExtraDataLen;
			iMetaDta = pak.pop_varstr32();
			iExtraData = pak.pop_varstr32();
			unmarshal_container(pak, inserter(iTsInfos, iTsInfos.begin()));
			if (!pak.empty()) {
				pak >> width >> height;
			}
			if (!pak.empty()) {
				header = pak.pop_varstr();
			}
		}
	};

/**
 * @brief 用于配置UDP连接的结构体。
 */
struct UDPConfig : ljtransfer::mediaSox::Marshallable {
    uint64_t relayId; /**< 中继ID。*/
    std::string remoteIP; /**< 远程IP地址。*/
    uint32_t remotePort; /**< 远程端口。*/
    uint32_t remoteSessionId; /**< 远程会话ID。*/
    uint32_t netType; /**< 网络类型。*/

    /**
     * @brief 默认构造函数。
     */
    UDPConfig()
        : remoteIP("61.155.136.210"),
          relayId(122222),
          remotePort(30001),
          remoteSessionId(10089),
          netType(2) {
    }

    /**
     * @brief 将结构体数据序列化为二进制数据。
     * @param pk 序列化器对象。
     */
    void marshal(ljtransfer::mediaSox::Pack &pk) const {
        pk << relayId << remoteIP << remotePort << remoteSessionId << netType;
    }

    /**
     * @brief 将二进制数据反序列化为结构体数据。
     * @param up 反序列化器对象。
     */
    void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
        up >> relayId >> remoteIP >> remotePort >> remoteSessionId >> netType;
    }
};

/**
 * @brief 用于配置MIE传输的结构体。
 */
struct MIETransferConfig : ljtransfer::mediaSox::Marshallable {
    std::vector<UDPConfig> configs; /**< UDP配置列表。*/
    std::string p2pSignalServer; /**< P2P信令服务器地址。*/
    uint32_t transferMode; /**< 传输模式。*/
    uint64_t appID; /**< 应用程序ID。*/
    uint64_t userID; /**< 用户ID。*/
    std::string channelID; /**< 频道ID。*/
    std::string token; /**< 令牌。*/
    uint32_t localIp; /**< 本地IP地址（网络字节顺序）。*/

    /**
     * @brief 默认构造函数。
     */
    MIETransferConfig()
        : transferMode(1),
          p2pSignalServer("61.155.136.209:9988"),
          appID(0),
          userID(0),
          channelID(""),
          token(""),
          localIp(0) {
    }

    /**
     * @brief 将结构体数据序列化为二进制数据。
     * @param pk 序列化器对象。
     */
    virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
        pk << p2pSignalServer << transferMode;
        marshal_container(pk, configs);
        pk << appID << userID << channelID << token;
    }

    /**
     * @brief 将二进制数据反序列化为结构体数据。
     * @param up 反序列化器对象。
     */
    virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
        up >> p2pSignalServer >> transferMode;
        unmarshal_container(up, inserter(configs, configs.begin()));
        up >> appID >> userID >> channelID >> token;
    }
};

	/**
	 * @brief 用于表示MIE静音媒体事件的结构体。
	 */
	struct MIEMuteMediaEvent : ljtransfer::mediaSox::Marshallable {
		uint32_t mediaType; /**< 媒体类型。*/
		bool mute; /**< 是否静音。*/

		/**
		 * @brief 默认构造函数。
		 */
		MIEMuteMediaEvent()
			: mediaType(0), mute(false) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pk 序列化器对象。
		 */
		virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
			pk << mediaType << mute;
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param up 反序列化器对象。
		 */
		virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
			up >> mediaType >> mute;
		}
	};

	/**
	 * @brief 用于配置MIE音频上传的结构体。
	 */
	struct MIEAudioUploadConfig : ljtransfer::mediaSox::Marshallable {
		uint32_t sampleRate; /**< 采样率。*/
		uint32_t channels; /**< 声道数。*/
		uint32_t bitsPerSample; /**< 每个样本的位数。*/
		uint32_t audioBitrateInbps; /**< 音频比特率（每秒位数）。*/
		uint32_t audioType; /**< 音频类型。*/

		/**
		 * @brief 默认构造函数。
		 */
		MIEAudioUploadConfig()
			: sampleRate(0), channels(0), bitsPerSample(0), audioBitrateInbps(0), audioType(0) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pk 序列化器对象。
		 */
		void marshal(ljtransfer::mediaSox::Pack &pk) const {
			pk << sampleRate << channels << bitsPerSample << audioBitrateInbps << audioType;
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param up 反序列化器对象。
		 */
		void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
			up >> sampleRate >> channels >> bitsPerSample >> audioBitrateInbps >> audioType;
		}
	};

	/**
	 * @brief 用于配置MIE视频上传的结构体。
	 */
	struct MIEVideoUploadConfig : ljtransfer::mediaSox::Marshallable {
		uint32_t encodeWidth; /**< 编码宽度。*/
		uint32_t encodeHeight; /**< 编码高度。*/
		uint32_t minVideoBitrateInbps; /**< 最小视频比特率（每秒位数）。*/
		uint32_t maxVideoBitrateInbps; /**< 最大视频比特率（每秒位数）。*/
		uint32_t realVideoBitrateInbps; /**< 实际视频比特率（每秒位数）。*/
		uint32_t codecType; /**< 编解码器类型。*/
		uint32_t fps; /**< 帧率。*/
		uint32_t mirror; /**< 是否镜像。*/
		uint32_t keyFrameInterval; /**< 关键帧间隔。*/
		uint32_t mode; /**< 模式。 0:BITRATE_MODE_CQ, 1:BITRATE_MODE_VBR, 2:BITRATE_MODE_CBR */

		/**
		 * @brief 默认构造函数。
		 */
		MIEVideoUploadConfig()
				: encodeWidth(0), encodeHeight(0), minVideoBitrateInbps(0), maxVideoBitrateInbps(0),
				  realVideoBitrateInbps(0), codecType(0), fps(0), mirror(0), keyFrameInterval(3), mode(2) {

		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pk 序列化器对象。
		 */
		void marshal(ljtransfer::mediaSox::Pack &pk) const {
			pk << encodeWidth << encodeHeight << minVideoBitrateInbps << maxVideoBitrateInbps
			   << realVideoBitrateInbps << codecType << fps << mirror << keyFrameInterval << mode;
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param up 反序列化器对象。
		 */
		void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
			up >> encodeWidth >> encodeHeight >> minVideoBitrateInbps >> maxVideoBitrateInbps
			   >> realVideoBitrateInbps >> codecType >> fps >> mirror >> keyFrameInterval >> mode;
		}

		/**
		 * @brief 返回结构体信息的字符串表示形式。
		 * @return 字符串表示形式。
		 */
		std::string toString() {
			std::stringstream stream;
			stream << "VideoUploadConfig encodeWidth " << encodeWidth<< " encodeHeight " << encodeHeight
					<< " minVideoBitrateInbps " << minVideoBitrateInbps << " maxVideoBitrateInbps " << maxVideoBitrateInbps
					<< " realVideoBitrateInbps " << realVideoBitrateInbps << " codecType " << codecType
					<< " fps " << fps << " mirror " << mirror << " keyFrameInterval " << keyFrameInterval << " mode " << mode
			<< std::endl;
			return stream.str();
		}
	};

	/**
	 * @brief 用于配置RTC引擎的结构体。
	 */
	struct RTCEngineConfig
			: public ljtransfer::mediaSox::Marshallable {
		bool enableLog; /**< 是否启用日志。*/

		/**
		 * @brief 默认构造函数。
		 */
		RTCEngineConfig()
				: enableLog(true) {
		}

		/**
		 * @brief 将结构体数据序列化为二进制数据。
		 * @param pak 序列化器对象。
		 */
		virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
			pak << enableLog;
		}

		/**
		 * @brief 将二进制数据反序列化为结构体数据。
		 * @param pak 反序列化器对象。
		 */
		virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
			pak >> enableLog;
		}
	};

	/**
	 * @brief 用于配置MIE上传的结构体。
	 */
	struct MIEUploadConfig : ljtransfer::mediaSox::Marshallable {
		uint32_t enableAudio; /**< 是否启用音频。*/
		uint32_t enableVideo; /**< 是否启用视频。*/
		MIEAudioUploadConfig audioUploadConfig; /**< 音频上传配置。*/
		MIEVideoUploadConfig videoUploadConfig; /**< 视频上传配置。*/
		MIETransferConfig transferConfig; /**< 传输配置。*/

        MIEUploadConfig() : enableAudio(1), enableVideo(1) {

        }

        void updateConfig(MIEUploadConfig config) {
            enableAudio = config.enableAudio;
            enableVideo = config.enableVideo;
            if (config.audioUploadConfig.sampleRate != 0) {
                audioUploadConfig.audioType = config.audioUploadConfig.audioType;
                audioUploadConfig.sampleRate = config.audioUploadConfig.sampleRate;
                audioUploadConfig.channels = config.audioUploadConfig.channels;
                audioUploadConfig.bitsPerSample = config.audioUploadConfig.bitsPerSample;
                audioUploadConfig.audioBitrateInbps = config.audioUploadConfig.audioBitrateInbps;
            }
            if (config.videoUploadConfig.encodeHeight != 0) {
                videoUploadConfig.encodeHeight = config.videoUploadConfig.encodeHeight;
                videoUploadConfig.encodeWidth = config.videoUploadConfig.encodeWidth;
                videoUploadConfig.codecType = config.videoUploadConfig.codecType;
                videoUploadConfig.maxVideoBitrateInbps = config.videoUploadConfig.maxVideoBitrateInbps;
                videoUploadConfig.realVideoBitrateInbps = config.videoUploadConfig.realVideoBitrateInbps;
                videoUploadConfig.minVideoBitrateInbps = config.videoUploadConfig.minVideoBitrateInbps;
                videoUploadConfig.fps = config.videoUploadConfig.fps;
                videoUploadConfig.mirror = config.videoUploadConfig.mirror;
                videoUploadConfig.keyFrameInterval = config.videoUploadConfig.keyFrameInterval;
                videoUploadConfig.mode = config.videoUploadConfig.mode;
            }

            transferConfig.configs = config.transferConfig.configs;
            transferConfig.transferMode = config.transferConfig.transferMode;
        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << enableAudio << enableVideo << audioUploadConfig << videoUploadConfig
               << transferConfig;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> enableAudio >> enableVideo >> audioUploadConfig >> videoUploadConfig
               >> transferConfig;
        }

    };

	/**
	 * @brief WebSocket消息结构体，用于传输命令和消息内容。
	 */
	struct WebSocketMessage : ljtransfer::mediaSox::Marshallable {
		std::string cmd; /**< 命令字符串。*/
		std::string msg; /**< 消息内容字符串。*/

        WebSocketMessage() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << cmd << msg;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> cmd >> msg;
        }
    };

	/**
	 * @brief 音频播放器事件结构体，用于控制音频播放行为。
	 */
	struct AudioPlayerEvent : ljtransfer::mediaSox::Marshallable {
		bool callbackDecodeData = false; /**< 是否需要回调音频数据。*/
		bool renderAudioData = false; /**< 数据是否需要播放，false 则直接静音。*/
		bool directDecode = false; /**< 收到远端音频直接解码，不需要经过JitterBuffer。*/
		bool directCallback = false; /**< rudp收到数据包啥也不做，直接返回未解码数据，回调IAudioProcessor。*/
        AudioPlayerEvent() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << callbackDecodeData << renderAudioData << directDecode;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> callbackDecodeData >> renderAudioData >> directDecode;
        }
    };

	/**
	 * @brief 软件解码器事件类型枚举，用于表示软件解码器的不同事件类型。
	 */
	enum SoftDecodeInvokeEventType {
		START_SOFT_DECODE_EVENT = 0, /**< 开始软件解码事件。*/
		STOP_SOFT_DECODE_EVENT = 1, /**< 停止软件解码事件。*/
		ON_SOFT_DECODE_SURFACE_CHANGE = 2, /**< 软件解码表面变化事件。*/
	};


	/**
	 * @brief MIE软件解码器事件结构体，用于传输软件解码器相关的事件信息。
	 */
	struct MIESoftDecodeEvent : ljtransfer::mediaSox::Marshallable {
		uint32_t evtType; /**< 事件类型。*/
		uint32_t subEvtType; /**< 子事件类型。*/
        MIESoftDecodeEvent()
                : evtType(100), subEvtType(START_SOFT_DECODE_EVENT) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << evtType << subEvtType;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> evtType >> subEvtType;
        }
    };
	/**
	 * @brief MIE表面变化事件结构体，用于传输表面变化相关信息。
	 */
	struct MIESurfaceChange : ljtransfer::mediaSox::Marshallable {
		uint32_t evtType; /**< 事件类型。*/
		uint32_t subEvtType; /**< 子事件类型。*/
		uint32_t width; /**< 表面宽度。*/
		uint32_t height; /**< 表面高度。*/
		bool remove; /**< 是否移除表面。*/

        MIESurfaceChange()
                : evtType(100), subEvtType(ON_SOFT_DECODE_SURFACE_CHANGE), width(16), height(16),
                  remove(false) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << evtType << subEvtType << width << height << remove;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> evtType >> subEvtType >> width >> height >> remove;
        }
    };


    struct TestSaveVideoFileEvent : ljtransfer::mediaSox::Marshallable {
        bool enable;

        TestSaveVideoFileEvent() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pk) const {
            pk << enable;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &up) {
            up >> enable;
        }
    };

    struct AudioConfig : public ljtransfer::mediaSox::Marshallable {
		uint32_t sampleRate = 48000; /**< 采样率。*/
		uint32_t channels = 1; /**< 声道数。*/
		uint32_t framePerBuffer = 480; /**< 每缓冲帧数。*/
		uint32_t bitrateInbps = 64000; /**< 比特率。*/
		uint32_t audioType = 1; /**< 音频类型。*/
		uint32_t micVolume = 100; /**< 麦克风音量。*/
		uint32_t sourceType = 1; /**< 音频源类型。*/
		uint32_t aacProfile = 2; /**< AAC配置文件。*/
		bool needAdts = false; /**< 是否需要ADTS头。*/
		bool isHardEncode = false; /**< 是否进行硬编码。*/
		bool callbackDecodeData = false; /**< 是否回调解码数据。*/
		bool renderAudioData = true; /**< 是否渲染音频数据。*/
		bool directDecode = false; /**< 是否直接解码。*/
		uint32_t captureType = 3; /**< 捕获类型。*/

        AudioConfig() {
        }

        void assign(AudioConfig config) {
            sampleRate = config.sampleRate;
            channels = config.channels;
            framePerBuffer = config.framePerBuffer;
            bitrateInbps = config.bitrateInbps;
            audioType = config.audioType;
            micVolume = config.micVolume;
            sourceType = config.sourceType;
            aacProfile = config.aacProfile;
            needAdts = config.needAdts;
            isHardEncode = config.isHardEncode;
            callbackDecodeData = config.callbackDecodeData;
            renderAudioData = config.renderAudioData;
            directDecode = config.directDecode;
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << sampleRate << channels << framePerBuffer << bitrateInbps << audioType
                << micVolume << sourceType
                << aacProfile << needAdts << isHardEncode << callbackDecodeData << renderAudioData
                << directDecode << captureType;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> sampleRate >> channels >> framePerBuffer >> bitrateInbps >> audioType
                >> micVolume >> sourceType
                >> aacProfile >> needAdts >> isHardEncode >> callbackDecodeData >> renderAudioData
                >> directDecode >> captureType;
        }
    };

    struct CommonStatistic : ljtransfer::mediaSox::Marshallable {
        std::map<uint16_t, uint16_t> delayData;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            marshal_container(pak, delayData);
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            unmarshal_container(pak, inserter(delayData, delayData.begin()));
        }
    };

    struct VideoFrameRateControl : ljtransfer::mediaSox::Marshallable {
        uint32_t frameRate;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << frameRate;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> frameRate;
        }
    };

    struct LinkStatusEvent : ljtransfer::mediaSox::Marshallable {
        uint32_t result;

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << result;
        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> result;
        }
    };

    struct AvailableBands : ljtransfer::mediaSox::Marshallable {
        std::map<uint32_t, uint32_t> m_availableBands;

        AvailableBands() {

        }

        ~AvailableBands() {
            m_availableBands.clear();
        }

        void marshal(ljtransfer::mediaSox::Pack &pak) const {
            marshal_container(pak, m_availableBands);
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            unmarshal_container(pak, inserter(m_availableBands, m_availableBands.begin()));
        }
    };

    struct NetworkQuality : ljtransfer::mediaSox::Marshallable {
        uint8_t m_localQuality;
        uint8_t m_remoteQuality;

        NetworkQuality() {

        }

        void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << m_localQuality << m_remoteQuality;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> m_localQuality >> m_remoteQuality;
        }
    };

    struct LeaveChannelEvent : ljtransfer::mediaSox::Marshallable {
        uint8_t leaveChannel = 0;

        LeaveChannelEvent() {

        }

        ~LeaveChannelEvent() {
        }

        void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << leaveChannel;
        }

        void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> leaveChannel;
        }
    };
    enum CaptureType {
        AudioRecorder = 3,
        AAudio = 2,
        openSLES = 1,
    };

    enum AudioProfile {
        /**
         0：默认设置。
         通信场景下，该选项代表指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
         直播场景下，该选项代表指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
         */
        AUDIO_PROFILE_DEFAULT = 0,
        /**
         1：指定 32 kHz 采样率，语音编码，单声道，编码码率最大值为 18 Kbps。
         */
        AUDIO_PROFILE_SPEECH_STANDARD = 1,
        /**
         2：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 64 Kbps。
         */
        AUDIO_PROFILE_MUSIC_STANDARD = 2,
        /**
         3：指定 48 kHz采样率，音乐编码，双声道，编码码率最大值为 80 Kbps。
         */
        AUDIO_PROFILE_MUSIC_STANDARD_STEREO = 3,

        /**
         * 4：指定 48 kHz 采样率，音乐编码，单声道，编码码率最大值为 96 Kbps。
         */
        AUDIO_PROFILE_MUSIC_HIGH_QUALITY = 4,

        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 128 Kbps。
         */
        AUDIO_PROFILE_MUSIC_HIGH_QUALITY_STEREO = 5,
        /**
         * 5：指定 48 kHz 采样率，音乐编码，双声道，编码码率最大值为 80 Kbps,拿解码数据，不播放
         */
        AUDIO_PROFILE_CALLBACK_DATA_NORENDER = 6,

        AUDIO_PROFILE_NORENDER_NO_CALLBACK = 7,
    };

    enum AudioScenario {
        /**
         * 0：默认的音频应用场景。
         */
        AUDIO_SCENARIO_DEFAULT = 0,
        /**
         * 1：娱乐场景，适用于用户需要频繁上下麦的场景。
         */
        AUDIO_SCENARIO_CHATROOM_ENTERTAINMENT = 1,
        /**
         * 2：教育场景，适用于需要高流畅度和稳定性的场景。
         */
        AUDIO_SCENARIO_EDUCATION = 2,
        /**
         * 3：高音质语聊房场景，适用于音乐为主的场景。
         */
        AUDIO_SCENARIO_GAME_STREAMING = 3,
        /**
         * 4：秀场场景，适用于需要高音质的单主播场景。
         */
        AUDIO_SCENARIO_SHOWROOM = 4,
        /**
         * 5：游戏开黑场景，适用于只有人声的场景。
         */
        AUDIO_SCENARIO_CHATROOM_GAMING = 5,
        /**
         * 6: IoT（物联网）场景，适用于使用低功耗 IoT 设备的场景。
         */
        AUDIO_SCENARIO_IOT = 6,
        /**
         * 8: 会议场景，适用于人声为主的多人会议。
         */
        AUDIO_SCENARIO_MEETING = 8,
        AUDIO_SCENARIO_NUM = 9,
    };
    enum MixingAction {
        ACTION_START = 0,
        ACTION_STOP = 1,
        ACTION_PAUSE = 2,
        ACTION_RESUME = 3,
        ACTION_SET_POST = 4
        // 0 start 1 stop 2 pause 3 resume 4 set position
    };

    struct AudioEnableEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t evtType;
        bool enabled;


        AudioEnableEvent()
                : evtType((uint32_t)1000), enabled(false) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << evtType << enabled;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> evtType >> enabled;
        }
    };

    struct AudioAdjustEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t evtType;
        uint32_t val;


        AudioAdjustEvent()
                : evtType((uint32_t)1000), val(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << evtType << val;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> evtType >> val;
        }
    };

    struct AudioProfileEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t profile;
        uint32_t scenario;


        AudioProfileEvent()
                : profile(AUDIO_PROFILE_DEFAULT), scenario(0) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << profile << scenario;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> profile >> scenario;
        }
    };

    struct AudioVolumeIndicationEvent
            : public ljtransfer::mediaSox::Marshallable {
        uint32_t interval;
        uint32_t smooth;
        bool reportVad;

        void assign(AudioVolumeIndicationEvent event) {
            interval = event.interval;
            smooth = event.smooth;
            reportVad = event.reportVad;
        }

        AudioVolumeIndicationEvent()
                : interval(0), smooth(0), reportVad(false) {
        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << interval << smooth << reportVad;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> interval >> smooth >> reportVad;
        }
    };

    class RTCAudioConfiguration {
    public:
        int audioProfile = AUDIO_PROFILE_DEFAULT;
        int audioScenario;

        AudioConfig *createAudioConfig() {
            AudioConfig *audioConfig = new AudioConfig();
            audioConfig->bitrateInbps = 64000;
            audioConfig->framePerBuffer = 480; //
            audioConfig->sampleRate = 48000;
            audioConfig->channels = 1;
            audioConfig->audioType = 1;
            audioConfig->isHardEncode = false;
            audioConfig->needAdts = false;
            if (audioProfile == AUDIO_PROFILE_DEFAULT) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 1;
                audioConfig->bitrateInbps = 80000;
            } else if (audioProfile == AUDIO_PROFILE_MUSIC_STANDARD_STEREO) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 2;
                audioConfig->bitrateInbps = 80000;
            } else if (audioProfile == AUDIO_PROFILE_CALLBACK_DATA_NORENDER) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 2;
                audioConfig->bitrateInbps = 80000;
                audioConfig->callbackDecodeData = true;
                audioConfig->renderAudioData = false;
                audioConfig->directDecode = true;
            }else if (audioProfile == AUDIO_PROFILE_NORENDER_NO_CALLBACK) {
                audioConfig->sampleRate = 48000;
                audioConfig->channels = 2;
                audioConfig->bitrateInbps = 80000;
                audioConfig->callbackDecodeData = false;
                audioConfig->renderAudioData = false;
                audioConfig->directDecode = false;
            }
            return audioConfig;
        }
    };

    struct EnumerateAudioDevicesEvent : public ljtransfer::mediaSox::Marshallable {
        uint32_t type = 0; // 0 Record devices 1 play devices

        EnumerateAudioDevicesEvent() : type(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type;
        }
    };

    struct AudioDevice : public ljtransfer::mediaSox::Marshallable {
        std::string name;
        uint32_t id;

        AudioDevice() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << name << id;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> name >> id;
        }
    };

    struct AudioDevicesEvent : public ljtransfer::mediaSox::Marshallable {
        std::vector<AudioDevice> devices;

        AudioDevicesEvent() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            marshal_container(pak, devices);

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            unmarshal_container(pak, inserter(devices, devices.begin()));
        }
    };

    struct SetDeviceInfoEvent : public ljtransfer::mediaSox::Marshallable {
        AudioDevice audioDevice;
        uint32_t type;

        SetDeviceInfoEvent() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type << audioDevice;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type >> audioDevice;
        }
    };

    struct MuteDeviceEvent : public ljtransfer::mediaSox::Marshallable {
        bool isMute;
        uint32_t type;

        MuteDeviceEvent() {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << type << isMute;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> type >> isMute;
        }
    };

    struct AudioMixingEvent : public ljtransfer::mediaSox::Marshallable {
        std::string filePath;
        bool loopback;
        uint32_t cycle;
        uint32_t startPos;
        uint32_t acton; // 0 start 1 stop 2 pause 3 resume 4 set position

        AudioMixingEvent() : filePath(""), loopback(false), startPos(0), cycle(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << filePath << loopback << cycle << startPos << acton;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> filePath >> loopback>> cycle >> startPos >> acton;
        }
    };

    struct AudioVolumeEvent : public ljtransfer::mediaSox::Marshallable {
        uint32_t uid;
        uint32_t volume;
        std::string channelId;


        AudioVolumeEvent() : uid(0), volume(0), channelId("") {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << uid << volume << channelId;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> uid >> volume >> channelId;
        }
    };

    struct DefaultEvent : public ljtransfer::mediaSox::Marshallable {
        uint32_t enable;


        DefaultEvent() : enable(0) {

        }

        virtual void marshal(ljtransfer::mediaSox::Pack &pak) const {
            pak << enable;

        }

        virtual void unmarshal(const ljtransfer::mediaSox::Unpack &pak) {
            pak >> enable;
        }
    };
}
#endif //LJSDK_TRANSCONSTANTS_H

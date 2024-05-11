using LJ.RTC.Common;

namespace LJ.RTC.Video
{

    public enum MIRROR_TYPE {
        MIRROR_NONE = 0,
        MIRROR_Y = 1,
        MIRROR_X = 2,
    }

    public enum ProgramType
    {
        TEXTURE_2D = 0,
        TEXTURE_2D_SRGB = 1,
        TEXTURE_EXT = 2,
        TEXTURE_WATERMARK =3,
        TEXTURE_YUV = 4,
    };

    public enum CropType
    {
        BOTTOM_LEFT = 0,
        TOP_LEFT = 1,
    };
    public class CaptureVideoFrame : HPMarshaller
    {

        ///
        /// <summary>
        /// The video type. See VIDEO_BUFFER_TYPE .
        /// </summary>
        ///
        public VIDEO_BUFFER_TYPE type;

        ///
        /// <summary>
        /// The pixel format. See VIDEO_PIXEL_FORMAT .
        /// </summary>
        ///
        public VIDEO_PIXEL_FORMAT format;

        ///
        /// <summary>
        /// Video frame buffer.
        /// </summary>
        ///
        public byte[] buffer;

        ///
        /// <summary>
        /// Line spacing of the incoming video frame, which must be in pixels instead of bytes. For textures, it is the width of the texture.
        /// </summary>
        ///
        public int stride;

        public int width;
        ///
        /// <summary>
        /// Height of the incoming video frame.
        /// </summary>
        ///
        public int height;

        ///
        /// <summary>
        /// Raw data related parameter 0~1. The number of pixels trimmed from the left. The default value is 0.
        /// </summary>
        ///
        public float cropLeft;

        ///
        /// <summary>
        /// Raw data related parameter0~1. The number of pixels trimmed from the top. The default value is 0.
        /// </summary>
        ///
        public float cropTop;

        ///
        /// <summary>
        /// Raw data related parameter0~1. The number of pixels trimmed from the right. The default value is 0.
        /// </summary>
        ///
        public float cropRight;

        ///
        /// <summary>
        /// Raw data related parameter0~1. The number of pixels trimmed from the bottom. The default value is 0.
        /// </summary>
        ///
        public float cropBottom;

        ///
        /// <summary>
        /// Raw data related parameter. The clockwise rotation of the video frame. You can set the rotation angle as 0, 90, 180, or 270. The default value is 0.
        /// </summary>
        ///
        public int rotation;

        ///
        /// <summary>
        /// Timestamp (ms) of the incoming video frame. An incorrect timestamp results in frame loss or unsynchronized audio and video.
        /// </summary>
        ///
        public long timestamp;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format.When using the OpenGL interface (javax.microedition.khronos.egl.*) defined by Khronos, set eglContext to this field.When using the OpenGL interface (android.opengl.*) defined by Android, set eglContext to this field.
        /// </summary>
        ///
        public byte[] eglContext;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. Texture ID of the frame.
        /// </summary>
        ///
        public EGL_CONTEXT_TYPE eglType;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. Incoming 4 x 4 transformational matrix. The typical value is a unit matrix.
        /// </summary>
        ///
        public int textureId;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. The MetaData buffer. The default value is NULL.
        /// </summary>
        ///
        public byte[] metadata_buffer;

        ///
        /// <summary>
        /// This parameter only applies to video data in Texture format. The MetaData size. The default value is 0.
        /// </summary>
        ///
        public int metadata_size;

        public int mirror = (int)MIRROR_TYPE.MIRROR_NONE;
        public int programType = (int)ProgramType.TEXTURE_2D_SRGB;
        public int corpType = (int)CropType.TOP_LEFT;

        public override byte[] marshall()
        {
            pushInt((int)type);
            pushInt((int)format);
            pushInt(stride);
            pushInt(width);
            pushInt(height);
            pushInt(rotation);
            pushInt((int)eglType);
            pushInt(textureId);
            pushInt64(timestamp);
            pushInt(metadata_size);
            pushBytes32(metadata_buffer);
            pushInt(mirror);
            pushInt((int)(cropLeft * 10000));
            pushInt((int)(cropRight * 10000));
            pushInt((int)(cropTop * 10000));
            pushInt((int)(cropBottom * 10000));
            pushInt(programType);
            pushInt(corpType);
            return base.marshall();
        }

        public override void unmarshall(byte[] buf)
        {
            base.unmarshall(buf);
            type = (VIDEO_BUFFER_TYPE)popInt();
            format = (VIDEO_PIXEL_FORMAT)popInt();
            stride = popInt();
            width = popInt();
            height = popInt();
            rotation = popInt();
            eglType = (EGL_CONTEXT_TYPE)popInt();
            textureId = popInt();
            timestamp = popInt64();
            metadata_size = popInt();
            metadata_buffer = popBytes32();
            mirror = popInt();
            cropLeft = popInt();
            cropRight = popInt();
            cropTop = popInt();
            cropBottom = popInt();
            programType = popInt();
            corpType = popInt();
        }
    }

    public class GetCameraDeviceBusyInfo : HPMarshaller
    {
       string deviceName;
        public GetCameraDeviceBusyInfo(string name)
        {
            deviceName = name;
        }

        public override byte[] marshall()
        {
            pushString16(deviceName);
            return base.marshall();
        }
    };

    public class GetCameraDeviceBusyInfoResult : HPMarshaller
    {
        public bool isBusy = false;
        public string processName;

        public GetCameraDeviceBusyInfoResult() {
        }
        public override void unmarshall(byte[] buf) {
            base.unmarshall(buf);
            isBusy = popBool();
            processName = popString16();
        }

    };

///
/// @ignore
///
public enum EGL_CONTEXT_TYPE
    {
        ///
        /// @ignore
        ///
        EGL_CONTEXT10 = 0,

        ///
        /// @ignore
        ///
        EGL_CONTEXT14 = 1,
    };

    ///
    /// @ignore
    ///
    public enum CAPTURE_MIRROR_TYPE
    {
        ///
        /// @ignore
        ///
        NOT_MIRROR = 0, //非镜像，举右手，预览显示手在右边

        ///
        /// @ignore
        ///
        MIRROR = 1, ////镜像，举右手，预览显示手在左边
    };

    public enum VIDEO_BUFFER_TYPE
    {
        ///
        /// <summary>
        /// 1: The video buffer in the format of raw data.
        /// </summary>
        ///
        VIDEO_BUFFER_RAW_DATA = 1,

        ///
        /// <summary>
        /// 2: The video buffer in the format of raw data.
        /// </summary>
        ///
        VIDEO_BUFFER_ARRAY = 2,

        ///
        /// <summary>
        /// 3: The video buffer in the format of Texture.
        /// </summary>
        ///
        VIDEO_BUFFER_TEXTURE = 3,
    };

    /// <summary>
    /// The video pixel format.
    /// </summary>
    ///
    public enum VIDEO_PIXEL_FORMAT
    {
        ///
        /// <summary>
        /// 0: Raw video pixel format.
        /// </summary>
        ///
        VIDEO_PIXEL_DEFAULT = 0,

        ///
        /// <summary>
        /// 1: The format is I420.
        /// </summary>
        ///
        VIDEO_PIXEL_I420 = 1,

        ///
        /// @ignore
        ///
        VIDEO_PIXEL_BGRA = 2,

        ///
        /// @ignore
        ///
        VIDEO_PIXEL_NV21 = 3,

        ///
        /// <summary>
        /// 4: The format is RGBA.
        /// </summary>
        ///
        VIDEO_PIXEL_RGBA = 4,

        ///
        /// <summary>
        /// 8: The format is NV12.
        /// </summary>
        ///
        VIDEO_PIXEL_NV12 = 8,

        ///
        /// @ignore
        ///
        VIDEO_TEXTURE_2D = 10,

        ///
        /// @ignore
        ///
        VIDEO_TEXTURE_OES = 11,

        ///
        /// @ignore
        ///
        VIDEO_CVPIXEL_NV12 = 12,

        ///
        /// @ignore
        ///
        VIDEO_CVPIXEL_I420 = 13,

        ///
        /// @ignore
        ///
        VIDEO_CVPIXEL_BGRA = 14,

        ///
        /// <summary>
        /// 16: The format is I422.
        /// </summary>
        ///
        VIDEO_PIXEL_I422 = 16,

        VIDEO_PIXEL_RGB24 = 17,

        VIDEO_PIXEL_H264 = 18,
    };

    public enum CAMERA_CAPTURE_ERROR
    { 
        SUCCESS = 0,
        DEVICE_NOT_FOUND = -1,
        PERMISSION_DENICE = -2,
        CAMERA_PLAY_ERROR = -3,
        NO_CAMERA_DEVICE = -4,
        PARAM_ERROR = -5,
        CAMERA_BUSY = -6,
    }
    public enum CAMERA_ACTION
    {
        ACTION_START = 0,
        ACTION_STOP = 1,
    }
    public class VideoFrame
    {
        public int format;
        public int type;
        public int width;
        public int height;
        public int yStride;
        public int uStride;
        public int vStride;
        public int rotation;
        public long renderTimeMs;
        public int avsync_type;
        public int textureId;
        public byte[] data;
        public int codecType; // 0 H264 1 H265
        public int frameType;

        public VideoFrame()
        {

        }

        public VideoFrame(int textureId, int width, int height, long renderTimeMs)
        {
            this.textureId = textureId;
            this.width = width;
            this.height = height;
            this.renderTimeMs = renderTimeMs;
        }

        public VideoFrame(int type, int width, int height, int yStride, int uStride, int vStride, int rotation, long renderTimeMs, int avsync_type)
        {
            this.type = type;
            this.width = width;
            this.height = height;
            this.yStride = yStride;
            this.uStride = uStride;
            this.vStride = vStride;

            this.rotation = rotation;
            this.renderTimeMs = renderTimeMs;
            this.avsync_type = avsync_type;
        }
    }


}

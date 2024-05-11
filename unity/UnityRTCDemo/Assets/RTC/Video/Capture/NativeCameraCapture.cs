using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using LJ.RTC.Common;

namespace LJ.RTC.Video
{
    class CaptureConfig : HPMarshaller
    {

        public int width = 640;
        public int height = 480;
        public int fps = 30;
        public int oriMode = 0;
        public int fillMode = 0;

        public int screenOriMode = 0;

        public override byte[] marshall()
        {
            pushInt(width);
            pushInt(height);
            pushInt(fps);
            pushInt(oriMode);
            pushInt(fillMode);
            pushInt(screenOriMode);

            return base.marshall();
        }
    }
    internal class NativeCameraCapture : ICameraCapture
    {

        string activeCameraDeviceName;

        VideoConfig mVideoConfig;

        private RawImage mRender;

        protected bool m_hasAttach = false;

        private Texture2D rgb_texture;

        private Texture2D rgba_texture;

        private Texture2D m_YImageTex;
        private Texture2D m_UVImageTex;

        private int mPixelFormat;
        private int mCaptureWidth;
        private int mCaptureHeight;

        private byte[] mCaptureBuffer;
        private byte[] mUVBuffer;

        private Material mCacheRenderMaterial = null;

        private CaptureVideoFrame mCaptureVideoFrame;

        //private CaptureVideoFrame mCacheVideoFrame;

        private OnCameraParamCallback mCameraParamCallback;

        private RtcEngineNavite mRtcEngineNavite;


        private ReaderWriterLockSlim rwlock;

        public NativeCameraCapture(VideoConfig videoConfig, RtcEngineNavite rtcEngineNavite)
        {
            this.mVideoConfig = videoConfig;
            this.mRtcEngineNavite = rtcEngineNavite;
            this.rwlock = new System.Threading.ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public override void OnCreate()
        {
            mCaptureVideoFrame = new CaptureVideoFrame();
        }

        public override void OnDestroy()
        {
            StopPreview();
            if (mRender != null)
            {
                mRender.material = mCacheRenderMaterial;
            }
            mRender = null;
            mCameraParamCallback = null;
        }

        /// TODO handle the camera play error
        // 需要考虑虚拟摄像头的处理
        public override CAMERA_CAPTURE_ERROR StartPreview()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                JLog.Error("StartPreview do not has webCam permission");
                return CAMERA_CAPTURE_ERROR.PERMISSION_DENICE;
            }
            if (mVideoConfig == null)
            {
                JLog.Error("mVideoConfig == null");
                return CAMERA_CAPTURE_ERROR.PARAM_ERROR;
            }

            if (mRtcEngineNavite == null)
            {
                JLog.Error("StartPreview mRtcEngineNavite == null");
                return CAMERA_CAPTURE_ERROR.PARAM_ERROR;
            }

            VideoDeviceManagerFactory.GetDeviceManager().GetDevice(ref activeCameraDeviceName);
            JLog.Debug("StartCameraDevice" + activeCameraDeviceName);
            startCameraCapture();
            mIsStop = false;

            return CAMERA_CAPTURE_ERROR.SUCCESS;
        }

        private void startCameraCapture()
        {
            CaptureConfig config = new CaptureConfig();
            config.width = mVideoConfig.encodeWidth;
            config.height = mVideoConfig.encodeHeight;
            config.fps = 30;
            config.fillMode = (int)mVideoConfig.fillMode;
            config.oriMode = mVideoConfig.orientationMode;
            int oriMode = 0;
            if (mVideoConfig.mScreenOrientation == (int)ScreenOrientation.LandscapeLeft)
            {
                oriMode = 1;
            }
            else if (mVideoConfig.mScreenOrientation == (int)ScreenOrientation.LandscapeRight)
            {
                oriMode = 2;
            }
            config.screenOriMode = oriMode;
            mRtcEngineNavite.StartVideoCaptureWithConfig(activeCameraDeviceName, config.HPmarshall());
            mRtcEngineNavite.SubscribeCaptureVideo(onVideoCapture, this);
        }

        public static void onVideoCapture(IntPtr buf, Int32 len, Int32 width, Int32 height, int pixel_fmt, System.Object context)
        {
            NativeCameraCapture instance = (NativeCameraCapture)context;
            if (instance.mIsStop)
            {
                return;
            }

            instance.rwlock.EnterWriteLock();
            instance.mCaptureVideoFrame.width = width;
            instance.mCaptureVideoFrame.height = height;
            instance.mCaptureVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
#if UNITY_EDITOR || UNITY_STANDALONE
            instance.mCaptureVideoFrame.rotation = 180;
#endif
            if (pixel_fmt == 4)
            {
                if (instance.mCaptureBuffer == null || instance.mCaptureBuffer.Length != len)
                {

                    instance.mPixelFormat = pixel_fmt;
                    instance.mCaptureBuffer = new byte[len];
                    instance.mCaptureWidth = width;
                    instance.mCaptureHeight = height;

                    if (instance.mCameraParamCallback != null)
                    {
                        instance.mCameraParamCallback(width, height, instance.mVideoConfig.cameraFacing, 0);
                    }
                    instance.mCaptureVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGB24;
                }

                Marshal.Copy(buf, instance.mCaptureBuffer, 0, len);

                //flip red and blue
                byte[] mCachePixel = instance.mCaptureBuffer;
                for (int i = 0; i < len; i += 3)
                {
                    byte tmp = mCachePixel[i];
                    mCachePixel[i] = mCachePixel[i + 2];
                    mCachePixel[i + 2] = tmp;
                }
            }
            else if (pixel_fmt == 1)
            {
                if (instance.mCaptureBuffer == null || instance.mCaptureBuffer.Length != len)
                {

                    instance.mPixelFormat = pixel_fmt;
                    instance.mCaptureBuffer = new byte[len];
                    instance.mCaptureWidth = width;
                    instance.mCaptureHeight = height;
                }
                Marshal.Copy(buf, instance.mCaptureBuffer, 0, len);
                instance.mCaptureVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
            }
            else if (pixel_fmt == 5)
            {
                int ySize = width * height;

                if (instance.mCaptureBuffer == null || instance.mCaptureBuffer.Length != ySize)
                {
                    instance.mPixelFormat = pixel_fmt;
                    instance.mCaptureWidth = width;
                    instance.mCaptureHeight = height;

                    instance.mCaptureBuffer = new byte[ySize];
                    instance.mUVBuffer = new byte[ySize / 2];
                }

                Marshal.Copy(buf, instance.mCaptureBuffer, 0, ySize);
                Marshal.Copy(buf + ySize, instance.mUVBuffer, 0, ySize / 2);
                instance.mCaptureVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_CVPIXEL_NV12;
            }
            instance.rwlock.ExitWriteLock();
        }

        public override CAMERA_CAPTURE_ERROR StartCameraDevice(string cameraDeviceName)
        {
            JLog.Debug("StartCameraDevice" + cameraDeviceName);

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                JLog.Error("StartPreview do not has webCam permission");
                return CAMERA_CAPTURE_ERROR.PERMISSION_DENICE;
            }

            if (cameraDeviceName.Equals(activeCameraDeviceName))
            {
                return CAMERA_CAPTURE_ERROR.SUCCESS;
            }

            if (mVideoConfig == null)
            {
                JLog.Error("mVideoConfig == null");
                return CAMERA_CAPTURE_ERROR.PARAM_ERROR;
            }

            activeCameraDeviceName = cameraDeviceName;
            startCameraCapture();
            mIsStop = false;
            return CAMERA_CAPTURE_ERROR.SUCCESS;
        }

        private CAMERA_CAPTURE_ERROR SetActiveCamera()
        {
            //activeCameraTexture = cameraToUse;
            try
            {
                SetRender(mRender, mCameraParamCallback);
            }
            catch (Exception e)
            {
                /// TODO handle the camera play error
                // 需要考虑虚拟摄像头的处理
                //JLog.Error("deviceName:" + activeCameraTexture.deviceName);
                JLog.Error(e.ToString());

                StopPreview();
                return CAMERA_CAPTURE_ERROR.CAMERA_PLAY_ERROR;
            }

            //InitTime(mVideoConfig.frameRate);

            //JLog.Info("..." + activeCameraTexture.width + ":" + activeCameraTexture.height + ":" + Screen.width + ":" + Screen.height);
            return CAMERA_CAPTURE_ERROR.SUCCESS;
        }

        private bool mIsStop = false;
        public override void StopPreview()
        {
            mIsStop = true;
            if (mRtcEngineNavite == null)
            {
                JLog.Error("StopPreview mRtcEngineNavite == null");
                return;
            }

            mRtcEngineNavite.StopVideoCapture();

            if (rgb_texture != null)
            {
                UnityEngine.Object.Destroy(rgb_texture);
                rgb_texture = null;
            }

            if (m_YImageTex != null)
            {
                UnityEngine.Object.Destroy(m_YImageTex);
                m_YImageTex = null;

                UnityEngine.Object.Destroy(m_UVImageTex);
                m_UVImageTex = null;
            }

            if (rgba_texture != null)
            {
                UnityEngine.Object.Destroy(rgba_texture);
                rgba_texture = null;
            }
        }

        public override void SetRender(Component component, OnCameraParamCallback callback)
        {
            if (component == null)
            {
                JLog.Error("SetRender component == null");
                return;
            }
            if (component is RawImage)
            {
                mRender = component as RawImage;
                mCacheRenderMaterial = mRender.material;
                mCameraParamCallback = callback;
            }
            else
            {
                JLog.Error("SetRender component is not RawImage");
            }
        }

        private void SetRenderInfo(RawImage render, int width, int height)
        {
            RectTransform rectTransform = mRender.transform.GetComponent<RectTransform>();
            Vector3 scale = rectTransform.localScale;
            scale.x *= scale.x < 0 ? 1 : -1;
            scale.y = Math.Abs(scale.y);
            //scale.x *= scale.x < 0 ? -1 : 1;
            //scale.y *= scale.y < 0 ? 1 : -1;
            int renderWidth = (int)rectTransform.sizeDelta.x;
            int renderHeight = (int)rectTransform.sizeDelta.y;
            rectTransform.localScale = scale;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
#if (UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
            if (mVideoConfig.mScreenOrientation == (int)ScreenOrientation.Portrait) {
                rectTransform.sizeDelta = new Vector2(Math.Min(renderWidth, renderHeight), Math.Max(renderWidth, renderHeight));
                
            } else {
                rectTransform.sizeDelta = new Vector2(Math.Max(renderWidth, renderHeight), Math.Min(renderWidth, renderHeight));
            }
            JLog.Debug("SetRenderInfo change sizeDelta :" + rectTransform.sizeDelta.x + ":" + rectTransform.sizeDelta.y);
#endif

            float retRatio = rectTransform.sizeDelta.x / rectTransform.sizeDelta.y;
            float textureRatio = 1f * width / (1f * height);
            Rect rect = new Rect(0, 0, 1, 1);
            if (textureRatio < retRatio)
            {
                rect.height = textureRatio / retRatio;
                rect.y = (1 - rect.height) * 0.5f;
            }
            else
            {
                rect.width = retRatio / textureRatio;
                rect.x = (1 - rect.width) * 0.5f;
            }
            render.uvRect = rect;
        }

        // 这是个耗时方法，GetPixels 耗时会比较大
        private int index = 0;
        public override Texture ReadCameraPixel(bool encode)
        {
            if (mIsStop)
            {
                return null;
            }
            if (mRender == null)
            {
                //JLog.Debug("mRender == null");
                return null;
            }

            if (mCaptureBuffer == null)
            {
                //JLog.Debug("mCaptureBuffer == null");
                return null;
            }

            rwlock.EnterReadLock();

            //int len = mCaptureBuffer.Length;
            mCaptureVideoFrame.timestamp = TimeHelper.GetMillSecond();
            if (mPixelFormat == 4) //rgb24
            {
                if (rgb_texture == null)
                {
                    rgb_texture = new Texture2D(mCaptureWidth, mCaptureHeight, TextureFormat.RGB24, false);
                    mRender.texture = rgb_texture;
                    mRender.material.mainTexture = rgb_texture;
                    SetRenderInfo(mRender, rgb_texture.width, rgb_texture.height);
                    mCameraParamCallback(mCaptureWidth, mCaptureHeight, mVideoConfig.cameraFacing, 0);
                }

                rgb_texture.LoadRawTextureData(mCaptureBuffer);
                rgb_texture.Apply();
                if (HasCallback())
                {
                    mCaptureVideoFrame.buffer = mCaptureBuffer;
                    InvokeFrameEvent(mCaptureVideoFrame, false);
                }
            }
            else if (mPixelFormat == 1) //rgba
            {
                if (rgba_texture == null)
                {
                    rgba_texture = new Texture2D(mCaptureWidth, mCaptureHeight, TextureFormat.RGBA32, false);
                    mRender.texture = rgba_texture;
                    mRender.material.mainTexture = rgba_texture;
                    SetRenderInfo(mRender, rgba_texture.width, rgba_texture.height);
                    mCameraParamCallback(mCaptureWidth, mCaptureHeight, mVideoConfig.cameraFacing, 0);
                }
                rgba_texture.LoadRawTextureData(mCaptureBuffer);
                rgba_texture.Apply();
                if (HasCallback())
                {
                    mCaptureVideoFrame.buffer = mCaptureBuffer;
                    InvokeFrameEvent(mCaptureVideoFrame, false);
                }
            }
            else if (mPixelFormat == 5)//nv12
            {
                if (m_YImageTex == null)
                {
                    m_YImageTex = new Texture2D(mCaptureWidth, mCaptureHeight, TextureFormat.Alpha8, false);
                    m_UVImageTex = new Texture2D(mCaptureWidth / 2, mCaptureHeight / 2, TextureFormat.RGBA4444, false);

                    mRender.material = new Material(Shader.Find("UI/RendererShader602"));
                    mRender.material.mainTexture = m_YImageTex;
                    mRender.material.SetTexture("_UVTex", m_UVImageTex);
                    mRender.material.SetFloat("_RotationAngle", 0.0f);
                    mCameraParamCallback(m_YImageTex.width, m_YImageTex.height, mVideoConfig.cameraFacing, 0);
                    SetRenderInfo(mRender, m_YImageTex.width, m_YImageTex.height);
                }

                m_YImageTex.LoadRawTextureData(mCaptureBuffer);
                m_UVImageTex.LoadRawTextureData(mUVBuffer);

                m_YImageTex.Apply();
                m_UVImageTex.Apply();
                if (HasCallback())
                {
                    if (mCaptureVideoFrame.buffer == null)
                    {
                        mCaptureVideoFrame.buffer = new byte[mCaptureBuffer.Length + mUVBuffer.Length];
                    }
                    Buffer.BlockCopy(mCaptureBuffer, 0, mCaptureVideoFrame.buffer, 0, mCaptureBuffer.Length);
                    Buffer.BlockCopy(mUVBuffer, 0, mCaptureVideoFrame.buffer, mCaptureBuffer.Length, mUVBuffer.Length);
                    InvokeFrameEvent(mCaptureVideoFrame, false);
                }
            }

            rwlock.ExitReadLock();

            return null;
        }
    }
}
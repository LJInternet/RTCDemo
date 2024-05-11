using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LJ.RTC.Common;
using LJ.RTC;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering;
using System.IO;
using Unity.Collections;
using UnityEngine.Rendering;
using System.Collections.Concurrent;

namespace LJ.RTC.Video
{
    enum FITMODE
    {
        FIT_X = 2,
        FIT_Y = 1,
        FIT_NONE = 0,
    }

    internal class CameraCapture : ICameraCapture
    {
        WebCamDevice frontCameraDevice;
        WebCamDevice backCameraDevice;
        WebCamDevice activeCameraDevice;

        WebCamTexture activeCameraTexture;

        VideoConfig mVideoConfig;

        private RawImage mRender;

        private Texture2D mResizeTexture;

        private Rect mPixelRect;

        private byte[] mCachePixel;
        private Color32[] mCacheColor;
        private CaptureVideoFrame mCacheVideoFrame;

        private OnCameraParamCallback mCameraParamCallback;

        private RenderTexture mRenderTexture;

        private Rect mResizeRect;
        private Rect mCorpRect;
        private Rect mEncodeRect;

        private Texture2D mFitXYTexture;
        private bool mFitXYColorInited = false;
        private FITMODE mFitMode = FITMODE.FIT_NONE;
        private int mCopyOffset = 0;
        private bool isStop = false;

        RenderTexture mCameraRenderTexture = null;
        private Texture2D mIosTexture2D = null;
        private ConcurrentDictionary<AsyncGPUReadbackRequest, long> mTimeDictionary = new ConcurrentDictionary<AsyncGPUReadbackRequest, long>();
        private bool isFitXY()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
            return false;
#else
            return mVideoConfig.fillMode == FILL_MODE.FIT_XY;
#endif

        }

        private bool FitX()
        {
            return mFitMode == FITMODE.FIT_X;
        }

        public CameraCapture(VideoConfig videoConfig)
        {
            mVideoConfig = videoConfig;
            mVideoCaptureTracker.Init(6);
        }

        public override void OnCreate() 
        {

        }

        public override void OnDestroy()
        {
            StopPreview();
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
            if (WebCamTexture.devices.Length == 0)
            {
                JLog.Error("No devices cameras found");
                return CAMERA_CAPTURE_ERROR.NO_CAMERA_DEVICE;
            }
            if (mVideoConfig == null)
            {
                JLog.Error("mVideoConfig == null");
                return CAMERA_CAPTURE_ERROR.PARAM_ERROR;
            }
            if (UnityEngine.Object.ReferenceEquals(activeCameraDevice, null))
            {
                JLog.Error("activeCameraDevice == null");
                return CAMERA_CAPTURE_ERROR.SUCCESS;
            }

            // Get the device's cameras and create WebCamTextures with them
            frontCameraDevice = WebCamTexture.devices.Last();
            backCameraDevice = WebCamTexture.devices.First();
            WebCamTexture camTexture = null;
            string deviceName = "";
            VideoDeviceManagerFactory.GetDeviceManager().GetDevice(ref deviceName);
            JLog.Error("GetDeviceManager deviceName:" + deviceName);
            if (deviceName == "")
            {
                if (mVideoConfig.cameraFacing == (int)CameraFaceType.FRONT)
                {
                    deviceName = frontCameraDevice.name;
                }
                else
                {
                    deviceName = backCameraDevice.name;
                }
            }
            int fps = 30;
#if UNITY_EDITOR || UNITY_STANDALONE
            //fps = 60;
#endif
            camTexture = new WebCamTexture(deviceName, mVideoConfig.previewWidth, mVideoConfig.previewHeight, fps);

            JLog.Error("deviceName:" + deviceName + " fps:" + fps + "WxH:" + mVideoConfig.previewWidth + ":" + mVideoConfig.previewHeight);
            // Set camera filter modes for a smoother looking image
            camTexture.filterMode = FilterMode.Trilinear;

            return SetActiveCamera(camTexture);
        }

        public override CAMERA_CAPTURE_ERROR StartCameraDevice(string cameraDeviceName)
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                JLog.Error("StartPreview do not has webCam permission");
                return CAMERA_CAPTURE_ERROR.PERMISSION_DENICE;
            }
            if (activeCameraDevice.name == cameraDeviceName)
            {
                return CAMERA_CAPTURE_ERROR.SUCCESS;
            }

            WebCamDevice webCamDevice = WebCamTexture.devices.Last();
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                if (string.Equals(device.name, cameraDeviceName))
                {
                    webCamDevice = device;
                    break;
                }
            }

            if (webCamDevice.name != cameraDeviceName)
            {
                return CAMERA_CAPTURE_ERROR.DEVICE_NOT_FOUND;
            }
            int fps = 30;
#if UNITY_EDITOR || UNITY_STANDALONE
            //fps = 60;
#endif
            JLog.Error("StartCameraDevice fps:" + fps + "WxH:" + mVideoConfig.previewWidth + ":" + mVideoConfig.previewHeight);
            WebCamTexture camTexture = new WebCamTexture(webCamDevice.name, mVideoConfig.previewWidth, mVideoConfig.previewHeight, fps);
            camTexture.filterMode = FilterMode.Trilinear;
            return SetActiveCamera(camTexture);
        }

        public CAMERA_CAPTURE_ERROR SetActiveCamera(WebCamTexture cameraToUse)
        {
            isStop = false;
            activeCameraTexture = cameraToUse;
            activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
                device.name == cameraToUse.deviceName);
            try
            {
                activeCameraTexture.Play();
                JLog.Error("StartCameraDevice isPlaying:" + activeCameraTexture.isPlaying);
                if (!activeCameraTexture.isPlaying)
                {
                    return CAMERA_CAPTURE_ERROR.CAMERA_PLAY_ERROR;
                }
                SetRender(mRender, mCameraParamCallback);
            }
            catch (Exception e)
            {
                /// TODO handle the camera play error
                // 需要考虑虚拟摄像头的处理
                JLog.Error("deviceName:" + activeCameraTexture.deviceName);
                JLog.Error(e.ToString());

                StopPreview();
                return CAMERA_CAPTURE_ERROR.CAMERA_PLAY_ERROR;
            }
            int encodeWidth = mVideoConfig.encodeWidth;
            int encodeHeight = mVideoConfig.encodeHeight;
            mPixelRect = new Rect(0, 0, encodeWidth, encodeHeight);
            mCachePixel = new byte[encodeWidth * encodeHeight * 4];
            mFitXYColorInited = false;

            JLog.Info("CAMERA..." + activeCameraTexture.width + ":" + activeCameraTexture.height
                + ":" + Screen.width + ":" + Screen.height + ":" + activeCameraTexture.videoVerticallyMirrored 
                + ":" + activeCameraTexture.videoRotationAngle);
            return CAMERA_CAPTURE_ERROR.SUCCESS;
        }

        public override void StopPreview()
        {
            isStop = true;
            if (activeCameraTexture != null)
            {
                if (activeCameraTexture.isPlaying)
                {
                    activeCameraTexture.Stop();
                }
                UnityEngine.Object.Destroy(activeCameraTexture);
                activeCameraTexture = null;
            }

            if (mResizeTexture != null)
            {
                UnityEngine.Object.Destroy(mResizeTexture);
                mResizeTexture = null;
            }
            if (mFitXYTexture != null)
            {
                UnityEngine.Object.Destroy(mFitXYTexture);
                mFitXYTexture = null;
            }
            if (mCameraRenderTexture != null)
            {
                mCameraRenderTexture.Release();
                mCameraRenderTexture = null;
            }
            if (mIosTexture2D != null) {
                UnityEngine.Object.Destroy(mIosTexture2D);
                mIosTexture2D = null;
            }
            try
            {
                if (mRenderTexture != null)
                {
                    if (RenderTexture.active == mRenderTexture)
                    {
                        RenderTexture.active = null;
                    }
                    mRenderTexture.Release();
                    mRenderTexture = null;
                }

            }
            catch (Exception e)
            {

            }

            mTimeDictionary.Clear();
        }

        public override void SetRender(Component component, OnCameraParamCallback callback)
        {
            if (component == null)
            {
                JLog.Error("SetRender component == null");
            }
            mCameraParamCallback = callback;
            if (component != null && component is RawImage)
            {
                mRender = component as RawImage;
            }
            else
            {
                JLog.Error("SetRender component is not RawImage");
            }
            if (activeCameraTexture == null)
            {
                JLog.Error("SetRender activeCameraTexture == null");
                return;
            }

            if (!activeCameraTexture.isPlaying)
            {
                activeCameraTexture.Play();
                if (!activeCameraTexture.isPlaying)
                {
                    return;
                }
            }
            if (isFitXY())
            {
                SetRenderInfo(mRender, mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
            }
            else
            {
                SetRenderInfo(mRender, activeCameraTexture.width, activeCameraTexture.height);
            }
        }

        private RectTransform renderRectTransform;
        private int mCameraRotation = 0;
        private float mRenderWidth = 0;
        private float mRenderHeight = 0;

        private float mCameraWidth = 0;
        private float mCameraHeight = 0;


        private void SetRenderInfo(RawImage render, int srcW, int srcH)
        {
            if (!isChangeHVOK()) {
                return;
            }
            if (renderRectTransform == null && render != null)
            {
                renderRectTransform = render.transform.GetComponent<RectTransform>();
            }
            int textureWidth = activeCameraTexture.width;
            int textureHeight = activeCameraTexture.height;
            int textureRotation = activeCameraTexture.videoRotationAngle;
            int renderWidth = renderRectTransform == null ? textureWidth : (int)renderRectTransform.sizeDelta.x;
            int renderHeight = renderRectTransform == null ? textureHeight : (int)renderRectTransform.sizeDelta.y;
            if (textureRotation == mCameraRotation && mRenderWidth == 
                renderWidth && mRenderHeight == renderHeight
                && textureWidth == mCameraWidth && textureHeight == mCameraHeight)
            {
                return;
            }
            bool needCalculateResize = textureRotation != mCameraRotation
                || textureWidth != mCameraWidth || textureHeight != mCameraHeight;
            JLog.Info("needCalculateResize:" + needCalculateResize);
            JLog.Debug("SetRenderInfo new " + textureRotation + ":" + renderWidth 
                + ":" + renderHeight+ " old " + mCameraRotation + ":" + mRenderWidth + ":" + mRenderHeight);
            if (mCameraParamCallback != null) {
#if UNITY_EDITOR || UNITY_STANDALONE
                mCameraParamCallback(textureWidth, textureHeight, (int)(activeCameraDevice.isFrontFacing ?
                        CameraFaceType.FRONT: CameraFaceType.BACK), textureRotation);
#else
                mCameraParamCallback(textureWidth, textureHeight, (int)(activeCameraDevice.isFrontFacing ?
                        CameraFaceType.FRONT : CameraFaceType.BACK), activeCameraDevice.isFrontFacing ? (540 - textureRotation)
                        % 360 : (180 + textureRotation) % 360);
#endif
            }

            mCameraRotation = textureRotation;

#if (UNITY_ANDROID || UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
            if (renderRectTransform != null && textureRotation % 180 != 0) {
                renderRectTransform.sizeDelta = new Vector2(Math.Max(renderWidth, renderHeight), Math.Min(renderWidth, renderHeight));
                JLog.Debug("SetRenderInfo change sizeDelta " + textureRotation +
                     ":" + renderRectTransform.sizeDelta.x + ":" + renderRectTransform.sizeDelta.y
                    + " old " + mCameraRotation + ":" + renderWidth + ":" + renderHeight);
            }
#endif
            mRenderWidth = renderRectTransform == null ? textureWidth : (int)renderRectTransform.sizeDelta.x;
            mRenderHeight = renderRectTransform == null ? textureHeight : (int)renderRectTransform.sizeDelta.y;
            mCameraWidth = activeCameraTexture.width;
            mCameraHeight = activeCameraTexture.height; 
            Vector3 scale = renderRectTransform == null ? new Vector3(1, 1, 1) : renderRectTransform.localScale;


#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_EDITOR
            if (activeCameraDevice.isFrontFacing)
            {
                scale.x *= scale.x < 0 ? 1 : -1;
                scale.y *= scale.y < 0 ? 1 : -1;
                if (renderRectTransform != null) {
                    renderRectTransform.localEulerAngles = new Vector3(180, 0f, activeCameraTexture.videoRotationAngle);
                }
            }
            else
            {
                scale.x *= scale.x < 0 ? -1 : 1;
                scale.y *= scale.y < 0 ? 1 : -1;
                if (renderRectTransform != null) {
                    renderRectTransform.localEulerAngles = new Vector3(180, 0f, -activeCameraTexture.videoRotationAngle);
                }
            }
#elif UNITY_IOS
            if (activeCameraDevice.isFrontFacing)
            {
                scale.x *= scale.x < 0 ? 1 : -1;
                scale.y *= scale.y < 0 ? 1 : -1;
                if (renderRectTransform != null) {
                    renderRectTransform.localEulerAngles = new Vector3(0, 0f, activeCameraTexture.videoRotationAngle);
                }
            }
            else
            {
                scale.x *= scale.x < 0 ? -1 : 1;
                scale.y *= scale.y < 0 ? 1 : -1;
                if (renderRectTransform != null) {
                    renderRectTransform.localEulerAngles = new Vector3(0, 0f, -activeCameraTexture.videoRotationAngle);
                }
            }
#else
            if (activeCameraDevice.isFrontFacing)
            {
                scale.x *= scale.x < 0 ? 1 : -1;
                if (renderRectTransform != null)
                {
                    renderRectTransform.localEulerAngles = new Vector3(0f, 0f, activeCameraTexture.videoRotationAngle);
                }
            }
            else
            {
                scale.x *= scale.x < 0 ? -1 : 1;
                if (renderRectTransform != null)
                {
                    renderRectTransform.localEulerAngles = new Vector3(0f, 0f, -activeCameraTexture.videoRotationAngle);
                }
            }
#endif
            if (renderRectTransform != null)
            {
                renderRectTransform.localScale = scale;
            }
            setRenderUVRect(render, srcW, srcH);
            if (needCalculateResize || (mRender != null && mRender.texture == null)) {
                CalculateResize();
            }
            if (renderRectTransform != null)
            {
                JLog.Debug("SetRenderInfo renderRectTransform :" + renderRectTransform.ToString());
            }
        }

        private void CalculateResize() {
            if (mFitXYTexture != null)
            {
                UnityEngine.Object.Destroy(mFitXYTexture);
                mFitXYTexture = null;
            }
            if (mResizeTexture != null)
            {
                UnityEngine.Object.Destroy(mResizeTexture);
                mResizeTexture = null;
            }
            JLog.Error("eH:" + mVideoConfig.encodeHeight + "eW:" + mVideoConfig.encodeWidth
                    + "height:" + activeCameraTexture.height + "width:" + activeCameraTexture.width);
            bool needResize = mVideoConfig.encodeHeight != activeCameraTexture.height || mVideoConfig.encodeWidth != activeCameraTexture.width;
            if (isFitXY() && needResize)
            {
                mResizeRect = CalculateFitXYResizeRect(activeCameraTexture.width, activeCameraTexture.height, mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                mFitXYTexture = new Texture2D(mVideoConfig.encodeWidth, mVideoConfig.encodeHeight, TextureFormat.RGBA32, false);
                mResizeTexture = Resize(activeCameraTexture, (int)mResizeRect.width, (int)mResizeRect.height, (int)mResizeRect.width, (int)mResizeRect.height);
                if (mRender != null) {
                    mRender.texture = mFitXYTexture;
                    mRender.material.mainTexture = mFitXYTexture;
                }
            }
            else
            {
                mResizeRect = CalculateResizeRect(activeCameraTexture.width, activeCameraTexture.height, mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                mCorpRect = CalculateCorpRect((int)mResizeRect.width, (int)mResizeRect.height, mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                mResizeTexture = Resize(activeCameraTexture, (int)mCorpRect.width, (int)mCorpRect.height, (int)mResizeRect.width, (int)mResizeRect.height);
                if (mRender != null) {
                    mRender.texture = activeCameraTexture;
                    mRender.material.mainTexture = activeCameraTexture;
                }
            }

        }

        private void CalculateAndroidEncodeCrop() {
            if (!isChangeHVOK()) {
                return;
            }
            if (mEncodeRect != null) {
                return;
            }
            bool needResize = (mVideoConfig.encodeHeight != activeCameraTexture.height) || (mVideoConfig.encodeWidth != activeCameraTexture.width);
            if (needResize)
            {
                JLog.Debug("EH:" + mVideoConfig.encodeHeight + " EW:" + mVideoConfig.encodeWidth);
                JLog.Debug(" TH:" + activeCameraTexture.height + " TW:" + activeCameraTexture.width);
                bool isLand = activeCameraTexture.videoRotationAngle % 180 == 0;
                int width = activeCameraTexture.width;
                int height = activeCameraTexture.height;
                int encodeWidth = mVideoConfig.encodeWidth;
                int encodeHeight = mVideoConfig.encodeHeight;
                if (isLand)
                {
                    width = Math.Max(activeCameraTexture.width, activeCameraTexture.height);
                    height = Math.Min(activeCameraTexture.width, activeCameraTexture.height);
                    encodeWidth = Math.Max(mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                    encodeHeight = Math.Min(mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                }
                else {
                    height = Math.Max(activeCameraTexture.width, activeCameraTexture.height);
                    width = Math.Min(activeCameraTexture.width, activeCameraTexture.height);
                    encodeHeight = Math.Max(mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                    encodeWidth = Math.Min(mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                }
                mEncodeRect = CalculateEncodeRect(width, height, encodeWidth, encodeHeight);
                JLog.Debug("mEncodeRect.height:" + mEncodeRect.height + " mEncodeRect.width:" + mEncodeRect.width);
            }
        }

        private void setRenderUVRect(RawImage render, int srcW, int srcH)
        {
            if (render == null) {
                return;
            }
            float retRatio = renderRectTransform.sizeDelta.x / renderRectTransform.sizeDelta.y;
            float textureRatio = 1f * srcW / (1f * srcH);
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
            JLog.Debug("SetRenderInfo uvRect " + rect.ToString());
        }

        private void CreatFitXYCacheIfNeed(){
            if (!mFitXYColorInited)
            {
                mFitXYColorInited = true;
                int len = mCachePixel.Length / 4;
                Color32[] temp = new Color32[len];
                for (int i = 0; i < len; i++)
                {
                    temp[i].a = 255;
                }
                Span<byte> bytes = MemoryMarshal.Cast<Color32, byte>(temp);
                bytes.CopyTo(mCachePixel);
            }
        }

        private byte[] mWindowsByteCache;
        private void OnReadbackWindowsComplete(AsyncGPUReadbackRequest request)
        {
            long time = 0;
            mTimeDictionary.TryRemove(request, out time); 
            if (request.hasError)
            {
                JLog.Info("Async GPU readback encountered an error.");
                return;
            }
            if (isStop)
            {
                return;
            }
            NativeArray<byte> readbackData = request.GetData<byte>();

            if (mResizeTexture != null && mRenderTexture != null)
            {
                if (isFitXY())
                {
                    if (mWindowsByteCache == null || mWindowsByteCache.Length != readbackData.Length)
                    {
                        mWindowsByteCache = new byte[readbackData.Length];
                    }
                    readbackData.CopyTo(mWindowsByteCache);
                    CreatFitXYCacheIfNeed();
                    if (FitX())
                    {
                        if (mCopyOffset == 0)
                        {
                            int diffHeight = ((int)(mFitXYTexture.height - mResizeRect.height) / 2) * 2;
                            mCopyOffset = (int)((diffHeight) * mFitXYTexture.width * 2);
                        }
                        System.Buffer.BlockCopy(mWindowsByteCache, 0, mCachePixel, mCopyOffset, mWindowsByteCache.Length);
                    }
                    else
                    {
                        if (mCopyOffset == 0)
                        {
                            int diffWidth = ((int)(mFitXYTexture.width - mResizeRect.width) / 2) * 2;
                            mCopyOffset = diffWidth * 2;
                        }
                        int copySize = (int)mResizeRect.width * 4;
                        int destWSize = (int)mFitXYTexture.width * 4;
                        for (int i = 0; i < mResizeRect.height; i++)
                        {
                            int offset = i * copySize;
                            int destOffset = i * destWSize + mCopyOffset;
                            System.Buffer.BlockCopy(mWindowsByteCache, offset, mCachePixel, destOffset, copySize);
                        }
                    }
                    mFitXYTexture.LoadRawTextureData(mCachePixel);
                    mFitXYTexture.Apply();
                }
                else
                {
                    readbackData.CopyTo(mCachePixel);
                }
            }
            else
            {
                readbackData.CopyTo(mCachePixel);
            }
            mCacheVideoFrame.buffer = mCachePixel;
            VIDEO_MIRROR_MODE_TYPE mirrorType = mVideoConfig.mirrorMode;
            if (activeCameraDevice.isFrontFacing)
                {
                mCacheVideoFrame.rotation = (540 - activeCameraTexture.videoRotationAngle) % 360;
                }
            else
            {
                //左右镜像
                mirrorType = mVideoConfig.mirrorMode == VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED
                    ? VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED
                    : VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED;
                mCacheVideoFrame.rotation = (180 + activeCameraTexture.videoRotationAngle) % 360;
            }
            mCacheVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            mCacheVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
            mCacheVideoFrame.stride = (int)mPixelRect.width;
            mCacheVideoFrame.width = (int)mPixelRect.width;
            mCacheVideoFrame.height = (int)mPixelRect.height;
            mCacheVideoFrame.timestamp = time == 0 ? TimeHelper.GetMillSecond() : time;
            mCacheVideoFrame.mirror = mirrorType == VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED
                ? (int)MIRROR_TYPE.MIRROR_Y : (int)MIRROR_TYPE.MIRROR_NONE;
            InvokeFrameEvent(mCacheVideoFrame, true);
            }
        private void GetEncodePixelData(bool encode, long startCaptureTime)
        {
            if (!encode && !isFitXY())
            {
                return;
        }

            if (mResizeTexture != null && mRenderTexture != null)
            {
                AsyncGPUReadbackRequest request;
                Graphics.Blit(activeCameraTexture, mRenderTexture);
                if (isFitXY())
                {
                    request = AsyncGPUReadback.Request(mRenderTexture, 0, (int)mResizeRect.x, (int)mResizeRect.width,
                        (int)mResizeRect.y, (int)mResizeRect.height, 0, 1, OnReadbackWindowsComplete);
                }
                else
                {
                    request = AsyncGPUReadback.Request(mRenderTexture, 0, (int)mCorpRect.x, (int)mCorpRect.width,
                        (int)mCorpRect.y, (int)mCorpRect.height, 0, 1, OnReadbackWindowsComplete);
                }
                mTimeDictionary.TryAdd(request, startCaptureTime);
            }
            else
            {
                if (mCameraRenderTexture != null && mCameraRenderTexture.width != activeCameraTexture.width
                    && activeCameraTexture.height != mCameraRenderTexture.height)
                {
                    mCameraRenderTexture.Release();
                    mCameraRenderTexture = null;
                }
                if (mCameraRenderTexture == null)
                {
                    mCameraRenderTexture = RenderTexture.GetTemporary(activeCameraTexture.width, activeCameraTexture.height);
                }
                Graphics.Blit(activeCameraTexture, mCameraRenderTexture);
                AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(mCameraRenderTexture, 0, OnReadbackWindowsComplete);
                mTimeDictionary.TryAdd(request, startCaptureTime);
            }
        }

        // 这是个耗时方法，GetPixels 耗时会比较大
        private int index = 0;
        private bool isUserHardEncode = true;
        private FpsCounter mFpsCounter = new FpsCounter("camera", 5000);
        public override Texture ReadCameraPixel(bool encode)
        {

            // 要求竖屏，但是屏幕的宽高还没换过来，先返回
            if (!isChangeHVOK())
            {
                return null;
            }
            if (isStop)
            {
                return null;
            }
            if (activeCameraTexture != null && mPixelRect != null && activeCameraTexture.isPlaying)
            {
                long startCaptureTime = TimeHelper.GetMillSecond();
                mVideoCaptureTracker.OnCaptureStart(encode, startCaptureTime);
                if (isFitXY())
                {
                    SetRenderInfo(mRender, mVideoConfig.encodeWidth, mVideoConfig.encodeHeight);
                }
                else
                {
                    SetRenderInfo(mRender, activeCameraTexture.width, activeCameraTexture.height);
                }
#if (UNITY_ANDROID || UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
                if (!encode) {
                    return null;
                }
#endif
                //readPixelAsync(encode, startCaptureTime);
                readPixelSync(encode);
                mVideoCaptureTracker.OnCaptureStop(encode, TimeHelper.GetMillSecond());
            }
            return null;
        }

        private void readPixelAsync(bool encode, long startCaptureTime) {
            
            if (mCacheVideoFrame == null)
            {
                mCacheVideoFrame = new CaptureVideoFrame();
            }

            if (DoAndroidReadPixel(encode, startCaptureTime))
            {
                return;
            }
            if (DoIOSReadPixel(encode, startCaptureTime))
            {
                long endCaptureTime = TimeHelper.GetMillSecond();
                mFpsCounter.addFrame(endCaptureTime - startCaptureTime);
                return;
            }

            GetEncodePixelData(encode, startCaptureTime);
        }

        private bool DoAndroidReadPixel(bool encode, long startCaptureTime)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (isUserHardEncode && encode) {
                CalculateAndroidEncodeCrop();
                mCacheVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_TEXTURE;
                mCacheVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_TEXTURE_2D;
                mCacheVideoFrame.stride = (int)mPixelRect.width;
                mCacheVideoFrame.width = (int)mPixelRect.width;
                mCacheVideoFrame.height = (int)mPixelRect.height;
                mCacheVideoFrame.timestamp = startCaptureTime;
                mCacheVideoFrame.rotation = activeCameraTexture.videoRotationAngle;
                mCacheVideoFrame.textureId = activeCameraTexture.GetNativeTextureID();
                mCacheVideoFrame.mirror = activeCameraDevice.isFrontFacing ?
                (int)MIRROR_TYPE.MIRROR_Y : (int)MIRROR_TYPE.MIRROR_NONE;
                if (mEncodeRect != null) {
                    mCacheVideoFrame.cropLeft = mEncodeRect.x;
                    mCacheVideoFrame.cropTop = mEncodeRect.y;
                    mCacheVideoFrame.cropBottom = mEncodeRect.height;
                    mCacheVideoFrame.cropRight = mEncodeRect.width;

                }
                InvokeFrameEvent(mCacheVideoFrame, true);
            }
            return true;
#else
            return false;
#endif
        }

        private bool DoIOSReadPixel(bool encode, long startCaptureTime)
        {
#if (UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
             if (encode) {
                if (mResizeTexture != null && mRenderTexture != null) {
                    RenderTexture.active = mRenderTexture;
                    Graphics.Blit(activeCameraTexture, mRenderTexture);
                    if (mResizeTexture.width != mRenderTexture.width
                            || mRenderTexture.height != mResizeTexture.height)
                    {
                        UnityEngine.Object.Destroy(mResizeTexture);
                        mResizeTexture = null;
                    }
                    if (mResizeTexture == null)
                    {
                        mResizeTexture = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGBA32, mRenderTexture.mipmapCount, false);
                    }
                    Graphics.CopyTexture(mRenderTexture, mResizeTexture);
                    // 执行异步GPU读取
                    AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(mResizeTexture, 0, (int)mCorpRect.x, (int)mCorpRect.width,
                        (int)mCorpRect.y, (int)mCorpRect.height, 0, 1, TextureFormat.RGBA32, OnReadbackIOSComplete);

                    mTimeDictionary.TryAdd(request, startCaptureTime);
                    RenderTexture.active = null;
                } else {
                    if (mCameraRenderTexture != null && (mCameraRenderTexture.width != activeCameraTexture.width
                            || activeCameraTexture.height != mCameraRenderTexture.height))
                    {
                        mCameraRenderTexture.Release();
                        mCameraRenderTexture = null;
                    }
                    if (mCameraRenderTexture == null)
                    {
                        mCameraRenderTexture = RenderTexture.GetTemporary(activeCameraTexture.width, activeCameraTexture.height, 0) ;
                    }
                    if (mIosTexture2D != null && mIosTexture2D.width != activeCameraTexture.width
                            && activeCameraTexture.height != mIosTexture2D.height)
                    {
                        UnityEngine.Object.Destroy(mIosTexture2D);
                        mIosTexture2D = null;
                    }
                    if (mIosTexture2D == null)
                    {
                        mIosTexture2D = new Texture2D(activeCameraTexture.width, activeCameraTexture.height, TextureFormat.RGBA32, activeCameraTexture.mipmapCount, false);
                    }
                    RenderTexture.active = mCameraRenderTexture;
                    Graphics.CopyTexture(activeCameraTexture, mCameraRenderTexture);
                    Graphics.CopyTexture(mCameraRenderTexture, mIosTexture2D);
                    // 执行异步GPU读取
                    AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(mIosTexture2D, 0, TextureFormat.RGBA32, OnReadbackIOSComplete);
                    mTimeDictionary.TryAdd(request, startCaptureTime);
                    RenderTexture.active = null;
                }
             }
            return true;
#else
            return false;
#endif
        }

        private void OnReadbackIOSComplete(AsyncGPUReadbackRequest request)
        {
            long time = 0;
            mTimeDictionary.TryRemove(request, out time);
            if (request.hasError)
            {
                JLog.Info("Async GPU readback encountered an error.");
                return;
            }
            if (isStop)
            {
                return;
            }
            NativeArray<byte> readbackData = request.GetData<byte>();
            readbackData.CopyTo(mCachePixel);
            mCacheVideoFrame.buffer = mCachePixel;
            VIDEO_MIRROR_MODE_TYPE mirrorType = mVideoConfig.mirrorMode;
            if (activeCameraDevice.isFrontFacing)
            {
                mirrorType = mVideoConfig.mirrorMode == VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED
                    ? VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_DISABLED
                    : VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED;
                mCacheVideoFrame.rotation = (360 - activeCameraTexture.videoRotationAngle) % 360;
            }
            else
            {
                mCacheVideoFrame.rotation = activeCameraTexture.videoRotationAngle;
            }
            mCacheVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            mCacheVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
            mCacheVideoFrame.stride = (int)mPixelRect.width;
            mCacheVideoFrame.width = (int)mPixelRect.width;
            mCacheVideoFrame.height = (int)mPixelRect.height;
            mCacheVideoFrame.timestamp = time == 0 ? TimeHelper.GetMillSecond() : time;
            mCacheVideoFrame.mirror = mirrorType == VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED
                ? (int)MIRROR_TYPE.MIRROR_Y : (int)MIRROR_TYPE.MIRROR_NONE;
            InvokeFrameEvent(mCacheVideoFrame, true);
        }
       
        /// <summary>
        /// 切换横竖屏是否ok，目前只有android版本有用
        /// </summary>
        /// <returns></returns>
        private bool isChangeHVOK()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
            // 要求竖屏，但是屏幕的宽高还没换过来，先返回
            if (mVideoConfig.mScreenOrientation == (int)ScreenOrientation.Portrait && Screen.width > Screen.height)
            {
                JLog.Debug("mVideoConfig.mScreenOrientation == (int)ScreenOrientation.Portrait && Screen.width > Screen.height");
                return false;
            }
            if ((mVideoConfig.mScreenOrientation == (int)ScreenOrientation.LandscapeRight ||
                mVideoConfig.mScreenOrientation == (int)ScreenOrientation.LandscapeLeft) && Screen.height > Screen.width)
            {
                JLog.Debug("mVideoConfig.mScreenOrientation == (int)ScreenOrientation.Landscape && Screen.width < Screen.height");
                return false;
            }
            if (activeCameraTexture != null && activeCameraTexture.width == 16) {
                JLog.Debug("activeCameraTexture != null && activeCameraTexture.width == 16");
                return false;
            }
#endif
            return true;
        }

        private Rect CalculateResizeRect(int srcWidth, int srcHeight, int destWidth, int destHeight)
        {
            float scale;
            int viewPortWidth = destWidth;
            int viewPortHeight = destHeight;
            if (srcWidth * destHeight > destWidth * srcHeight)
            {
                // 高满, 裁剪宽
                scale = (float)destHeight / (float)srcHeight;
                viewPortWidth = (int)(srcWidth * scale);

            }
            else
            {
                // 宽满, 裁剪高
                scale = (float)destWidth / (float)srcWidth;
                viewPortHeight = (int)(srcHeight * scale);
            }
            return new Rect(0, 0, viewPortWidth, viewPortHeight);
        }

        private Rect CalculateFitXYResizeRect(int srcWidth, int srcHeight, int destWidth, int destHeight)
        {
            float scale;
            int viewPortWidth = destWidth;
            int viewPortHeight = destHeight;
            if (srcWidth * destHeight > destWidth * srcHeight)
            {
                // 宽=destWidth，计算高
                scale = (float)destWidth / (float)srcWidth;
                viewPortHeight = ((int)(srcHeight * scale) / 2) * 2;
                mFitMode = FITMODE.FIT_X;

            }
            else
            {
                // 高=destHeight，计算宽
                scale = (float)destHeight / (float)srcHeight;
                viewPortWidth = ((int)(srcWidth * scale) / 2) * 2;
                mFitMode = FITMODE.FIT_Y;
            }
            return new Rect(0, 0, viewPortWidth, viewPortHeight);
        }

        private Rect CalculateCorpRect(int srcWidth, int srcHeight, int destWidth, int destHeight)
        {
            float scale;
            int viewportX = 0;
            int viewportY = 0;
            int viewPortWidth = destWidth;
            int viewPortHeight = destHeight;
            if (srcWidth * destHeight > destWidth * srcHeight)
            {
                // 高满, 裁剪宽
                scale = (float)destHeight / (float)srcHeight;
                viewPortWidth = (int)(srcWidth * scale);
                viewportX = (int)((destWidth - viewPortWidth) * 0.5f);
            }
            else
            {
                // 宽满, 裁剪高
                scale = (float)destWidth / (float)srcWidth;
                viewPortHeight = (int)(srcHeight * scale);
                viewportY = (int)((destHeight - viewPortHeight) * 0.5f);
            }
            return new Rect(Math.Abs(viewportX), Math.Abs(viewportY), destWidth, destHeight);
        }


        private Rect CalculateEncodeRect(int width, int height, int encodeWidth, int encodeHeight)
        {
            float scale;
            float viewportX = 0;
            float viewportY = 0;
            float viewPortWidth = encodeWidth;
            float viewPortHeight = encodeHeight;
            if (width * encodeHeight > encodeWidth * height)
            {
                // 高满, 裁剪宽
                scale = (float)encodeHeight / (float)height;
                viewPortWidth = (int)(width * scale);
                viewportX = 1.0f * (encodeWidth - viewPortWidth) / (2 * viewPortWidth);
                viewPortHeight = 1;
                viewPortWidth = Math.Abs(viewportX) + encodeWidth * 1.0f / viewPortWidth;
                viewportY = 0;
            }
            else
            {
                // 宽满, 裁剪高
                scale = (float)encodeWidth / (float)width;
                viewPortHeight = height * scale;
                viewportY = 1.0f * (encodeHeight - viewPortHeight) / (2 * viewPortHeight);
                viewPortWidth = 1;
                viewPortHeight = Math.Abs(viewportY) + encodeHeight * 1.0f / viewPortHeight;
                viewportX = 0;
            }
            return new Rect(Math.Abs(viewportX), Math.Abs(viewportY), viewPortWidth, viewPortHeight);
        }

        Texture2D Resize(Texture texture, int textureWidth, int textureHeight, int targetX, int targetY)
        {
            if (mResizeTexture != null)
            {
                return mResizeTexture;
            }
            mRenderTexture = RenderTexture.GetTemporary(targetX, targetY);
            RenderTexture.active = mRenderTexture;
            Graphics.Blit(texture, mRenderTexture);
            Texture2D result = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, mRenderTexture.mipmapCount, false);
            result.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
            result.Apply();

            return result;
        }

        //==========================sync function===============================
        private void GetEncodePixelDataSync(bool encode)
        {
            if (!encode && !isFitXY())
            {
                return;
            }
            if (mResizeTexture != null && mRenderTexture != null)
            {
                RenderTexture.active = mRenderTexture;
                Graphics.Blit(activeCameraTexture, mRenderTexture);
                if (isFitXY())
                {
                    mResizeTexture.ReadPixels(mResizeRect, 0, 0);
                    byte[] resizeBytes = mResizeTexture.GetRawTextureData();
                    if (!mFitXYColorInited)
                    {
                        mFitXYColorInited = true;
                        int len = mCachePixel.Length / 4;
                        Color32[] temp = new Color32[len];
                        for (int i = 0; i < len; i++)
                        {
                            temp[i].a = 255;
                        }
                        Span<byte> bytes = MemoryMarshal.Cast<Color32, byte>(temp);
                        bytes.CopyTo(mCachePixel);
                    }
                    if (FitX())
                    {
                        if (mCopyOffset == 0)
                        {
                            int diffHeight = ((int)(mFitXYTexture.height - mResizeRect.height) / 2) * 2;
                            mCopyOffset = (int)((diffHeight) * mFitXYTexture.width * 2);
                        }
                        System.Buffer.BlockCopy(resizeBytes, 0, mCachePixel, mCopyOffset, resizeBytes.Length);
                    }
                    else
                    {
                        if (mCopyOffset == 0)
                        {
                            int diffWidth = ((int)(mFitXYTexture.width - mResizeRect.width) / 2) * 2;
                            mCopyOffset = diffWidth * 2;
                        }
                        int copySize = (int)mResizeRect.width * 4;
                        int destWSize = (int)mFitXYTexture.width * 4;
                        for (int i = 0; i < mResizeRect.height; i++)
                        {
                            int offset = i * copySize;
                            int destOffset = i * destWSize + mCopyOffset;
                            System.Buffer.BlockCopy(resizeBytes, offset, mCachePixel, destOffset, copySize);
                        }
                    }
                    mFitXYTexture.LoadRawTextureData(mCachePixel);
                    mFitXYTexture.Apply();
                }
                else
                {
                    mResizeTexture.ReadPixels(mCorpRect, 0, 0);
                    mCachePixel = mResizeTexture.GetRawTextureData();
                }

                RenderTexture.active = null;
            }
            else
            {
                int colorSize = activeCameraTexture.width * activeCameraTexture.height;
                if (mCacheColor?.Length != colorSize)
                {
                    mCacheColor = new Color32[colorSize];
                }

                Color32[] color = activeCameraTexture.GetPixels32(mCacheColor);
                Span<byte> bytes = MemoryMarshal.Cast<Color32, byte>(color);
                bytes.CopyTo(mCachePixel);
            }
        }

        private void readPixelSync(bool encode) {
            long startTime = System.DateTime.Now.Ticks / 10000;
            long startCaptureTime = TimeHelper.GetMillSecond();
            
            if (mCacheVideoFrame == null)
            {
                mCacheVideoFrame = new CaptureVideoFrame();
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            if (isUserHardEncode && encode) {
                CalculateAndroidEncodeCrop();
                mCacheVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_TEXTURE;
                mCacheVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_TEXTURE_2D;
                mCacheVideoFrame.stride = (int)mPixelRect.width;
                mCacheVideoFrame.width = (int)mPixelRect.width;
                mCacheVideoFrame.height = (int)mPixelRect.height;
                mCacheVideoFrame.timestamp = startCaptureTime;
                mCacheVideoFrame.rotation = activeCameraTexture.videoRotationAngle;
                mCacheVideoFrame.textureId = activeCameraTexture.GetNativeTextureID();
                mCacheVideoFrame.mirror = activeCameraDevice.isFrontFacing ?
                (int)MIRROR_TYPE.MIRROR_Y : (int)MIRROR_TYPE.MIRROR_NONE;
                if (mEncodeRect != null) {
                    mCacheVideoFrame.cropLeft = mEncodeRect.x;
                    mCacheVideoFrame.cropTop = mEncodeRect.y;
                    mCacheVideoFrame.cropBottom = mEncodeRect.height;
                    mCacheVideoFrame.cropRight = mEncodeRect.width;

                }
                InvokeFrameEvent(mCacheVideoFrame, true);
                return;
            }
#endif
            GetEncodePixelDataSync(encode);
            if (!encode) {
                return;
            }
            //long endCaptureTime = TimeHelper.GetMillSecond();
            //mFpsCounter.addFrame(endCaptureTime - startCaptureTime);

            mCacheVideoFrame.buffer = mCachePixel;


#if (UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
            if (activeCameraDevice.isFrontFacing)
            {
                    mVideoConfig.mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED;
                mCacheVideoFrame.rotation = (360 - activeCameraTexture.videoRotationAngle) % 360;
            }
            else
            {
                mCacheVideoFrame.rotation = activeCameraTexture.videoRotationAngle;
                }
#else

            if (activeCameraDevice.isFrontFacing)
            {
                mCacheVideoFrame.rotation = (540 - activeCameraTexture.videoRotationAngle) % 360;
            }
            else
            {
                //左右镜像
                int offset, left, right;
                for (int i = 0; i < mPixelRect.height; i++)
                {
                    offset = i * (int)mPixelRect.width * 4;
                    for (int j = 0; j < mPixelRect.width / 2; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            left = offset + j * 4 + k;
                            right = offset + ((int)mPixelRect.width - 1 - j) * 4 + k;
                            byte tmp = mCachePixel[left];
                            mCachePixel[left] = mCachePixel[right];
                            mCachePixel[right] = tmp;
                        }
                    }
                }
                mCacheVideoFrame.rotation = (180 + activeCameraTexture.videoRotationAngle) % 360;
            }
#endif
            //JLog.Debug("activeCameraTexture.videoRotationAngle " + activeCameraTexture.videoRotationAngle
            //    + ":" + mCacheVideoFrame.rotation + ":" + Screen.width + ":" + Screen.height);
            mCacheVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            mCacheVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
            mCacheVideoFrame.stride = (int)mPixelRect.width;
            mCacheVideoFrame.width = (int)mPixelRect.width;
            mCacheVideoFrame.height = (int)mPixelRect.height;
            mCacheVideoFrame.timestamp = startCaptureTime;
            mCacheVideoFrame.mirror = mVideoConfig.mirrorMode == VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED
                ? (int)MIRROR_TYPE.MIRROR_Y : (int)MIRROR_TYPE.MIRROR_NONE;
            mVideoCaptureTracker.OnEncodeStop(TimeHelper.GetMillSecond());
            InvokeFrameEvent(mCacheVideoFrame, true);
        }
    }
}

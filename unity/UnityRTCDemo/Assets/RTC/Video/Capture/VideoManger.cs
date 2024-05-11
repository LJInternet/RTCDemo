using LJ.RTC.Video;
using LJ.RTC.Common;
using UnityEngine.UI;
using LJ.RTC;
using System;

internal class VideoManager : BaseRtcEngineModule
{
    private VideoConfig mVideoConfig;
    private FpsCounter mFpsCounter;

    private ICameraCapture mCameraCapture;

    private IRtcEngineEventHandler.OnCameraParam mCameraParamCallback;

    private RawImage mRender;

    private RtcEngineNavite mRtcEngineNavite;

    private int mLastFps = 0;
    public VideoManager(IRtcEngineApi rtcEngineApi, RtcEngineNavite rtcEngineNavite) : base(rtcEngineApi) {
        mFpsCounter = new FpsCounter("capture", 5000);
        mRtcEngineNavite = rtcEngineNavite;
    }


    public void Start(VideoConfig videoConfig) {
        JLog.Info("VideoManager Start");
        mVideoConfig = videoConfig;
    }

    public void UpdateVideoConfig(VideoConfig videoConfig)
    {
        mVideoConfig = videoConfig;
    }

    public void Stop() {
        JLog.Info("VideoManager Stop");
        DestroyCameraCapture();
    }

    public CAMERA_CAPTURE_ERROR StartPreview() {
        JLog.Info("VideoManager StartPreview");

        if (mVideoConfig == null) {
            if (mRtcEngineApi != null)
            {
                mRtcEngineApi.OnCameraActionResult((int)CAMERA_ACTION.ACTION_START, (int)CAMERA_CAPTURE_ERROR.PARAM_ERROR, "");
            }
            return CAMERA_CAPTURE_ERROR.PARAM_ERROR;
        }
        MainThreadHelper.QueueOnMainThread((object obj) => {
            DestroyCameraCapture();

            CreateCameraCapture(mRtcEngineApi.IsUseNativeCamera());
            string deviceName = "";
            CAMERA_CAPTURE_ERROR result = CAMERA_CAPTURE_ERROR.SUCCESS;
            string msg = "";
            VideoDeviceManagerFactory.GetDeviceManager().GetDevice(ref deviceName);
            if (deviceName == "" || deviceName.Length == 0) {
                result = CAMERA_CAPTURE_ERROR.DEVICE_NOT_FOUND;
            } else {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
#if UNITY_IOS || UNITY_ANDROID
//              nothing
#else
                bool isDeviceBusy = false;

                if (mRtcEngineApi != null)
                {
                    GetCameraDeviceBusyInfo evnet = new GetCameraDeviceBusyInfo(deviceName);
                    byte[] eventResult = mRtcEngineApi.GetMediaEvent((int)MediaInvokeEventType.VIDEO_CAMERA_BUSY_INFO, evnet.HPmarshall());
                    if (eventResult != null)
                    {
                        GetCameraDeviceBusyInfoResult devicesEventReslut = new GetCameraDeviceBusyInfoResult();
                        devicesEventReslut.unmarshall(eventResult);
                        isDeviceBusy = devicesEventReslut.isBusy;
                        msg = devicesEventReslut.processName;
                        JLog.Info("VideoManager CameraDeviceBusyInfo processName:" + devicesEventReslut.processName);
                    }
                }
                JLog.Info("VideoManager isDeviceBusy " + isDeviceBusy);
                if (isDeviceBusy)
                {
                    result = CAMERA_CAPTURE_ERROR.CAMERA_BUSY;
                }
                else
#endif
#endif
                {
                    result = mCameraCapture.StartPreview();
                }
            }
            if (mRtcEngineApi != null)
            {
                mRtcEngineApi.OnCameraActionResult((int)CAMERA_ACTION.ACTION_START, (int)result, msg);
            }
        }, null);
        return CAMERA_CAPTURE_ERROR.SUCCESS;
    }


    private void CreateCameraCapture(bool use_native_capture)
    {
        if (mCameraCapture == null)
        {
            if (use_native_capture)
            {
                mCameraCapture = new NativeCameraCapture(mVideoConfig, mRtcEngineNavite);
            }
            else 
            {
                mCameraCapture = new CameraCapture(mVideoConfig);
            }
           
            mCameraCapture.OnCreate();
        }
        SetRender(mRender, mCameraParamCallback);
        mCameraCapture.mVideoFrameEvent += OnCaptureVideoFrame;
    }

    public void StopPreview() {
        JLog.Info("VideoManager StopPreview");
        MainThreadHelper.QueueOnMainThread((object obj) => {
            DestroyCameraCapture();
        }, null);
    }

    public override void OnCreate()
    {
        JLog.Info("VideoManager OnCreate");
    }

    public override void OnDestroy()
    {
        JLog.Info("VideoManager OnDestroy");
        Stop();
        mCameraParamCallback = null;
        mRender = null;
        base.OnDestroy();
    }

    private void DestroyCameraCapture() {
        JLog.Info("VideoManager DestroyCameraCapture");
        if (mCameraCapture != null)
        {
            mCameraCapture.mVideoFrameEvent -= OnCaptureVideoFrame;
            mCameraCapture.StopPreview();
            mCameraCapture.OnDestroy();
            mCameraCapture = null;
        }
    }

    public void SetRender(RawImage rawImage, IRtcEngineEventHandler.OnCameraParam callback)
    {
        JLog.Info("VideoManager SetRender");
        mCameraParamCallback = callback;
        mRender = rawImage;
        if (rawImage == null && callback == null)
        {
            JLog.Info("VideoManager SetRender rawImage == null && callback == null");
            return;
        }
        if (mCameraCapture != null)
        {
            mCameraCapture.SetRender(rawImage, OnCameraParamCallback);
        }
  
    }

    public void OnCameraParamCallback(int width, int height, int facing, int rotation)
    {
        JLog.Info("OnCameraParamCallback " + width +":" + height + ":" + facing + ":" + rotation + ":" + mCameraParamCallback);
        if (mCameraParamCallback != null)
        {
            mCameraParamCallback(width, height, facing, rotation, mVideoConfig.frameRate);
        }
    }

    public void  ReadCameraPixel(bool encode)
    {
        if (mCameraCapture != null)
        {
            if (encode) {
                mFpsCounter.addFrame();
            }

            mCameraCapture.ReadCameraPixel(encode);
        }
    }

    public void OnCaptureVideoFrame(CaptureVideoFrame videoFrame, bool push)
    {
        if (mRtcEngineApi != null)
        {
            mRtcEngineApi.OnCaptureVideoFrame(videoFrame);
            if (push) {
                mRtcEngineApi.PushVideoCaptureFrame(videoFrame);
            }
            
        }

    }

    public CAMERA_CAPTURE_ERROR StartCameraDevice(string cameraDeviceName) {
        DestroyCameraCapture();

        CreateCameraCapture(mRtcEngineApi.IsUseNativeCamera());

        if (mCameraCapture != null)
        {
            return mCameraCapture.StartCameraDevice(cameraDeviceName);
        }
        return CAMERA_CAPTURE_ERROR.SUCCESS;
    }

    internal void onFrameRateControl(VideoFrameRateControl control)
    {   if (mLastFps != control.frameRate) {
            JLog.Debug("control fps:" + control.frameRate);
        }

        if (mCameraParamCallback != null)
        {
            mCameraParamCallback(0, 0, 0, 0, control.frameRate);
        }
    }
}

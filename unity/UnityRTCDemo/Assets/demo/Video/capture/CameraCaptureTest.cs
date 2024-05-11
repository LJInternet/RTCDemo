using UnityEngine.UI;
using UnityEngine;
using LJ.RTC.Common;
using LJ.RTC;
using LJ.RTC.Video;
using LJ.RTC.Utils;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using static UnityEngine.RectTransform;
using LJ.demo;

using LJ.Log;
using LJ.Feedback;
using Fancy;

public class CameraCaptureTest : MonoBehaviour
{
    internal IRtcEngine mRtcEngine;
    public Dropdown _Dropdown;

    public CanvasScaler mCanvasScaler;
    private LjRudpUtils mRtm;
    private bool userNativeCamera = true;


    void OnApplicationQuit() {
        LJ.Report.ReportCenter.Release();
        Debug.Log("ReportCenter Release");
    }

    private void StartRtcEngine()
    {
        mRtcEngine = InitHelper.StartRtcEngine(false);
        mRtcEngine.EnableAudio(true);
        mRtcEngine.RegisterCaptureFrame(OnCaptureVideoFrame);
        userNativeCamera = mRtcEngine.IsUseNativeCamera();
        mRtcEngine.JoinChannel(InitHelper.GetChannelConfig());
    }

    void OnCaptureVideoFrame(CaptureVideoFrame videoFrame) {

    }

    private void OnGUI()
    {

        if (GUI.Button(new Rect (30, 100, 100, 40), "开始")) {
            StartCoroutine(StartCamera());
        }

        if (GUI.Button(new Rect(30, 150, 100, 40), "停止")) {
            Stop();
            //StopAllCoroutines();
        }

        if (GUI.Button(new Rect(30, 200, 100, 40), "JoinChannel"))
        {
            int sessionId = 131313;
            ChannelConfig channelConfig = new ChannelConfig();
            UdpInitConfig initConfig = new UdpInitConfig();
            initConfig.remoteSessionId = sessionId;

            UdpInitConfig initConfig2 = new UdpInitConfig();
            initConfig2.remoteSessionId = sessionId;
            initConfig2.relayId = sessionId;
            initConfig2.netType = 2;
            initConfig2.remoteIP = "114.236.138.71";
            initConfig2.remotePort = 30001;

            channelConfig.configs.Add(initConfig);
            channelConfig.configs.Add(initConfig2);

            mRtcEngine.JoinChannel(channelConfig);


            //makeImageSurface("preview");
            JLog.Info("StartPreview=====");
            //mRtcEngine.StartPreview();
        }

        if (GUI.Button(new Rect(30, 250, 100, 40), "leaveChannel"))
        {
            mRtcEngine = IRtcEngine.Get();
            Stop();
            if (mRtcEngine != null)
            {
                mRtcEngine.LeaveChannel();
            }
            //GameObject preview = GameObject.Find("preview");
            //Destroy(preview);
            Debug.Log("OnDestroy: ");
        }

        if (GUI.Button(new Rect(30, 300, 100, 40), "EnableAudio"))
        {
            StartCoroutine(startAudio());
        }

        if (GUI.Button(new Rect(30, 350, 100, 40), "disableAudio"))
        {
            mRtcEngine = IRtcEngine.Get();
            if (mRtcEngine != null)
            {
               mRtcEngine.EnableAudio(false);
            }
        }
        if (GUI.Button(new Rect(135, 100, 100, 40), "enableSubMix"))
        {
            mRtcEngine = IRtcEngine.Get();
            if (mRtcEngine != null)
            {
                mRtcEngine.EnableSubMix(true);
            }
        }
        if (GUI.Button(new Rect(135, 150, 100, 40), "disableSubMix"))
        {
            mRtcEngine = IRtcEngine.Get();
            if (mRtcEngine != null)
            {
                mRtcEngine.EnableSubMix(false);
            }
        }
        if (GUI.Button(new Rect(135, 200, 100, 40), "saveH264"))
        {
            mRtcEngine = IRtcEngine.Get();
            if (mRtcEngine != null)
            {
                mRtcEngine.SaveEncodedVideo(true);
            }
        }
        if (GUI.Button(new Rect(135, 250, 100, 40), "RegisterDecodeVideo"))
        {
            mRtcEngine = IRtcEngine.Get();
            if (mRtcEngine != null)
            {
                mRtcEngine.RegisterDecodedFrame(OnDecodedVideoFrame);
            }
        }

        if (GUI.Button(new Rect(135, 300, 100, 40), "横屏"))
        {
            SetLandscape();
            mRtcEngine.StopPreview();
            mRtcEngine.StartPreview();
        }

        if (GUI.Button(new Rect(135, 350, 100, 40), "竖屏"))
        {
            SetPortrait();
            mRtcEngine.StopPreview();
            mRtcEngine.StartPreview();
        }

        if (GUI.Button(new Rect(135, 400, 100, 40), "反馈"))
        {
            FeedbackMgr.SendFeedBack("test", "test", FLog.GetLogPath(), "111");
            FLog.Info("feeback filePath:" + FLog.GetLogPath());
        }

        if (GUI.Button(new Rect(135, 450, 100, 40), "NativeCamera"))
        {
            userNativeCamera = !userNativeCamera;
            mRtcEngine.StopPreview();
            mRtcEngine.SetUseNativeCamera(userNativeCamera);
            SetDrowOption();
            mRtcEngine.StartPreview();
        }
    }

    private IEnumerator startAudio() {

        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
        mRtcEngine = IRtcEngine.Get();
        if (mRtcEngine != null)
        {
            mRtcEngine.EnableAudio(true);
        }
    }


    public void SetPortrait()
    {

        mCanvasScaler.referenceResolution = new Vector2(1080, 1920);
        mCanvasScaler.matchWidthOrHeight = 0;
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.Portrait;
        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(640, 480);
        encoderConfiguration.frameRate = 30;
        encoderConfiguration.mScreenOrientation = (int)ScreenOrientation.Portrait;
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT;
        mRtcEngine.StopPreview();
        mRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        mRtcEngine.StartPreview();
    }

    public void SetLandscape()
    {
        mCanvasScaler.referenceResolution = new Vector2(1920, 1080);
        mCanvasScaler.matchWidthOrHeight = 1;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(640, 480);
        encoderConfiguration.frameRate = 30;
        encoderConfiguration.mScreenOrientation = (int)ScreenOrientation.LandscapeLeft;
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_LANDSCAPE;
        mRtcEngine.StopPreview();
        mRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        mRtcEngine.StartPreview();
    }

    public void OnDecodedVideoFrame(VideoFrame videoFrame) {
        JLog.Debug("OnDecodedVideoFrame videoFrame " + videoFrame.frameType);
    }


    private IEnumerator StartCamera()
    {
        JLog.Debug("StartCamera : 1");
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return  Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }
        JLog.Debug("StartCamera : 2");
        if (mRtcEngine != null)
        {
            mRtcEngine.StartPreview();
        }
    }

    private static LJVideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        // to be renderered onto
        RawImage rawImage = go.AddComponent<RawImage>();
        //rawImage.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, 1f);
        //rawImage.rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, 1f);
        // make the object draggable
        go.AddComponent<UIElementDrag>();
        GameObject canvas = GameObject.Find("VideoCanvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
            Debug.Log("add video view");
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }
        // set up transform

        go.transform.localPosition = Vector3.zero;
        // configure videoSurface
        LJVideoSurface videoSurface = go.AddComponent<LJVideoSurface>();
        int width = InitHelper.GetEncodeWidth();
        int height = InitHelper.GetEncodeHeight();
        float temp = 1.0f * height / width;
        width = Math.Min(InitHelper.GetEncodeWidth(), 300);
        height = (int)(width * temp);
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        return videoSurface;
    }

    private void Stop()
    {
        mRtcEngine = IRtcEngine.Get();
        if (mRtcEngine != null) {
            mRtcEngine.StopPreview();
        }
    }

    public static void Upload(string msg) {
        FLog.Info("Upload Report:" + msg );
    }


    public void ReportEvent(int result, string msg)
    {
        FLog.Info("result:" + result + ", msg:" + msg);
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
    // Start is called before the first frame update
    public void Start()
    {
        CalculateCorpRect(480, 640, 544, 960);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        InitHelper.InitReport(ReportEvent);
        InitHelper.InitFeedBack();

        mRtm = InitHelper.StartRtm();

        StartRtcEngine();
        SetDrowOption();

        makeImageSurface("preview");
    }

    void SetDrowOption() {
        IVideoDeviceManager manager = mRtcEngine.GetVideoDeviceManager();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        if (manager != null)
        {
            DeviceInfo[] infos = manager.EnumerateVideoDevices();
            foreach (DeviceInfo deviceInfo in infos)
            {
                list.Add(new Dropdown.OptionData(deviceInfo.deviceName));
            }
        }
        else
        {
            string[] deviceNames = mRtcEngine.GetCameraDeviceNames();

            foreach (string deviceName in deviceNames)
            {
                list.Add(new Dropdown.OptionData(deviceName));
            }
        }
        //list.Add(new Dropdown.OptionData("a"));
        _Dropdown.ClearOptions();
        _Dropdown.AddOptions(list);
        _Dropdown.onValueChanged.AddListener((value) => {
            if (_Dropdown == null)
            {
                return;

            }
            Dropdown.OptionData data = _Dropdown.options[value];
            if (mRtcEngine != null)
            {
                manager = mRtcEngine.GetVideoDeviceManager();
                FLog.Info("onValueChanged:" + data.text);
                if (manager.SetDevice(data.text) == 0)
                {
                    mRtcEngine.StopPreview();
                    mRtcEngine.StartPreview();
                }

            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        //JLog.Debug("update");
        //StartCoroutine(readPixel());aa
        //FLog.Debug("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    }

    private void OnDestroy()
    {
        if (mRtm != null)
        {
            mRtm.StopRudp();
            mRtm = null;
        }
        Stop();
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.OnDestroy();
            mRtcEngine = null;
        }
        FeedbackMgr.Destroy();
        Debug.Log("OnDestroy: ");
        FLog.Uninit();

    }

    void OnDisable()
    {
#if UNITY_2018_1_OR_NEWER
        if (mRtm != null)
        {
            mRtm.StopRudp();
            mRtm = null;
        }
        LJ.Report.ReportCenter.Release();
        JLog.Info("ReportCenter Release");
        JLog.Info("OnDisable");
#endif
    }
}
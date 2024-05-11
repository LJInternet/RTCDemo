using LJ.RTC.Common;
using LJ.RTC;
using LJ.RTC.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LJ.RTC.Utils;
using LJ.demo;
using LJ.RTC.Audio;
using LJ.Log;
using LJ.RTM;
using LJ.RTMP;
using UnityEngine.Experimental.Rendering;
using System.IO;
using System.Collections;
using System.Threading;

public class RTMPEngineTest : MonoBehaviour
{
    internal IRtcEngine mRtcEngine;
    internal RTMPEngine rtmpEngine;
    public Dropdown _Dropdown;
    public Text _Text;
    int height;

    void OnCaptureVideoFrame(CaptureVideoFrame videoFrame)
    {
        //JLog.Info("OnCaptureVideoFrame");
        //byte[] msg = videoFrame.HPmarshall();
        //rtmpEngine.WriteVideo(videoFrame.buffer,
        //            videoFrame.buffer == null ? 0 : videoFrame.buffer.Length, msg, msg.Length, 1);
    }

    bool OnCaptureAudioFrame(AudioFrame videoFrame)
    {
        //JLog.Info("OnCaptureAudioFrame");
        rtmpEngine.WriteAudio(videoFrame.buffer, videoFrame.buffer.Length/2/ videoFrame.channelCount, videoFrame.sampleRate, videoFrame.channelCount, 2 );
        return false;
    }

    public void status_cb(RudpStatus status) { 
    }

    private RenderTexture mRenderTexture;

    private Timer mTimer;
    private bool mRunning = false;
    private static long TIMER_TIME = 1000;

    public void StartTimer()
    {
        if (mTimer != null)
        {
            return;
        }
        mRunning = true;
        mTimer = new Timer(DoTimerCallback, 1, TIMER_TIME, Timeout.Infinite);
    }

    public virtual void StopTimer()
    {
        if (mTimer == null)
        {
            return;
        }
        mRunning = false;
    }

    public void DoTimerCallback(object state)
    {
        if (!mRunning)
        {
            mTimer.Change(-1, Timeout.Infinite);
            mTimer.Dispose();
            mTimer = null;
            return;
        }
        rtmpEngine = RTMPEngine.getInstance();
        int ret = rtmpEngine.open("rtmp://103.215.36.233/live/test11111", 720, 1280, 30, 1800000);
        if (ret < 0) {
            rtmpEngine.close();
            TIMER_TIME = TIMER_TIME * 2;
            mTimer.Change(TIMER_TIME, Timeout.Infinite);
        }
        JLog.Info("DoTimerCallback");
    }
    private void OnRtmpStatusCallback(RTMPStatus status) {
        if (status == RTMPStatus.CONNECT_LOST) {
            rtmpEngine = RTMPEngine.getInstance();
            rtmpEngine.close();
            StartTimer();
        }
    }
    public void Start()
    {
        // 创建一个临时的RenderTexture，大小与屏幕分辨率一致
        mRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        mTexture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, mRenderTexture.mipmapCount, false);
        mEncodeRect = CalculateEncodeRect(Screen.width, Screen.height, 720, 1280);

        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        rtmpEngine = RTMPEngine.getInstance();
        float scale = 720.0f / Screen.width;
        height = (int)(Screen.height * scale);
        rtmpEngine.open("rtmp://103.215.36.233/live/test11111", 720, 1280, 30, 1800000);
        rtmpEngine.SetStatusCallback(OnRtmpStatusCallback);
        FancyRudpUtils.RegisterStatusCallback(status_cb);

        RtcEngineConfig config = new RtcEngineConfig();
        config.mAppId = 111;
        config.isTestEv = true;
        config.mJLog = new RtcLogImpl();
        IRtcEngine mRtcEngine = IRtcEngine.CreateRtcEngine(config);

        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.dimensions = new VideoDimensions(640, 480);
        encoderConfiguration.frameRate = 15;
        //encoderConfiguration.mirrorMode = VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED;
        mRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        mRtcEngine.EnableVideo(true);
        mRtcEngine.EnableAudio(true);
        mRtcEngine.EnableSubMix(true);
        mRtcEngine.muteLocalAudioStream(true);
        //mRtcEngine.muteLocalAudioStream(true);
        mRtcEngine.saveRecordCallbackAudio(true);
        mRtcEngine.JoinChannel(InitHelper.GetChannelConfig());

        mRtcEngine.RegisterCaptureAudioFrame(OnCaptureAudioFrame, AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_ONLY);
        //mRtcEngine.RegisterCaptureFrame(OnCaptureVideoFrame);

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
        _Dropdown.AddOptions(list);
        _Dropdown.onValueChanged.AddListener((value) => {
            Dropdown.OptionData data = _Dropdown.options[value];
            if (manager != null && mRtcEngine != null)
            {
                if (manager.SetDevice(data.text) == 0)
                {
                    mRtcEngine.StopPreview();
                    mRtcEngine.StartPreview();
                }

            }
        });

        makeImageSurface("preview");
    }

    private Texture2D mTexture2D;
    private CaptureVideoFrame mCaptureVideoFrame;
    int index = 0;
    int textIndex = 0;
    Rect rect = new Rect(0,0, 640, 480);
    public void Update()
    {
        textIndex++;
        _Text.text = textIndex + "";

    }

    void LateUpdate()
    {
        index++;
        if (index % 2 == 0)
        {
            StartCoroutine(ReadPixel(true));
        }
    }
    private IEnumerator ReadPixel(bool encode)
    {

        yield return new WaitForEndOfFrame();
        Camera.main.targetTexture = mRenderTexture;
        Camera.main.Render();
        Camera.main.targetTexture = null;
        Graphics.CopyTexture(mRenderTexture, mTexture2D);
        if (mCaptureVideoFrame == null && mTexture2D != null)
        {
            mCaptureVideoFrame = new CaptureVideoFrame();
        }
        mCaptureVideoFrame.width = 720;
        mCaptureVideoFrame.height = 1280;
        mCaptureVideoFrame.stride = 720;
        mCaptureVideoFrame.rotation = 0;
        mCaptureVideoFrame.textureId = mTexture2D.GetNativeTextureID();
        byte[] msg = mCaptureVideoFrame.HPmarshall();
        if (mEncodeRect != null) {
            mCaptureVideoFrame.cropLeft = mEncodeRect.x;
            mCaptureVideoFrame.cropTop = mEncodeRect.y;
            mCaptureVideoFrame.cropBottom = mEncodeRect.height;
            mCaptureVideoFrame.cropRight = mEncodeRect.width;
        }
        rtmpEngine.WriteVideo(null, 0, msg, msg.Length, 1);
    }

    private Rect mEncodeRect;
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

    bool isEnableAudio = false;
    public void OnGUI()
    {
        if (GUI.Button(new Rect(30, 100, 100, 40), "开始推流"))
        {
            IRtcEngine rtcEngine = IRtcEngine.Get();
            if (rtcEngine != null) {
                rtcEngine.EnableAudio(false);
                rtcEngine.EnableSubMix(false);

                rtcEngine.muteLocalAudioStream(true);
            }
            rtmpEngine = RTMPEngine.getInstance();
            StopTimer();
            if (rtmpEngine != null)
            {
                rtmpEngine.close();
                rtmpEngine.SetStatusCallback(null);
                rtmpEngine = null;
            }
        }
    }

    private void OnDestroy()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.OnDestroy();
            mRtcEngine = null;
        }

        if (rtmpEngine != null) {
            rtmpEngine.close();
            rtmpEngine.SetStatusCallback(null);
            rtmpEngine = null;
        }
        if (mTexture2D != null)
        {
            UnityEngine.Object.Destroy(mTexture2D);
            mTexture2D = null;
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
        StopTimer();
        FLog.Uninit();
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
        videoSurface.mParamCallback += (int width, int height, int facing, int rotation) => {

            //go.transform.localScale = new Vector3(3f, 4f, 1f);

            //RawImage rawImage = videoSurface.GetComponent<RawImage>();
            //RectTransform rectTransform = rawImage.transform.GetComponent<RectTransform>();
            //rectTransform.localEulerAngles = new Vector3(0f, 0f, -rotation);

            //go.transform.Rotate(0f, 0f, -rotation);

            float scale = (float)height / (float)width;
#if UNITY_EDITOR
#elif UNITY_ANDROID || UNITY_IOS
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                scale = 1.0f * Math.Max(height, width) / Math.Min(height, width);
            }
            else {
                scale = 1.0f * Math.Min(height, width) / Math.Max(height, width);
            }
#endif
            rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 500 * scale);
            Debug.Log("OnTextureSizeModify: " + width + "  " + height + "  " + facing + "  " + rotation);
        };
        return videoSurface;
    }
}


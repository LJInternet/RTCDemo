using LJ.demo;
using LJ.Log;
using LJ.RTC;
using LJ.RTC.Audio;
using LJ.RTC.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioTest : MonoBehaviour
{

    internal IRtcEngine mRtcEngine;

    private Button _mixingButton;
    private bool _isMixing;

    private Button _volumeIndication;
    private bool _isVolumeIndication;

    private Button _enableAudio;
    private bool _isEnableAudio;

    private Button _enableSubMix;
    private bool _isEnableSubMix;

    private Button _MuteLocalStream;
    private bool _isMuteLocalStream;

    private Button _muteRemoteStream;
    private bool _isMuteRemoteStream;

    private RtcEventHandler rtcEventHandler;

    private FpsCounter mCaptureFpsCounter = new FpsCounter("AudioCaptureFps", 3000);

    void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        StartRtcEngine();
        _mixingButton = GameObject.Find("MixngBtn").GetComponent<Button>();
        _mixingButton.onClick.AddListener(HandleAudioMixingButton);

        _volumeIndication = GameObject.Find("VolumeBtn").GetComponent<Button>();
        _volumeIndication.onClick.AddListener(HandleAudioVolumeButton);

        _enableAudio = GameObject.Find("EnableAudio").GetComponent<Button>();
        _enableAudio.onClick.AddListener(HandleEnalbeAudioButton);

        _enableSubMix = GameObject.Find("EnableSubMix").GetComponent<Button>();
        _enableSubMix.onClick.AddListener(HandleEnalbeSubMixButton);

        _MuteLocalStream = GameObject.Find("MuteLocalStream").GetComponent<Button>();
        _MuteLocalStream.onClick.AddListener(HandleMuteLocalStreamButton);

        _muteRemoteStream = GameObject.Find("MuteRemoteStream").GetComponent<Button>();
        _muteRemoteStream.onClick.AddListener(HandleMuteRemoteButton);
    }

    private void HandleMuteRemoteButton()
    {
        _isMuteRemoteStream = !_isMuteRemoteStream;
        mRtcEngine.EnableSubMix(_isMuteRemoteStream);
        _muteRemoteStream.GetComponentInChildren<Text>().text = (_isMuteRemoteStream ? "UnMuteRemoteStream" : "MuteRemoteStream");
    }

    private void HandleMuteLocalStreamButton()
    {
        _isMuteLocalStream = !_isMuteLocalStream;
        mRtcEngine.EnableSubMix(_isMuteLocalStream);
        _MuteLocalStream.GetComponentInChildren<Text>().text = (_isMuteLocalStream ? "UnMuteLocalStream" : "MuteLocalStream");
    }

    private void HandleEnalbeSubMixButton()
    {
        _isEnableSubMix = !_isEnableSubMix;
        mRtcEngine.EnableSubMix(_isEnableSubMix);
        _enableSubMix.GetComponentInChildren<Text>().text = (_isEnableSubMix ? "DisableSubMix" : "EnableSubMix");
    }

    private void HandleEnalbeAudioButton()
    {
        _isEnableAudio = !_isEnableAudio;
        mRtcEngine.EnableAudio(_isEnableAudio);
        _enableAudio.GetComponentInChildren<Text>().text = (_isEnableAudio ? "DisableAudio" : "EnableAudio");
    }

    private void HandleAudioMixingButton()
    {
        if (_isMixing)
        {
            mRtcEngine.StopAudioMixing();
        }
        else
        {
            StartAudioMixing();
        }

        _isMixing = !_isMixing;
        _mixingButton.GetComponentInChildren<Text>().text = (_isMixing ? "Stop Mixing" : "Start Mixing");
    }

    private void HandleAudioVolumeButton()
    {
        if (_isVolumeIndication)
        {
            mRtcEngine.enableAudioVolumeIndication(0, 0, false);
        }
        else
        {
            mRtcEngine.enableAudioVolumeIndication(300, 0, false);
        }

        _isVolumeIndication = !_isVolumeIndication;
        _volumeIndication.GetComponentInChildren<Text>().text = (_isMixing ? "DisableVolume" : "EnableVolume");
    }

    private void StartAudioMixing()
    {
        var ret = mRtcEngine.StartAudioMixing(Application.streamingAssetsPath + "/music.mp3", true, -1, 0);
        Debug.Log("StartAudioMixing returns: " + ret);
    }

    class RtcEventHandler : IRtcEngineEventHandler
    {
        public override void onAudioVolumeIndication(AudioVolumeEvent info)
        {
            Debug.Log("StartAudioMixing returns: " + info.volume);
        }
    }

    private void StartRtcEngine()
    {
        mRtcEngine = InitHelper.StartRtcEngine(false);
        rtcEventHandler = new RtcEventHandler();
        mRtcEngine.InitEventHandler(rtcEventHandler);
        mRtcEngine.RegisterCaptureAudioFrame(OnCaptureAudioFrame, AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE);
        mRtcEngine.JoinChannel(InitHelper.GetChannelConfig());

    }

    byte[] bytes = new byte[1];
    public bool OnCaptureAudioFrame(AudioFrame audioFrame) {
        //Debug.Log("OnCaptureAudioFrame: " + audioFrame.buffer.Length);
        mCaptureFpsCounter.addFrame(audioFrame.buffer);
        if (bytes.Length != audioFrame.buffer.Length) {
            bytes = new byte[audioFrame.buffer.Length];
            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] = 0;
            }
        }
        audioFrame.buffer = bytes;
        return false;
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.OnDestroy();
            mRtcEngine = null;
        }
        Debug.Log("OnDestroy: ");

        FLog.Uninit();
    }
}

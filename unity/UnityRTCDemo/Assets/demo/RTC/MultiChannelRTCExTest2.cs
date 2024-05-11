using LJ.demo;
using LJ.Log;
using LJ.RTC;
using LJ.RTC.Common;
using LJ.RTC.Video;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiChannelRTCExTest2 : MonoBehaviour
{
    internal IRtcEngineEx mRtcEngine;
    RtcEventHandler mRtcEventHandler;

    LJChannel _channel1;
    LJChannel _channel2;
    LJChannel _channel3;

    public RawImage _channel1Render;
    public RawImage _channel2Render;
    public RawImage _channel3Render;

    private string _defaultToken = InitHelper._token;
    private string _defaultChannelId = "954523222";
    private long _defualtUserId = TimeHelper.GetCurrenTime();
    public Dropdown _Dropdown;

    private Stack<RawImage> _views = new Stack<RawImage>();
    private ConcurrentDictionary<UInt64, RawImage> _remoteViews = new ConcurrentDictionary<UInt64, RawImage>();
    private static readonly object _lock = new object();

    private byte[] buffer;
    public void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        mRtcEngine = InitHelper.StartRtcEngineEx(false);
        mRtcEngine.EnableAudio(true);
        mRtcEngine.EnableVideo(true);
        GameObject canvas = GameObject.Find("Canvas");
        canvas.AddComponent<LJVideoSurface>();
        SetDrowOption();
        lock (_lock) {
            _views.Push(_channel3Render);
            _views.Push(_channel2Render);
            _views.Push(_channel1Render);
        }

    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(30, 100, 100, 40), "加入频道"))
        {
            if (_channel1 != null) {
                return;
            }
            _channel1 = mRtcEngine.CreateChannel(_defaultChannelId, _defualtUserId);
            ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions();
            //channelMediaOptions.autoSubscribeAudio = true;
            channelMediaOptions.publishMicrophoneTrack = true;
            channelMediaOptions.publishCameraTrack = true;
            _channel1.JoinChannel(_defaultToken, 1, _defualtUserId, channelMediaOptions);
            _channel1.ChannelOnUserJoined = Channel1OnUserJoinedHandler;
            _channel1.ChannelOnUserOffLine = Channel1OnUserLeavedHandler;
        }

        if (GUI.Button(new Rect(130, 100, 200, 40), "退出频道"))
        {
            if (_channel1 != null)
            {
                _channel1.LeaveChannel();
                _channel1.ReleaseChannel();
                _channel1 = null;
            }
        }
    }

    void Channel1OnUserJoinedHandler(string channelId, UInt64 uid, int elapsed) {
        Debug.Log($"Channel1OnUserJoinedHandler {channelId} {uid} {elapsed}");
        lock (_lock)
        {
            if (_remoteViews.ContainsKey(uid)) {
                return;
            }
            RawImage view = _views.Pop();
            if (view == null) {
                return;
            }
            _channel1.SetForMultiChannelUser(view, (long)uid, 30);
            _remoteViews.TryAdd(uid, view);
        }
        
    }

    void Channel1OnUserLeavedHandler(string channelId, UInt64 uid)
    {
        Debug.Log($"Channel1OnUserJoinedHandler {channelId} {uid}");
        lock (_lock)
        {
            RawImage view;
            _remoteViews.TryRemove(uid, out view);
            if (view != null)
            {
                _views.Push(view);
            }
           
        }
    }
    void SetDrowOption()
    {
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

    public void Update()
    {
        //if (mRtcEngine != null && connection3 != null) {
            //mRtcEngine.PushAudioFrameEx(connection3, buffer, 1, 1, 1);
        //}
        
    }

    void OnApplicationQuit()
    {
    }


    private void OnDestroy()
    {
        FLog.Info("OnDestroy");
        lock (_lock) {
            _views.Clear();
        }

        if (mRtcEngine != null)
        {


            if (_channel1 != null) {
                _channel1.LeaveChannel();
                _channel1.ReleaseChannel();
                _channel1 = null;
            }
            mRtcEngine.LeaveChannel();
            mRtcEngine.OnDestroy();
            mRtcEngine = null;
        }
        FLog.Uninit();
    }

    class RtcEventHandler : IRtcEngineEventHandler
    {

        public override void onJoinChannelSuccess(string channelId, long uid, string msg)
        {
            FLog.Info("onJoinChannelSuccess channelId:" + channelId + " : uid : " + uid + " msg:" + msg);
        }

        public override void onJoinChannelFail(string channelId, long uid, string msg)
        {
            FLog.Info("onJoinChannelFail channelId:" + channelId + " uid : " + uid + " msg:" + msg);
        }

        public override void onLeavehannelSuccess(string channelId, long uid, string msg)
        {
            FLog.Info("onLeavehannelSuccess channelId:" + channelId + " uid : " + uid + " msg:" + msg);
        }

        public override void onLeaveChannelFail(string channelId, long uid, string msg)
        {
            FLog.Info("onLeaveChannelFail channelId:" + channelId + " uid : " + uid + " msg:" + msg);
        }
    }

}


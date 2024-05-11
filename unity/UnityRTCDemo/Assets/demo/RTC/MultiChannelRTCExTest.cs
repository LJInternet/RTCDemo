using LJ.demo;
using LJ.Log;
using LJ.RTC;
using LJ.RTC.Common;
using System.Collections.Generic;
using UnityEngine;
public class MultiChannelRTCExTest : MonoBehaviour
{
    internal IRtcEngineEx mRtcEngine;
    RtcEventHandler mRtcEventHandler;

    LJRtcConnection connection;
    LJRtcConnection connection1;
    LJRtcConnection connection2;
    LJRtcConnection connection3;

    private byte[] buffer;
    public void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        mRtcEngine = InitHelper.StartRtcEngineEx(false);
        mRtcEngine.EnableAudio(true);
        mRtcEventHandler = new RtcEventHandler();
        mRtcEngine.InitEventHandler(mRtcEventHandler);
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(30, 100, 100, 40), "1加入频道"))
        {
            connection = new LJRtcConnection("911111121", 1111);
            ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions();
            channelMediaOptions.autoSubscribeAudio = true;
            channelMediaOptions.publishMicrophoneTrack = true;
            mRtcEngine.JoinChannelEx(InitHelper._token, 0x70FFFFFF, connection, channelMediaOptions);
        }
        if (GUI.Button(new Rect(30, 150, 100, 40), "2加入频道"))
        {
            connection1 = new LJRtcConnection("911111121", 2222);
            ChannelMediaOptions channelMediaOptions1 = new ChannelMediaOptions();
            channelMediaOptions1.autoSubscribeAudio = true;
            //channelMediaOptions1.publishMicrophoneTrack = true;
            mRtcEngine.JoinChannelEx(InitHelper._token, 0x70FFFFFF, connection1, channelMediaOptions1);
        }
        if (GUI.Button(new Rect(30, 200, 100, 40), "3加入频道"))
        {
            connection2 = new LJRtcConnection("911111121", 3333);
            ChannelMediaOptions channelMediaOptions2 = new ChannelMediaOptions();
            channelMediaOptions2.autoSubscribeAudio = true;
            //channelMediaOptions2.publishMicrophoneTrack = true;
            mRtcEngine.JoinChannelEx(InitHelper._token, 0x70FFFFFF, connection2, channelMediaOptions2);
        }
        if (GUI.Button(new Rect(30, 250, 100, 40), "4加入频道"))
        {
            connection3 = new LJRtcConnection("911111121", 4444);
            ChannelMediaOptions channelMediaOptions3 = new ChannelMediaOptions();
            channelMediaOptions3.autoSubscribeAudio = true;
            //channelMediaOptions3.publishMicrophoneTrack = true;
            mRtcEngine.JoinChannelEx(InitHelper._token, 0x70FFFFFF, connection3, channelMediaOptions3);
        }
    }

    
    public void Update()
    {
        if (mRtcEngine != null && connection3 != null) {
            //mRtcEngine.PushAudioFrameEx(connection3, buffer, 1, 1, 1);
        }
        
    }

    void OnApplicationQuit()
    {
    }


    private void OnDestroy()
    {
        FLog.Info("OnDestroy");
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.LeaveChannelEx(connection);
            mRtcEngine.LeaveChannelEx(connection1);
            mRtcEngine.LeaveChannelEx(connection2);
            mRtcEngine.LeaveChannelEx(connection3);
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


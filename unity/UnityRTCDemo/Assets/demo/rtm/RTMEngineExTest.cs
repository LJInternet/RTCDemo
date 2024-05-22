using LJ.demo;
using LJ.Log;
using LJ.RTM;
using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class RTMEngineExTest : MonoBehaviour
{
    RTMEngineEx _RTMEngineEx;
    RTMChannel _RTMChannel1;
    RTMChannel _RTMChannel2;
    RTMChannel _RTMChannel3;
    RTMChannel _RTMChannel4;
    IRTMEngineEventHandlerEx _IRTMEngineEventHandler1;
    IRTMEngineEventHandlerEx _IRTMEngineEventHandler2;
    IRTMEngineEventHandlerEx _IRTMEngineEventHandler3;
    public void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        _RTMEngineEx = new RTMEngineEx(InitHelper._token, 3032155965, false);
        _IRTMEngineEventHandler1 = new IRTMEngineEventHandlerEx(1111);
        _IRTMEngineEventHandler2 = new IRTMEngineEventHandlerEx(2222);
        _IRTMEngineEventHandler3 = new IRTMEngineEventHandlerEx(3333);
        _RTMChannel1 = _RTMEngineEx.CreateRTMChannel(DataWorkMode.LOCK_STEP_SEND_RECV,
            1111, "911111111", _IRTMEngineEventHandler1);
        _RTMChannel2 = _RTMEngineEx.CreateRTMChannel(DataWorkMode.LOCK_STEP_SEND_RECV,
            2222, "911111111", _IRTMEngineEventHandler2);
        _RTMChannel3 = _RTMEngineEx.CreateRTMChannel(DataWorkMode.LOCK_STEP_SEND_RECV,
            3333, "911111111", _IRTMEngineEventHandler3);
    }

    public void OnGUI()
    {
        String format = "{\"seq\":\"飞车手游霸霸\",\"cmd\":\"飞车手游霸霸\",\"data\":飞车手游霸霸}";
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(format);

        if (GUI.Button(new Rect(30, 100, 100, 40), "加入发送"))
        {
            _RTMChannel1.Join();
            _RTMChannel2.Join(InitHelper._token);
            _RTMChannel3.Join(InitHelper._token);
        }
        if (GUI.Button(new Rect(150, 100, 100, 40), "1 send"))
        {
            _RTMChannel1.SendMsg("飞车手游霸霸");
            byte[] byteArray1 = System.Text.Encoding.UTF8.GetBytes("飞车手游霸霸");
            _RTMChannel1.SendMsg(byteArray1);
        }
        if (GUI.Button(new Rect(30, 150, 100, 40), "2 send"))
        {
            _RTMChannel2.SendMsg(byteArray);
        }

        if (GUI.Button(new Rect(150, 150, 100, 40), "3 send"))
        {
            _RTMChannel3.SendMsg(byteArray);
        }
    }

    private void OnDestroy()
    {
        if (_RTMChannel1 != null)
        {
            _RTMChannel1.Leave();
            _RTMChannel1.Dispose();
        }
        if (_RTMChannel2 != null)
        {
            _RTMChannel2.Leave();
            _RTMChannel2.Dispose();
        }
        if (_RTMChannel3 != null)
        {
            _RTMChannel3.Leave();
            _RTMChannel3.Dispose();
        }
        if (_RTMEngineEx != null)
        {
            _RTMEngineEx.Release();
        }
        FLog.Uninit();
    }

    class IRTMEngineEventHandlerEx : IRTMEngineEventHandler {

        private long localUid;
        public IRTMEngineEventHandlerEx(long uid) {
            localUid = uid;
        }

        public override void OnRecvMessage(byte[] message, long uid, string channelId)
        {
            FLog.Info("OnRecvMessage: uid: " + uid + " : localUid" + localUid + System.Text.Encoding.UTF8.GetString(message));
        }

        public override void OnRecvExtMessage(byte[] message, long uid, string channelId)
        {
            FLog.Info("OnRecvExtMessage: uid: " + uid + " : localUid" + localUid + System.Text.Encoding.UTF8.GetString(message));
        }

        public override void OnJoinChannelFail()
        {
            FLog.Info("OnJoinChannelFail: localUid " + localUid);
        }

        public override void OnJoinChannelSuccess()
        {
            FLog.Info("OnJoinChannelSuccess: localUid " + localUid);
        }

        public override void OnLeaveChannelSuccess()
        {
            FLog.Info("OnLeaveChannelSuccess: localUid " + localUid);
        }

        public override void OnLeaveChannelFail()
        {
            FLog.Info("OnLeaveChannelFail: localUid " + localUid);
        }

        public override void OnRemoteUserJoined(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserJoined1:" + userId);
        }

        public override void OnRemoteUserOffLine(UInt64 userId)
        {
            UnityEngine.Debug.Log("OnRemoteUserOffLine1:" + userId);
        }
    }

}


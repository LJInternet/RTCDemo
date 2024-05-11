
using LJ.demo;
using LJ.Log;
using LJ.RTC;
using LJ.RTC.Common;
using LJ.RTM;
using UnityEngine;
using UnityEngine.UI;

public class RTMEngineTest : MonoBehaviour
{
    internal IRtcEngine mRtcEngine;
    public RawImage mRemoteRender;
    public RTMEngine mRTMEngine;

    public void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        StartRtcEngine();
        RTMEngineEventHandler handler = new RTMEngineEventHandler(InitHelper.userId);
        RUDPConfig config = new RUDPConfig();
        config.isDebug = true;
        config.role = (int)RUDPRole.RUDP_CONTROLLER;
        config.dataWorkMode = (int)DataWorkMode.SEND_AND_RECV;
        config.appId = 1;
        config.token = "ssss";
        mRTMEngine = new RTMEngine(config, handler);
        int status = mRTMEngine.JoinChannel(InitHelper.userId, InitHelper._sessionId + "");
        Debug.Log("JoinChannel status " + status);
    }
    private void StartRtcEngine()
    {
        mRtcEngine = InitHelper.StartRtcEngine(false);
        mRtcEngine.EnableAudio(false);
        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
        mRtcEngine.JoinChannel(InitHelper.GetChannelConfig());
        mRtcEngine.SetRemoteRender(mRemoteRender);
    }

    private void OnGUI()
    {

        if (GUI.Button(new Rect(30, 100, 100, 40), "发送消息"))
        {
            if (mRTMEngine != null) {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("飞车手游霸霸");
                mRTMEngine.SendMsg(byteArray);
            }
        }
    }

        public void OnDestroy()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.OnDestroy();
            mRtcEngine = null;
        }
        if (mRTMEngine != null) {
            mRTMEngine.LeaveChannel();
            mRTMEngine.Destroy();
            mRTMEngine = null;
        }
        FLog.Uninit();
    }

    class RTMEngineEventHandler : IRTMEngineEventHandler
    {

        private long localUid;
        public RTMEngineEventHandler(long uid)
        {
            localUid = uid;
        }

        public override void OnRecvMessage(byte[] message, long uid, string channelId)
        {
            FLog.Info("OnRecvMessage: uid: " + uid + " : localUid " + localUid + System.Text.Encoding.UTF8.GetString(message));
        }

        public override void OnRecvExtMessage(byte[] message, long uid, string channelId)
        {
            FLog.Info("OnRecvExtMessage: uid: " + uid + " : localUid " + localUid + System.Text.Encoding.UTF8.GetString(message));
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
    }
}

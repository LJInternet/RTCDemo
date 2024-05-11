
using LJ.demo;
using LJ.Log;
using LJ.RTC;
using LJ.RTC.Common;
using UnityEngine;
using UnityEngine.UI;

public class PullStreamDemo : MonoBehaviour
{
    internal IRtcEngine mRtcEngine;
    public RawImage mRemoteRender;

    void OnApplicationQuit()
    {
        LJ.Report.ReportCenter.Release();
        Debug.Log("ReportCenter Release");
    }

    public void ReportEvent(int result, string msg)
    {
        FLog.Info("result:" + result + ", msg:" + msg);
    }

    public void Start()
    {
        InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
        InitHelper.InitReport(ReportEvent);
        StartRtcEngine();
    }
    private void StartRtcEngine()
    {
        mRtcEngine = InitHelper.StartRtcEngine(false);
        mRtcEngine.EnableAudio(false);
        mRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
        mRtcEngine.JoinChannel(InitHelper.GetChannelConfig());
        mRtcEngine.SetRemoteRender(mRemoteRender);
    }

    public void OnDestroy()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine.OnDestroy();
            mRtcEngine = null;
        }
        FLog.Uninit();
    }
}

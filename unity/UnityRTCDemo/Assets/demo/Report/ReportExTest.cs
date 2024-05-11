using LJ.demo;
using LJ.Log;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace LJ.Report
{
    public class ReportExTest : MonoBehaviour
    {
        private Timer mTimer;
        private bool mRunning = false;
        private static long TIMER_TIME = 1000;

        public static string NETWORK_SLOT = "NetInfo";
        public static string NETWORK_SLOT1 = "NetInfo11";
        public void Start()
        {
            InitHelper.InitLog(Application.temporaryCachePath + "/log", "Fancy", 3);
            LJ.Report.ReportCenterConfig cfg = new LJ.Report.ReportCenterConfig();
            cfg.eventCb = ReportEvent;
            cfg.collectDuration = 10000;
            cfg.isTestEvn = true;
            cfg.appId = cfg.isTestEvn ? 1003 : 1002;
            LJ.Report.ReportCenter.InitEx(cfg);
            LJ.Report.ReportCenter.SetUserInfo(InitHelper._token, 123);
            Dictionary<string, System.Object> info = new Dictionary<string, System.Object>();
            info.Add("appid", cfg.appId);
            info.Add("liveid", 345678);
            LJ.Report.ReportCenter.SetCommonAttrs(info);
            LJ.Report.ReportCenter.EnablePerformance(true);
            StartTimer();
        }

        private void StartTimer() {
            if (mTimer != null)
            {
                return;
            }
            mRunning = true;
            mTimer = new Timer(DoTimerCallback, 1, TIMER_TIME, Timeout.Infinite);
        }

        private void StopTimer() {
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
            Dictionary<string, System.Object> info = new Dictionary<string, object>();
            info.Add("test1", 1);
            info.Add("test2", "strin111g");
            info.Add("test3", (long)1212);
            ReportCenter.Report(NETWORK_SLOT, info);
            ReportCenter.ReportEvent(NETWORK_SLOT1, info);
            FLog.Info("DoTimerCallback");
            mTimer.Change(TIMER_TIME, Timeout.Infinite);
        }

        public void OnDestroy()
        {
            StopTimer();
            LJ.Report.ReportCenter.Release();
            FLog.Uninit();
        }

        public void ReportEvent(int result, string msg) {
            FLog.Info("result:" + result + ", msg:" + msg);
        }
    }
}

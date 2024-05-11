
using LJ.Log;
using LJ.Report;
using System;
using System.Threading;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif
namespace LJ.Report
{

    public abstract class Performance {

        private Timer mTimer;
        private bool mRunning = false;
        private static long TIMER_TIME = 2000;

        public static string NETWORK_SLOT = "NetInfo";
        public static string MEMORY_USAGE_SLOT = "memoryusage";
        public virtual void Start() {
            if (mTimer != null) {
                return;
            }
            mRunning = true;
            mTimer = new Timer(DoTimerCallback, 1, TIMER_TIME, Timeout.Infinite);
        }

        public virtual void Stop() {
            if (mTimer == null)
            {
                return;
            }
            mRunning = false;
        }

        public void DoTimerCallback(object state) {
            if (!mRunning) {
                mTimer.Change(-1, Timeout.Infinite);
                mTimer.Dispose();
                mTimer = null;
                return;
            }
            OnTimeCallback();

            mTimer.Change(TIMER_TIME, Timeout.Infinite);
        }

        public abstract void OnTimeCallback();

        public static Performance CreatePerformance()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return null;
#elif UNITY_ANDROID
            return new AndroidPerformance();
#else
            return null;
#endif
        }
    }



#if UNITY_ANDROID && !UNITY_EDITOR
    public class AndroidPerformance : Performance
    {
        private AndroidJavaObject mContext;
        private AndroidJavaObject mPerformanceUtils;

        public override void Start() {
            InitAndroidEvent();
            base.Start();
        }

        public override void Stop() {
            base.Stop();
            DestroyAndroidEvent();


        }

        private void DestroyAndroidEvent()
        {
            mContext = null;
            mPerformanceUtils = null;
        }

        private void InitAndroidEvent() {
            FLog.Info("InitAndroidEvent");
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                FLog.Info("!Permission.FineLocation");
                PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
                permissionCallbacks.PermissionGranted += OnPermissionGranted;
                permissionCallbacks.PermissionDenied += OnPermissionDenied;
                permissionCallbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.FineLocation, permissionCallbacks);
            }
            try
            {
                AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                mContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
                mPerformanceUtils = new AndroidJavaObject("com.linjing.base.apm.PerformanceUtils");
                if (mPerformanceUtils == null)
                {
                    FLog.Info("mPerformanceUtils is null!!");
                }
            }
            catch (Exception e)
            {
                FLog.Error("mPerformanceUtils " + e.StackTrace);
            }
        }

        public void OnPermissionGranted(string action) {
        
        }

        public void OnPermissionDenied(string action)
        {

        }

        public void OnPermissionDeniedAndDontAskAgain(string action)
        {

        }

        public override void OnTimeCallback()
        {

            if (mContext == null || mPerformanceUtils == null) {
                FLog.Error("OnTimeCallback mContext == null || mPerformanceUtils == null!!");
                return;
            }
            AndroidJNI.AttachCurrentThread();
            string networkInfo = mPerformanceUtils.Call<string>("getNetInfo", mContext);
            string memoryInfo = mPerformanceUtils.Call<string>("getMemoryInfo", mContext);
            AndroidJNI.DetachCurrentThread();
            if (networkInfo != null)
            {
                ReportCenter.Report(NETWORK_SLOT, networkInfo);
            }
            else {
                FLog.Error("getNetInfo result == null");
            }
            if (memoryInfo != null)
            {
                ReportCenter.Report(MEMORY_USAGE_SLOT, memoryInfo);
            }
            else
            {
                FLog.Error("getMemoryInfo result == null");
            }
        }
    }
#endif
}

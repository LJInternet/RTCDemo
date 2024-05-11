using LJ.RTC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LJ.RTC.Video
{
    public class VideoCaptureTracker
    {
        public static string CAPTURE_SLOT = "VCapture";

        private int mCVType;
        private long mCFps;
        private long mCVEFps;
        private long mLastReprotTime;
        private static int REPORT_DURATION = 2000;
        private long mStartTime;
        private long mStopTime;
        private long mPreviewStopTime;
        private long mTotalCost = 0;
        private long mPreviewCost = 0;

        Dictionary<string, System.Object> reprotInfo = new Dictionary<string, System.Object>();

        public void Init(int type) {
            mCVType = type;
        }

        public void OnCaptureStart(bool isEncode, long startTime) {
            mStartTime = startTime;
            if (isEncode) {
                mCVEFps++;
            }
            mCFps++;
        }

        public void OnEncodeStop(long time) {
            mPreviewStopTime = time;
            
        }

        public void OnCaptureStop(bool isEncode, long time) {
            mStopTime = time;
            mTotalCost += mStopTime - mStartTime;
            if (isEncode) {
                mPreviewCost += mPreviewStopTime - mStartTime;
            }
            if (mLastReprotTime <= 0) {
                mLastReprotTime = time;
                return;
            }
            if (mCFps == 0 || mPreviewCost == 0) {
                return;
            }
            if (time - mLastReprotTime >= REPORT_DURATION) {
                mLastReprotTime = time;
                long avgTotalCost = mTotalCost / mCFps;
                long avgEncodeCost = mPreviewCost / mCVEFps;
                reprotInfo.Clear();
                reprotInfo.Add("CVFps", (int)mCFps / 2);
                reprotInfo.Add("CVPFps", (int)mPreviewCost / 2);
                reprotInfo.Add("CVPCost", (int)avgEncodeCost);
                reprotInfo.Add("CVCost", (int)avgTotalCost);
                reprotInfo.Add("CVType", (int)mCVType);
                IRtcEngine.DoReport(CAPTURE_SLOT, JsonConvert.SerializeObject(reprotInfo));
                mTotalCost = mPreviewCost = mCFps = mCVEFps = 0;
            }

        }

        



    }
}

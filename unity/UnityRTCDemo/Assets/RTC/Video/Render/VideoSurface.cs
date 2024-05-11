using System.Collections;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace LJ.RTC.Video
{
    public class LJVideoSurface : MonoBehaviour
    {

        public delegate void OnCaptureParamChange(int width, int height, int facing, int rotation);

        public event OnCaptureParamChange mParamCallback;
        private IRtcEngine mRtcEngine;
        private long mLastReadTime = 0;
        private int fpsTime = 0;
        private int mFps;
        private int mFrameCount;
        private long mLeftTime = 1000;

#if (UNITY_ANDROID|| UNITY_IOS) && !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
        private static int mRenderFps = 40;
#else
        private static int mRenderFps = 50;
#endif
        private int mRenderTime = 1000 / mRenderFps;
        private long mLastRenderTime = 0;
        private int mRenderCount = 0;
        private long mLeftRenderTime = 1000;
        void Start()
        {
            mRtcEngine = IRtcEngine.Get();
            RawImage rawImage = GetComponent<RawImage>();
            if (mRtcEngine != null)
            {
                mRtcEngine.SetRender(rawImage, OnCaptureParamCallback);
            }
        }

        // Update is called once per frame
        void Update()
        {
            long currTime = System.DateTime.Now.Ticks / 10000;
            if (mLastRenderTime == 0)
            {
                mLastRenderTime = currTime;
            }
            // 这里减去5是因为每一帧回调的时间有一定的误差，当两帧间隔在帧率-5的范围内，认为需要进行编码
            if ((currTime - mLastRenderTime) >= mRenderTime - 5)
            {
                mRenderCount++;
                if (!ReadCameraPixel(currTime))
                {
                    StartCoroutine(ReadPixel(false));
                }

                if (mRenderCount >= mRenderFps)
                {
                    mRenderCount = 0;
                    mLeftRenderTime = 1000;
                    mRenderTime = (int)mLeftRenderTime / mRenderFps;
                }
                else
                {
                    int leftCount = (mRenderFps - mRenderCount);
                    mLeftRenderTime = mLeftRenderTime - (currTime - mLastRenderTime);
                    mRenderTime = (int)mLeftRenderTime / leftCount;
                }
                mLastRenderTime = currTime;
            }
        }
               

        private bool ReadCameraPixel(long currTime) {
            if (fpsTime == 0)
            {
                if (mFps != 0)
                {
                    fpsTime = 1000 / mFps;
                }
                return false;
            }
            
            if (mLastReadTime == 0)
            {
                mLastReadTime = currTime;
            }
            // 这里减去5是因为每一帧回调的时间有一定的误差，当两帧间隔在帧率-5的范围内，认为需要进行编码
            if ((currTime - mLastReadTime) >= fpsTime - 5)
            {
                StartCoroutine(ReadPixel(true));
                mFrameCount++;
                if (mFrameCount >= mFps)
                {
                    mFrameCount = 0;
                    mLeftTime = 1000;
                    fpsTime = (int)mLeftTime / mFps;
                }
                else
                {
                    int leftCount = (mFps - mFrameCount);
                    mLeftTime = mLeftTime - (currTime - mLastReadTime);
                    fpsTime = (int)mLeftTime / leftCount;
                }
                mLastReadTime = currTime;
                return true;
            }
            return false;
        }

        private IEnumerator ReadPixel(bool encode)
        {

            yield return new WaitForEndOfFrame();

            if (mRtcEngine != null)
            {
                mRtcEngine.ReadCameraPixel(encode);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            mRtcEngine = IRtcEngine.Get();
            if (mRtcEngine != null)
            {
                mRtcEngine.SetRender(null, null);
            }
        }

        public void OnCaptureParamCallback(int width, int height, int facing, int rotation, int fps)
        {
            mFps = fps;
            JLog.Info("fpsTime " + mFps);
            if (mParamCallback != null && width != 0)
            {
                mParamCallback(width, height, facing, rotation);
            }
        }
    }
}

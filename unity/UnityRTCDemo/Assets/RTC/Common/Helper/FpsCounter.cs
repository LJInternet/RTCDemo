using LJ.RTC.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FpsCounter
{
    private static string TAG = "Fps";

    private string mTag;

    private long mDefaultDuration = 1000;
    private long mLastTime = 0;
    private long mBitrate = 0;
    private long mFps = 0;

    private List<long> mFpsList;

    public FpsCounter(string parentTag)
    {
        mTag = parentTag + "_" + TAG;
    }

    public FpsCounter(string parentTag, long defaultDuration)
    {
        mTag = parentTag + "_" + TAG;
        this.mDefaultDuration = defaultDuration;
    }

    public void setDefaultDuration(long defaultDuration)
    {
        this.mDefaultDuration = defaultDuration;
    }

    public void addFrame(byte[] data)
    {
        addFrame(data != null ? data.Length * 8 : 0);
    }

    public void addFrame()
    {
        addFrame(0);
    }

    public void addFrame(int bits)
    {
        long currTime = System.DateTime.Now.Ticks / 10000;
        if (currTime - mLastTime >= mDefaultDuration)
        {
            if (mLastTime > 0)
            {
                if (bits == 0)
                {
                    JLog.Info(mTag + "mFps=" + mFps);
                }
                else {
                    JLog.Info(mTag + "byteSize=" + mBitrate + ":mFps=" + mFps);
                }  
            }
            mLastTime = currTime;
            mBitrate = 0;
            mFps = 0;
        }

        mFps++;
        mBitrate += bits;
    }

    public void addFrame(long time)
    {
        long currTime = System.DateTime.Now.Ticks / 10000;
        if (mFpsList == null)
        {
            mFpsList = new List<long>();
        }
        mFpsList.Add(time);
        if (currTime - mLastTime >= mDefaultDuration)
        {
            if (mLastTime > 0)
            {
                JLog.Info(mTag, /*Arrays.toString(mFpsList.toArray()) +*/ "mFps = " + mFps  + ",avg=" + getAvg());
                mFpsList.Clear();
            }
            mLastTime = currTime;
            mFps = 0;
        }
        mFps++;
    }

    private int getAvg()
    {
        int size = mFpsList != null ? mFpsList.Count : 0;
        if (size == 0)
        {
            return 0;
        }
        long sum = 0;
        for (int i = 0; i < size; i++)
        {
            sum += mFpsList[i];
        }
        return (int)(sum * 1.0f / size);
    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AudioCaptureTest : MonoBehaviour
{

    private AudioClip mAudioClip;
    private bool mRecording = false;
    private float[] samples;
    private string deviceName;
    private int lastSample;
    private FpsCounterTest mFpsCounter = new FpsCounterTest("AudioCapture", 1000);

    private void Start()
    {
        StartCapture();
    }

    private void FixedUpdate()
    {
        CaptureProcess();
    }

    private void OnDestroy()
    {
        StopAudioCapture();
        if (mAudioClip != null)
        {
            Destroy(mAudioClip);
            mAudioClip = null;
        }
    }

    public int StartCapture()
    {

        mRecording = true;
        InitCaptureThread();
        return 0;
    }

    private void InitCaptureThread()
    {

        deviceName = Microphone.devices.Last();
        int min;
        int max;
        Microphone.GetDeviceCaps(deviceName, out min, out max);

        mAudioClip = Microphone.Start(deviceName, true, 10, max);
        samples = new float[480];
    }

    public void StopAudioCapture()
    {
        mRecording = false;
        Microphone.End(deviceName);
    }



    private void CaptureProcess()
    {
        if (mAudioClip == null)
        {
            return;
        }
        int pos = Microphone.GetPosition(deviceName);
        int diff = pos - lastSample;
        if (diff > 0)
        {
            mAudioClip.GetData(samples, lastSample);
            mFpsCounter.addFrame(samples.Length * 2);
            UnityEngine.Debug.Log("lastSample =" + diff);
            //File.WriteAllBytes(Application.temporaryCachePath + "/" + "image_" + str + ".jpg", tempByte);
        }
        lastSample = pos;
    }

    static byte[] GetBytes(float[] values)
    {
        var result = new byte[values.Length * sizeof(double)];
        Buffer.BlockCopy(values, 0, result, 0, result.Length);
        return result;
    }

}

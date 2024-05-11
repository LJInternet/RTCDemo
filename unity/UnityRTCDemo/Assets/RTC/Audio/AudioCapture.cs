using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AudioCapture : MonoBehaviour
{
    private AudioClip mAudioClip;
    private bool mRecording = false;
    private float[] samples;
    private string deviceName;
    private int lastSample;

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
        if (mAudioClip != null) {
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

    public void StopAudioCapture() {
        mRecording = false;
        Microphone.End(deviceName);
    }

    

    private void CaptureProcess()
    {
        if (mAudioClip == null) {
            return;
        }
        int pos = Microphone.GetPosition(deviceName);
        int diff = pos - lastSample;
        if (diff > 0)
        {
            mAudioClip.GetData(samples, lastSample);
        }
        lastSample = pos;
    }
}

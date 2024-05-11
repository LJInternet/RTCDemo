using LJ.RTC.Common;
using System;
using UnityEngine;

namespace LJ.RTC.Audio
{
    enum MixingAction
    {
        ACTION_START = 0,
        ACTION_STOP = 1,
        ACTION_PAUSE = 2,
        ACTION_RESUME = 3,
        ACTION_SET_POST = 4
        // 0 start 1 stop 2 pause 3 resume 4 set position
    };

    class AndrdoidSubMixProxy : AndroidJavaProxy {

        public Action<int> onCallback;
        public AndrdoidSubMixProxy() : base("com.linjing.capture.audio.IMediaProjectionListener") {
        
        }
        public void onGetProjection(int result) {
            JLog.Info("JniEvnHelper unity onGetProjection");
            onCallback?.Invoke(result);
        }

    }
    internal class AudioManger : BaseRtcEngineModule
    {

        private AndroidJavaObject mJniHelper;

        public AudioManger(IRtcEngineApi rtcEngineApi) : base(rtcEngineApi)
        {
            OnCreate();
        }

        public override void OnCreate()
        {
            if (mRtcEngineApi != null)
            {
                AudioEnableEvent createEvent = new AudioEnableEvent();
                createEvent.evtType = (int)MediaInvokeEventType.AUDIO_CREATE;
                mRtcEngineApi.SendMediaEvent(createEvent.evtType, createEvent.HPmarshall());

                SetAudioProfile((int)AudioProfile.AUDIO_PROFILE_DEFAULT, 0);
            }
        }

        public int EnableAudio(bool enable)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            InintAndroidJniEvn();
#endif
            return SendAudioEnableEvent(enable);
        }

        private void InintAndroidJniEvn() {
            try
            {
                mJniHelper = new AndroidJavaObject("com.linjing.capture.audio.jni.JniEvnHelper");
                mJniHelper.Call("setContext");
            } catch (Exception e) {
                JLog.Error(e.StackTrace);
            }

        }

        private int SendAudioEnableEvent(bool enable)
        {
            if (mRtcEngineApi != null)
            {
                AudioEnableEvent captureEvent = new AudioEnableEvent();
                captureEvent.evtType = (int)MediaInvokeEventType.AUDIO_ENABLE_EVENT;
                captureEvent.enabled = enable;
                return mRtcEngineApi.SendMediaEvent(captureEvent.evtType, captureEvent.HPmarshall());
            }
            return -1;
        }

        public override void OnDestroy()
        {
            if (mRtcEngineApi != null)
            {
                AudioEnableEvent captureEvent = new AudioEnableEvent();
                captureEvent.evtType = (int)MediaInvokeEventType.AUDIO_DESTROY;
                captureEvent.enabled = false;
                mRtcEngineApi.SendMediaEvent(captureEvent.evtType, captureEvent.HPmarshall());
            }
            if (mJniHelper != null) {
                try
                {
                    mJniHelper.Call("destroy");
                }
                catch (Exception e)
                {
                    JLog.Error(e.StackTrace);
                }
            }
            base.OnDestroy();
        }

        internal int SetAudioProfile(int profile, int scenario)
        {
            if (mRtcEngineApi != null)
            {
                AudioProfileEvent captureEvent = new AudioProfileEvent();
                captureEvent.profile = profile;
                captureEvent.scenario = scenario;
                mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_SET_PROFILE_EVENT, captureEvent.HPmarshall());
                return 0;
            }
            return -1;
        }

        internal int EnableSubMix(bool enable)
        {
            if (mJniHelper != null)
            {
                try
                {
                    if (enable)
                    {
                        AndrdoidSubMixProxy subMixProxy = new AndrdoidSubMixProxy();
                        subMixProxy.onCallback = onSubMixGetProjection;
                        mJniHelper.Call("startScreenCapture", subMixProxy);
                    }
                    else {
                        return CallSubmixEnable(false);
                    }
                }
                catch (Exception e)
                {
                    JLog.Error(e.StackTrace);
                }
            }
            else {
                return CallSubmixEnable(true);
            }
            return 0;
        }
        public void onSubMixGetProjection(int result)
        {
            JLog.Info("JniEvnHelper unity onSubMixGetProjection");
            CallSubmixEnable(true);
        }

        private int CallSubmixEnable(bool enable) {
            if (mRtcEngineApi != null)
            {
                AudioEnableEvent subMixEvent = new AudioEnableEvent();
                subMixEvent.enabled = enable;
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_SUBMIX_EVENT, subMixEvent.HPmarshall());
            }
            return -1;
        }

        internal int AdjustMicVolume(int volume)
        {
            if (mRtcEngineApi != null)
            {
                AudioAdjustEvent subMixEvent = new AudioAdjustEvent();
                subMixEvent.val = volume;
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_ADJUST_MIC_VOLUME_EVENT, subMixEvent.HPmarshall());
            }
            return 0;
        }

        internal int AdjustSubMixVolume(int volume)
        {
            if (mRtcEngineApi != null)
            {
                AudioAdjustEvent subMixEvent = new AudioAdjustEvent();
                subMixEvent.val = volume;
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_ADJUST_SUBMIX_VOLUME_EVENT, subMixEvent.HPmarshall());
            }
            return 0;
        }

        internal int StartAudioMixing(string filePath, bool loopback, int cycle, int startPos)
        {
            AudioMixingEvent audioMixing = new AudioMixingEvent();
            audioMixing.filePath = filePath;
            audioMixing.loopback = loopback;
            audioMixing.cycle = cycle;
            audioMixing.startPos = startPos;
            audioMixing.acton = (int)MixingAction.ACTION_START;
            return doAudioMixingEvent(audioMixing);
        }

        internal int StopAudioMixing()
        {
            AudioMixingEvent audioMixing = new AudioMixingEvent();
            audioMixing.acton = (int)MixingAction.ACTION_STOP;
            return doAudioMixingEvent(audioMixing);
        }

        internal int PauseAudioMixing()
        {
            AudioMixingEvent audioMixing = new AudioMixingEvent();
            audioMixing.acton = (int)MixingAction.ACTION_PAUSE;
            return doAudioMixingEvent(audioMixing);
        }

        internal int ResumeAudioMixing()
        {
            AudioMixingEvent audioMixing = new AudioMixingEvent();
            audioMixing.acton = (int)MixingAction.ACTION_RESUME;
            return doAudioMixingEvent(audioMixing);
        }

        internal int SetAudioMixingPosition(int pos)
        {
            AudioMixingEvent audioMixing = new AudioMixingEvent();
            audioMixing.acton = (int)MixingAction.ACTION_RESUME;
            audioMixing.startPos = pos;
            return doAudioMixingEvent(audioMixing);
        }

        internal int doAudioMixingEvent(AudioMixingEvent audioMixing)
        {
            if (mRtcEngineApi != null)
            {
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_MIXING_EVENT, audioMixing.HPmarshall());
            }
            return 0;
        }

        public int enableAudioVolumeIndication(int interval, int smooth, bool report_vad) {
            if (mRtcEngineApi != null)
            {
                AudioVolumeIndicationEvent audioVolumeIndicationEvent = new AudioVolumeIndicationEvent();
                audioVolumeIndicationEvent.interval = interval;
                audioVolumeIndicationEvent.smooth = smooth;
                audioVolumeIndicationEvent.reportVad = report_vad;
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_VOLUME_INDICATION_EVENT, audioVolumeIndicationEvent.HPmarshall());
            }
            return 0;
        }

        public int muteLocalAudioStream(bool muted)
        {
            if (mRtcEngineApi != null)
            {
                AudioEnableEvent enableEvent = new AudioEnableEvent();
                enableEvent.enabled = muted;
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_MUTE_LOCAL_STREAM_EVENT, enableEvent.HPmarshall());
            }
            return 0;
        }

        public int muteRemoteAudioStream(bool muted) {
            if (mRtcEngineApi != null)
            {
                MediaMuteEvent enableEvent = new MediaMuteEvent(MediaMuteEvent.EVENT_AUDIO, muted);
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.MIET_MUTE_MEDIA_EVENT, enableEvent.HPmarshall());
            }
            return 0;
        }

        public int saveRecordCallbackAudio(bool enabled) {
            if (mRtcEngineApi != null)
            {
                AudioEnableEvent enableEvent = new AudioEnableEvent();
                enableEvent.enabled = enabled;
                return mRtcEngineApi.SendMediaEvent((int)MediaInvokeEventType.AUDIO_DEBUG_SAVE_CALLBACK_FILE, enableEvent.HPmarshall());
            }
            return 0;
        }

    }
}

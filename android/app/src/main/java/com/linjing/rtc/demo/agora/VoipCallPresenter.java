package com.linjing.rtc.demo.agora;

import android.content.Context;
import android.view.SurfaceView;
import android.widget.FrameLayout;

import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.LJRtcEngine;
import com.linjing.rtc.api.ChannelConfig;
import com.linjing.rtc.api.RtcEngineConfig;
import com.linjing.rtc.base.IRtcEngine;
import com.linjing.sdk.api.DelayData;
import com.linjing.sdk.api.IDecodeObserver;
import com.linjing.sdk.api.RTCEngineConstants;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.VideoEncoderConfiguration;
import com.linjing.transfer.views.VideoViews;

import java.lang.ref.WeakReference;
import java.util.Map;

public class VoipCallPresenter {

    private WeakReference<VoipCallActivity> mView;

    IRtcEngine mRtcEngine;
    long uid = System.currentTimeMillis();

    public VoipCallPresenter(VoipCallActivity view) {
        mView = new WeakReference<VoipCallActivity>(view);
        try {
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngine.create(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
            mRtcEngine.setAudioProfile(RTCEngineConstants.AudioProfile.AUDIO_PROFILE_MUSIC_STANDARD_STEREO, 0);
            VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration(544, 960,
                    30, 1200, VideoEncoderConfiguration.OrientationMode.ORIENTATION_MODE_FIXED_PORTRAIT);
            mRtcEngine.setVideoEncoderConfiguration(encoderConfiguration);
            mRtcEngine.enableVideo();
            mRtcEngine.enableAudio();

            mRtcEngine.registerDecodeObserver(new IDecodeObserver() {
                @Override
                public void onDelayStatisticData(int frameId, byte[] extraData, Map<Long, Long> delayMap) {

                }

                @Override
                public void onDelayStaticsEvent(DelayData delayData) {
                    if (mView != null && mView.get() != null) {
                        mView.get().setDelayData(delayData);
                        JLog.info("onDelayStaticsEvent", delayData.toString());
                    }

                }
            });
        } catch (Exception e) {

        }

    }

    public void startVoip() {
        LJRtcEngine.setDebugEnv(true);
        ChannelConfig channelConfig = new ChannelConfig();
        channelConfig.appID = BuildConfig.appId;
        channelConfig.userID = uid;
        channelConfig.channelID = BuildConfig.channelId + "";
        channelConfig.token = BuildConfig.token;
        mRtcEngine.joinChannel(channelConfig);
    }

    public void stopVoip() {
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
        }
    }

    public void setRtcRole(int role) {
        if (mRtcEngine != null) {
            mRtcEngine.setClientRole(role);
        }
    }

    public void setupRemoteUi(Context context, FrameLayout group) {
        JLog.info("MediaPlayer", " setupRemoteUi ");
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupRemoteVideo(views);
        surfaceView.setKeepScreenOn(true);
        group.addView(surfaceView);
    }

    public void setupLocalVideo(Context context, FrameLayout group) {
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        surfaceView.setKeepScreenOn(true);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupLocalVideo(views);
        group.addView(surfaceView);
    }

    public void stop() {
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
            mRtcEngine.registerDecodeObserver(null);
            mRtcEngine.destroy();
            mRtcEngine = null;
        }
    }

}

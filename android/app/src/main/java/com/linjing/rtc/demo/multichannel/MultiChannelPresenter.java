package com.linjing.rtc.demo.multichannel;

import android.view.SurfaceView;
import android.widget.FrameLayout;

import com.linjing.rtc.api.RtcEngineConfig;
import com.linjing.rtc.base.IRtcEngineEx;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.sdk.api.RTCEngineConstants;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.VideoEncoderConfiguration;
import com.linjing.transfer.multichannel.ILJChannelEventHandler;
import com.linjing.transfer.multichannel.LJChannel;
import com.linjing.transfer.upload.api.ChannelMediaOptions;
import com.linjing.transfer.views.VideoViews;

public class MultiChannelPresenter extends ILJChannelEventHandler {

    IRtcEngineEx mRtcEngine;

    LJChannel _channel;

    private MultiChannelActivity mActivity;

    long _userId = System.currentTimeMillis();

    public MultiChannelPresenter(MultiChannelActivity activity) {
        try {
            mActivity = activity;
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngineEx.CreateRtcEngineEx(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
            mRtcEngine.setAudioProfile(RTCEngineConstants.AudioProfile.AUDIO_PROFILE_DEFAULT, 0);
            VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration(1080, 1920,
                    30, 1200, VideoEncoderConfiguration.OrientationMode.ORIENTATION_MODE_FIXED_PORTRAIT);
//            encoderConfiguration.codecType = VideoEncoderConfiguration.CODEC_TYPE_H265;
//            encoderConfiguration.keyFrameInterval = -1;
//            encoderConfiguration.bitrateMode = 1;
            mRtcEngine.setVideoEncoderConfiguration(encoderConfiguration);
            mRtcEngine.enableVideo();
            mRtcEngine.enableAudio();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void startUpload(String sessionId) {
        if (_channel != null) {
            return;
        }
        _channel = mRtcEngine.CreateChannel(sessionId, _userId);
        ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions();
        channelMediaOptions.publishMicrophoneTrack = true;
        channelMediaOptions.publishCameraTrack = true;
        _channel.JoinChannel(BuildConfig.token, BuildConfig.appId, _userId, channelMediaOptions);
        _channel.setRtcChannelEventHandler(this);
    }

    public void stopUpload() {
        if (_channel != null) {
            _channel.LeaveChannel();
            _channel.ReleaseChannel();
            _channel = null;
        }
    }

    public void setupLocalVideo(MultiChannelActivity multiChannelActivity, FrameLayout mLocalLayout) {
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(multiChannelActivity);
        surfaceView.setKeepScreenOn(true);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupLocalVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        mLocalLayout.addView(surfaceView);
    }

    public void switchCamera() {
        if (mRtcEngine != null) {
            mRtcEngine.switchCamera();
        }
    }

    @Override
    public void onJoinChannelSuccess(String channelId, long uid, long elapsed) {
        JLog.info("onJoinChannelSuccess");
    }

    @Override
    public void onLeaveChannelSuccess(LJChannel ljChannel) {
        JLog.info("onLeaveChannelSuccess");
    }

    @Override
    public void onLinkStatus(LJChannel ljChannel, int result) {
        JLog.info("onLinkStatus status " + ljChannel.getConnection().key + " : " + result);
    }


    @Override
    public void onUserJoined(LJChannel ljChannel, long uid, int elapsed) {
        JLog.info("onUserJoined uid " + ljChannel.getConnection().key + " : " + uid);
        if (mActivity != null && mRtcEngine != null) {
            mActivity.onUserJoined(mRtcEngine, ljChannel, uid, 30);
        }
    }

    @Override
    public void onUserOffLine(LJChannel ljChannel, long uid) {
        JLog.info("onUserOffLine uid " + ljChannel.getConnection().key + " : " + uid);
        if (mActivity != null && mRtcEngine != null) {
            mActivity.onUserOffLine(mRtcEngine, ljChannel, uid);
        }
    }

    public void destroy() {
        stopUpload();
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
            mRtcEngine.destroy();
            mRtcEngine = null;
        }
    }
}

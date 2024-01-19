package com.linjing.rtc.demo.agora;

import android.app.Activity;
import android.content.Context;
import android.view.SurfaceView;
import android.widget.FrameLayout;

import com.linjing.rtc.LJRtcEngine;
import com.linjing.rtc.api.ChannelConfig;
import com.linjing.rtc.api.RtcEngineConfig;
import com.linjing.rtc.base.IRtcEngine;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rudp.RUDPCallback;
import com.linjing.rudp.RudpEngineJni;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.api.RTCEngineConstants;
import com.linjing.sdk.api.audio.AudioFrame;
import com.linjing.sdk.api.audio.AudioParams;
import com.linjing.sdk.api.audio.IAudioFrameObserver;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.IVideoFrameObserver;
import com.linjing.sdk.api.video.VideoEncoderConfiguration;
import com.linjing.sdk.api.video.VideoFrame;
import com.linjing.transfer.views.VideoViews;

public class AgoraCameraPushPresenter extends IAgoraPushPresenter implements RUDPCallback {

    IRtcEngine mRtcEngine;
    private RudpEngineJni mRudpEngine;

    public AgoraCameraPushPresenter() {
        try {
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngine.create(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
            mRtcEngine.setAudioProfile(RTCEngineConstants.AudioProfile.AUDIO_PROFILE_DEFAULT, 0);
            VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration(1080, 1920,
                    30, 1200, VideoEncoderConfiguration.OrientationMode.ORIENTATION_MODE_FIXED_PORTRAIT);
//            encoderConfiguration.codecType = VideoEncoderConfiguration.CODEC_TYPE_H265;
            encoderConfiguration.keyFrameInterval = -1;
            encoderConfiguration.bitrateMode = 1;
            mRtcEngine.setVideoEncoderConfiguration(encoderConfiguration);
            mRtcEngine.enableVideo();
            mRtcEngine.enableAudio();
            mRtcEngine.registerVideoFrameObserver(new IVideoFrameObserver() {
                @Override
                public boolean onCaptureVideoFrame(VideoFrame videoFrame) {
                    JLog.info("onCaptureVideoFrame");
                    return false;
                }

                @Override
                public boolean onRenderVideoFrame(int uid, VideoFrame videoFrame) {
                    JLog.info("onRenderVideoFrame");
                    return false;
                }

                @Override
                public boolean onPreDecodeVideoFrame(int uid, VideoFrame videoFrame) {
                    JLog.info("onPreDecodeVideoFrame");
                    return  true;
                }

                @Override
                public int getObservedFramePosition() {
                    return POSITION_PRE_DECODE;
                }
            });

            mRtcEngine.registerAudioFrameObserver(new IAudioFrameObserver() {
                @Override
                public boolean onRecordFrame(AudioFrame audioFrame) {
                    return false;
                }

                @Override
                public boolean onPlaybackFrame(AudioFrame audioFrame) {
                    return false;
                }

                @Override
                public boolean onPlaybackFrameBeforeMixing(AudioFrame audioFrame, int uid) {
                    return false;
                }

                @Override
                public boolean onMixedFrame(AudioFrame audioFrame) {
                    return false;
                }

                @Override
                public boolean isMultipleChannelFrameWanted() {
                    return false;
                }

                @Override
                public boolean onPlaybackFrameBeforeMixingEx(AudioFrame audioFrame, int uid, String channelId) {
                    return false;
                }

                @Override
                public int getObservedAudioFramePosition() {
                    return POSITION_RECORD;
                }
                AudioParams audioParams = new AudioParams();

                @Override
                public AudioParams getRecordAudioParams() {
                    audioParams.mode = AudioParams.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE;
                    return audioParams;
                }

                @Override
                public AudioParams getPlaybackAudioParams() {
                    return null;
                }

                @Override
                public AudioParams getMixedAudioParams() {
                    return null;
                }
            });
        } catch (Exception e) {
            e.printStackTrace();
        }

        mRudpEngine = new RudpEngineJni();
        mRudpEngine.create(this);
        LJSDK.instance().postDelayMsg(new Runnable() {

            @Override
            public void run() {
                if (mRudpEngine != null) {
                    mRudpEngine.sendMessage("testdfdfssdfadfsdfadf;adkfja;dkgjas;fgai9rtgyhaf;gasdlkfajs;dfkjaSD:fkajs;df".getBytes());
                    LJSDK.instance().postDelayMsg(this, 100);
                }

            }
        }, 0);
    }

    public void stop() {
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
            mRtcEngine.destroy();
            mRtcEngine = null;
        }

        if (mRudpEngine != null) {
            mRudpEngine.leaveChannel();
            mRudpEngine.destroy();
            mRudpEngine = null;
        }
    }

    @Override
    public void startDubbing(Activity activity, boolean isDubbing) {
    }

    public void setupLocalVideo(Context context, FrameLayout group) {
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        surfaceView.setKeepScreenOn(true);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupLocalVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        group.addView(surfaceView);
    }

    public void startUpload(String sessionId) {
        LJRtcEngine.setDebugEnv(true);
        ChannelConfig channelConfig = new ChannelConfig();
//        UdpInitConfig initConfig = new UdpInitConfig();
//        initConfig.remoteSessionId = BuildConfig.sessionId;
//
//        UdpInitConfig initConfig2 = new UdpInitConfig();
//        initConfig2.remoteSessionId = BuildConfig.sessionId;
//        initConfig2.relayId = BuildConfig.sessionId;
//        initConfig2.netType = 2;
//        initConfig2.remoteIP = "114.236.138.71";
//        initConfig2.remotePort = 30001;

//        ScreenCaptureParameters parameters = new ScreenCaptureParameters();
//        parameters.captureAudio = true;
//        parameters.captureVideo = false;
//        mRtcEngine.startScreenCapture(parameters);

//        channelConfig.configs.add(initConfig);
//        channelConfig.configs.add(initConfig2);

        channelConfig.appID = BuildConfig.appId;
        channelConfig.userID = BuildConfig.userId;
        channelConfig.channelID = sessionId;
        channelConfig.token = BuildConfig.token;

        mRtcEngine.joinChannel(channelConfig);
        if (mRudpEngine != null) {
            mRudpEngine.joinChannel(BuildConfig.token, 0, true, 0, 1, 1, sessionId);
        }


//        String msg = "{\"p2pSignalServerIp\":\"61.155.136.209\", \"p2pSignalServerPort\":9988, \"liveid\": " + BuildConfig.sessionId  + "}";
//        mRtcEngine.onRecvMessage("UniRelayStartP2P",msg);
    }

    public void stopUpload() {
        if (mRtcEngine != null) {
            mRtcEngine.stopScreenCapture();
            mRtcEngine.leaveChannel();
        }
        if (mRudpEngine != null) {
            mRudpEngine.leaveChannel();
        }
    }


    public void switchCamera() {
        mRtcEngine.switchCamera();
//        if (mRudpEngine != null) {
//            mRudpEngine.sendMessage("testMsg".getBytes());
//        }
    }

    @Override
    public void onDataCallback(long uid, String channelId, byte[] data) {

    }

    @Override
    public void onEventCallback(long uid, String channelId, int type, int length, String msg) {

    }
}

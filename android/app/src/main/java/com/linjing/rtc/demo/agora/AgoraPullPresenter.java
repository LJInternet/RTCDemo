package com.linjing.rtc.demo.agora;

import android.content.Context;
import android.os.Environment;
import android.view.SurfaceView;
import android.widget.FrameLayout;

import com.linjing.decode.api.DecodeConstants;
import com.linjing.rtc.LJRtcEngine;
import com.linjing.rtc.api.ChannelConfig;
import com.linjing.rtc.api.RtcEngineConfig;
import com.linjing.rtc.base.IRtcEngine;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rudp.RUDPCallback;
import com.linjing.rudp.RudpEngineJni;
import com.linjing.sdk.api.DelayData;
import com.linjing.sdk.api.IDecodeObserver;
import com.linjing.sdk.api.RTCEngineConstants;
import com.linjing.sdk.api.audio.AudioFrame;
import com.linjing.sdk.api.audio.AudioParams;
import com.linjing.sdk.api.audio.AudioVolumeInfo;
import com.linjing.sdk.api.audio.IAudioFrameObserver;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.IVideoFrameObserver;
import com.linjing.sdk.api.video.VideoFrame;
import com.linjing.transfer.api.IRtcEngineEventHandler;
import com.linjing.transfer.views.VideoViews;

import java.io.File;
import java.lang.ref.WeakReference;
import java.util.Map;

public class AgoraPullPresenter implements RUDPCallback {

    private static final File OUTPUT_DIR = Environment.getExternalStorageDirectory();

    IRtcEngine mRtcEngine;
    private RudpEngineJni mRudpEngine;

    private IRtcEngineEventHandler mEventHandler = new IRtcEngineEventHandler() {
        @Override
        public void onNetworkQuality(int uid, int localQuality, int remoteQuality) {
            JLog.info("onNetworkQuality", "localQuality " + localQuality + " remoteQuality " + remoteQuality);
        }

        @Override
        public void onAudioVolumeIndication(AudioVolumeInfo info) {

        }
    };

    private WeakReference<AgoraPullActivity> mView;
    public AgoraPullPresenter(AgoraPullActivity view) {
        mView = new WeakReference<>(view);
        try {
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngine.create(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PULL);

            JLog.info("MediaAgoraPullPresenter create");
//            mRtcEngine.enableAudio();//启用音频模块
            mRtcEngine.enableLocalVideo(false);//关闭使用本地摄像头设备
            mRtcEngine.muteLocalVideoStream(true);//取消发布
            mRtcEngine.setDecodeType(DecodeConstants.DecodeType.HARD);
            mRtcEngine.setDecodeLowLatency(false);
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
//                    JLog.info("onPreDecodeVideoFrame " + videoFrame.toString());
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
//                    JLog.info("onRecordFrame " + audioFrame.toString());
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

                @Override
                public AudioParams getRecordAudioParams() {
                    return null;
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

            mRtcEngine.addHandler(mEventHandler);
        } catch (Exception e) {
            e.printStackTrace();
        }

        mRudpEngine = new RudpEngineJni();
        mRudpEngine.create(this);
    }

    public void stopPullStream() {
        if (mRudpEngine != null) {
            mRudpEngine.leaveChannel();
        }
        if (mRtcEngine != null) {
            mRtcEngine.removeHandler(mEventHandler);
            mRtcEngine.leaveChannel();
            mRtcEngine.destroy();
            mRtcEngine = null;
        }
    }

    public void startPullStream(String sessionIdStr, String serviceIpStr, String portIdStr, boolean rudpMode) {
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
//
//        channelConfig.configs.add(initConfig);
//        channelConfig.configs.add(initConfig2);
        channelConfig.appID = BuildConfig.appId;
        channelConfig.userID = 2;
        channelConfig.channelID = sessionIdStr;
        channelConfig.token = BuildConfig.token;
        mRtcEngine.setClientRole(rudpMode ? RTCEngineConstants.ClientRole.CLIENT_ROLE_PULL : RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
        mRtcEngine.joinChannel(channelConfig);
        if (mRudpEngine != null) {
            mRudpEngine.joinChannel(BuildConfig.token, 1, true, 0, 2, 1, sessionIdStr);
        }

//        String msg = "{\"p2pSignalServerIp\":\"61.155.136.209\", \"p2pSignalServerPort\":9988, \"liveid\": " + BuildConfig.sessionId  + "}";
//        mRtcEngine.onRecvMessage("UniRelayStartP2P",msg);
    }

    public void setupRemoteUi(Context context, FrameLayout group) {
        JLog.info("MediaPlayer", " setupRemoteUi ");
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupRemoteVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        surfaceView.setKeepScreenOn(true);
        group.addView(surfaceView);
    }


    public void destroy() {
        if (mRtcEngine != null) {
            mRtcEngine.removeHandler(mEventHandler);
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

    public void pushAudio(boolean startedPush) {
        if (startedPush) {
            mRtcEngine.enableAudio();
//            ScreenCaptureParameters parameters = new ScreenCaptureParameters();
//            parameters.captureAudio = true;
//            parameters.captureVideo = false;
//            mRtcEngine.startScreenCapture(parameters);
//            mRtcEngine.enableAudioProcess(false);
//            mRtcEngine.enableAudioVolumeIndication(300, 1, false);
        } else {
//            mRtcEngine.stopScreenCapture();
            mRtcEngine.disableAudio();

        }
    }

    @Override
    public void onDataCallback(long uid, String channelId, byte[] data) {
        JLog.info("onDataCallback uid: " + uid + " msg " + new String(data));
    }

    @Override
    public void onEventCallback(long uid, String channelId, int type, int result, String msg) {
        JLog.info("onEventCallback uid: " + uid + " msg " + msg + " result " + result);
    }
}

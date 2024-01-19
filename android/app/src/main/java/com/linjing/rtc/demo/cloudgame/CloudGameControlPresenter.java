package com.linjing.rtc.demo.cloudgame;

import android.content.Context;
import android.view.MotionEvent;
import android.view.Surface;
import android.view.SurfaceView;
import android.view.View;
import android.widget.FrameLayout;

import com.google.gson.Gson;
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
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.IVideoFrameObserver;
import com.linjing.sdk.api.video.VideoFrame;
import com.linjing.transfer.views.ISurfaceObserver;
import com.linjing.transfer.views.RenderSurfaceView;
import com.linjing.transfer.views.VideoViews;

import java.lang.ref.WeakReference;
import java.nio.charset.StandardCharsets;
import java.util.Map;

public class CloudGameControlPresenter implements View.OnTouchListener, RUDPCallback, ToutchEventHelper.SendControllerMsg {
    private static final String TAG = "CGControl";
    private IRtcEngine mRtcEngine;
    private RudpEngineJni mRudpEngine;
    private long currentUid = System.currentTimeMillis();
    private WeakReference<CloudGameControlActivity> mView;
    private Gson mGson = new Gson();
    private ToutchEventHelper mEventHelper = new ToutchEventHelper();
    public CloudGameControlPresenter(CloudGameControlActivity view) {
        mView = new WeakReference<>(view);
        try {
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngine.create(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PULL);
            JLog.info(TAG, "CloudGameControlPresenter create");
            mRtcEngine.enableLocalVideo(false);//关闭使用本地摄像头设备
            mRtcEngine.muteLocalVideoStream(true);//取消发布
            mRtcEngine.registerVideoFrameObserver(new IVideoFrameObserver() {
                @Override
                public boolean onCaptureVideoFrame(VideoFrame videoFrame) {
                    return false;
                }

                @Override
                public boolean onRenderVideoFrame(int uid, VideoFrame videoFrame) {
                    mEventHelper.setFrameWH(videoFrame.width, videoFrame.height);
                    return false;
                }

                @Override
                public int getObservedFramePosition() {
                    return POSITION_PRE_RENDERER;
                }
            });
            mRtcEngine.registerDecodeObserver(new IDecodeObserver() {
                @Override
                public void onDelayStatisticData(int frameId, byte[] extraData, Map<Long, Long> delayMap) {

                }

                @Override
                public void onDelayStaticsEvent(DelayData delayData) {
                }
            });
        } catch (Exception e) {
            e.printStackTrace();
        }
        createRTMEngine();
    }

    public void stopPullStream() {
        if (mRtcEngine != null) {
            mRtcEngine.registerVideoFrameObserver(null);
            mRtcEngine.leaveChannel();
            mRtcEngine.destroy();
            mRtcEngine = null;
        }
        doLeaveChannel();
        destroyRTMEngine();
    }

    public void startPullStream(int channelId) {
        LJRtcEngine.setDebugEnv(true);
        ChannelConfig channelConfig = new ChannelConfig();

        channelConfig.appID = BuildConfig.appId;
        channelConfig.userID = currentUid;
        channelConfig.channelID = channelId + "";
        channelConfig.token = BuildConfig.token;
        mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
        mRtcEngine.joinChannel(channelConfig);
        doJoinChannel(channelId);
    }

    public void setupRemoteUi(Context context, FrameLayout group) {
        JLog.info(TAG, " setupRemoteUi ");
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        VideoViews views = new VideoViews(surfaceView);
        mRtcEngine.setupRemoteVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        surfaceView.setKeepScreenOn(true);
        group.addView(surfaceView);
        surfaceView.setOnTouchListener(this);
        RenderSurfaceView renderSurfaceView = (RenderSurfaceView)surfaceView;
        renderSurfaceView.addSurfaceObserver(new ISurfaceObserver() {
            @Override
            public void surfaceCreated(Surface holder) {

            }

            @Override
            public void surfaceChanged(Surface holder, int width, int height) {
                mEventHelper.setSurfaceWH(renderSurfaceView.getSurfaceWidth(), renderSurfaceView.getSurfaceHeight());
            }

            @Override
            public void surfaceDestroyed(Surface holder) {

            }
        });
    }

    public void destroy() {
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
            mRtcEngine.destroy();
            mRtcEngine = null;
        }
        destroyRTMEngine();
    }

    @Override
    public void sendMessage(ControlMessage event) {
        String dataStr = mGson.toJson(event, ControlMessage.class);

        if (mRudpEngine != null) {
            mRudpEngine.sendMessage(dataStr.getBytes(StandardCharsets.UTF_8));
        }
    }

    private void createRTMEngine() {
        mRudpEngine = new RudpEngineJni();
        mRudpEngine.create(this);
    }

    private void destroyRTMEngine() {
        if (mRudpEngine != null) {
            mRudpEngine.destroy();
            mRudpEngine = null;
        }
    }

    private void doLeaveChannel() {
        if (mRudpEngine != null) {
            mRudpEngine.leaveChannel();
        }
    }

    private void doJoinChannel(int channelId) {
        if (mRudpEngine != null) {
            mRudpEngine.joinChannel(BuildConfig.token, 0, true, 0, currentUid, 1, channelId+ "");
        }
    }

    @Override
    public void onDataCallback(long uid, String channelId, byte[] data) {

    }

    @Override
    public void onEventCallback(long uid, String channelId, int type, int result, String msg) {
        JLog.info(TAG, "uid  = " + uid + ", channelId = " + channelId + ", msg " + msg +  ", result "  + result+  ", type "  +  type);
    }


    @Override
    public boolean onTouch(View v, MotionEvent event) {
        mEventHelper.onTouch(v, event, this);
        return true;
    }
}

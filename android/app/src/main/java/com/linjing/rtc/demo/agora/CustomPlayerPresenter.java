package com.linjing.rtc.demo.agora;

import android.annotation.SuppressLint;
import android.content.Context;
import android.os.Looper;
import android.view.MotionEvent;
import android.view.View;
import android.widget.FrameLayout;

import com.linjing.capture.api.CaptureError;
import com.linjing.capture.api.LiveMode;
import com.linjing.capture.api.camera.CameraParam;
import com.linjing.decode.api.data.LJVideoFrame;
import com.linjing.decode.api.data.VideoDecodedFrame;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.api.ChannelConfig;
import com.linjing.rtc.api.RtcEngineConfig;
import com.linjing.rtc.base.IRtcEngine;
import com.linjing.rtc.demo.UserInfo;
import com.linjing.rtc.demo.camera.bean.ResolutionParam;
import com.linjing.rtc.demo.camera.helper.MediaConfigHelper;
import com.linjing.rtc.demo.camera.helper.ResolutionOptions;
import com.linjing.rtc.player.custom.CustomPlayMode;
import com.linjing.rtc.player.custom.CustomRemotePlayer;
import com.linjing.rtc.player.custom.ICustomPlayerCallback;
import com.linjing.sdk.api.DelayData;
import com.linjing.sdk.api.IDecodeObserver;
import com.linjing.sdk.api.RTCEngineConstants;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.VideoEncodedFrame;
import com.linjing.sdk.api.video.VideoEncoderConfiguration;
import com.linjing.sdk.encode.api.video.IVideoEncoder;
import com.linjing.sdk.encode.api.video.core.IEncodeCore;
import com.linjing.sdk.wrapper.camera.CameraMediaClient;
import com.linjing.sdk.wrapper.video.VideoConfig;
import com.linjing.sdk.wrapper.video.VideoStream;
import com.linjing.sdk.wrapper.video.api.DrawFrameListener;
import com.linjing.sdk.wrapper.video.api.VideoProvider;
import com.linjing.sdk.wrapper.video.api.VideoRenderCallback;

import java.lang.ref.WeakReference;
import java.util.Map;

public class CustomPlayerPresenter implements VideoRenderCallback, ICustomPlayerCallback {

    private WeakReference<CustomPlayerActivity> mView;

    IRtcEngine mRtcEngine;

    private CameraMediaClient mVideoStream;
    private CustomRemotePlayer mPlayer;

    public CustomPlayerPresenter(CustomPlayerActivity view) {
        mView = new WeakReference<CustomPlayerActivity>(view);
        try {
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngine.create(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);
            mRtcEngine.setAudioProfile(RTCEngineConstants.AudioProfile.AUDIO_PROFILE_MUSIC_STANDARD_STEREO, 0);
            VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration(544, 960,
                    30, 1200, VideoEncoderConfiguration.OrientationMode.ORIENTATION_MODE_FIXED_PORTRAIT);
            mRtcEngine.setVideoEncoderConfiguration(encoderConfiguration);
            mRtcEngine.enableAudio();
            mRtcEngine.enableLocalVideo(false);//关闭使用本地摄像头设备
            mRtcEngine.muteLocalVideoStream(true);//取消发布

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

        initVideoStream();

        mPlayer = new CustomRemotePlayer(720, 1280, isLand);
        mPlayer.updateRenderMode(CustomPlayMode.FULL_MIN_MODE,  isLand, 720, 1280);
        mPlayer.setCustomCallback(this);
    }

    private VideoConfig getVideoConfig(int captureType) {
        ResolutionParam resolutionParam = ResolutionOptions.screenMid.clone();
        resolutionParam.setLand(false);
//        if (captureType == 1) {
//            FileUtil.copyAssetsTosSd(VideoFileCapture.DefaultPath, VideoFileCapture.DefaultName);
//            VideoConfig config = MediaConfigHelper.createVideoConfig(LiveMode.VIDEO_FILE, resolutionParam);
//            config.videoFilePath = VideoFileCapture.DefaultPath + VideoFileCapture.DefaultName;
//            if (!FileUtil.isExists(config.videoFilePath)) {
//                JLog.error("file not found!!!");
//            }
//            return config;
//        }
        return MediaConfigHelper.createVideoConfig(LiveMode.CAMERA, resolutionParam);
    }

    private void initVideoStream() {
        mVideoStream = new CameraMediaClient();
        ResolutionParam resolutionParam = ResolutionOptions.screenMid.clone();
        resolutionParam.setLand(false);
        VideoConfig videoConfig = getVideoConfig(0);
//        videoConfig.showBitmap = BitmapFactory.decodeResource(LJSDK.instance().getAppContext().getResources(),
//                R.drawable.ic_test);
        videoConfig.needYFLip = true;
        videoConfig.yuvOnly = false;
        videoConfig.listener = this;
        mVideoStream.startVideoStream(videoConfig);
        mVideoStream.setDrawFrameListener(new DrawFrameListener() {
            @Override
            public int onPreviewDrawFrame(int textureId, int width, int height, long timestampNs) {
                return 0;
            }

            @Override
            public int onPreviewDrawFrame(byte[] nv21, int width, int height, int rotation, long timestampNs) {
                return 0;
            }

            private LJVideoFrame mVideoDecodeData;
            @Override
            public int onPreviewDrawFrame(int textureId, byte[] nv21, int width, int height, int rotation, long timestampNs) {
                if (mPlayer != null) {
                    mPlayer.onLocalTexture(textureId, width, height, false);
                    if (mVideoDecodeData == null) {
                        mVideoDecodeData = new LJVideoFrame();
                        mVideoDecodeData.mVideoDecodedFrame = new VideoDecodedFrame();
                    }
                    mVideoDecodeData.mData = nv21;
                    mVideoDecodeData.mVideoDecodedFrame.width = width;
                    mVideoDecodeData.mVideoDecodedFrame.height = height;
                    mVideoDecodeData.mVideoDecodedFrame.len = nv21.length;
                    mVideoDecodeData.mVideoDecodedFrame.widthY = width;
                    mVideoDecodeData.mVideoDecodedFrame.widthUV = width / 2;
                    mVideoDecodeData.mVideoDecodedFrame.heightY = height;
                    mVideoDecodeData.mVideoDecodedFrame.heightUV = height;
                    mVideoDecodeData.mVideoDecodedFrame.offsetY = 0;
                    mVideoDecodeData.mVideoDecodedFrame.offsetU = width * height;
                    mVideoDecodeData.mVideoDecodedFrame.offsetV = width * height + width * height / 4;
                    mPlayer.onDecodeVideoFrame(mVideoDecodeData);
                }
                return 0;
            }

            @Override
            public int getFrameFormat() {
                return FrameFormat.YUV_420;
            }

            @Override
            public boolean isTextureAndData() {
                return true;
            }
        });
    }

    public void startVoip() {
        ChannelConfig channelConfig = new ChannelConfig();
        channelConfig.appID = BuildConfig.appId;
        channelConfig.userID = UserInfo.userId;
        channelConfig.channelID = BuildConfig.channelId+ "";
        channelConfig.token = BuildConfig.token;
        mRtcEngine.joinChannel(channelConfig);

        String msg = "{\"p2pSignalServerIp\":\"61.155.136.209\", \"p2pSignalServerPort\":9988, \"liveid\": " + BuildConfig.channelId  + "}";
        mRtcEngine.onRecvMessage("UniRelayStartP2P",msg);
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

    private Context mContext;
    private FrameLayout mGroup;
    @SuppressLint("ClickableViewAccessibility")
    public void setupRemoteUi(Context context, FrameLayout group) {
        mContext = context;
        mGroup = group;
        if (mPlayer != null) {
            mPlayer.setupRemoteView(context, group);
        }
        if (mGroup != null) {
            mGroup.setOnTouchListener(new View.OnTouchListener() {
                @Override
                public boolean onTouch(View v, MotionEvent event) {
                    if (mPlayer != null) {
                        mPlayer.onTouchEvent(event);
                    }
                    return true;
                }
            });
        }
    }

    public void stop() {
        if (mVideoStream != null) {
            mVideoStream.startVideoStream(null);
            mVideoStream = null;
        }
        if (mPlayer != null) {
            mPlayer.stop();
            mPlayer = null;

        }
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
            mRtcEngine.registerDecodeObserver(null);
            mRtcEngine.destroy();
            mRtcEngine = null;
        }
    }

    @Override
    public void onEGLContextResult(long eglContextNativeHandle) {
        if (mPlayer != null) {
            mPlayer.start(mVideoStream.videoStream().getEglCore().getContext());
        }
    }

    @Override
    public void onPreviewHasStop() {

    }

    @Override
    public void onCameraStart(CameraParam param) {

    }

    @Override
    public void onCaptureError(CaptureError captureError) {

    }

    @Override
    public void onVideoActionCallback(int result, int actionType, String msg) {

    }

    boolean isLand = false;
    public void updateRenderMode(Context context, int mode) {
        if (mGroup == null) {
            return;
        }

        if (isLand) {
            mPlayer.updateRenderMode(mode, isLand, 1280, 720);
            return;
        }
        if (mode == CustomPlayMode.HALF_RIGHT_MODE || mode == CustomPlayMode.HALF_LEFT_MODE) {
            mPlayer.updateRenderMode(mode, isLand, 720, 640);
        } else {
            mPlayer.updateRenderMode(mode, isLand, 720, 1280);
        }
    }

    public void updateOrientation(boolean isLandscape) {
        isLand = isLandscape;
        updateRenderMode(mContext, CustomPlayMode.FULL_MODE);
    }

    public void enableEncode(boolean enable) {
        if (enable) {
            mPlayer.startEncode(2000);
        } else {
            mPlayer.stopEncode();
        }
    }

    @Override
    public void onDrawFrameCallback(int textureId, int width, int height) {

    }

    @Override
    public void onEncodeVideoData(VideoEncodedFrame encodedFrame) {

    }
}

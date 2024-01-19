package com.linjing.rtc.demo.camera.push;

import android.content.Context;
import android.graphics.BitmapFactory;
import android.os.Environment;
import android.util.Log;
import android.view.Surface;
import android.view.SurfaceView;
import android.widget.FrameLayout;

import com.linjing.capture.api.LiveMode;
import com.linjing.capture.videocapture.VideoFileCapture;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.UserInfo;
import com.linjing.rtc.demo.camera.bean.ResolutionParam;
import com.linjing.rtc.demo.camera.helper.MediaConfigHelper;
import com.linjing.rtc.demo.camera.helper.ResolutionOptions;
import com.linjing.rtc.demo.utils.SPHelper;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.encode.api.video.VideoEncodeConfig;
import com.linjing.sdk.utils.FileUtil;
import com.linjing.sdk.wrapper.audio.AudioConfig;
import com.linjing.sdk.wrapper.camera.CameraCEPWrapper;
import com.linjing.sdk.wrapper.video.VideoConfig;
import com.linjing.transfer.LJMediaEngine;
import com.linjing.transfer.api.TransferMode;
import com.linjing.transfer.upload.api.TransferConfig;
import com.linjing.transfer.upload.api.UdpInitConfig;
import com.linjing.transfer.upload.api.VideoCaptureConfig;
import com.linjing.transfer.views.RenderSurfaceView;
import com.linjing.transfer.views.VideoViews;

import java.io.FileOutputStream;
import java.io.IOException;

public class NativeCameraPushPresenter {

//    private CameraMediaClient mCameraMediaClient;
    private LJMediaEngine mLJMediaEngine;
    private int mCaptureType = 0;

    public NativeCameraPushPresenter() {
//        mCameraMediaClient = new CameraMediaClient();
        mLJMediaEngine = new LJMediaEngine();
        mCaptureType = SPHelper.getIntSp(LJSDK.instance().getAppContext(), "captureType");
    }

    public void startStream() {
        VideoConfig videoConfig = getVideoConfig(0);
        videoConfig.showBitmap = BitmapFactory.decodeResource(LJSDK.instance().getAppContext().getResources(),
                R.drawable.ic_test);
        videoConfig.needYFLip = false;
        videoConfig.yuvOnly = true;
//        mCameraMediaClient.startVideoStream(videoConfig);
    }

    private VideoConfig getVideoConfig(int captureType) {
        ResolutionParam resolutionParam = ResolutionOptions.screenLow.clone();
        resolutionParam.setLand(true);
        if (captureType == 1) {
            FileUtil.copyAssetsTosSd(VideoFileCapture.DefaultPath, VideoFileCapture.DefaultName);
            VideoConfig config = MediaConfigHelper.createVideoConfig(LiveMode.VIDEO_FILE, resolutionParam);
            config.videoFilePath = VideoFileCapture.DefaultPath + VideoFileCapture.DefaultName;
            if (!FileUtil.isExists(config.videoFilePath)) {
                JLog.error("file not found!!!");
            }
            return config;
        }
        return MediaConfigHelper.createVideoConfig(LiveMode.CAMERA, resolutionParam);
    }

    public void stop() {
        if (mLJMediaEngine != null) {
            mLJMediaEngine.stop();
            mLJMediaEngine = null;
        }

    }

    public void startUpload() {
        if (mLJMediaEngine == null) {
            return;
        }
        TransferConfig transferConfig = new TransferConfig();

        transferConfig.appID = BuildConfig.appId;
        transferConfig.userID = UserInfo.userId;
        transferConfig.channelID = BuildConfig.channelId+ "";
        transferConfig.token = BuildConfig.token;
        transferConfig.transferMode = TransferMode.PUSH_MODE;

        ResolutionParam resolutionParam1 = ResolutionOptions.SuperParam.clone();
        VideoEncodeConfig encodeConfig = MediaConfigHelper.createEncodeConfig(
                resolutionParam1.getVideoBitrate() / 1000,
                resolutionParam1.getMaxVideoBitrate() / 1000,
                resolutionParam1.getMinVideoBitrate() / 1000);
        encodeConfig.encodeWidth = resolutionParam1.videoWidth();
        encodeConfig.encodeHeight = resolutionParam1.videoHeight();
//        encodeConfig.mEglCore = mCameraMediaClient.videoStream().getEglCore();

//        mLJMediaEngine.startVideoPublish(encodeConfig, new H264AnnexBMux(), new AsyncHardVideoEncoder2());

        AudioConfig audioConfig = MediaConfigHelper.createAudioConfig();
        mLJMediaEngine.startAudio();

        mLJMediaEngine.start(transferConfig);
    }

    public void startVideoCapture(){
        VideoCaptureConfig cfg = new VideoCaptureConfig();
        cfg.deviceCode = "";
        cfg.width = 480;
        cfg.height = 640;
        cfg.fps = 30;
        mLJMediaEngine.startVideoCapture(cfg);
    }

    public void setupNativeLocalVideo(Context context, FrameLayout group) {
        JLog.info("MediaPlayer", " setupLocalUi ");
        SurfaceView surfaceView = new RenderSurfaceView(context);
        VideoViews views = new VideoViews(surfaceView);
        mLJMediaEngine.setupNativeLocalVideo(views);
        surfaceView.setZOrderMediaOverlay(true);
        surfaceView.setKeepScreenOn(true);
        group.addView(surfaceView);
    }

    public void stopUpload() {
        if (mLJMediaEngine == null) {
            return;
        }
        mLJMediaEngine.stopVideoPublish();
        mLJMediaEngine.stopAudio();
        mLJMediaEngine.stop();
    }

    public void onOrientationChanged(int orientation) {
//        mCameraMediaClient.videoStream().onConfigurationChanged(orientation);
    }

    public void surfaceCreated(Surface surface) {
//        mCameraMediaClient.startVideoPreview(surface);
//        startUpload();
//        startVideoCapture();
    }

    public void surfaceChanged(Surface surface, int width, int height) {
//        mCameraMediaClient.updateVideoPreviewSize(width, height);
    }

    public void surfaceDestroyed(Surface surface) {
//        mCameraMediaClient.stopVideoPreview();
//        mCameraMediaClient.stopVideoCapture();
    }

    public void setCameraStateListener(CameraCEPWrapper.CameraStateListener listener) {
        Log.d("soft_codec", "setCameraStateListener: ");

    }

    public void changeScreenOrientation() {
        ResolutionParam resolutionParam = ResolutionOptions.HighParam.clone();
        resolutionParam.setLand(false);
        VideoConfig config = MediaConfigHelper.createVideoConfig(LiveMode.CAMERA, resolutionParam);
        config.showBitmap = BitmapFactory.decodeResource(LJSDK.instance().getAppContext().getResources(), R.drawable.ic_test);
//        mCameraMediaClient.restartStream(config);
    }

    public void startEncode() {
//        mCameraMediaClient.switchLiveModeWithConfig(LiveMode.VIDEO_FILE, getVideoConfig(1));
    }

    public void stopEncode() {
//        mCameraMediaClient.switchLiveModeWithConfig(LiveMode.CAMERA, getVideoConfig(0));
    }

    public void switchCamera() {
//        mCameraMediaClient.switchCamera();
    }

    public void changeCaptureType(int newType) {
        mCaptureType = newType;
    }

    private FileOutputStream mFos = null;
    private void testSetYUVData(byte[] data, int dataLen) {
        try {
            if (mFos == null) {
                String TEST_FILE = Environment.getExternalStorageDirectory().getPath() + "/link-data3434.yuv";
                mFos = new FileOutputStream(TEST_FILE);
            }
            mFos.write(data, 0, dataLen);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}

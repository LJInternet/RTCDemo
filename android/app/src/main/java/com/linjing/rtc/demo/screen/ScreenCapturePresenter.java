package com.linjing.rtc.demo.screen;

import android.content.Context;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Display;
import android.view.WindowManager;

import com.linjing.capture.api.LiveMode;
import com.linjing.rtc.demo.camera.bean.ResolutionParam;
import com.linjing.rtc.demo.camera.helper.MediaConfigHelper;
import com.linjing.rtc.demo.camera.helper.ResolutionOptions;
import com.linjing.sdk.wrapper.screen.BaseProjectionClient;
import com.linjing.sdk.wrapper.video.VideoConfig;
import com.linjing.transfer.upload.api.TransferConfig;
import com.linjing.transfer.upload.api.UdpInitConfig;
import com.linjing.transfer.upload.api.UploadConfig;

import java.lang.ref.WeakReference;
import java.util.Random;

public class ScreenCapturePresenter {

    private BaseProjectionClient mMediaProjectionClient;

    private WeakReference<IScreenCaptureView> mView;

    public ScreenCapturePresenter(IScreenCaptureView captureView) {
        mView = new WeakReference<>(captureView);
        mMediaProjectionClient = new BaseProjectionClient();
    }


    Random mRandom = new Random();
    public void startScreenCapture(Context context) {
        if (mMediaProjectionClient != null) {
            ResolutionParam resolutionParam = ResolutionOptions.screenMid.clone();
            resolutionParam.setLand(false);
            DisplayMetrics metrics = new DisplayMetrics();
            WindowManager windowManager = (WindowManager) context.getSystemService(Context.WINDOW_SERVICE);
            Display display = windowManager.getDefaultDisplay();
            display.getRealMetrics(metrics);

            int width = resolutionParam.videoWidth();
            float rate = 1f *  width / metrics.widthPixels;
            int height = (int) (metrics.heightPixels * rate);
            resolutionParam.setVideoWidth(width);
            resolutionParam.setVideoHeight(height);
            Log.d("ScreenCapturePresenter", "getWindowManager: width=" + width + ",width=" + height);
            VideoConfig videoConfig = MediaConfigHelper.createVideoConfig(LiveMode.SCREEN_CAPTURE, resolutionParam);
            mMediaProjectionClient.startStream(videoConfig, MediaConfigHelper.createAudioConfig());
            // 默认就行
            UploadConfig uploadConfig = new UploadConfig();
            TransferConfig transferConfig = new TransferConfig();

            UdpInitConfig initConfig = new UdpInitConfig();
            initConfig.remoteSessionId = mRandom.nextInt(100000);
            transferConfig.configs.add(initConfig);
            WeakReference<IScreenCaptureView> weakView = ScreenCaptureMgr.getInstance().getView();
            if (weakView != null && weakView.get() != null) {
                weakView.get().setSessionId(initConfig.remoteSessionId);
            }
            ScreenCaptureMgr.getInstance().setSessionId(initConfig.remoteSessionId);
            uploadConfig.transferConfig = transferConfig;
            mMediaProjectionClient.startPushMedia(uploadConfig);
        }
    }

    public void stopScreenCapture(Context context) {
        if (mMediaProjectionClient != null) {
            mMediaProjectionClient.stopPushMedia();
            mMediaProjectionClient.stopVideoStream();
        }
    }

    public void destroy() {
        if (mMediaProjectionClient != null) {
            mMediaProjectionClient.stopPushMedia();
            mMediaProjectionClient.stopVideoStream();
            mMediaProjectionClient.release();
            mMediaProjectionClient = null;
        }
    }
}

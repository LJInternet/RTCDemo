package com.linjing.rtc.demo.screen;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;

import androidx.annotation.Nullable;

public class ScreenCaptureService extends Service implements IScreenCaptureView {

    public static final String TAG = "ScreenCapture";

    private ScreenCapturePresenter mScreenCapturePresenter;

    @Override
    public void onCreate() {
        super.onCreate();
        mScreenCapturePresenter = new ScreenCapturePresenter(this);
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (mScreenCapturePresenter != null) {
            StopScreenCapture();
            mScreenCapturePresenter.destroy();
            mScreenCapturePresenter = null;
        }
    }

    @Override
    public void setSessionId(int remoteSessionId) {

    }

    @Override
    public boolean onUnbind(Intent intent) {
        return super.onUnbind(intent);
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        return super.onStartCommand(intent, flags, startId);
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        NotificationHelper.startForegroundNotification(ScreenCaptureService.this, ScreenCaptureService.this, 1001);
        startScreenCapture();
        return new ScreenCaptureBinder(ScreenCaptureService.this);
    }

    public void startScreenCapture() {
        if (mScreenCapturePresenter != null) {
            mScreenCapturePresenter.startScreenCapture(this);
        }
    }

    public void StopScreenCapture() {
        if (mScreenCapturePresenter != null) {
            mScreenCapturePresenter.stopScreenCapture(this);
        }
    }
}

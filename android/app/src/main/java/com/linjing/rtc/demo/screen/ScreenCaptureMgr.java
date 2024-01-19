package com.linjing.rtc.demo.screen;

import android.content.Context;
import android.content.Intent;

import java.lang.ref.WeakReference;

public class ScreenCaptureMgr {

    private CaptureServiceConnection mServiceConnection;
    private boolean isBindService;

    private Intent mPrjIntent;

    public int mSessionId = 0;

    public void setSessionId(int remoteSessionId) {
        mSessionId = remoteSessionId;
    }

    private static class ScreenCaptureHolder {
        static ScreenCaptureMgr sInstance = new ScreenCaptureMgr();
    }

    public static ScreenCaptureMgr getInstance() {
        return ScreenCaptureHolder.sInstance;
    }

    private ScreenCaptureMgr() {

    }

    public void stopScreenCapture(Context context) {
        mSessionId = 0;
        if (!isBindService || mServiceConnection == null) {
            return;
        }
        try {
            context.unbindService(mServiceConnection);
        } catch (Exception  e) {
            e.printStackTrace();
        }
        isBindService = false;
        mServiceConnection = null;
    }

    public CaptureServiceConnection getServiceConnection() {
        return mServiceConnection;
    }

    public void startScreenCapture(Context context) {
        if (isBindService || mServiceConnection != null) {
            return;
        }
        mServiceConnection = new CaptureServiceConnection();
        isBindService = context.bindService(new Intent(context, ScreenCaptureService.class),
                mServiceConnection,  Context.BIND_AUTO_CREATE);
        return;
    }

    public void cachePrjIntent(Intent intent) {
        mPrjIntent = intent;
    }

    public boolean hasCachePrjIntent() {
        return mPrjIntent != null;
    }

    public Intent getPrjIntent() {
        return mPrjIntent;
    }

    /**
     * 这里随便写的,应该用viewModule的
     */
    private WeakReference<IScreenCaptureView> mView;
    public void setViewListener(IScreenCaptureView view) {
        if (view == null) {
            mView.clear();
            mView = null;
        } else {
            mView = new WeakReference<>(view);
        }
    }

    public WeakReference<IScreenCaptureView> getView() {
        return mView;
    }

}

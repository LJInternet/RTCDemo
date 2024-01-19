package com.linjing.rtc.demo.screen;

import android.content.ComponentName;
import android.content.ServiceConnection;
import android.os.IBinder;

public class CaptureServiceConnection implements ServiceConnection {

    private ScreenCaptureBinder binder;

    @Override
    public void onServiceConnected(ComponentName name, IBinder service) {
        binder = (ScreenCaptureBinder) service;
    }

    @Override
    public void onServiceDisconnected(ComponentName name) {
        binder = null;
    }

    public ScreenCaptureBinder getBinder() {
        return binder;
    }
}

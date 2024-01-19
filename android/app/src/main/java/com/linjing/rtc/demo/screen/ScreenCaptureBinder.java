package com.linjing.rtc.demo.screen;

import android.os.Binder;

public class ScreenCaptureBinder extends Binder {

    private ScreenCaptureService mScreenCaptureService;


    public ScreenCaptureBinder(ScreenCaptureService service) {
        mScreenCaptureService = service;
    }

    public ScreenCaptureService getService() {
        return mScreenCaptureService;
    }
}

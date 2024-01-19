package com.linjing.rtc.demo.camera.helper;


import com.linjing.capture.api.LiveMode;
import com.linjing.rtc.demo.camera.bean.ResolutionParam;

public class LiveConfigHelper {

    private static final class InstanceHolder {
        public static final LiveConfigHelper sInstance = new LiveConfigHelper();
    }

    public static LiveConfigHelper getInstance() {
        return InstanceHolder.sInstance;
    }

    public int liveMode = LiveMode.CAMERA;

    public boolean mIsLandscape;

    public ResolutionParam mResolutionParam = ResolutionOptions.HighParam;


    public int getVideoWidth() {
        return mResolutionParam.videoWidth();
    }

    public int getVideoHeight() {
        return mResolutionParam.videoHeight();
    }
}

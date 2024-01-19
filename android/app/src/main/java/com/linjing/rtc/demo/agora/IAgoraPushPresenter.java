package com.linjing.rtc.demo.agora;

import android.app.Activity;
import android.content.Context;
import android.widget.FrameLayout;

public abstract class IAgoraPushPresenter {

    public abstract void stop();

    public abstract void setupLocalVideo(Context context, FrameLayout group);

    public abstract void startUpload(String sessionId);

    public abstract void stopUpload();

    public abstract void switchCamera();

    public void startDubbing(Activity activity, boolean isDubbing) {

    }
}

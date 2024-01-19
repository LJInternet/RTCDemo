package com.linjing.rtc.demo.camera;

import android.view.MotionEvent;

import com.linjing.sdk.api.log.JLog;


/**
 * author:guoyiqu
 * date:2020/10/16
 **/
public class ZoomHandler {
    private static final String TAG = "ZoomHandler";

    /**
     * 两个手指间的距离
     */
    private float mDist;
    private Callback mCallback;
    private float mCurZoom = 1f;
    private float mZoomFactor = 0.01f;
    private float[] mZoomRange;

    public interface Callback {
        void onSetZoom(float zoom);
    }

    public boolean onTouchEvent(MotionEvent event) {
        if (mCallback == null) {
            return false;
        }
        int action = event.getAction();

        if (event.getPointerCount() > 1) {
            if (action == MotionEvent.ACTION_POINTER_DOWN) {
                //获取两个手指初始距离
                mDist = spacing(event);
            } else if (action == MotionEvent.ACTION_MOVE) {
                handleZoom(event);
            }
            return true;
        } else {
            return false;
        }
    }

    private void handleZoom(MotionEvent event) {
        //动态调节变焦敏感度
        if (mCurZoom > 1.5f) {
            mZoomFactor = 0.1f;
        } else {
            mZoomFactor = 0.02f;
        }

        float newDist = spacing(event);
        if (newDist < mDist && mCurZoom - mZoomFactor > mZoomRange[0]) {
            mCurZoom -= mZoomFactor;
        } else if (newDist > mDist) {
            if (mCurZoom + mZoomFactor > mZoomRange[1]) {
                mCurZoom = mZoomRange[1];
            } else {
                mCurZoom += mZoomFactor;
            }
        }

        mDist = newDist;
        JLog.debug(TAG, "zoom:" + mCurZoom);
        mCallback.onSetZoom(mCurZoom);
    }

    /**
     * 计算两个手指的距离
     */
    private float spacing(MotionEvent event) {
        float x = event.getX(0) - event.getX(1);
        float y = event.getY(0) - event.getY(1);
        return (float) Math.sqrt(x * x + y * y);
    }

    public void setZoomRange(float[] range) {
        mZoomRange = range;
    }

    public void setCallback(Callback callback) {
        this.mCallback = callback;
    }
}

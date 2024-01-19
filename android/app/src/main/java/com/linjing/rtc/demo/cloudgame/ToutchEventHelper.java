package com.linjing.rtc.demo.cloudgame;

import android.graphics.Rect;
import android.view.MotionEvent;
import android.view.View;

import androidx.annotation.NonNull;

import com.linjing.sdk.api.log.JLog;

public class ToutchEventHelper {
    private static final String TAG = "GameTouch";
    private ControlMessage mTouchMessage = new ControlMessage();
    private Rect mDrawRect = null;
    private int mFixMode  = -1;
    private double mWHRatio = 1;
    private static final int FIT_X = 0;
    private static final int FIT_Y = 1;
    private int mSurfaceWidth = 0;
    private int mSurfaceHeight = 0;
    private int mFrameWidth = 0;
    private int mFrameHeight = 0;

    public interface SendControllerMsg {
        void sendMessage(ControlMessage msg);
    }

    public ToutchEventHelper() {
        mTouchMessage.type = ControlMessage.TYPE_INJECT_TOUCH_EVENT;
    }

    private void calculateDrawRect() {
        if (mSurfaceWidth == 0 || mSurfaceHeight == 0 || mFrameWidth == 0 || mFrameHeight == 0) {
            JLog.warn(TAG, "calculateDrawRect w h is 0");
            return;
        }
        mDrawRect = aspectFitRectRatio(mFrameWidth, mFrameHeight, mSurfaceWidth, mSurfaceHeight);
    }

    public void setSurfaceWH(int w, int h) {
        if (mSurfaceWidth != w || mSurfaceHeight == h) {
            mSurfaceWidth = w;
            mSurfaceHeight = h;
            calculateDrawRect();
        }
    }

    public void setFrameWH(int w, int h) {
        if (mFrameWidth != w || mFrameHeight != h) {
            mFrameWidth = w;
            mFrameHeight = h;
            calculateDrawRect();
        }
    }

    private Position createEventXY(float x, float y) {
//        if (x < mDrawRect.left || x > mDrawRect.left + mDrawRect.right) {
//            return null;
//        }
//        if (y < mDrawRect.top || y > mDrawRect.top + mDrawRect.bottom) {
//            return null;
//        }
        x = x - mDrawRect.left;
        y = y - mDrawRect.top;
        if (mFixMode == FIT_Y) {
            //高铺满View，左右流黑边
            x = (float) (x / mWHRatio);
            y = (float) (y / mWHRatio);

        } else {
            x = (float) (x / mWHRatio);
            y = (float) (y / mWHRatio);
        }
        return new Position((int)x, (int)y, mFrameWidth, mFrameHeight);
    }

    public Rect aspectFitRectRatio(int imageWidth, int imageHeight, int viewWidth, int viewHeight) {
        double f; //fit layer percentage
        float width = 0;
        float height = 0;
        float x = 0;
        float y = 0;
        if(viewHeight * imageWidth < viewWidth * imageHeight) {
            mFixMode = FIT_Y;
            f = 1.0 * viewHeight / imageHeight;
            width = (int)(f * imageWidth + 0.5);
            height = viewHeight;
            x = (viewWidth - width) / 2;
            y = 0;
        } else {
            f = 1.0 * viewWidth / imageWidth;
            width = viewWidth;
            height = (int)(f * imageHeight + 0.5);
            x = 0;
            y = (viewHeight - height) / 2;
            mFixMode = FIT_X;
        }
        mWHRatio = f;
        return new Rect((int)x, (int)y, (int)width, (int)height);
    }

    public boolean onTouch(View v, MotionEvent event, @NonNull SendControllerMsg sender) {
        if (mDrawRect == null) {
            return true;
        }
//        JLog.info(TAG, event.toString());
        switch (event.getActionMasked()) {
            case MotionEvent.ACTION_DOWN:
            case MotionEvent.ACTION_POINTER_DOWN:
            case MotionEvent.ACTION_UP:
            case MotionEvent.ACTION_POINTER_UP:
                int pointerIndex = event.getActionIndex();
                Position position = createEventXY(event.getX(pointerIndex), event.getY(pointerIndex));
                if (position == null) {
                    return true;
                }
                mTouchMessage.actionButton = event.getActionButton();
                mTouchMessage.action = getEventAction(event.getActionMasked());
                mTouchMessage.pressure = event.getPressure(pointerIndex);
                mTouchMessage.pointerId = event.getPointerId(pointerIndex);
                mTouchMessage.position = position;
                mTouchMessage.buttons = event.getButtonState();
                sender.sendMessage(mTouchMessage);
                break;

            case MotionEvent.ACTION_MOVE:
                int size = event.getHistorySize();
                for (int i = 0; i < event.getPointerCount(); i++) {
                    if (size > 0) {
                        float historicalX = event.getHistoricalX(i, 0); // notice：如果取size - 1的话会导致丢弃过多move事件，从而无法很好地模拟轨迹
                        float historicalY = event.getHistoricalY(i, 0);
                        if (historicalX == event.getX(i) && historicalY == event.getY(i)) {
                            continue;
                        }
                    }
                    Position movePosition = createEventXY(event.getX(i), event.getY(i));
                    if (movePosition == null) {
                        continue;
                    }
                    mTouchMessage.actionButton = event.getActionButton();
                    mTouchMessage.action = MotionEvent.ACTION_MOVE;
                    mTouchMessage.pressure = event.getPressure(i);
                    mTouchMessage.pointerId = event.getPointerId(i);
                    mTouchMessage.position = movePosition;
                    mTouchMessage.buttons = event.getButtonState();
                    sender.sendMessage(mTouchMessage);
                }
                break;
            case MotionEvent.ACTION_CANCEL: // 所有手指将会被抬起
                mTouchMessage.actionButton = event.getActionButton();
                mTouchMessage.action = MotionEvent.ACTION_UP;
                mTouchMessage.pressure = event.getPressure(event.getActionIndex());
                mTouchMessage.pointerId = event.getPointerId(event.getActionIndex());
                mTouchMessage.position = null;
                mTouchMessage.buttons = event.getButtonState();
                sender.sendMessage(mTouchMessage);
                break;

            default:
                JLog.info("onTouch: Not support event!!! event = " + event.getActionMasked());
                break;
        }

        return true;
    }

    private int getEventAction(int action) {
        if (action == MotionEvent.ACTION_POINTER_UP) {
            return MotionEvent.ACTION_UP;
        }
        if (action == MotionEvent.ACTION_POINTER_DOWN) {
            return MotionEvent.ACTION_DOWN;
        }
        return action;
    }
}

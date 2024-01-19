package com.linjing.rtc.demo.screen;

import android.app.Activity;
import android.content.Intent;
import android.media.projection.MediaProjectionManager;
import android.os.Bundle;
import android.view.Gravity;
import android.widget.Button;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.R;


public class ScreenCaptureActivity extends AppCompatActivity implements IScreenCaptureView {

    private Button mStartCapture;
    private Button mStopCapture;

    public static final int CODE_QUERY_CREATE_SCREEN_CAPTURE = 11;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_screen_capture);

        mStartCapture = findViewById(R.id.startCapture);
        mStopCapture = findViewById(R.id.stopCapture);
        mStartCapture.setText(String.format("开始录屏（sessionId:%s）", ScreenCaptureMgr.getInstance().mSessionId));
        mStartCapture.setOnClickListener(v -> {
            startScreenCapture();
        });

        mStopCapture.setOnClickListener(v -> {
            stopScreenCapture();
        });

        ScreenCaptureMgr.getInstance().setViewListener(this);
    }

    private void stopScreenCapture() {
        setSessionId(0);
        ScreenCaptureMgr.getInstance().stopScreenCapture(this);

    }

    private void startScreenCapture() {
        if (!createScreenCaptureIntent()) {
            return;
        }
        ScreenCaptureMgr.getInstance().startScreenCapture(this);
    }

    private boolean createScreenCaptureIntent() {
        if (!ScreenCaptureMgr.getInstance().hasCachePrjIntent()) {
            MediaProjectionManager manager = (MediaProjectionManager) getSystemService(MEDIA_PROJECTION_SERVICE);
            if (manager != null) {
                Intent intent = manager.createScreenCaptureIntent();
                startActivityForResult(intent, CODE_QUERY_CREATE_SCREEN_CAPTURE);
            }
            return false;
        }
        return true;
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == Activity.RESULT_OK) {
            ScreenCaptureMgr.getInstance().cachePrjIntent(data);

            switch (requestCode) {
                case CODE_QUERY_CREATE_SCREEN_CAPTURE:
                    startScreenCapture();
                    break;
            }

        } else {
            Toast toast = Toast.makeText(this, "获取截屏权限失败", Toast.LENGTH_SHORT);
            toast.setGravity(Gravity.CENTER, 0, 0);
            toast.show();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        mStartCapture.setText(String.format("开始录屏（sessionId:%s）", ScreenCaptureMgr.getInstance().mSessionId));
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        ScreenCaptureMgr.getInstance().setViewListener(null);
    }

    @Override
    public void setSessionId(int remoteSessionId) {
        mStartCapture.setText(String.format("开始录屏（sessionId:%s）", remoteSessionId));
    }
}


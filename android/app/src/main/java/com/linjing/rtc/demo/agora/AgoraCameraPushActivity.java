package com.linjing.rtc.demo.agora;

import android.Manifest;
import android.content.Intent;
import android.content.res.Configuration;
import android.net.Uri;
import android.os.Bundle;
import android.provider.Settings;
import android.text.TextUtils;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.utils.ThemeUtil;
import com.linjing.sdk.api.log.JLog;

public class AgoraCameraPushActivity extends AppCompatActivity  {

    public static final String IS_EXTRA_VIDEO_SOURCE = "extraVideoSource";

    private FrameLayout mFrameLayout;

    private Button mStartUploadBtn;
    private Button mStopUploadBtn;

    private IAgoraPushPresenter mCameraPresenter;
    private EditText mEtSessionId;


    private Button mSwitchCamera;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_agora_push_camera);
        ThemeUtil.translucentStatusBar(this, true, false);
        mFrameLayout = findViewById(R.id.surface_view_container);
        mEtSessionId = findViewById(R.id.et_sessionId);
        mEtSessionId.setText(BuildConfig.channelId+"");

        mStartUploadBtn = findViewById(R.id.start_upload);
        mStopUploadBtn = findViewById(R.id.stop_upload);

        Intent intent = getIntent();
        boolean isExtraVideoSource = false;
        if (intent != null && intent.hasExtra(IS_EXTRA_VIDEO_SOURCE)) {
            isExtraVideoSource = intent.getBooleanExtra(IS_EXTRA_VIDEO_SOURCE, false);
        }
        if (isExtraVideoSource) {
            mCameraPresenter = new AgoraExtraVideoSourcePresenter();
        } else {
            mCameraPresenter = new AgoraCameraPushPresenter();
        }
        initDubbing();

        mStartUploadBtn.setOnClickListener(v -> {
            String sessionId = mEtSessionId.getText().toString();
            if (TextUtils.isEmpty(sessionId)) {
                Toast.makeText(getApplicationContext(), "SessionId 不能为空", Toast.LENGTH_LONG).show();
                return;
            }
            mCameraPresenter.startUpload(sessionId);
        });

        mStopUploadBtn.setOnClickListener(v -> {
            mCameraPresenter.stopUpload();
        });

        mCameraPresenter.setupLocalVideo(this, mFrameLayout);

        initCameraBtn();

    }

    private void initCameraBtn() {
        mSwitchCamera = findViewById(R.id.switch_camera);
        mSwitchCamera.setOnClickListener(v -> {
            mCameraPresenter.switchCamera();
        });
    }

    private boolean isDubbing = false;
    private void initDubbing() {
        Button dubbing = findViewById(R.id.dubbing);
        dubbing.setOnClickListener(v -> {
            isDubbing = !isDubbing;
            mCameraPresenter.startDubbing(this, isDubbing);
        });
    }


    @Override
    public void onConfigurationChanged(@NonNull Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (mCameraPresenter != null) {
            mCameraPresenter.stop();
            mCameraPresenter = null;
        }
    }

}
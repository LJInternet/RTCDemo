package com.linjing.rtc.demo.camera.push;

import android.Manifest;
import android.content.Intent;
import android.content.res.Configuration;
import android.net.Uri;
import android.os.Bundle;
import android.provider.Settings;
import android.view.SurfaceHolder;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.camera.helper.LiveConfigHelper;
import com.linjing.rtc.demo.utils.SPHelper;
import com.linjing.rtc.demo.utils.ThemeUtil;
import com.linjing.sdk.api.log.JLog;

public class NativeCameraPushActivity extends AppCompatActivity implements SurfaceHolder.Callback {

//    private SurfaceView mPreviewView;
    private FrameLayout mVideoLayout;

    private Button mStartEncodeBtn;
    private Button mStopEncodeBtn;
    private Button mStartUploadBtn;
    private Button mStopUploadBtn;

    private NativeCameraPushPresenter mCameraPresenter;
    private boolean mNeedRestartStream;


    private Button mSwitchCamera;
    private EditText mEditText;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_native_push_camera);
        ThemeUtil.translucentStatusBar(this, true, false);
//        mPreviewView = findViewById(R.id.surface_view);
        mVideoLayout = findViewById(R.id.video_view_layout);


        mStartUploadBtn = findViewById(R.id.start_upload);
        mStopUploadBtn = findViewById(R.id.stop_upload);
        mCameraPresenter = new NativeCameraPushPresenter();
        mCameraPresenter.setCameraStateListener(cameraParam -> JLog.debug("cameraParam:" + cameraParam.toString()));

        mStartEncodeBtn = findViewById(R.id.start_encode);
        mStopEncodeBtn = findViewById(R.id.stop_encode);

        mStartEncodeBtn.setOnClickListener(v -> {
            mCameraPresenter.startEncode();
        });

        mStopEncodeBtn.setOnClickListener(v -> {
            mCameraPresenter.stopEncode();
        });

        mStartUploadBtn.setOnClickListener(v -> {
            mCameraPresenter.startUpload();
        });

        mStopUploadBtn.setOnClickListener(v -> {
            mCameraPresenter.stopUpload();
        });

        Button captureType = findViewById(R.id.capture_type);
        int iCaptureType = SPHelper.getIntSp(this, "captureType");
        captureType.setText(iCaptureType == 0 ? "相机采集" : "文件采集");
        captureType.setOnClickListener(v-> {
            int tempType = SPHelper.getIntSp(this, "captureType");
            int newType = tempType == 0 ? 1 : 0;
            SPHelper.writeIntSp(this, "captureType", newType);
            mCameraPresenter.changeCaptureType(newType);
            finish();
        });

        initCameraBtn();
        initEditText();

//        mPreviewView.getHolder().addCallback(this);
//        mPreviewView.setKeepScreenOn(true);
        mCameraPresenter.startStream();
        mCameraPresenter.startUpload();
        mCameraPresenter.startVideoCapture();
        mCameraPresenter.setupNativeLocalVideo(this,mVideoLayout);
    }

    private void initEditText() {
        mEditText = findViewById(R.id.et_sessionId);
        Button button = findViewById(R.id.et_server_ip_confirm);
        String sessionId = SPHelper.getStringSp(getApplicationContext(), "push_sessionId", "13");
        mEditText.setText(sessionId);
        button.setOnClickListener(v-> {
            SPHelper.writeStringSp(getApplicationContext(), "push_sessionId", mEditText.getText().toString());
        });
    }

    private void initCameraBtn() {
        mSwitchCamera = findViewById(R.id.switch_camera);
        mSwitchCamera.setOnClickListener(v -> {
            mCameraPresenter.switchCamera();
        });
    }



    @Override
    public void onConfigurationChanged(@NonNull Configuration newConfig) {
        super.onConfigurationChanged(newConfig);

        LiveConfigHelper.getInstance().mIsLandscape = newConfig.orientation == Configuration.ORIENTATION_LANDSCAPE;
        if (mNeedRestartStream) {
            mNeedRestartStream = false;
            mCameraPresenter.changeScreenOrientation();
//            mCameraPresenter.surfaceCreated(mPreviewView.getHolder().getSurface());
        } else {
            mCameraPresenter.onOrientationChanged(newConfig.orientation);
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (mCameraPresenter != null) {
            mCameraPresenter.stop();
            mCameraPresenter = null;
        }
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {


//        if (mCameraPresenter != null) {
//            mCameraPresenter.surfaceCreated(holder.getSurface());
//        }
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {

//        if (mCameraPresenter != null) {
//            mCameraPresenter.surfaceChanged(holder.getSurface(), width, height);
//        }
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        if (mCameraPresenter != null) {
            mCameraPresenter.surfaceDestroyed(holder.getSurface());
        }
    }
}
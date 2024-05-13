package com.linjing.rtc.demo.multichannel;

import android.os.Bundle;
import android.text.TextUtils;
import android.view.SurfaceView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.base.IRtcEngineEx;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.utils.ThemeUtil;
import com.linjing.sdk.api.log.JLog;
import com.linjing.transfer.multichannel.LJChannel;
import com.linjing.transfer.views.VideoViews;

import java.util.Stack;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

public class MultiChannelActivity extends AppCompatActivity {

    private FrameLayout mLocalLayout;

    private FrameLayout mRemoteLayout1;

    private FrameLayout mRemoteLayout2;

    private FrameLayout mRemoteLayout3;

    private Button mStartUploadBtn;
    private Button mStopUploadBtn;

    private EditText mEtSessionId;

    private Button mSwitchCamera;

    private MultiChannelPresenter mMultiChannelPresenter;

    private ConcurrentMap<Long, FrameLayout> _remoteViews = new ConcurrentHashMap<>();
    private Stack<FrameLayout> _remoteViewsSet = new Stack<>();
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_agora_multi_channel);

        ThemeUtil.translucentStatusBar(this, true, false);
        mLocalLayout = findViewById(R.id.surface_view_container);
        mRemoteLayout1 = findViewById(R.id.remote1_view_container);
        mRemoteLayout2 = findViewById(R.id.remote2_view_container);
        mRemoteLayout3 = findViewById(R.id.remote3_view_container);
        _remoteViewsSet.push(mRemoteLayout1);
        _remoteViewsSet.push(mRemoteLayout2);
        _remoteViewsSet.push(mRemoteLayout3);

        mEtSessionId = findViewById(R.id.et_sessionId);
        mEtSessionId.setText(BuildConfig.multiChannelId+"");
        mStartUploadBtn = findViewById(R.id.start_upload);
        mStopUploadBtn = findViewById(R.id.stop_upload);
        mMultiChannelPresenter = new MultiChannelPresenter(this);
        mStartUploadBtn.setOnClickListener(v -> {
            String sessionId = mEtSessionId.getText().toString();
            if (TextUtils.isEmpty(sessionId)) {
                Toast.makeText(getApplicationContext(), "SessionId 不能为空", Toast.LENGTH_LONG).show();
                return;
            }
            if (mMultiChannelPresenter != null) {
                mMultiChannelPresenter.startUpload(sessionId);
            }
        });

        mStopUploadBtn.setOnClickListener(v -> {
            if (mMultiChannelPresenter != null) {
                mMultiChannelPresenter.stopUpload();
            }
        });

        mMultiChannelPresenter.setupLocalVideo(this, mLocalLayout);

        initCameraBtn();

    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (mMultiChannelPresenter != null) {
            mMultiChannelPresenter.destroy();
            mMultiChannelPresenter = null;
        }
    }

    private void initCameraBtn() {
        mSwitchCamera = findViewById(R.id.switch_camera);
        mSwitchCamera.setOnClickListener(v -> {
            if (mMultiChannelPresenter != null) {
                mMultiChannelPresenter.switchCamera();
            }
        });
    }

    public void onUserJoined(IRtcEngineEx mRtcEngine, LJChannel ljChannel, long uid, int fps) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {

                if (_remoteViews.containsKey(uid)) {
                    JLog.info("multiChannel", "onUserJoined _remoteViews containsKey " + uid);
                    return;
                }
                FrameLayout frameLayout = _remoteViewsSet.pop();
                if (frameLayout == null) {
                    JLog.info("multiChannel", "onUserJoined frameLayout is null ");
                    return;
                }
                SurfaceView surfaceView = mRtcEngine.CreateRendererView(MultiChannelActivity.this);
                surfaceView.setKeepScreenOn(true);
                VideoViews views = new VideoViews(surfaceView);
                surfaceView.setZOrderMediaOverlay(true);
                ljChannel.SetForMultiChannelUser(views, uid, fps);
                frameLayout.addView(surfaceView);
                _remoteViews.put(uid, frameLayout);
            }
        });
    }

    public void onUserOffLine(IRtcEngineEx mRtcEngine, LJChannel ljChannel, long uid) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                FrameLayout frameLayout = _remoteViews.remove(uid);
                if (frameLayout != null) {
                    frameLayout.removeAllViews();
                    _remoteViewsSet.push(frameLayout);
                }
            }
        });
    }
}

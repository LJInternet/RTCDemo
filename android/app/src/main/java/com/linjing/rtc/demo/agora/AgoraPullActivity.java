package com.linjing.rtc.demo.agora;

import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;
import com.linjing.sdk.api.DelayData;
import com.linjing.sdk.api.log.JLog;

public class AgoraPullActivity extends AppCompatActivity {

    private EditText mSessionId;
    private EditText mPortId;
    private EditText mServiceIp;

    private Button mStartPull;
    private Button mStopPull;

    private Button mPushAudio;
    private Button mPullMode;

    private FrameLayout mVideoLayout;

    private AgoraPullPresenter mPullSteamPresenter;

    private TextView mTvDelay;

    private boolean startedPush = false;
    private boolean rudpMode = true;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_agora_pull_stream);

        mSessionId = findViewById(R.id.et_sessionId);
        mServiceIp = findViewById(R.id.service_ip);
        mPortId = findViewById(R.id.et_port);
        mStartPull = findViewById(R.id.start_pull);
        mStopPull = findViewById(R.id.stop_pull);
        mPushAudio = findViewById(R.id.push_audio);
        mVideoLayout = findViewById(R.id.video_view_layout);
        mTvDelay = findViewById(R.id.tvDelay);
        mPullMode = findViewById(R.id.pull_mode);
        String sessionIdStr = BuildConfig.channelId + "";
        mSessionId.setText(sessionIdStr);
        mPullMode.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                rudpMode = !rudpMode;
                mPullMode.setText(rudpMode ? "PULL" : "PUSH");
            }
        });

        mPullSteamPresenter = new AgoraPullPresenter(this);

        mStartPull.setOnClickListener(v->{
            String sessionId = mSessionId.getText().toString();
            String serviceIp = mServiceIp.getText().toString();
            String portId = mPortId.getText().toString();
            if (TextUtils.isEmpty(sessionId) || TextUtils.isEmpty(serviceIp) || TextUtils.isEmpty(portId)) {
                JLog.error("开始拉流，参数为空");
                return;
            }
            startPullStream(sessionId, serviceIp, portId, rudpMode);
        });

        mStopPull.setOnClickListener(v->{
            stopPullStream();
            finish();
        });

        mPushAudio.setOnClickListener(v-> {
            startedPush = !startedPush;
            mPushAudio.setText(startedPush ? "结束声音" : "推声音");
            mPullSteamPresenter.pushAudio(startedPush);
        });
        mPullSteamPresenter.setupRemoteUi(this, mVideoLayout);

    }

    private void stopPullStream() {
        mPullSteamPresenter.stopPullStream();
        finish();
    }

    private void startPullStream(String sessionId, String serviceIp, String portId, boolean rudpMode) {
        mPullSteamPresenter.startPullStream(sessionId, serviceIp, portId, rudpMode);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        mPullSteamPresenter.destroy();
    }

    public void setDelayData(DelayData delayData) {
        runOnUiThread(new Runnable() {

            @Override
            public void run() {
                if (mTvDelay != null && delayData !=null) {
                    mTvDelay.setText(delayData.toString());
                }
            }
        });
    }
}

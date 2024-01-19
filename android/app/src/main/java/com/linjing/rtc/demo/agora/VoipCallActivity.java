package com.linjing.rtc.demo.agora;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.utils.ThemeUtil;
import com.linjing.sdk.api.DelayData;
import com.linjing.sdk.api.RTCEngineConstants;

public class VoipCallActivity extends AppCompatActivity {

    private FrameLayout mLocalLayout;
    private FrameLayout mRemoteLayout;
    private TextView mTvDelay;
    private Button mBtnStart;
    private Button mBtnRole;

    private VoipCallPresenter mPresenter;
    private long lastClickTime  = 0;

    private boolean mStarted = false;

    private boolean mSetRole = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_voip_call);
        ThemeUtil.translucentStatusBar(this, true, false);

        mLocalLayout = findViewById(R.id.fl_local_view);
        mRemoteLayout = findViewById(R.id.fl_remote_view);
        mTvDelay = findViewById(R.id.tvDelay);
        mBtnStart = findViewById(R.id.btn_start);
        mPresenter = new VoipCallPresenter(this);

        mBtnRole = findViewById(R.id.btn_Role);
        mBtnRole.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (mPresenter == null) {
                    return;
                }
                mPresenter.setRtcRole(mSetRole ? RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH : RTCEngineConstants.ClientRole.CLIENT_ROLE_PULL);
                mSetRole = !mSetRole;
                mBtnRole.setText(mSetRole ? "PULL" : "PUSH");
            }
        });

        mBtnStart.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {
                if (mPresenter == null || (System.currentTimeMillis() - lastClickTime < 2000)) {
                    return;
                }
                lastClickTime = System.currentTimeMillis();
                if (mStarted) {
                    mPresenter.stopVoip();
                } else {
                    mPresenter.startVoip();
                }
                mStarted = !mStarted;
                mBtnStart.setText(mStarted ? "结束" : "开始");
            }
        });

        mPresenter.setupRemoteUi(this, mRemoteLayout);
        mPresenter.setupLocalVideo(this, mLocalLayout);
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

    @Override
    protected void onDestroy() {
        super.onDestroy();

        if (mPresenter != null) {
            mPresenter.stop();
            mPresenter = null;
        }
    }
}

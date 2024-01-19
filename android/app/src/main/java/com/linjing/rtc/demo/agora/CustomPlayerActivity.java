package com.linjing.rtc.demo.agora;

import android.content.pm.ActivityInfo;
import android.content.res.Configuration;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.utils.ThemeUtil;
import com.linjing.rtc.player.custom.CustomPlayMode;
import com.linjing.sdk.api.DelayData;

public class CustomPlayerActivity extends AppCompatActivity {

    private FrameLayout mLocalLayout;
    private FrameLayout mRemoteLayout;
    private TextView mTvDelay;
    private Button mBtnStart;
    private Button mBtnRole;

    private CustomPlayerPresenter mPresenter;
    private long lastClickTime  = 0;

    private boolean mStarted = false;

    private boolean mSetRole = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_custom_player);
        ThemeUtil.translucentStatusBar(this, true, false);

        mRemoteLayout = findViewById(R.id.fl_remote_view);
        mTvDelay = findViewById(R.id.tvDelay);
        mBtnStart = findViewById(R.id.btn_start);
        mPresenter = new CustomPlayerPresenter(this);

        mBtnStart.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {
                if (mPresenter == null || (System.currentTimeMillis() - lastClickTime < 2000)) {
                    return;
                }
                lastClickTime = System.currentTimeMillis();
                if (mStarted) {
                    mPresenter.enableEncode(false);
                } else {
                    mPresenter.enableEncode(true);
                }
                mStarted = !mStarted;
                mBtnStart.setText(mStarted ? "结束" : "开始");
            }
        });

        mPresenter.setupRemoteUi(this, mRemoteLayout);
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

    public void onFullMode(View view) {
        if (mPresenter != null) {
            mPresenter.updateRenderMode(this, CustomPlayMode.FULL_MODE);
        }

//        mPresenter.enableEncode(true);
    }

    public void onFullMinMOde(View view) {
        if (mPresenter != null) {
            mPresenter.updateRenderMode(this, CustomPlayMode.FULL_MIN_MODE);
        }
//        mPresenter.enableEncode(false);
    }

    public void onLeftMode(View view) {
        if (mPresenter != null) {
            mPresenter.updateRenderMode(this, CustomPlayMode.HALF_LEFT_MODE);
        }
    }

    public void onRightMode(View view) {
        if (mPresenter != null) {
            mPresenter.updateRenderMode(this, CustomPlayMode.HALF_RIGHT_MODE);
        }
    }

    public void onClickOri(View view) {
        boolean isLand = false;
        if(getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
            setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
            isLand = true;
        } else {
            setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
        }
        if (mPresenter != null) {
            mPresenter.updateOrientation(isLand);
        }
    }
}

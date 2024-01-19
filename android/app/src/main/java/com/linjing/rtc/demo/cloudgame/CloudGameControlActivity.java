package com.linjing.rtc.demo.cloudgame;

import android.content.Intent;
import android.os.Bundle;
import android.widget.FrameLayout;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;

public class CloudGameControlActivity extends AppCompatActivity {

    public static final String CHANNEL_ID = "ChannleId";

    CloudGameControlPresenter mCloudGameControlPresenter;
    private FrameLayout mVideoLayout;
    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_cloud_game);

        mVideoLayout = findViewById(R.id.video_view_layout);
        mCloudGameControlPresenter = new CloudGameControlPresenter(this);
        mCloudGameControlPresenter.setupRemoteUi(this, mVideoLayout);
        Intent intent = getIntent();
        int channelId = BuildConfig.channelId;
        if (intent != null && intent.hasExtra(CHANNEL_ID)) {
            channelId = intent.getIntExtra(CHANNEL_ID, channelId);
        }

        mCloudGameControlPresenter.startPullStream(channelId);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (mCloudGameControlPresenter != null) {
            mCloudGameControlPresenter.stopPullStream();
            mCloudGameControlPresenter.destroy();
            mCloudGameControlPresenter = null;
        }
    }
}

package com.linjing.rtc.demo.cloudgame;

import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.EditText;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.linjing.rtc.demo.R;


public class CloudGameControlJoinActivity extends AppCompatActivity {
    private EditText mEtChannelId;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_cloud_game_join);

        mEtChannelId = (EditText) findViewById(R.id.et_channel_id);
        findViewById(R.id.bt_join).setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {
                String channelIdStr = mEtChannelId.getText().toString();
                if (TextUtils.isEmpty(channelIdStr)) {
                    return;
                }
                int channelId = Integer.parseInt(channelIdStr);
                Intent intent = new Intent(CloudGameControlJoinActivity.this, CloudGameControlActivity.class);
                intent.putExtra(CloudGameControlActivity.CHANNEL_ID, channelId);
                startActivity(intent);
                finish();
            }
        });
    }
}

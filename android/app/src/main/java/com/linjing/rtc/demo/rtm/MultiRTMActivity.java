package com.linjing.rtc.demo.rtm;

import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;


import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;
import com.linjing.rtc.demo.UserInfo;
import com.linjing.rudp.RudpEngineWrapperJni;

import java.nio.charset.StandardCharsets;

public class MultiRTMActivity extends AppCompatActivity implements RudpEngineWrapperJni.RUDPExCallback {

    private Button mBtnJoin;
    private Button mBtnLeave;
    private Button mBtnSend;
    private Button mBtnPush;
    private EditText mEtChannelId;
    private EditText mEtMessage;
    private TextView mTvMessage;
    private boolean mPush = true;
    private RudpEngineWrapperJni mRudpEngine;
    private long currentUid = UserInfo.userId;
    private String currentChannelId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_rtm_test);

        mBtnJoin = (Button) findViewById(R.id.btn_join);
        mBtnLeave = (Button) findViewById(R.id.btn_leave);
        mBtnPush = (Button) findViewById(R.id.btn_push);
        mBtnSend = (Button) findViewById(R.id.btn_send);
        mTvMessage = (TextView) findViewById(R.id.tv_msg);
        mEtChannelId = (EditText) findViewById(R.id.et_channel);
        mEtMessage = (EditText) findViewById(R.id.et_msg);
        mBtnJoin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                doJoinChannel();
            }
        });
        mBtnLeave.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                doLeaveChannel();
            }
        });

        mBtnSend.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                doSendMessage();
            }
        });
        mBtnPush.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mPush = !mPush;
                mBtnPush.setText(mPush ? "推流" : "拉流");
            }
        });
        mRudpEngine = new RudpEngineWrapperJni();
        mRudpEngine.create(this);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (mRudpEngine != null) {
            doLeaveChannel();
            mRudpEngine.destroy();
            mRudpEngine = null;
        }
    }

    private void doSendMessage() {
        String message = mEtMessage.getText().toString();
        if (TextUtils.isEmpty(message)) {
            Toast toast = Toast.makeText(this, "消息为空", Toast.LENGTH_SHORT);
            toast.setGravity(Gravity.CENTER, 0, 0);
            toast.show();
            return;
        }
        if (mRudpEngine != null) {
            mRudpEngine.sendMessage(currentUid, currentChannelId, message.getBytes(StandardCharsets.UTF_8));
        }
    }

    private void doLeaveChannel() {
        if (mRudpEngine != null) {
            mRudpEngine.leaveChannel(currentChannelId, currentUid);
        }
    }

    private void doJoinChannel() {
        String channelStr = mEtChannelId.getText().toString();
        if (TextUtils.isEmpty(channelStr)) {
            Toast toast = Toast.makeText(this, "ChannelId为空", Toast.LENGTH_SHORT);
            toast.setGravity(Gravity.CENTER, 0, 0);
            toast.show();
            return;
        }
        currentChannelId = channelStr;
        if (mRudpEngine != null) {
            mRudpEngine.joinChannel(BuildConfig.token, true, 0, currentUid, BuildConfig.appId, currentChannelId);
        }
    }

    @Override
    public void onDataCallback(long uid, String channelId, byte[] data) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                String text = mTvMessage.getText().toString();
                text = text + "\n uid= " + uid + " message = " + new String(data);
                mTvMessage.setText(text);
            }
        });
    }

    @Override
    public void onEventCallback(long uid, String channelId, int type, int length, String msg) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                String text = mTvMessage.getText().toString();
                text = text + "\n uid= " + uid + " type = " + type + " length = " + length + " msg = " + msg;
                mTvMessage.setText(text);
            }
        });
    }

    @Override
    public void onUserJoined(long userId) {
        Log.d("TRMTest", "onUserJoined " + userId);
    }

    @Override
    public void onUserOffLine(long userId) {
        Log.d("TRMTest", "onUserOffLine " + userId);
    }

    @Override
    public void onLinkStatus(long uid, String channelId, int status) {
        Log.d("TRMTest", "status " + status);
    }

    @Override
    public void onJoinChannelSuccess() {
        Log.d("TRMTest", "onJoinChannelSuccess ");
    }

    @Override
    public void onLeveChannelSuccess() {
        Log.d("TRMTest", "onLeveChannelSuccess");
    }
}

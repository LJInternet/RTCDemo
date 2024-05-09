package com.linjing.rtc.demo;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.linjing.base.apm.PerformanceUtils;
import com.linjing.rtc.demo.agora.AgoraCameraPushActivity;
import com.linjing.rtc.demo.agora.AgoraPullActivity;
import com.linjing.rtc.demo.agora.CustomPlayerActivity;
import com.linjing.rtc.demo.agora.VoipCallActivity;
import com.linjing.rtc.demo.cloudgame.CloudGameControlJoinActivity;
import com.linjing.rtc.demo.rtm.MultiRTMActivity;
import com.linjing.rtc.demo.rtm.P2PRTMActivity;
import com.linjing.rtc.demo.screen.ScreenCaptureActivity;
import com.linjing.rtc.demo.MainListAdapter.MainData;
import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    private RecyclerView mRecyclerView;
    private Handler mHandler;
    PerformanceUtils performance = new PerformanceUtils();
    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);

        mRecyclerView = findViewById(R.id.recyclerView);
        GridLayoutManager layoutManager = new GridLayoutManager(this, 2);//第二个参数为网格的列数
        mRecyclerView.setLayoutManager(layoutManager);
        MainListAdapter adapter = new MainListAdapter();
        List<MainData> list = new ArrayList<>();
        list.add(new MainData("屏幕采集", 1, ScreenCaptureActivity.class));
        list.add(new MainListAdapter.MainData("声网API推流", 1, AgoraCameraPushActivity.class));
        list.add(new MainData("声网API自定义输入推流", 2, AgoraCameraPushActivity.class));
        list.add(new MainData("声网API拉流", 1, AgoraPullActivity.class));
        list.add(new MainData("通话模式", 1, VoipCallActivity.class));
        list.add(new MainData("自定义播放器", 1, CustomPlayerActivity.class));
        list.add(new MainData("多人rtm测试", 1, MultiRTMActivity.class));
        list.add(new MainData("rtm测试", 1, P2PRTMActivity.class));
        list.add(new MainData("云手机控制", 1, CloudGameControlJoinActivity.class));
//        mHandler = new Handler();
//        mHandler.postDelayed(new Runnable() {
//
//            @Override
//            public void run() {
//                Log.d("performance", performance.getNetInfo(MainActivity.this));
//                Log.d("performance", performance.getMemoryInfo(MainActivity.this));
//                Log.d("performance", performance.getCpuInfo(MainActivity.this));
//                mHandler.postDelayed(this, 1000);
//            }
//        }, 1000);

        adapter.setData(list);
        adapter.setItemListener(new MainListAdapter.OnItemClick() {

            @Override
            public void onMainItemClick(MainData data) {
//                FeedBackManager.getInstance().sendFeedback("test", "ssss", JLog.getLogPath(), "");
                if (data.actionKey == 2) {
                    Intent intent = new Intent(MainActivity.this, data.mClazz);
                    intent.putExtra(AgoraCameraPushActivity.IS_EXTRA_VIDEO_SOURCE, true);
                    startActivity(intent);
                    return;
                }
                startActivity(new Intent(MainActivity.this, data.mClazz));
            }
        });
        mRecyclerView.setAdapter(adapter);
    }


}

package com.linjing.rtc.demo;

import android.app.Application;
import android.os.Build;

import com.linjing.reporter.ReportCenter;
import com.linjing.reporter.ReportCenterConfig;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.api.log.JLog;

import java.util.HashMap;
import java.util.Map;

public class MyApplication extends Application {


    @Override
    public void onCreate() {
        super.onCreate();
        LJSDKHelper.initLJSDK(this);
    }

    @Override
    public void onTerminate() {
        super.onTerminate();
        LJSDK.instance().destroy();
    }

    private void initReport() {
        long uid = UserInfo.userId;
        ReportCenterConfig config = new ReportCenterConfig();
        config.isTestEv = true;
        config.collectDuration = 10000;
        config.appId = config.isTestEv ? 1001 : 1000;
        config.eventCallback = (result, msg) -> {
            JLog.info("report : result "+ result + ", msg :" + msg);
        };
        boolean result = ReportCenter.instance().initEx(config);
        ReportCenter.instance().setUserInfo(BuildConfig.token, uid);

        Map<String,Object> attrs = new HashMap<>();
        attrs.put("appid",config.appId);
        attrs.put("ua","ljsdkdemo&0.0.1&test");
        attrs.put("userId", String.valueOf(uid));
        attrs.put("platform", "android");
        attrs.put("platfromVer",  String.valueOf(Build.VERSION.SDK_INT));
        attrs.put("monitorVer",  "0.0.1");
        attrs.put("product",  Build.PRODUCT);
        attrs.put("rtcMode",  1);
        attrs.put("liveid", nextID());
        ReportCenter.instance().setCommonAttrs(attrs);
        JLog.info("LJ_BASE:init report center result:"+result);
    }

    private static long previousTimeMillis = System.currentTimeMillis();
    private static long counter = 0L;

    public static synchronized long nextID() {
        long currentTimeMillis = System.currentTimeMillis();
        counter = (currentTimeMillis == previousTimeMillis) ? (counter + 1L) & 1048575L : 0L;
        previousTimeMillis = currentTimeMillis;
        long timeComponent = (currentTimeMillis & 8796093022207L) << 20;
        return timeComponent | counter;
    }
}

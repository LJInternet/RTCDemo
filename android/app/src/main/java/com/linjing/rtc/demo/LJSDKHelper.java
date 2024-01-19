package com.linjing.rtc.demo;

import android.app.Application;
import android.content.Context;
import android.os.Build;

import com.linjing.reporter.ReportCenter;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.LJSDKConfig;
import com.linjing.sdk.api.log.IJLog;
import com.linjing.sdk.api.report.IReportApi;
import com.linjing.sdk.wrapper.live.BuildConfig;

import java.util.Map;

public class LJSDKHelper {


    public static void initLJSDK(Application context) {
        LJSDKConfig config = new LJSDKConfig.Builder()
                .setAppId("ssss")
                .setDebugMode(true)
                .setAppUa("test&1.1.0&offical")
                .setTestEv(true)
                .setJLog(initLogger(context))
                .setReportApi(new IReportApi() {
                    @Override
                    public void report(String tag, Map<String, Object> info) {
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                            ReportCenter.instance().report(tag, info);
                        }
                    }

                    @Override
                    public void registerSlot(String tag) {
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                            ReportCenter.instance().registerSlot(tag);
                        }
                    }

                    @Override
                    public void unregisterSlot(String tag) {
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                            ReportCenter.instance().unregisterSlot(tag);
                        }
                    }

                    @Override
                    public void addCommonAttrs(Map<String, Object> info) {
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                            ReportCenter.instance().addCommonAttrs(info);
                        }
                    }
                }).build();

        LJSDK.instance().init(context, config, null);
    }

    private static IJLog initLogger(Context context) {
        String logPath = context.getFilesDir().getAbsolutePath() + "/xlog";
        LJLogImpl logImpl = new LJLogImpl();
        logImpl.init(context, "LJSDK", logPath, BuildConfig.DEBUG);
        return logImpl;
    }
}

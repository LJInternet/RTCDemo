package com.linjing.rtc.demo;

import android.content.Context;

import com.linjing.sdk.api.log.IJLog;
import com.tencent.mars.xlog.Log;
import com.tencent.mars.xlog.Xlog;

public class LJLogImpl implements IJLog {

    private boolean isLogEnabled = true;
    private int mLogLevel = Xlog.LEVEL_DEBUG;
    String mLogPath = "";
    @Override
    public void init(Context context, String tag, String logPath, boolean isTest) {
        Xlog xlog = new Xlog();
        Log.setLogImp(xlog);
        Log.d("test", logPath);
        Xlog.setConsoleLogOpen(true);
        mLogPath = logPath;
        Xlog.appenderOpen(Xlog.LEVEL_DEBUG, Xlog.AppednerModeAsync, "", logPath, "LJLog", 2, "");
        Xlog.setMaxFileSize(10 * 1024 * 1024); // 10M
    }

    @Override
    public String getLogPath() {
        return mLogPath;
    }

    @Override
    public void setLogLevel(int level) {
        Xlog.setLogLevel(level);
    }

    @Override
    public boolean isLogLevelEnabled(int level) {
        return level <= mLogLevel;
    }

    @Override
    public void setSysLogEnabled(boolean enabled) {
        Xlog.setConsoleLogOpen(enabled);
    }

    @Override
    public boolean isLogEnable() {
        return isLogEnabled;
    }

    @Override
    public void setLogEnable(boolean enable) {
        isLogEnabled = enable;
    }

    @Override
    public void setMaxFileCount(int maxCount) {

    }

    @Override
    public void setMaxFileSize(int byteSize) {
        Xlog.setMaxFileSize(byteSize);
    }

    @Override
    public void flushToDisk() {
        Log.appenderFlush(true);
    }

    @Override
    public void verbose(Object obj, String format, Object... args) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_VERBOSE)) {
            Log.v(obj.toString(), String.format(format, args));
        }
    }

    @Override
    public void verbose(Object obj, String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_VERBOSE)) {
            Log.v(obj.toString(), msg);
        }
    }

    @Override
    public void verbose(String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_VERBOSE)) {
            Log.v("verbose", msg);
        }
    }

    @Override
    public void debug(Object obj, String format, Object... args) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_DEBUG)) {
            Log.d(obj.toString(), String.format(format, args));
        }
    }

    @Override
    public void debug(Object obj, String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_DEBUG)) {
            Log.d(obj.toString(), msg);
        }
    }

    @Override
    public void debug(Object obj, Throwable t) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_DEBUG)) {
            Log.d(obj.toString(), t.toString());
        }
    }

    @Override
    public void debug(String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_DEBUG)) {
            Log.d("debug:", msg);
        }
    }

    @Override
    public void debug(String msg, Throwable t) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_DEBUG)) {
            Log.d("debug:", msg, t);
        }
    }

    @Override
    public void info(Object obj, String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.i(obj.toString(), msg);
        }
    }

    @Override
    public void info(Object obj, String format, Object... args) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.i(obj.toString(), String.format(format, args));
        }
    }

    @Override
    public void info(String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.i("info", msg);
        }
    }

    @Override
    public void warn(Object obj, String format, Object... args) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_WARNING)) {
            Log.w(obj.toString(), String.format(format, args));
        }
    }

    @Override
    public void warn(Object obj, String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_WARNING)) {
            Log.w(obj.toString(), msg);
        }
    }

    @Override
    public void warn(String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_WARNING)) {
            Log.w("warn", msg);
        }
    }

    @Override
    public void error(Object obj, String format, Object... args) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.e(obj.toString(), String.format(format, args));
        }
    }

    @Override
    public void error(Object obj, String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.e(obj.toString(), msg);
        }
    }

    @Override
    public void error(Object obj, Throwable t) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.e(obj.toString(), t.toString());
        }
    }

    @Override
    public void error(String msg) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.e("error", msg);
        }
    }

    @Override
    public void error(String msg, Throwable t) {
        if (isLogEnabled && isLogLevelEnabled(Xlog.LEVEL_ALL)) {
            Log.e("error", msg, t);
        }
    }

    @Override
    public void destroy() {
        Log.appenderClose();
    }
}

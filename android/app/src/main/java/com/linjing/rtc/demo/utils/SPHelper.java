package com.linjing.rtc.demo.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class SPHelper {

    public static final String SP_FILE_NAME = "testSp";
    public static void writeIntSp(Context context, String key, int value) {
        SharedPreferences sharedPref = context.getSharedPreferences(SP_FILE_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPref.edit();
        editor.putInt(key, value);
        editor.apply();
    }

    public static int getIntSp(Context context, String key) {
        SharedPreferences sharedPref = context.getSharedPreferences(SP_FILE_NAME, Context.MODE_PRIVATE);
        return sharedPref.getInt(key, 0);
    }

    public static void writeStringSp(Context context, String key, String value) {
        SharedPreferences sharedPref = context.getSharedPreferences(SP_FILE_NAME, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPref.edit();
        editor.putString(key, value);
        editor.apply();
    }

    public static String getStringSp(Context context, String key) {
        SharedPreferences sharedPref = context.getSharedPreferences(SP_FILE_NAME, Context.MODE_PRIVATE);
        return sharedPref.getString(key, "61.155.136.210");
    }

    public static String getStringSp(Context context, String key, String defaultValue) {
        SharedPreferences sharedPref = context.getSharedPreferences(SP_FILE_NAME, Context.MODE_PRIVATE);
        return sharedPref.getString(key, defaultValue);
    }

}

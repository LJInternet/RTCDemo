package com.linjing.rtc.demo.screen;

import android.annotation.TargetApi;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.graphics.BitmapFactory;
import android.os.Build;

import androidx.core.app.NotificationCompat;

import com.linjing.rtc.demo.R;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.api.log.JLog;

public class NotificationHelper {

    private static final String TAG = "NotificationHelper";

    public static void startForegroundNotification(Context context, Service service, int id) {
        try {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                startForegroundWithAndroidO(context, service, id);
            } else {
                showNotification(context, service, id);
            }
        } catch (Exception e) {
            JLog.info(TAG, "startForegroundNotification exception:" + e.getMessage());
        }
    }

    public static void showNotification(Context context, Service service, int id) {
        NotificationCompat.Builder builder = new NotificationCompat.Builder(LJSDK.instance().getAppContext())
                .setContentTitle(context.getString(R.string.app_name))
                .setLargeIcon(BitmapFactory.decodeResource(LJSDK.instance().getAppContext().getResources(), R.mipmap.ic_launcher_round))
                .setContentText(context.getString(R.string.liveing))
                .setAutoCancel(true)
                .setWhen(System.currentTimeMillis())
                .setTicker(context.getString(R.string.app_name));
        service.startForeground(id, builder.build());
    }

    @TargetApi(26)
    private static void startForegroundWithAndroidO(Context context, Service service, int id) {
        String channelId = context.getPackageName();
        String channelName = "NimoBroadcaster RtmpService";

        NotificationManager manager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        NotificationChannel notificationChannel = new NotificationChannel(channelId, channelName, NotificationManager.IMPORTANCE_NONE);
        manager.createNotificationChannel(notificationChannel);

        NotificationCompat.Builder builder = new NotificationCompat.Builder(LJSDK.instance().getAppContext(), channelId)
                .setContentTitle(context.getString(R.string.app_name))
                .setLargeIcon(BitmapFactory.decodeResource(LJSDK.instance().getAppContext().getResources(), R.mipmap.ic_launcher_round))
                .setContentText(context.getString(R.string.liveing))
                .setAutoCancel(true)
                .setWhen(System.currentTimeMillis())
                .setTicker(context.getString(R.string.app_name));
        service.startForeground(id, builder.build());
    }
}

package com.linjing.rtc.demo.camera.helper;

import android.media.MediaCodecInfo;

import com.linjing.capture.api.LiveMode;
import com.linjing.capture.api.camera.CameraConfig;
import com.linjing.capture.api.camera.CameraFaceType;
import com.linjing.rtc.demo.camera.bean.ResolutionParam;
import com.linjing.rtc.demo.screen.ScreenCaptureMgr;
import com.linjing.rtc.demo.utils.ThemeUtil;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.api.audio.AudioType;
import com.linjing.sdk.encode.api.video.EncodeConfig;
import com.linjing.sdk.encode.api.video.VideoEncodeConfig;
import com.linjing.sdk.wrapper.audio.AudioConfig;
import com.linjing.sdk.wrapper.video.VideoConfig;
import com.linjing.sdk.wrapper.video.frameRatePolicy.HandlerFrameRatePolicy;

import java.lang.ref.WeakReference;

/**
 * Created by cbw on 2020/7/21
 */
public class MediaConfigHelper {

    /**
     * 用于做一些测试
     */
    public static boolean AppTestMode = true;

    public static VideoConfig createVideoConfig(int liveMode, ResolutionParam resolutionParam) {

        VideoConfig videoConfig = new VideoConfig();
        videoConfig.context = LJSDK.instance().getAppContext();
        videoConfig.liveMode = liveMode;
        videoConfig.captureBitmapWhenCameraNotCall = false;
        videoConfig.ignoreFrameWhenOpenCamera = true;
        videoConfig.cameraType = CameraConfig.CameraType.Camera1;
        videoConfig.cameraFacing = CameraFaceType.FRONT;
        videoConfig.enableCameraFaceDetection = false;
        videoConfig.previewWidth = resolutionParam.videoWidth();
        videoConfig.previewHeight = resolutionParam.videoHeight();


        // 编码16对齐
        int encodeWidth = (int) Math.ceil(videoConfig.previewWidth / 16f) * 16;
        int encodeHeight = (int) Math.ceil(videoConfig.previewHeight / 16f) * 16;
        videoConfig.encodeWidth = encodeWidth;
        videoConfig.encodeHeight = encodeHeight;
        videoConfig.frameRate = resolutionParam.getVideoFrameRate();
        videoConfig.enableCameraDropFrame = true;
        videoConfig.lookupPreviewFps = true;
        videoConfig.ignoreFrameWhenOpenCamera = true;
        if (LiveMode.isScreen(liveMode)) {
            videoConfig.resultData = ScreenCaptureMgr.getInstance().getPrjIntent();
            videoConfig.dpi = ThemeUtil.getDpi(LJSDK.instance().getAppContext());
            videoConfig.frameRatePolicy = HandlerFrameRatePolicy.TAG;
        }
        /**
         * 编码参数
         */
        videoConfig.encodeConfig = createEncodeConfig(resolutionParam.getVideoBitrate() / 1000,
                resolutionParam.getMaxVideoBitrate() / 1000, resolutionParam.getMinVideoBitrate() / 1000);

        return videoConfig;
    }
    public static AudioConfig createAudioConfig() {
        AudioConfig audioConfig = new AudioConfig();
        audioConfig.weakContext = new WeakReference<>(LJSDK.instance().getAppContext().getApplicationContext());
        audioConfig.bitrateInbps = 64000;
        audioConfig.framePerBuffer = 320; //
        audioConfig.sampleRate = 32000;
        audioConfig.channels = 1;
        audioConfig.audioType = AudioType.OPUS;
        audioConfig.isHardEncode = true;  // !isHuyaUpload || isAudioHardEncode;
        audioConfig.needAdts = false;      // isHuyaUpload && isAudioHardEncode;
        return audioConfig;
    }

    public static AudioConfig createAACAudioConfig() {
        AudioConfig audioConfig = new AudioConfig();
        audioConfig.weakContext = new WeakReference<>(LJSDK.instance().getAppContext().getApplicationContext());
        audioConfig.bitrateInbps = 64000;
        audioConfig.framePerBuffer = 960; //
        audioConfig.sampleRate = 48000;
        audioConfig.channels = 1;
        audioConfig.audioType = AudioType.AAC;
        audioConfig.isHardEncode = true;  // !isHuyaUpload || isAudioHardEncode;
        audioConfig.needAdts = false;      // isHuyaUpload && isAudioHardEncode;
        return audioConfig;
    }

    public static VideoEncodeConfig createEncodeConfig(int bitrate, int maxBitrate, int minBitRate) {
        final EncodeConfig.CodecType codecType = EncodeConfig.CodecType.H264;
        final int encoderType = VideoEncodeConfig.EncoderType.AsyncHard;
        final int bitrateMode = MediaCodecInfo.EncoderCapabilities.BITRATE_MODE_CBR;
        return new VideoEncodeConfig(encoderType, codecType, bitrateMode, bitrate, true, maxBitrate, minBitRate);
    }

}

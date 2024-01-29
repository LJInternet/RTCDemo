package com.linjing.rtc.demo.agora;

import android.content.Context;
import android.graphics.BitmapFactory;
import android.opengl.GLES20;
import android.os.Environment;
import android.view.Surface;
import android.view.SurfaceView;
import android.widget.FrameLayout;

import com.linjing.capture.api.LiveMode;
import com.linjing.rtc.demo.BuildConfig;
import com.linjing.rtc.demo.R;
import com.linjing.rtc.LJRtcEngine;
import com.linjing.rtc.api.ChannelConfig;
import com.linjing.rtc.api.RtcEngineConfig;
import com.linjing.rtc.base.IRtcEngine;
import com.linjing.rtc.demo.UserInfo;
import com.linjing.rtc.demo.camera.bean.ResolutionParam;
import com.linjing.rtc.demo.camera.helper.MediaConfigHelper;
import com.linjing.rtc.demo.camera.helper.ResolutionOptions;
import com.linjing.sdk.LJSDK;
import com.linjing.sdk.api.RTCEngineConstants;
import com.linjing.sdk.api.audio.AudioFrame;
import com.linjing.sdk.api.audio.AudioParams;
import com.linjing.sdk.api.audio.IAudioFrameObserver;
import com.linjing.sdk.api.video.IVideoFrameConsumer;
import com.linjing.sdk.api.video.TextureRelease;
import com.linjing.sdk.api.video.VideoEncoderConfiguration;
import com.linjing.sdk.gpuImage.util.GlHelper;
import com.linjing.sdk.wrapper.camera.CameraMediaClient;
import com.linjing.sdk.wrapper.video.VideoConfig;
import com.linjing.sdk.wrapper.video.api.DrawFrameListener;
import com.linjing.transfer.video.IVideoSource;
import com.linjing.transfer.views.ISurfaceObserver;
import com.linjing.transfer.views.RenderSurfaceView;

import java.io.FileOutputStream;
import java.io.IOException;

public class AgoraExtraVideoSourcePresenter extends IAgoraPushPresenter {

    private IRtcEngine mRtcEngine;

    private CameraMediaClient mCameraMediaClient;

    private IVideoFrameConsumer mVideoFrameConsumer;

    public AgoraExtraVideoSourcePresenter() {
        try {
            RtcEngineConfig config = new RtcEngineConfig();
            mRtcEngine = IRtcEngine.create(config);
            mRtcEngine.setClientRole(RTCEngineConstants.ClientRole.CLIENT_ROLE_PUSH);

            VideoEncoderConfiguration.VideoDimensions dimensions = new VideoEncoderConfiguration.
                    VideoDimensions(1920, 1080, 4000, 3000, 30);

            VideoEncoderConfiguration encoderConfiguration = new VideoEncoderConfiguration(30, 4000,
                    dimensions, VideoEncoderConfiguration.OrientationMode.ORIENTATION_MODE_FIXED_LANDSCAPE);

//            encoderConfiguration.enableFramePolicy =true;
            mRtcEngine.setVideoEncoderConfiguration(encoderConfiguration);
            mRtcEngine.enableLocalVideo(false);
            mRtcEngine.enableAudio();

            mRtcEngine.setVideoSource(new IVideoSource() {
                @Override
                public boolean onInitialize(IVideoFrameConsumer consumer) {
                    mVideoFrameConsumer = consumer;
                    return true;
                }

                @Override
                public boolean onStart() {
                    return true;
                }

                @Override
                public void onStop() {

                }

                @Override
                public void onDispose() {

                }

                @Override
                public int getBufferType() {
                    return 0;
                }

                @Override
                public int getCaptureType() {
                    return 0;
                }

                @Override
                public int getContentHint() {
                    return 0;
                }
            });
        } catch (Exception e) {
            e.printStackTrace();
        }
        initCameraInput();

        mRtcEngine.registerAudioFrameObserver(new IAudioFrameObserver() {
            byte[] outAudioData;
            @Override
            public boolean onRecordFrame(AudioFrame audioFrame) {
                return false;
            }

            @Override
            public boolean onPlaybackFrame(AudioFrame audioFrame) {
                return false;
            }

            @Override
            public boolean onPlaybackFrameBeforeMixing(AudioFrame audioFrame, int uid) {
                audioFrame.samples.position(0);
                if (outAudioData==null||outAudioData.length!=audioFrame.samples.remaining()){
                    outAudioData= new byte[audioFrame.samples.remaining()];
                }
//                    byte[] outData = new byte[byteBuffer.remaining()];
                audioFrame.samples.get(outAudioData, 0, outAudioData.length);
                testSetDecodeData(outAudioData, outAudioData.length);
                return false;
            }

            @Override
            public boolean onMixedFrame(AudioFrame audioFrame) {
                return false;
            }

            @Override
            public boolean isMultipleChannelFrameWanted() {
                return false;
            }

            @Override
            public boolean onPlaybackFrameBeforeMixingEx(AudioFrame audioFrame, int uid, String channelId) {
                return false;
            }

            @Override
            public int getObservedAudioFramePosition() {
                return POSITION_BEFORE_MIXING;
            }

            @Override
            public AudioParams getRecordAudioParams() {
                return null;
            }

            @Override
            public AudioParams getPlaybackAudioParams() {
                return null;
            }

            @Override
            public AudioParams getMixedAudioParams() {
                return null;
            }
        });

    }

    private void initCameraInput() {
        mCameraMediaClient = new CameraMediaClient();
        startStream();
    }

    private void startStream() {
        VideoConfig videoConfig = getVideoConfig();
        videoConfig.showBitmap = BitmapFactory.decodeResource(LJSDK.instance().getAppContext().getResources(),
                R.drawable.ic_test);
        mCameraMediaClient.startVideoStream(videoConfig);
        mCameraMediaClient.setDrawFrameListener(new DrawFrameListener() {
            float[] mTransform = GlHelper.newIdentityTransform();
            @Override
            public int onPreviewDrawFrame(int textureId, int width, int height, long timestampNs) {
                if (mVideoFrameConsumer != null) {
                    mVideoFrameConsumer.consumeTextureFrame(textureId, GLES20.GL_TEXTURE_2D, width, height,
                            0, timestampNs, mTransform, "aaaaaaaaaaaaaaaaaaaa".getBytes(), new TextureRelease() {
                                @Override
                                public void onTextureRelease(int textureId, long timestamp) {
//                                    Log.d("onTextureRelease ", "onTextureRelease texture Id:" + textureId);
                                }
                            });
                }
                return 0;
            }

            @Override
            public int onPreviewDrawFrame(byte[] nv21, int width, int height, int rotation, long timestampNs) {
                return 0;
            }

            @Override
            public int onPreviewDrawFrame(int textureId, byte[] nv21, int width, int height, int rotation, long timestampNs) {
                return 0;
            }

            @Override
            public int getFrameFormat() {
                return FrameFormat.TEXTURE;
            }

            @Override
            public boolean isTextureAndData() {
                return false;
            }
        });
    }

    private VideoConfig getVideoConfig() {
        ResolutionParam resolutionParam = ResolutionOptions.Blue3mParam.clone();
        resolutionParam.setLand(true);
        return MediaConfigHelper.createVideoConfig(LiveMode.CAMERA, resolutionParam);
    }

    public void stop() {
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
            mRtcEngine.destroy();
            mRtcEngine = null;
        }

        if (mCameraMediaClient != null) {
            mCameraMediaClient.stopVideoEncode();
            mCameraMediaClient.stopUpload();
            mCameraMediaClient.release();
            mCameraMediaClient = null;
        }
    }

    public void setupLocalVideo(Context context, FrameLayout group) {
        SurfaceView surfaceView = mRtcEngine.CreateRendererView(context);
        surfaceView.setZOrderMediaOverlay(true);
        surfaceView.setKeepScreenOn(true);
        RenderSurfaceView renderSurfaceView = (RenderSurfaceView) surfaceView;
        renderSurfaceView.setISurfaceObserver(new ISurfaceObserver() {
            @Override
            public void surfaceCreated(Surface holder) {
                if (mCameraMediaClient != null) {
                    mCameraMediaClient.startVideoPreview(holder);
                }
            }

            @Override
            public void surfaceChanged(Surface holder, int width, int height) {
                if (mCameraMediaClient != null) {
                    mCameraMediaClient.updateVideoPreviewSize(width, height);
                }
            }

            @Override
            public void surfaceDestroyed(Surface holder) {
                if (mCameraMediaClient != null) {
                    mCameraMediaClient.stopVideoPreview();
                    mCameraMediaClient.stopVideoCapture();
                }
            }
        });

        group.addView(surfaceView);
    }

    public void startUpload(String sessionId) {
        LJRtcEngine.setDebugEnv(true);
        ChannelConfig channelConfig = new ChannelConfig();

        channelConfig.appID = BuildConfig.appId;
        channelConfig.userID = UserInfo.userId;
        channelConfig.channelID = BuildConfig.channelId+"";
        channelConfig.token = BuildConfig.token;

        mRtcEngine.joinChannel(channelConfig);
    }

    public void stopUpload() {
        if (mRtcEngine != null) {
            mRtcEngine.leaveChannel();
        }
    }


    public void switchCamera() {
        if (mCameraMediaClient != null) {
            mCameraMediaClient.switchCamera();
        }
    }

    private FileOutputStream mFos2 = null;
    private void testSetDecodeData(byte[] data, int dataLen) {
        try {
            if (mFos2 == null) {
                String TEST_FILE = Environment.getExternalStorageDirectory().getPath() + "/receive.pcm";
                mFos2 = new FileOutputStream(TEST_FILE);
            }
            mFos2.write(data, 0, dataLen);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}

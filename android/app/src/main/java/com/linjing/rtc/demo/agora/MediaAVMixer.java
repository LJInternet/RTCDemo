package com.linjing.rtc.demo.agora;

import android.media.MediaCodec;
import android.media.MediaFormat;
import android.media.MediaMuxer;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.Looper;
import android.os.Message;
import android.util.Log;

import androidx.annotation.NonNull;

import com.linjing.sdk.api.audio.AudioFrame;
import com.linjing.sdk.api.log.JLog;
import com.linjing.sdk.api.video.VideoFrame;
import com.linjing.transfer.push.helper.VideoHelper;

import java.io.File;
import java.io.IOException;
import java.lang.ref.WeakReference;
import java.nio.ByteBuffer;

public class MediaAVMixer {

    public static final String TAG = "MediaOutputMixer";

    private HandlerThread mMuxerTread;

    private MixerHandler mHandler;

    private static final int MSG_START_MUXER = 0;
    private static final int MSG_STOP_MUXER = 1;
    private static final int MSG_SEND_VIDEO = 2;
    private static final int MSG_SEND_AUDIO = 3;

    public MediaAVMixer() {

    }

    public void startMixer(String outputPath) {
        if (mMuxerTread != null) {
            JLog.error(TAG, "startMixer mMuxer is already running");
            return;
        }
        mMuxerTread = new HandlerThread("MixerThread");
        mMuxerTread.start();

        JLog.info(TAG, "Output file is " + outputPath);

        mHandler = new MixerHandler(this, mMuxerTread.getLooper());

        Message message = Message.obtain(mHandler, MSG_START_MUXER, outputPath);
        mHandler.sendMessage(message);

    }


    public void stopMixer() {
        if (mMuxerTread == null || mHandler == null) {
            return;
        }
        Message.obtain(mHandler, MSG_STOP_MUXER).sendToTarget();
    }


    public void onVideoFrame(VideoFrame videoFrame) {
        if (mMuxerTread == null || mHandler == null) {
            return;
        }
        Message.obtain(mHandler, MSG_SEND_VIDEO, new Object[] {videoFrame.data, videoFrame.width,
                videoFrame.height, videoFrame.frameType}).sendToTarget();
    }

    public void onAudioFrame(AudioFrame audioFrame) {
        if (mMuxerTread == null || mHandler == null) {
            return;
        }
        Message.obtain(mHandler, MSG_SEND_AUDIO, new Object[] {audioFrame.bytes, audioFrame.channels,
                audioFrame.samplesPerSec}).sendToTarget();
    }

    private void release() {
        if (mMuxerTread != null) {
            mMuxerTread.quitSafely();
            mMuxerTread = null;
        }
        if (mHandler != null) {
            mHandler.removeCallbacksAndMessages(null);
        }
    }


    private static class MixerHandler extends Handler {

        private WeakReference<MediaAVMixer> mWrapperMuxer;

        private int mAudioTrackIndex;
        private int mVideoTrackIndex;
        private boolean mMuxerStarted;

        private MediaMuxer mMuxer;

        private long muxerStartTime = 0;

        private ByteBuffer mVideoBuffer;


        public MixerHandler(MediaAVMixer mixer, Looper looper) {
            super(looper);
            mWrapperMuxer = new WeakReference<>(mixer);
        }

        @Override
        public void handleMessage(@NonNull Message msg) {
            Object[] objects;
            switch (msg.what) {
                case MSG_START_MUXER:
                    realStartMuxer((String) msg.obj);
                    break;
                case MSG_STOP_MUXER:
                    realStopMuxer();
                    break;
                case MSG_SEND_VIDEO:
                    objects = (Object[]) msg.obj;
                    handleVideoFrame((byte[]) objects[0], (int)objects[1], (int)objects[2], (int)objects[3]);
                    break;
                case MSG_SEND_AUDIO:
                    objects = (Object[]) msg.obj;
                    handleAudioFrame((byte[]) objects[0], (int)objects[1], (int)objects[2]);

                    break;
            }
        }

        private void handleAudioFrame(byte[] data, int channel, int sampleRate) {
            // todo 添加音频PCM的编码以及写入，同时，这里要考虑音频做180ms的延时
            // todo start aac encode
            // todo encode aac
            // todo mAudioTrackIndex = mMuxer.addTrack(audioformat);
            //                if (mAudioTrackIndex != -1) {
            //                    mMuxer.start();
            //                    mMuxerStarted = true;
            //                    muxerStartTime = System.nanoTime() / 1000;
            //                }
//            todo if (mMuxerStarted && mAudioTrackIndex != -1) {
//                mMuxer.writeSampleData(mAudioTrackIndex, mVideoBuffer, info);
//            }


        }

        private void handleVideoFrame(byte[] data, int width, int height, int frameType) {
            if (mMuxer == null) {
                return;
            }
            if (mVideoTrackIndex == -1) {
                if (frameType != 3) {
                    return;
                }


                MediaFormat format = MediaFormat.createVideoFormat("video/avc", width, height);
                format.setInteger(MediaFormat.KEY_COLOR_TRANSFER, 3);
                format.setInteger(MediaFormat.KEY_BIT_RATE, 12000);
                format.setInteger(MediaFormat.KEY_FRAME_RATE, 30);
                format.setInteger(MediaFormat.KEY_COLOR_STANDARD, 2);
                format.setInteger(MediaFormat.KEY_COLOR_RANGE, 2);

                byte[] ppsBytes = VideoHelper.getPPS(data);
                if (ppsBytes == null) {
                    return;
                }
                ByteBuffer ppsBuffer = ByteBuffer.allocateDirect(ppsBytes.length);
                ppsBuffer.put(ppsBytes);
                ppsBuffer.position(0);
                format.setByteBuffer("csd-1", ppsBuffer);


                byte[] spsBytes = VideoHelper.getSPS(data);
                if (spsBytes == null) {
                    return;
                }
                ByteBuffer spsBuffer = ByteBuffer.allocateDirect(spsBytes.length);
                spsBuffer.put(spsBytes);
                spsBuffer.position(0);
                format.setByteBuffer("csd-0", spsBuffer);

                mVideoTrackIndex = mMuxer.addTrack(format);
                if (mVideoTrackIndex != -1) { // && mAudioTrackIndex != -1 todo 添加音频的判断
                    mMuxer.start();
                    mMuxerStarted = true;
                    muxerStartTime = System.nanoTime() / 1000;
                }

            }
            if (mMuxerStarted && mVideoTrackIndex != -1) {
                long currentTime = System.nanoTime() / 1000;
                MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
                if (frameType == 3) {
                    info.flags = MediaCodec.BUFFER_FLAG_KEY_FRAME;
                }
                info.offset = 0;
                info.size = data.length;
                info.presentationTimeUs = currentTime - muxerStartTime;
                if (mVideoBuffer == null) {
                    mVideoBuffer = ByteBuffer.allocateDirect(data.length);
                }
                if (mVideoBuffer.capacity() != data.length) {
                    mVideoBuffer = ByteBuffer.allocateDirect(data.length);
                }
                mVideoBuffer.put(data, 0, data.length);
                mVideoBuffer.position(0);
                mMuxer.writeSampleData(mVideoTrackIndex, mVideoBuffer, info);
            }
        }

        private void realStopMuxer() {
            mVideoTrackIndex = -1;
            mAudioTrackIndex = -1;
            if (mMuxer != null) {
                if (mMuxerStarted) {
                    mMuxer.stop();
                }
                mMuxer.release();
                mMuxer = null;
            }
            Log.d(TAG, "FileSize = " + new File(moutPath).length());
            mMuxerStarted = false;
            if (mWrapperMuxer != null && mWrapperMuxer.get() != null){
                mWrapperMuxer.get().release();
            }
        }


        private String moutPath = null;
        private void realStartMuxer(String outPath) {
            try {
                moutPath = outPath;
                mMuxer = new MediaMuxer(outPath, MediaMuxer.OutputFormat.MUXER_OUTPUT_MPEG_4);
            } catch (IOException ioe) {
                throw new RuntimeException("MediaMuxer creation failed", ioe);
            }

            mVideoTrackIndex = -1;
            mAudioTrackIndex = -1;
            mMuxerStarted = false;
        }
    }

}

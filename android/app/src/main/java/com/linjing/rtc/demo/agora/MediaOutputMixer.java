package com.linjing.rtc.demo.agora;

import android.media.MediaCodec;
import android.media.MediaFormat;
import android.media.MediaMuxer;
import android.os.Environment;
import android.util.Log;

import com.linjing.rtc.base.IRtcEngine;
import com.linjing.sdk.api.audio.AudioFrame;
import com.linjing.sdk.api.video.VideoFrame;

import java.io.File;
import java.io.IOException;
import java.nio.ByteBuffer;

public class MediaOutputMixer {

    public static final String TAG = "MediaOutputMixer";

    private static final File OUTPUT_DIR = Environment.getExternalStorageDirectory();

    private MediaMuxer mMuxer;
    private int mAudioTrackIndex;
    private int mVideoTrackIndex;
    private boolean mMuxerStarted;

    private IRtcEngine mRtcEngine;

    public MediaOutputMixer(IRtcEngine rtcEngine) {
        mRtcEngine = rtcEngine;
    }

    public void startMixer() {
        File sd = Environment.getExternalStorageDirectory();
        if (sd == null || !sd.canWrite()) {
            Log.e(TAG, "sd can not write.");
            return;
        }

        // Output filename.  Ideally this would use Context.getFilesDir() rather than a
        // hard-coded output directory.
        String outputPath = new File(OUTPUT_DIR, "test"  +System.currentTimeMillis() + ".mp4").toString();
        Log.i(TAG, "Output file is " + outputPath);

        // Create a MediaMuxer.  We can't add the video track and start() the muxer here,
        // because our MediaFormat doesn't have the Magic Goodies.  These can only be
        // obtained from the encoder after it has started processing data.
        //
        // We're not actually interested in multiplexing audio.  We just want to convert
        // the raw H.264 elementary stream we get from MediaCodec into a .mp4 file.
        try {
            mMuxer = new MediaMuxer(outputPath, MediaMuxer.OutputFormat.MUXER_OUTPUT_MPEG_4);
        } catch (IOException ioe) {
            throw new RuntimeException("MediaMuxer creation failed", ioe);
        }

        mVideoTrackIndex = -1;
        mAudioTrackIndex = -1;
        mMuxerStarted = false;
    }

    private boolean requestStopMix = false;
    public void stopMixer() {
        requestStopMix = true;

    }

    public void realStopMixer() {
        mVideoTrackIndex = -1;
        mAudioTrackIndex = -1;
        requestStopMix= false;
        mMuxerStarted = false;
        if (mMuxer != null) {
            mMuxer.stop();
            mMuxer.release();
            mMuxer = null;
        }
    }

    private long time = 0;
    private ByteBuffer mBuffer;
    public void onVideoFrame(VideoFrame frame) {
        if (mMuxer == null) {
            return;
        }
        if (mVideoTrackIndex == -1) {
            if (frame.frameType != 7 && frame.frameType != 4) {
                return;
            }
            MediaFormat format = MediaFormat.createVideoFormat("video/avc", frame.width, frame.height);
            format.setInteger(MediaFormat.KEY_COLOR_TRANSFER, 3);
            format.setInteger(MediaFormat.KEY_BIT_RATE, 12000);
            format.setInteger(MediaFormat.KEY_FRAME_RATE, 30);
            format.setInteger(MediaFormat.KEY_COLOR_STANDARD, 2);
            format.setInteger(MediaFormat.KEY_COLOR_RANGE, 2);
            ByteBuffer buffer = ByteBuffer.allocateDirect(9);
            byte[] bytes = {0, 0, 0, 1, 104, -18, 6, -30, -64};
            buffer.put(bytes);
            buffer.position(0);
            format.setByteBuffer("csd-1", buffer);


            ByteBuffer buffer1 = ByteBuffer.allocateDirect(22);
            byte[] bytes1 = {0, 0, 0, 1, 103, 100, 0, 31, -84, -76, 5, -96, 80, -46, -112, 80, 96, 96, 109, 10, 19, 80};
            buffer1.put(bytes1);
            buffer1.position(0);
            format.setByteBuffer("csd-0", buffer1);

            mVideoTrackIndex = mMuxer.addTrack(format);
            mMuxer.start();
            mMuxerStarted = true;
            time = System.nanoTime() / 1000;
        }
        if (mMuxerStarted && mVideoTrackIndex != -1) {
            long currentTime = System.nanoTime() / 1000;
            MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
            if (frame.frameType != 4) {
                info.flags = MediaCodec.BUFFER_FLAG_KEY_FRAME;
            }
            if (requestStopMix) {
                info.flags = MediaCodec.BUFFER_FLAG_END_OF_STREAM;
            }
            info.offset = 0;
            info.size = frame.data.length;
            info.presentationTimeUs = currentTime - time;
            if (mBuffer == null) {
                mBuffer = ByteBuffer.allocateDirect(frame.data.length);
            }
            if (mBuffer.capacity() != frame.data.length) {
                mBuffer = ByteBuffer.allocateDirect(frame.data.length);
            }
            mBuffer.put(frame.data, 0, frame.data.length);
            mBuffer.position(0);
            mMuxer.writeSampleData(mVideoTrackIndex, mBuffer, info);
            if (requestStopMix) {
                realStopMixer();
            }
        }
    }

    public void onAudioFrame(AudioFrame frame) {

    }

    public void destroy() {
        startMixer();
    }

}

using LJ.RTC.Common;
using LJ.RTC.Video;
using System;
using UnityEngine.UI;

namespace LJ.RTC
{
    public abstract class IRtcEngine : ILifecylce
    {
        private static IRtcEngineEx sInstance;

        private static object mLock = new();

        public static IRtcEngine CreateRtcEngine(RtcEngineConfig config) {
            if (sInstance == null)
            {
                lock (mLock) {
                    if (sInstance == null)
                    {
                        sInstance = new LJRtcEngine(config);
                    }
                }
            }
            return sInstance;
        }

        public static IRtcEngineEx CreateRtcEngineEx(RtcEngineConfig config)
        {
            if (sInstance == null)
            {
                lock (mLock)
                {
                    if (sInstance == null)
                    {
                        sInstance = new LJRtcEngine(config);
                    }
                }
            }
            return sInstance;
        }

        public static void DoReport(string key, string info) {
            if (sInstance == null) {
                return;
            }
            sInstance.Report(key, info);
        }

        public static string LJRtcVersion = "0.0.2-beta";
        public static IRtcEngine Get()
        {
            return sInstance;
        }

        public static IRtcEngineEx GetEx()
        {
            return sInstance;
        }

        protected abstract int Report(string key, string info);

        public abstract void OnCreate();
        public virtual void OnDestroy() {
            sInstance = null;
        }
        public abstract int EnableVideo(bool enable);
        public abstract void EnableAudio(bool enable);
        public abstract int StartPreview();
        public abstract int StopPreview();

        public abstract void SetUseNativeCamera(bool enable);

        public abstract int SetAudioProfile(int profile, int scenario);

        public abstract int AdjustMicVolume(int volume);

        public abstract int AdjustSubMixVolume(int volume);

        public abstract int SetVideoEncoderConfiguration(VideoEncoderConfiguration config);

        public abstract int SetRender(RawImage rawInage, IRtcEngineEventHandler.OnCameraParam callback);

        public abstract int SetRemoteRender(RawImage rawInage);

        public abstract void ReadCameraPixel(bool encode);

        public abstract string[] GetCameraDeviceNames();

        public abstract int StartCameraDevice(string cameraDeviceName);

        public abstract int SetChannelProfile(CHANNEL_PROFILE_TYPE profile);

        public abstract int SetClientRole(CLIENT_ROLE_TYPE role);
        
        public abstract int JoinChannel(ChannelConfig channelConfig);

        public abstract int LeaveChannel();

        public abstract void onRecvMessage(String cmdStr, String jsonStr);

        public abstract void registerMsgHandler(IMessageHandler handler);

        public abstract void RegisterCaptureFrame(OnCaptureVideoFrame capatureVideoFrame);

        public abstract void InitEventHandler(IRtcEngineEventHandler handler);

        public abstract int EnableSubMix(bool enable);

        public abstract IVideoDeviceManager GetVideoDeviceManager();
        public abstract IAudioDeviceManager GetAudioDeviceManager();

        public abstract int SaveEncodedVideo(bool enable);

        public abstract void RegisterDecodedFrame(OnDecodedVideoFrame videoFrame);

        public abstract void RegisterEncodeVideoFrame(OnEncodedVideoFrame videoFrame);

        public abstract void RegisterScreenCaptureVideoFrame(OnCaptureVideoFrame videoFrame);

        public abstract int StartScreenCapture(ScreenCaptureParam param);

        public abstract int StopScreenCapture();
        /// <summary>
        /// 当开了内录后，回调的是麦克风和内录的混音
        /// </summary>
        /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
        /// <returns>true 拦截当前采集流程，不进行编码推流，需要额外调用PushAudioFrame进行编码推流， false 不拦截当前流程</returns>
        public abstract void RegisterCaptureAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type);

        public abstract void RegisterDecodedAudioFrame(OnDecodedAudioFrame audioFrame);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
        /// <returns>true 当前麦克风声音将被拦截，麦克风声音将静音，false 不拦截当前流程</returns>
        public abstract void RegisterMicAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFrame">音频数据,当mode是对原始音频数据修改，例如变声，请把最后的修改结果重新赋值到AudioFrame的buffer中</param>
        /// <returns>true 拦截当前内录流程，内录音频将不会被编码推流到远端， false 不拦截当前流程</returns>
        public abstract void RegisterSubMixAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type);

        public abstract int saveRecordCallbackAudio(bool enabled);
        public abstract void UnRegisterCallbackFrame(CallbackType type);
        /// <summary>
        ///    interval 指定音量提示的时间间隔：
        ///   ≤ 0：禁用音量提示功能。
        ///   > 0：返回音量提示的间隔，单位为毫秒。建议设置到大于 200 毫秒。最小不得少于 10 毫秒，否则会收不到 onAudioVolumeIndication 回调。
        ///   smooth 平滑系数，指定音量提示的灵敏度。取值范围为[0, 10]，建议值为 3，数字越大，波动越灵敏；数字越小，波动越平滑。
        ///   report_vad 是否开启人声检测
        ///   true: 开启本地人声检测功能。开启后，onAudioVolumeIndication 回调的 vad 参数会报告是否在本地检测到人声。
        ///   false: （默认）关闭本地人声检测功能。除引擎自动进行本地人声检测的场景外，onAudioVolumeIndication 回调的 vad 参数不会报告是否在本地检测到人声。
        ///
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="smooth"></param>
        /// <param name="report_vad"></param>
        /// <returns></returns>
        public abstract int enableAudioVolumeIndication(int interval, int smooth, bool report_vad);

        /// <summary>
        /// 禁止远端音频播放以及解码
        /// </summary>
        /// <param name="muted"></param>
        /// <returns></returns>
        public abstract int muteRemoteAudioStream(bool muted);
        /// <summary>
        /// 禁止本地音频采集以及编码推流
        /// </summary>
        /// <param name="muted"></param>
        /// <returns></returns>
        public abstract int muteLocalAudioStream(bool muted);

        ///d
        /// <summary>
        /// Starts playing the music file.
        /// This method mixes the specified local or online audio file with the audio from the microphone, or replaces the microphone's audio with the specified local or remote audio file. A successful method call triggers the OnAudioMixingStateChanged (AUDIO_MIXING_STATE_PLAYING) callback. When the audio mixing file playback finishes, the SDK triggers the OnAudioMixingStateChanged(AUDIO_MIXING_STATE_STOPPED) callback on the local client.On Android, there are following considerations:To use this method, ensure that the Android device is v4.2 or later, and the API version is v16 or later.If you need to play an online music file, Agora does not recommend using the redirected URL address. Some Android devices may fail to open a redirected URL address.If you call this method on an emulator, ensure that the music file is in the /sdcard/ directory and the format is MP3.For the audio file formats supported by this method, see What formats of audio files the Agora RTC SDK support.You can call this method either before or after joining a channel. If you need to call StartAudioMixing [2/2] multiple times, ensure that the time interval between calling this method is more than 500 ms.If the local music file does not exist, the SDK does not support the file format, or the the SDK cannot access the music file URL, the SDK reports the warn code 701.
        /// </summary>
        ///
        /// <param name="filePath"> File path:Android: The file path, which needs to be accurate to the file name and suffix. Agora supports using a URI address, an absolute path, or a path that starts with /assets/. You might encounter permission issues if you use an absolute path to access a local file, so Agora recommends using a URI address instead. For example: content://com.android.providers.media.documents/document/audio%3A14441.Windows: The absolute path or URL address (including the suffixes of the filename) of the audio effect file. For example: C:\music\audio.mp4.iOS or macOS: The absolute path or URL address (including the suffixes of the filename) of the audio effect file. For example: /var/mobile/Containers/Data/audio.mp4.</param>
        ///
        /// <param name="loopback"> Whether to only play music files on the local client:true: Only play music files on the local client so that only the local user can hear the music.false: Publish music files to remote clients so that both the local user and remote users can hear the music.</param>
        ///
        /// <param name="cycle"> The number of times the music file plays.≥ 0: The number of playback times. For example, 0 means that the SDK does not play the music file while 1 means that the SDK plays once.-1: Play the audio file in an infinite loop.</param>
        ///
        /// <param name="startPos"> The playback position (ms) of the music file.</param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int StartAudioMixing(string filePath, bool loopback, int cycle, int startPos);
        ///
        /// <summary>
        /// Stops playing and mixing the music file.
        /// This method stops the audio mixing. Call this method when you are in a channel.
        /// </summary>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int StopAudioMixing();
        ///
        /// <summary>
        /// Pauses playing the music file.
        /// Call this method after joining a channel.
        /// </summary>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int PauseAudioMixing();

        ///
        /// <summary>
        /// Resumes playing and mixing the music file.
        /// This method resumes playing and mixing the music file. Call this method when you are in a channel.
        /// </summary>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int ResumeAudioMixing();
        ///
        /// <summary>
        /// Sets the audio mixing position.
        /// Call this method to set the playback position of the music file to a different starting position, rather than playing the file from the beginning.You need to call this method after calling StartAudioMixing [2/2] and receiving the OnAudioMixingStateChanged(AUDIO_MIXING_STATE_PLAYING) callback.
        /// </summary>
        ///
        /// <param name="pos"> Integer. The playback position (ms).</param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int SetAudioMixingPosition(int pos /*in ms*/);
        /// <summary>
        /// 发送PCM裸流给SDK编码并推流，pcm的长度计算是pcm.length / (sampleRate / 1000)
        /// </summary>
        /// <param name="pcm">PCM数据</param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="bytePerSample">Int16 2 int8 1 int32 4</param>
        /// <returns></returns>
        public abstract int PushAudioFrame(byte[] pcm, int sampleRate, int channelCount, int bytePerSample);

        public abstract int PushVideoCaptureFrame(CaptureVideoFrame videoFrame);
        public abstract bool IsUseNativeCamera();
    }

    public abstract class IRtcEngineEx : IRtcEngine {

        public abstract LJChannel CreateChannel(string channelId);

        public abstract LJChannel CreateChannel(string channelId, long uid);

        internal abstract void ReleaseChannel(string channelId);

        internal abstract void ReleaseChannel(string channelId, long uid);

        internal abstract int SetForMultiChannelUser(LJRtcConnection connection, RawImage imange, long uid, int fps);
        public abstract int removeForMultiChannelUser(LJRtcConnection connection, long uid);

        ///
        /// <summary>
        /// Joins a channel with the connection ID.
        /// You can call this method multiple times to join more than one channel.If you are already in a channel, you cannot rejoin it with the same user ID.If you want to join the same channel from different devices, ensure that the user IDs are different for all devices.Ensure that the app ID you use to generate the token is the same as IRtcEngine the app ID used when creating the instance.
        /// </summary>
        ///
        /// <param name="options"> The channel media options. See ChannelMediaOptions .</param>
        ///
        /// <param name="token"> The token generated on your server for authentication. </param>
        ///
        /// <param name="connection"> The connection information. See RtcConnection .</param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.-2: The parameter is invalid. For example, the token is invalid, the uid parameter is not set to an integer, or the value of a member in the ChannelMediaOptions structure is invalid. You need to pass in a valid parameter and join the channel again.-3: Failes to initialize the IRtcEngine object. You need to reinitialize the IRtcEngine object.-7: The IRtcEngine object has not been initialized. You need to initialize the IRtcEngine object before calling this method.-8: IRtcEngineThe internal state of the object is wrong. The typical cause is that you call this method to join the channel without calling StopEchoTest to stop the test after calling StartEchoTest [2/2] to start a call loop test. You need to call StopEchoTest before calling this method.-17: The request to join the channel is rejected. The typical cause is that the user is in the channel. Agora recommends using the OnConnectionStateChanged callback to get whether the user is in the channel. Do not call this method to join the channel unless you receive the CONNECTION_STATE_DISCONNECTED(1) state.-102: The channel name is invalid. You need to pass in a valid channel name inchannelId to rejoin the channel.-121: The user ID is invalid. You need to pass in a valid user ID in uid to rejoin the channel.
        /// </returns>
        ///
        public abstract int JoinChannelEx(string token, long appid, LJRtcConnection connection, ChannelMediaOptions options);

        ///
        /// <summary>
        /// Leaves a channel.
        /// This method lets the user leave the channel, for example, by hanging up or exiting the call.After calling JoinChannelEx to join the channel, this method must be called to end the call before starting the next call.This method can be called whether or not a call is currently in progress. This method releases all resources related to the session.This method call is asynchronous. When this method returns, it does not necessarily mean that the user has left the channel. After you leave the channel, the SDK triggers the OnLeaveChannel callback.A successful call of this method triggers the following callbacks: The local client: OnLeaveChannel.The remote client: OnUserOffline , if the user joining the channel is in the COMMUNICATION profile, or is a host in the LIVE_BROADCASTING profile.If you call Dispose immediately after calling this method, the SDK does not trigger the OnLeaveChannel callback.Calling LeaveChannel [1/2] will leave the channels when calling JoinChannel [2/2] and JoinChannelEx at the same time.
        /// </summary>
        ///
        /// <param name="connection"> The connection information. See RtcConnection .</param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int LeaveChannelEx(LJRtcConnection connection);

        ///
        /// <summary>
        /// Stops or resumes publishing the local audio stream.
        /// This method does not affect any ongoing audio recording, because it does not disable the audio capture device.
        /// </summary>
        ///
        /// <param name="connection"> The connection information. See RtcConnection .</param>
        ///
        /// <param name="mute"> Whether to stop publishing the local audio stream:true: Stops publishing the local audio stream.false: (Default) Resumes publishing the local audio stream.</param>
        ///
        /// <returns>
        /// 0: Success. &lt; 0: Failure.
        /// </returns>
        ///
        public abstract int MuteLocalAudioStreamEx(bool mute, LJRtcConnection connection);

        ///
        /// <summary>
        /// Stops or resumes publishing the local video stream.
        /// A successful call of this method triggers the OnUserMuteVideo callback on the remote client.This method does not affect any ongoing video recording, because it does not disable the camera.
        /// </summary>
        ///
        /// <param name="connection"> The connection information. See RtcConnection .</param>
        ///
        /// <param name="mute"> Whether to stop publishing the local video stream.true: Stop publishing the local video stream.false: (Default) Publish the local video stream.</param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int MuteLocalVideoStreamEx(bool mute, LJRtcConnection connection);

        ///
        /// <summary>
        /// Stops or resumes subscribing to the audio streams of all remote users.
        /// After successfully calling this method, the local user stops or resumes subscribing to the audio streams of all remote users, including the ones who join the channel subsequent to this call.Call this method after joining a channel.If you do not want to subscribe the audio streams of remote users before joining a channel, you can set autoSubscribeAudio as false when calling JoinChannel [2/2] .
        /// </summary>
        ///
        /// <param name="connection"> The connection information. See RtcConnection .</param>
        ///
        /// <param name="mute"> Whether to stop subscribing to the audio streams of all remote users:true: Stops subscribing to the audio streams of all remote users.false: (Default) Subscribes to the audio streams of all remote users.</param>
        ///
        /// <returns>
        /// 0: Success. &lt; 0: Failure.
        /// </returns>
        ///
        public abstract int MuteAllRemoteAudioStreamsEx(bool mute, LJRtcConnection connection);

        ///
        /// <summary>
        /// Stops or resumes subscribing to the video streams of all remote users.
        /// After successfully calling this method, the local user stops or resumes subscribing to the audio streams of all remote users, including all subsequent users.
        /// </summary>
        ///
        /// <param name="connection"> The connection information. See RtcConnection .</param>
        ///
        /// <param name="mute"> Whether to stop subscribing to the video streams of all remote users.
        ///  true: Stop subscribing to the video streams of all remote users.
        ///  false: (Default) Subscribe to the audio streams of all remote users by default. </param>
        ///
        /// <returns>
        /// 0: Success.&lt; 0: Failure.
        /// </returns>
        ///
        public abstract int MuteAllRemoteVideoStreamsEx(bool mute, LJRtcConnection connection);

        public abstract int PushAudioFrameEx(LJRtcConnection connection, byte[] pcm, int sampleRate, int channelCount, int bytePerSample);

        public abstract int PushVideoCaptureFrameEx(LJRtcConnection connection, CaptureVideoFrame videoFrame);

        public abstract int SubscriberAudioStream(LJRtcConnection connection, long subscriberUid);

        public abstract int UnsubscriberAudioStream(LJRtcConnection connection, long subscriberUid);

        public abstract int SubscriberVideoStream(LJRtcConnection connection, long subscriberUid);

        public abstract int UnsubscriberVideoStream(LJRtcConnection connection, long subscriberUid);
    }
}

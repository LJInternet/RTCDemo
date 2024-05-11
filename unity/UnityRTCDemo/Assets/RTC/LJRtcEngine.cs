using LJ.RTC.Audio;
using LJ.RTC.Common;
using LJ.RTC.Utils;
using LJ.RTC.Video;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace LJ.RTC
{
    internal class LJRtcEngine : IRtcEngineEx, IRtcEngineApi
    {
        private VideoManager mVideoManager;
        private RemoteVideoManager mRemoteVideoManager;
        private VideoEncoderConfiguration mEncoderConfiguration = new();

        private object mLock = new();

        private object mVideoLock = new();
        private object mComponentLock = new();

        private RtcEngineConfig mRtcConfig;

        private RtcEngineNavite mRtcEngineNavite;

        private IMessageHandler mIMessageHandler;

        private RTCStatus mRTCStatus = new RTCStatus();

        private int workingMode = (int)CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER;

        private IRtcEngineEventHandler mEventHandler;

        private List<BaseRtcEngineModule> mComponentList = new List<BaseRtcEngineModule>();

        private UploadConfig mUploadConfig = new UploadConfig();

        private IVideoDeviceManager mIVideoDeviceManager;

        private event OnCaptureVideoFrame mVideoFrameEvent;

        private AudioManger mAudioManager;

        private IAudioDeviceManager mIAudioDeviceManager;

        private OnDecodedVideoFrame mOnDecodedVideoFrame;
        private OnDecodedVideoFrame mLocalOnDecodedVideoFrame;
        private VideoFrame mVideoFrame;

        private OnEncodedVideoFrame mOnEncodedVideoFrame;
        private VideoFrame mEncodedVideoFrame;

        private OnCaptureVideoFrame mOnScreenVideoFrameCallback;
        private CaptureVideoFrame mScreenVideoFrame;

        private OnCaptureAudioFrame mOnCaptureAudioFrame;
        private AudioFrame mCaptureAudioFrame;

        private OnCaptureAudioFrame mOnMicAudioFrame;
        private AudioFrame mMicAudioFrame;

        private OnCaptureAudioFrame mOnSubMixAudioFrame;
        private AudioFrame mSubMixAudioFrame;

        private OnDecodedAudioFrame mOnDecodedudioFrame;
        private AudioFrame mDecodedAudioFrame;

        private MultiStreamManager mMultiStreamManager;

#if UNITY_IOS && UNITY_EDITOR
        private bool isUserNativeCamera = false;
#elif UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        private bool isUserNativeCamera = true;
#else
        private bool isUserNativeCamera = false;
#endif
        public LJRtcEngine(RtcEngineConfig config)
        {
            JLog.Init(config.mJLog);
            mRtcConfig = config;
            OnCreate();
        }

        public static void SetDebugEnv(bool debug)
        {
            RtcEngineNavite.SetDebugEnv(debug);
        }

        public override int EnableVideo(bool enable) {
            lock(mVideoLock)
            {
                if (enable)
                {
                    if (mVideoManager == null)
                    {
                        mVideoManager = new VideoManager(this, mRtcEngineNavite);
                        lock (mComponentList)
                        {
                            mComponentList.Add(mVideoManager);
                        }
                        VideoConfig config = VideoConfigHelper.CreateVideoConfig(mEncoderConfiguration);
                        mVideoManager.Start(config);
                    }
                }
                else
                {
                    DestroyVideoManger();
                }
            }
            return 0;
        }

        public override int SetVideoEncoderConfiguration(VideoEncoderConfiguration config)
        {
            mEncoderConfiguration = config;
            VideoConfig videoConfig = VideoConfigHelper.CreateVideoConfig(mEncoderConfiguration);
            UpdateUploadVideoConfig(videoConfig);
            return 0;
        }

        private void UpdateUploadVideoConfig(VideoConfig videoConfig)
        {
            VideoUploadConfig videoUploadConfig = new VideoUploadConfig();
            videoUploadConfig.encodeWidth = videoConfig.encodeWidth;
            videoUploadConfig.encodeHeight = videoConfig.encodeHeight;
            videoUploadConfig.realVideoBitrateInbps = videoConfig.encodeConfig.bitRate * 1000;
            videoUploadConfig.minVideoBitrateInbps = videoConfig.encodeConfig.minBitRate * 1000;
            videoUploadConfig.maxVideoBitrateInbps = videoConfig.encodeConfig.maxBitRate * 1000;
            videoUploadConfig.codecType = videoConfig.encodeConfig.codecType;
            videoUploadConfig.fps = videoConfig.frameRate;
            videoUploadConfig.keyFrameInterval = videoConfig.encodeConfig.keyFrameInterval;
            videoUploadConfig.bitrateMode = videoConfig.encodeConfig.bitrateMode;
            videoUploadConfig.mirror = videoConfig.mirrorMode == VIDEO_MIRROR_MODE_TYPE.VIDEO_MIRROR_MODE_ENABLED
                ? (int)MIRROR_TYPE.MIRROR_Y : (int)MIRROR_TYPE.MIRROR_NONE;
            mUploadConfig.videoUploadConfig = videoUploadConfig;
            lock (mVideoLock)
            {
                if (mVideoManager != null)
                {
                    mVideoManager.UpdateVideoConfig(videoConfig);
                }
            }
            UpdateUploadConfig(mUploadConfig);
        }

        public override void OnCreate()
        {

            mRtcEngineNavite = new RtcEngineNavite(mRtcConfig);
            mRtcEngineNavite.CreateRtcEngine();
            mRtcEngineNavite.RegisterEventListener(OnNativeEventCallback, this);

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
            InitRtcEngineObject();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.linjing.sdk.encode.UnityMediaInfoHelper");
                string mediaInfo = androidJavaClass.CallStatic<string>("getH264MediaInfo");
                Debug.Log("MediaCodecInfo " + mediaInfo);
            }
            catch (Exception e) {
            Debug.LogError("MediaCodecInfo " + e.ToString());
            }
#endif
            VideoDeviceManagerFactory.CreateDeviceManager(isUserNativeCamera);
            mIVideoDeviceManager = VideoDeviceManagerFactory.GetDeviceManager();
            mIVideoDeviceManager.Init();
            mAudioManager = new AudioManger(this);
            mMultiStreamManager = new MultiStreamManager(this);
            lock (mComponentList)
            {
                mComponentList.Add(mAudioManager);
                mComponentList.Add(mMultiStreamManager);
            }
            mIAudioDeviceManager = AudioDeviceManager.Instance(this);
        }

        public override void OnDestroy()
        {
            DestroyAudioManager();
            DestroyVideoManger();
            if (mMultiStreamManager != null) {
                mMultiStreamManager.OnDestroy();
                mMultiStreamManager = null;
            }
            lock (mComponentList)
            {
                mComponentList.Clear();
            }
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.DestroyRtcEngine();
                mRtcEngineNavite = null;
            }

            mIVideoDeviceManager = null;
            if (mIAudioDeviceManager != null) {
                mIAudioDeviceManager.Clear();
                mIAudioDeviceManager = null;
            }
            base.OnDestroy();
        }

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID
        private GameObject mRtcEngineObject;
        private void InitRtcEngineObject()
        {
            mRtcEngineObject = GameObject.Find("RtcEngineObj");
            if (mRtcEngineObject == null)
            {
                mRtcEngineObject = new GameObject("RtcEngineObj");
                UnityEngine.Object.DontDestroyOnLoad(mRtcEngineObject);
                mRtcEngineObject.hideFlags = HideFlags.HideInHierarchy;
                mRtcEngineObject.AddComponent<RtcEngineGameObject>();
            }
        }
#endif
        private void DestroyVideoManger()
        {
            if (mVideoManager != null)
            {
                mVideoManager.Stop();
                mVideoManager.OnDestroy();
                mVideoManager = null;
            }
        }

        private void DestroyAudioManager() {
            if (mAudioManager != null) {
                mAudioManager.OnDestroy();
                mAudioManager = null;
            }
        }

        public override int StartPreview()
        {
            lock (mVideoLock)
            {
                if (mVideoManager == null)
                {
                    JLog.Error("StartPreview mVideoManager == null");
                    return -1;
                }
                CAMERA_CAPTURE_ERROR errorCode = mVideoManager.StartPreview();
                if (errorCode != CAMERA_CAPTURE_ERROR.SUCCESS)
                {
                    return (int)errorCode;
                }
            }
            return 0;
        }

        public override int StopPreview()
        {
            lock (mVideoLock)
            {
                if (mVideoManager == null)
                {
                    JLog.Error("StopPreview mVideoManager == null");
                    return -1;
                }
                mVideoManager.StopPreview();
            }
            return 0;
        }

        public override int SetRender(RawImage rawInage, IRtcEngineEventHandler.OnCameraParam callback)
        {
            lock (mVideoLock)
            {
                if (mVideoManager == null)
                {
                    JLog.Error("SetRender mVideoManager == null");
                    return -1;
                }
                mVideoManager.SetRender(rawInage, callback);
            }
            return 0;
        }

        public override void ReadCameraPixel(bool encode)
        {
            lock (mVideoLock)
            {
                if (mVideoManager != null)
                {
                    mVideoManager.ReadCameraPixel(encode);
                }
            }
        }

        public override string[] GetCameraDeviceNames()
        {
            return mIVideoDeviceManager.GetCameraDeviceNames();
        }

        public override int StartCameraDevice(string cameraDeviceName) {
            lock (mVideoLock)
            {
                if (mVideoManager != null)
                {
                   return (int)mVideoManager.StartCameraDevice(cameraDeviceName);
                }
            }
            return -1;
        }

        public override int SetChannelProfile(CHANNEL_PROFILE_TYPE profile)
        {
            //if (mRtcEngineNavite != null)
            //{
                //TransferMsg msg = new TransferMsg((int)profile);
                //mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.WEB_MSG_EVENT, msg.HPmarshall());
           // }
            return 0;
        }

        public override int SetClientRole(CLIENT_ROLE_TYPE role)
        {
            workingMode = (int)role;
            //if (mRtcEngineNavite != null)
           // {
            //    workingMode = (int)role;
                //TransferMsg msg = new TransferMsg((int)role);
                //mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.SET_CLIENT_ROLE, msg.HPmarshall());
            //    return 0;
            //}
            return 0;
        }

        public override int JoinChannel(ChannelConfig channelConfig)
        {
            if (channelConfig.channelID == "") {
                throw new Exception("channeID can not be empty when join channel");
            }

            TransferConfig transferConfig = new TransferConfig();
            transferConfig.configs = channelConfig.configs;
            transferConfig.transferMode = workingMode;
            transferConfig.p2pSignalServer = channelConfig.p2pSignalServer;
            transferConfig.appID = channelConfig.appID;
            transferConfig.userID = channelConfig.userID;
            transferConfig.channelID = channelConfig.channelID;
            transferConfig.token = channelConfig.token;
            

            mUploadConfig.transferConfig = transferConfig;

            mUploadConfig.enableAudio = channelConfig.enableAudio;
            mUploadConfig.enableVideo = channelConfig.enableVideo;

            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.JOIN_CHANNEL, mUploadConfig.HPmarshall());
                return 0;
            }
            return -1;
        }

        public override int LeaveChannel()
        {
            if (mRtcEngineNavite != null)
            {
                TransferMsg msg = new TransferMsg(0);
                mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.LEAVE_CHANNEL, msg.HPmarshall());
            }
            return 0;
        }

        public override void onRecvMessage(string cmdStr, string jsonStr)
        {
            if (mRtcEngineNavite != null)
            {
                WebSocketMessage socketMessage = new WebSocketMessage(cmdStr, jsonStr);
                mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.WEB_MSG_EVENT, socketMessage.HPmarshall());
            }
        }

        public override void registerMsgHandler(IMessageHandler handler)
        {
            mIMessageHandler = handler;
        }

        void IRtcEngineApi.OnCaptureVideoFrame(CaptureVideoFrame videoFrame)
        {
            if (mVideoFrameEvent != null)
            {
                mVideoFrameEvent.Invoke(videoFrame);
            }
        }

        public override int PushVideoCaptureFrame(CaptureVideoFrame videoFrame) {
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.PushVideoCaptureFrame(videoFrame);
            }
            return 0;
        }

    private void UpdateUploadConfig(UploadConfig uploadConfig)
        {
            mUploadConfig = uploadConfig;
            if (mRtcEngineNavite == null)
            {
                return;
            }
            mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.UPDATE_UPLOAD_CONFIG, mUploadConfig.HPmarshall());
        }

        public void OnNativeEventCallback(int type, byte[] buf, System.Object context)
        {
            switch (type)
            {
                case (int)UDPMsgType.SEND_MSG:
                    if (mIMessageHandler != null)
                    {
                        string msg = TransferHelper.getSendMsgObjStr(buf);
                        JLog.Debug("MediaSdk", "SEND_MSG:" + msg);
                        mIMessageHandler.onSendMessage(msg);
                    }
                    break;
                case (int)UDPMsgType.UDP_LINK_OK:
                    mRTCStatus.isRUDPConnected = true;
                    lock (mComponentList)
                    {
                        foreach (var component in mComponentList)
                        {
                            component.onTansConnectStateChange(true);
                        }
                    }
                    break;
                case (int)UDPMsgType.NET_QUALITY:
                    NetQuality netQuality = new NetQuality();
                    netQuality.unmarshall(buf);
                    if (mEventHandler != null)
                    {
                        mEventHandler.onNetworkQuality(0, netQuality.mLocalQuality, netQuality.mRemoteQuality);
                    }
                    break;
                case (int)UDPMsgType.CB_LINK_STATUS:
                    if (mEventHandler != null)
                    {
                        LinkStatusEvent linkStatusEvent = new LinkStatusEvent();
                        linkStatusEvent.unmarshall(buf);

                        mEventHandler.onLinkStatus(linkStatusEvent.status);
                    }
                    break;
                case (int)UDPMsgType.VIDEO_FRAME_RATE_CONTROL:
                    VideoFrameRateControl control = new VideoFrameRateControl();
                    control.unmarshall(buf);
                    if (mVideoManager != null) {
                        mVideoManager.onFrameRateControl(control);
                    }
                    break;
                case (int)UDPMsgType.CB_AUDIO_CAPTURE_VOLUME:
                    AudioVolumeEvent volumeEvent = new AudioVolumeEvent();
                    volumeEvent.unmarshall(buf);
                    if (mEventHandler != null)
                    {
                        mEventHandler.onAudioVolumeIndication(volumeEvent);
                    }
                    break;
                case (int)UDPMsgType.CB_JOIN_CHANNEL:
                    MultiChannelEventResult joinResult = new MultiChannelEventResult();
                    joinResult.unmarshall(buf);
                    if (mEventHandler != null)
                    {
                        if (joinResult.result == 0)
                        {
                            mEventHandler.onJoinChannelSuccess(joinResult.channenlId, joinResult.uid, joinResult.msg);
                        }
                        else {
                            mEventHandler.onJoinChannelFail(joinResult.channenlId, joinResult.uid, joinResult.msg);
                        }
                       
                    }
                    break;
                case (int)UDPMsgType.CB_LEAVE_CHANNEL:
                    MultiChannelEventResult leaveResult = new MultiChannelEventResult();
                    leaveResult.unmarshall(buf);
                    if (mEventHandler != null)
                    {
                        if (leaveResult.result == 0)
                        {
                            mEventHandler.onLeavehannelSuccess(leaveResult.channenlId, leaveResult.uid, leaveResult.msg);
                        }
                        else
                        {
                            mEventHandler.onLeaveChannelFail(leaveResult.channenlId, leaveResult.uid, leaveResult.msg);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void InitEventHandler(IRtcEngineEventHandler handler)
        {
            mEventHandler = handler;
        }

        public void OnComponentDestroy(BaseRtcEngineModule baseRtcEngineModule)
        {
            lock (mComponentList)
            {
                mComponentList.Remove(baseRtcEngineModule);
            }
        }

        public bool GetRUDPConnectState()
        {
            return mRTCStatus.isRUDPConnected;
        }

        public override IVideoDeviceManager GetVideoDeviceManager()
        {
            return mIVideoDeviceManager;
        }

        public override IAudioDeviceManager GetAudioDeviceManager() {
            return mIAudioDeviceManager;
        }

        public override void RegisterCaptureFrame(OnCaptureVideoFrame capatureVideoFrame)
        {
            mVideoFrameEvent = capatureVideoFrame;
        }

        public override void EnableAudio(bool enable)
        {
            if (mAudioManager != null)
            {
                mAudioManager.EnableAudio(enable);
            }
        }

        public override int SetAudioProfile(int profile, int scenario)
        {
            if (mAudioManager != null) {
                return mAudioManager.SetAudioProfile(profile, scenario);
            }
           
            return -1;
        }

        public override int AdjustMicVolume(int volume)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.AdjustMicVolume(volume);
            }

            return -1;
        }

        public override int AdjustSubMixVolume(int volume)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.AdjustSubMixVolume(volume);
            }

            return -1;
        }

        public int SendMediaEvent(int eventType, byte[] msg, UnityEngine.Object extraObject = null)
        {
            if (mRtcEngineNavite != null)
            {
                return mRtcEngineNavite.SendMediaEvent(eventType, msg, extraObject);
            }
            return 0;
        }

        public byte[] GetMediaEvent(int eventType, byte[] msg)
        {
            if (mRtcEngineNavite != null)
            {
                return mRtcEngineNavite.GetMediaEvent(eventType, msg);
            }
            return null;
        }

        public override int EnableSubMix(bool enable)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.EnableSubMix(enable);
            }
            return 0;
        }

        public override int SaveEncodedVideo(bool enable)
        {
            if (mRtcEngineNavite != null)
            {
                TestSaveVideoFileEvent msg = new TestSaveVideoFileEvent(enable);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.TEST_SAVE_VIDEO_FILE, msg.HPmarshall());
             }
            return 0;
        }

        public override void RegisterDecodedFrame(OnDecodedVideoFrame videoFrame)
        {
            mOnDecodedVideoFrame = videoFrame;
            RegisterNativeDecodeFrame();
        }

        private byte[] mCacheDecodedVideoBytes;
        private void RegisterNativeDecodeFrame() {
            if (mOnDecodedVideoFrame == null && mLocalOnDecodedVideoFrame == null) {
                // todo UnRegisterNativeDecodeFrame
                return;
            }
            mRtcEngineNavite.SubscribeVideoNative((IntPtr buf, int len, Int32 width, Int32 height, int pixel_fmt, System.Object context) => {
                if (mOnDecodedVideoFrame == null && mLocalOnDecodedVideoFrame == null)
                {
                    // todo UnRegisterNativeDecodeFrame
                    return;
                }
                if(mVideoFrame == null)
                {
                    mVideoFrame = new VideoFrame();
                }
                if (mCacheDecodedVideoBytes == null || mCacheDecodedVideoBytes.Length != len) {
                    mCacheDecodedVideoBytes = new byte[len];
                }
                mVideoFrame.format = pixel_fmt;
                Marshal.Copy(buf, mCacheDecodedVideoBytes, 0, len);
                mVideoFrame.format = pixel_fmt;
                mVideoFrame.width = width;
                mVideoFrame.height = height;
                mVideoFrame.data = mCacheDecodedVideoBytes;
                if (mLocalOnDecodedVideoFrame != null)
                {
                    mLocalOnDecodedVideoFrame.Invoke(mVideoFrame);
                }
                if (mOnDecodedVideoFrame != null)
                {
                    mOnDecodedVideoFrame.Invoke(mVideoFrame);
                }
            }, this);
        }

        public override void RegisterEncodeVideoFrame(OnEncodedVideoFrame videoFrame)
        {
            mOnEncodedVideoFrame = videoFrame;
            mRtcEngineNavite.SubscribeEncodeVideoNative((IntPtr buf, int len, Int32 width, Int32 height, int frameType, System.Object context) => {
                if (mOnEncodedVideoFrame != null)
                {
                    if (mEncodedVideoFrame == null)
                    {
                        mEncodedVideoFrame = new VideoFrame();
                    }
                    if (mEncodedVideoFrame.data == null || mEncodedVideoFrame.data.Length != len)
                    {
                        mEncodedVideoFrame.data = new byte[len];
                    }
                    mVideoFrame.format = (int)VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_H264;
                    Marshal.Copy(buf, mEncodedVideoFrame.data, 0, len);
                    mEncodedVideoFrame.width = width;
                    mEncodedVideoFrame.height = height;
                    mEncodedVideoFrame.frameType = frameType;
                    mOnEncodedVideoFrame.Invoke(mVideoFrame);
                }
            }, this);
        }

        public override void RegisterCaptureAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type) {
            mOnCaptureAudioFrame = audioFrame;
            mRtcEngineNavite.SubscribeCaptureAudioCallback((byte[] buf, out byte[] outBuffer, int pts, int simple_rate, int channelCont, System.Object context) => {
                if (mOnCaptureAudioFrame != null)
                {
                    if (mCaptureAudioFrame == null)
                    {
                        mCaptureAudioFrame = new AudioFrame();
                    }
                    mCaptureAudioFrame.pts = pts;
                    mCaptureAudioFrame.sampleRate = simple_rate;
                    mCaptureAudioFrame.channelCount = channelCont;
                    mCaptureAudioFrame.buffer = buf;
                    bool result = mOnCaptureAudioFrame.Invoke(mCaptureAudioFrame);
                    if (type == AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE) {
                        outBuffer = mCaptureAudioFrame.buffer;
                    } else {
                        outBuffer = null;
                    }
                }
                else {
                    outBuffer = null;
                }
                return false;
            }, this, type);
        }

        public override void RegisterMicAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type)
        {
            mOnMicAudioFrame = audioFrame;
            mRtcEngineNavite.SubscribeMicAudioCallback((byte[] buf, out byte[] outBuffer, int pts, int simple_rate, int channelCont, System.Object context) => {
                if (mOnMicAudioFrame != null)
                {
                    if (mMicAudioFrame == null)
                    {
                        mMicAudioFrame = new AudioFrame();
                    }
                    mMicAudioFrame.pts = pts;
                    mMicAudioFrame.sampleRate = simple_rate;
                    mMicAudioFrame.channelCount = channelCont;
                    mMicAudioFrame.buffer = buf;
                    bool result = mOnMicAudioFrame.Invoke(mMicAudioFrame);
                    if (type == AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE) {
                        outBuffer = mMicAudioFrame.buffer;
                    } else {
                        outBuffer = null;
                    }
                }
                else
                {
                    outBuffer = null;
                }
                return false;
            }, this, type);
        }
        public override void RegisterSubMixAudioFrame(OnCaptureAudioFrame audioFrame, AudioFrameOpType type)
        {
            mOnSubMixAudioFrame = audioFrame;
            mRtcEngineNavite.SubscribeSubMixAudioCallback((byte[] buf, out byte[] outBuffer, int pts, int simple_rate, int channelCont, System.Object context) => {
                if (mOnSubMixAudioFrame != null)
                {
                    if (mSubMixAudioFrame == null)
                    {
                        mSubMixAudioFrame = new AudioFrame();
                    }
                    mSubMixAudioFrame.pts = pts;
                    mSubMixAudioFrame.sampleRate = simple_rate;
                    mSubMixAudioFrame.channelCount = channelCont;
                    mSubMixAudioFrame.buffer = buf;
                    bool result = mOnMicAudioFrame.Invoke(mSubMixAudioFrame);
                    if (type == AudioFrameOpType.RAW_AUDIO_FRAME_OP_MODE_READ_WRITE) {
                        outBuffer = mSubMixAudioFrame.buffer;
                    } else {
                        outBuffer = null;
                    }
                }
                else
                {
                    outBuffer = null;
                }
                return false;
            }, this, type);
        }
        public override void UnRegisterCallbackFrame(CallbackType type)
        {
            if (type == CallbackType.AUDIO_MIXED) {
                mOnCaptureAudioFrame = null;
                mCaptureAudioFrame = null;
            }
            else if (type == CallbackType.AUDIO_MIC)
            {
                mOnMicAudioFrame = null;
                mMicAudioFrame = null;
            }
            else if (type == CallbackType.AUDIO_SUBMIX)
            {
                mOnSubMixAudioFrame = null;
                mSubMixAudioFrame = null;
            }
            else if (type == CallbackType.AUDIO_DECODED)
            {
                mOnDecodedudioFrame = null;
                mDecodedAudioFrame = null;
            }
            else if (type == CallbackType.VIDEO_ENCODE)
            {
                mOnEncodedVideoFrame = null;
                mEncodedVideoFrame = null;
            }
            else if (type == CallbackType.VIDEO_DECODE)
            {
                mOnDecodedVideoFrame = null;
                return;
            }
            else if (type == CallbackType.VIDEO_CAPTURE)
            {
                mVideoFrameEvent = null;
                return;
            }
            else if (type == CallbackType.VIDEO_SCREEN_CAPTURE)
            {
                mOnScreenVideoFrameCallback = null;
                mScreenVideoFrame = null;
            }
            mRtcEngineNavite.UnSubscribeCallback(type);
        }
        public override void RegisterDecodedAudioFrame(OnDecodedAudioFrame audioFrame) {
            mOnDecodedudioFrame = audioFrame;
            mRtcEngineNavite.SubscribeDecodeAudioCallback((byte[] buf, out byte[] outBuffer, int pts, int simple_rate, int channelCont, System.Object context) => {
                if (mOnDecodedudioFrame != null)
                {
                    if (mDecodedAudioFrame == null)
                    {
                        mDecodedAudioFrame = new AudioFrame();
                    }
                    mDecodedAudioFrame.pts = pts;
                    mDecodedAudioFrame.sampleRate = simple_rate;
                    mDecodedAudioFrame.channelCount = channelCont;
                    mDecodedAudioFrame.buffer = buf;
                    mOnDecodedudioFrame.Invoke(mDecodedAudioFrame);
                }
                outBuffer = null;
                return false;
            }, this);
        }

        public override int enableAudioVolumeIndication(int interval, int smooth, bool report_vad) {
            if (mAudioManager != null)
            {
                return mAudioManager.enableAudioVolumeIndication(interval, smooth, report_vad);
            }
            return -1;
        }

        public override int muteRemoteAudioStream(bool muted) {
            if (mAudioManager != null)
            {
                return mAudioManager.muteRemoteAudioStream(muted);
            }
            return -1;
        }

        public override int muteLocalAudioStream(bool muted)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.muteLocalAudioStream(muted);
            }
            return -1;
        }

        public override int StartAudioMixing(string filePath, bool loopback, int cycle, int startPos)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.StartAudioMixing(filePath, loopback, cycle, startPos);
            }
            return -1;
        }

        public override int StopAudioMixing()
        {
            if (mAudioManager != null)
            {
                return mAudioManager.StopAudioMixing();
            }
            return -1;
        }

        public override int PauseAudioMixing()
        {
            if (mAudioManager != null)
            {
                return mAudioManager.PauseAudioMixing();
            }
            return -1;
        }

        public override int ResumeAudioMixing()
        {
            if (mAudioManager != null)
            {
                return mAudioManager.ResumeAudioMixing();
            }
            return -1;
        }

        public override int SetAudioMixingPosition(int pos)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.SetAudioMixingPosition(pos);
            }
            return -1;
        }

        public override int saveRecordCallbackAudio(bool enabled)
        {
            if (mAudioManager != null)
            {
                return mAudioManager.saveRecordCallbackAudio(enabled);
            }
            return -1;
        }

        public void RegisterDecodeVideoEx(OnDecodeVideoInternel onDecodeVideo)
        {
            if (onDecodeVideo == null) {
                return;
            }
            if (mRtcEngineNavite != null) {
                mRtcEngineNavite.RegisterDecodeVideoEx((IntPtr buf, Int32 len, Int32 width, Int32 height,
                    int pixel_fmt, IntPtr channelId, int channelIdLen, UInt64 uid, UInt64 localUid) =>
                {
                    onDecodeVideo.Invoke(buf, len, width, height, pixel_fmt, channelId, channelIdLen, uid, localUid);
                });
            }  
        }

        public void RegisterEventExListener(OnEventExCallbackInternel callbackEvent)
        {
            if (callbackEvent == null)
            {
                return;
            }
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.RegisterEventExListener((int type, IntPtr buf, int len, IntPtr channelId, int channelIdLen, UInt64 localUid) =>
                {
                    callbackEvent.Invoke(type, buf, len, channelId, channelIdLen, localUid);
                });
            }
        }

        

        public override int JoinChannelEx(string token, long appid, LJRtcConnection connection, ChannelMediaOptions options)
        {
            if (mRtcEngineNavite != null)
            {
                JoinChannelExConfig channelExConfig = new JoinChannelExConfig(token, mRtcConfig.isTestEv, appid, connection, options);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.JOIN_CHANNEL_EX, channelExConfig.HPmarshall());
            }
            return -1;
        }

        public override int LeaveChannelEx(LJRtcConnection connection)
        {
            if (mRtcEngineNavite != null)
            {
                MultiChannelEvent channelExConfig = new MultiChannelEvent(connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.LEAVE_CHANNEL_EX, channelExConfig.HPmarshall());
            }
            return -1;
        }

        public override int MuteLocalAudioStreamEx(bool mute, LJRtcConnection connection)
        {
            if (mRtcEngineNavite != null)
            {
                MultiChannelEnableEvent channelExConfig = new MultiChannelEnableEvent(mute, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.MUTE_LOCAL_AUDIO_STREAM_EX, channelExConfig.HPmarshall());
            }
            return -1;
        }

        public override int MuteLocalVideoStreamEx(bool mute, LJRtcConnection connection)
        {
            if (mRtcEngineNavite != null)
            {
                MultiChannelEnableEvent channelExConfig = new MultiChannelEnableEvent(mute, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.MUTE_LOCAL_VIDEO_STREAM_EX, channelExConfig.HPmarshall());
            }
            return -1;
        }

        public override int MuteAllRemoteAudioStreamsEx(bool mute, LJRtcConnection connection)
        {
            if (mRtcEngineNavite != null)
            {
                MultiChannelEnableEvent channelExConfig = new MultiChannelEnableEvent(mute, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.MUTE_ALL_REMOTE_AUDIO_STREAM_EX, channelExConfig.HPmarshall());
            }
            return -1;
        }

        public override int MuteAllRemoteVideoStreamsEx(bool mute, LJRtcConnection connection)
        {
            if (mRtcEngineNavite != null)
            {
                MultiChannelEnableEvent channelExConfig = new MultiChannelEnableEvent(mute, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.MUTE_ALL_REMOTE_VIDEO_STREAM_EX, channelExConfig.HPmarshall());
            }
            return -1;
        }

        public override int PushAudioFrameEx(LJRtcConnection connection, byte[] pcm, int sampleRate, int channelCount, int bytePerSample)
        {
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.PushAudioCaptureFrameEx(connection, pcm, sampleRate, channelCount, bytePerSample);
            }
            return 0;
        }

        public override int PushVideoCaptureFrameEx(LJRtcConnection connection, CaptureVideoFrame videoFrame) {
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.PushVideoCaptureFrameEx(videoFrame, connection.key);
            }
            return 0;
        }

        public override int SubscriberAudioStream(LJRtcConnection connection, long subscriberUid) {
            if (mRtcEngineNavite != null)
            {
                SubscriberStreamEvent subscriberEvent = new SubscriberStreamEvent(subscriberUid, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.SUBSCRIBE_AUDIO_STREAM_EX, subscriberEvent.HPmarshall());
            }
            return 0;
        }

        public override int UnsubscriberAudioStream(LJRtcConnection connection, long subscriberUid) {
            if (mRtcEngineNavite != null)
            {
                SubscriberStreamEvent subscriberEvent = new SubscriberStreamEvent(subscriberUid, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.UNSUBSCRIBE_AUDIO_STREAM_EX, subscriberEvent.HPmarshall());
            }
            return 0;
        
        }

        public override int SubscriberVideoStream(LJRtcConnection connection, long subscriberUid)
        {
            if (mRtcEngineNavite != null)
            {
                SubscriberStreamEvent subscriberEvent = new SubscriberStreamEvent(subscriberUid, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.SUBSCRIBE_VIDEO_STREAM_EX, subscriberEvent.HPmarshall());
            }
            return 0;
        }

        public override int UnsubscriberVideoStream(LJRtcConnection connection, long subscriberUid)
        {
            if (mRtcEngineNavite != null)
            {
                SubscriberStreamEvent subscriberEvent = new SubscriberStreamEvent(subscriberUid, connection);
                return mRtcEngineNavite.SendMediaEvent((int)MediaInvokeEventType.UNSUBSCRIBE_VIDEO_STREAM_EX, subscriberEvent.HPmarshall());
            }
            return 0;
        }

        public override LJChannel CreateChannel(string channelId) {
            LJChannel channel = null;
            if (mMultiStreamManager != null)
            {
                channel = mMultiStreamManager.GetChannel(channelId, 0);
                if (channel != null) {
                    return channel;
                }
                channel = new LJChannel(channelId);
                mMultiStreamManager.SetChannel(channel);
            }
            return channel;
        }

        public override LJChannel CreateChannel(string channelId, long uid)
        {
            LJChannel channel = null;
            if (mMultiStreamManager != null)
            {
                channel = mMultiStreamManager.GetChannel(channelId, (UInt64)uid);
                if (channel != null)
                {
                    return channel;
                }
                channel = new LJChannel(channelId, (UInt64)uid);
                mMultiStreamManager.SetChannel(channel);
            }
            return channel;
        }

        internal override void ReleaseChannel(string channelId)
        {
            if (mMultiStreamManager != null)
            {
                mMultiStreamManager.RemoveChannel(channelId);
            }
        }

        internal override void ReleaseChannel(string channelId, long uid)
        {
            if (mMultiStreamManager != null)
            {
                mMultiStreamManager.RemoveChannel(channelId, uid);
            }
        }

        internal override int SetForMultiChannelUser(LJRtcConnection connection, RawImage imange, long uid, int fps) {
            if (mMultiStreamManager != null) {
                return mMultiStreamManager.SetForMultiChannelUser(connection, imange, uid, fps);
            }
            return 0;
        }

        public override int removeForMultiChannelUser(LJRtcConnection connection, long uid) {
            if (mMultiStreamManager != null)
            {
                return mMultiStreamManager.removeForMultiChannelUser(connection, uid);
            }
            return 0;
        }

        public override int PushAudioFrame(byte[] pcm, int sampleRate, int channelCount, int bytePerSample)
        {
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.PushAudioCaptureFrame(pcm, sampleRate, channelCount, bytePerSample);
            }
            return 0;
        }

        public void OnCameraActionResult(int action, int result, string msg)
        {
            if (mEventHandler != null) {
                mEventHandler.OnCameraActionResult(action, result, msg);
            }
        }

        protected override int Report(string key, string info) {
            if (mRtcConfig != null && mRtcConfig.mReport != null) {
                mRtcConfig.mReport.DoReport(key, info);
                return 0;
            }
            return -1;
        }

        public override void SetUseNativeCamera(bool enable)
        {
            isUserNativeCamera = enable;
            VideoDeviceManagerFactory.ResetDeviceManager(isUserNativeCamera);
            mIVideoDeviceManager = VideoDeviceManagerFactory.GetDeviceManager();
        }

        public override bool IsUseNativeCamera()
        {
#if (UNITY_ANDROID) || (UNITY_EDITOR && UNITY_IOS)

            return false;
#else
            return isUserNativeCamera;
#endif
        }

        public void LocalRegisterDecodeVideo(OnDecodedVideoFrame videoFrame)
        {
            mLocalOnDecodedVideoFrame = videoFrame;
            RegisterNativeDecodeFrame();
        }

        public override int SetRemoteRender(RawImage rawImage)
        {
            if (rawImage == null && mRemoteVideoManager != null) {
                mRemoteVideoManager.OnDestroy();
                mRemoteVideoManager = null;
                return 0;
            }
            if (mRemoteVideoManager == null)
            {
                mRemoteVideoManager = new RemoteVideoManager(this);
                mRemoteVideoManager.SetRemodeRender(rawImage);
                lock (mComponentList)
                {
                    mComponentList.Add(mRemoteVideoManager);
                }
            }
            return 0;
        }

        public override void RegisterScreenCaptureVideoFrame(OnCaptureVideoFrame videoFrame)
        {
            mOnScreenVideoFrameCallback = videoFrame;
            if (mRtcEngineNavite != null) {
                mRtcEngineNavite.SubscribeScreenCaptureVideoCallbck((IntPtr buf, int len, Int32 width, 
                        Int32 height, int pixel_fmt, System.Object context) => {
                            LJRtcEngine instance = (LJRtcEngine)context;

                    if (instance.mOnScreenVideoFrameCallback != null) {
                        if (instance.mScreenVideoFrame == null) {
                            instance.mScreenVideoFrame = new CaptureVideoFrame();
                        }

                        if (instance.mScreenVideoFrame.buffer == null || instance.mScreenVideoFrame.buffer.Length != len) {
                            instance.mScreenVideoFrame.buffer = new byte[len];
                        }

                        instance.mScreenVideoFrame.type = VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;

                        instance.mScreenVideoFrame.format = VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_I420;
                        instance.mScreenVideoFrame.stride = width;
                        instance.mScreenVideoFrame.width = width;
                        instance.mScreenVideoFrame.height = height;
                        Marshal.Copy(buf, instance.mScreenVideoFrame.buffer, 0, len);
                        instance.mOnScreenVideoFrameCallback.Invoke(instance.mScreenVideoFrame);
                    }
                }, this);
            }
        }

        public override int StartScreenCapture(ScreenCaptureParam param)
        {
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.StartVideoScreenCapture(param);
            }
            return 0;
        }

        public override int StopScreenCapture()
        {
            if (mRtcEngineNavite != null)
            {
                mRtcEngineNavite.StopScreenCapture();
            }
            return 0;
        }
    }
}

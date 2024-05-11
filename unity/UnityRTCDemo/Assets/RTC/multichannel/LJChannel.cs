
using LJ.RTC.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
namespace LJ.RTC
{
    public struct ChannelVideoInfo {
        public int width;
        public int height;

        public ChannelVideoInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    public class LJChannel
    {
        public string _channelId;
        public UInt64 _uid = 0;
        private LJRtcConnection _LJRtcConnection;
        private IRtcEngineEx _rtcEngine;

        public ChannelOnJoinChannelSuccessHandler ChannelOnJoinChannelSuccess;
        public ChannelOnErrorHandler ChannelOnError;
        public ChannelOnLeaveChannelHandler ChannelOnLeaveChannelSuccess;
        public ChannelOnLeaveChannelErrorHandler ChannelOnLeaveChannelError;
        public ChannelOnUserJoinedHandler ChannelOnUserJoined;
        public ChannelOnUserOffLineHandler ChannelOnUserOffLine;
        public ChannelOnNetworkQualityHandler ChannelOnNetworkQuality;
        public ChannelOnVideoSizeChangedHandler ChannelOnVideoSizeChanged;
        public ChannelOnConnectionStateChangedHandler ChannelOnConnectionStateChanged;
        public ChannelOnFirstRemoteVideoFrameHandler ChannelOnFirstRemoteVideoFrame;
        private static readonly object _lock = new object();
        private ConcurrentDictionary<UInt64, ChannelVideoInfo> _callbackFirstMap = new ConcurrentDictionary<UInt64, ChannelVideoInfo>();
        private HashSet<UInt64> userViews = new HashSet<UInt64>();
        public UInt64 _joinChannelTime;

        public LJChannel(string channelId) {
            _channelId = channelId;
            _rtcEngine = IRtcEngine.GetEx();
        }

        public LJChannel(string channelId, UInt64 uid)
        {
            _channelId = channelId;
            _uid = uid;
            _rtcEngine = IRtcEngine.GetEx();
            if (_LJRtcConnection == null)
            {
                _LJRtcConnection = new LJRtcConnection(_channelId, (long)uid);
            }
        }

        public void SetClientRole(CLIENT_ROLE_TYPE role) {

        }

        public void JoinChannel(string token, long appId, long uid, ChannelMediaOptions options) {
            _joinChannelTime = (UInt64)TimeHelper.GetCurrenTime();
            if (_LJRtcConnection == null) {
                _LJRtcConnection = new LJRtcConnection(_channelId, uid);
            }
            if (_rtcEngine != null) {
                _rtcEngine.JoinChannelEx(token, appId, _LJRtcConnection, options);
            }
        }

        public void LeaveChannel() {
            if (_rtcEngine != null)
            {
                _rtcEngine.LeaveChannelEx(_LJRtcConnection);
                clearViews();
            }
        }

        public void ReleaseChannel() {
            LeaveChannel();
            if (_rtcEngine != null) {
                _rtcEngine.ReleaseChannel(_channelId);
                _rtcEngine.ReleaseChannel(_LJRtcConnection.key);
            }
        }

        private void clearViews()
        {
            lock (_lock)
            {
                IEnumerator<UInt64> enumerator = userViews.GetEnumerator();
                List<UInt64> elementsToRemove = new List<UInt64>();
                while (enumerator.MoveNext())
                {
                    UInt64 userId = enumerator.Current;
                    if (_rtcEngine != null) {
                        _rtcEngine.removeForMultiChannelUser(_LJRtcConnection, (long)userId);
                    }
                    if (ChannelOnUserOffLine != null)
                    {
                        ChannelOnUserOffLine.Invoke(_channelId, userId);
                    }
                }
                userViews.Clear();
            }
        }

        public void MuteAllRemoteAudioStreams(bool mute) {
            if (_rtcEngine != null)
            {
                _rtcEngine.MuteAllRemoteAudioStreamsEx(mute, _LJRtcConnection);
            }
        }

        internal void SetForMultiChannelUser(RawImage rawImage, long uid, int fps)
        {
            lock (_lock) {
                userViews.Add((UInt64)uid);
            }
            if (_rtcEngine != null)
            {
                _rtcEngine.SetForMultiChannelUser(_LJRtcConnection, rawImage, uid, fps);
            }
        }

        private void removeForMultiChannelUser(LJRtcConnection connection, long uid) {
            lock (_lock)
            {
                userViews.Remove((UInt64)uid);
            }
            if (_rtcEngine != null)
            {
                _rtcEngine.removeForMultiChannelUser(_LJRtcConnection, uid);
            }
        }

        public void MuteRemoteAudioStream(long userId, bool mute) {
            if (_rtcEngine != null)
            {
                if (mute)
                {
                    _rtcEngine.UnsubscriberAudioStream(_LJRtcConnection, userId);
                }
                else {
                    _rtcEngine.SubscriberAudioStream(_LJRtcConnection, userId);
                }
            }
        }
        public void MuteAllRemoteVideoStreams(bool mute) {
            if (_rtcEngine != null)
            {
                _rtcEngine.MuteAllRemoteVideoStreamsEx(mute, _LJRtcConnection);
            }
        }

        public void MuteRemoteVideoStream(long userId, bool mute) {
            if (_rtcEngine != null)
            {
                if (mute)
                {
                    _rtcEngine.UnsubscriberVideoStream(_LJRtcConnection, userId);
                }
                else
                {
                    _rtcEngine.SubscriberVideoStream(_LJRtcConnection, userId);
                }
            }
        }

        public void MuteLocalVideoStream(bool mute) {
            if (_rtcEngine != null)
            {
                _rtcEngine.MuteLocalVideoStreamEx(mute, _LJRtcConnection);
            }
        }

        public void MuteLocalAudioStream(bool mute) {
            if (_rtcEngine != null)
            {
                _rtcEngine.MuteLocalAudioStreamEx(mute, _LJRtcConnection);
            }
        }

        internal void OnEventExCallback(int type, IntPtr buf, int len)
        {
            byte[] data = new byte[len];
            Marshal.Copy(buf, data, 0, len);
            //JLog.Info("OnEventExCallback " + type);
            switch (type)
            {
                case (int)UDPMsgType.NET_QUALITY:
                    NetQuality netQuality = new NetQuality();
                    netQuality.unmarshall(data);
                    if (ChannelOnNetworkQuality != null)
                    {
                        ChannelOnNetworkQuality.Invoke(_channelId, _uid, netQuality.mLocalQuality, netQuality.mRemoteQuality);
                    }
                    break;
                case (int)UDPMsgType.CB_JOIN_CHANNEL:
                    MultiChannelEventResult joinResult = new MultiChannelEventResult();
                    joinResult.unmarshall(data);
                    UInt64 currentTime = (UInt64)TimeHelper.GetCurrenTime();
                    JLog.Info("OnEventExCallback CB_JOIN_CHANNEL " + joinResult.result);
                    if (joinResult.result == 0)
                    {
                        if (ChannelOnJoinChannelSuccess != null)
                        {
                            ChannelOnJoinChannelSuccess.Invoke(joinResult.channenlId, joinResult.uid, (int)(currentTime - _joinChannelTime));
                        }
                    }
                    else
                    {
                        if (ChannelOnError != null)
                        {
                            ChannelOnError.Invoke(joinResult.channenlId, joinResult.result, joinResult.msg);
                        }
                    }
                    break;
                case (int)UDPMsgType.CB_LEAVE_CHANNEL:
                    MultiChannelEventResult leaveResult = new MultiChannelEventResult();
                    leaveResult.unmarshall(data);
                    JLog.Info("OnEventExCallback CB_LEAVE_CHANNEL " + leaveResult.result);
                    if (leaveResult.result == 0)
                    {
                        if (ChannelOnLeaveChannelSuccess != null)
                        {
                            ChannelOnLeaveChannelSuccess.Invoke(leaveResult.channenlId);
                        }
                    }
                    else {
                        if (ChannelOnLeaveChannelError != null) {
                            ChannelOnLeaveChannelError.Invoke(leaveResult.channenlId, leaveResult.result, leaveResult.msg);
                        }
                    }
                    break;
                    case (int)UDPMsgType.CB_LINK_STATUS:
                    LinkStatusEvent status = new LinkStatusEvent();
                    status.unmarshall(data);
                    JLog.Info("OnEventExCallback CB_LINK_STATUS " + status.status);
                    if (ChannelOnConnectionStateChanged != null)
                    {

                        ChannelOnConnectionStateChanged.Invoke(_channelId, status.status);
                    }
                    break;
                    case (int)UDPMsgType.MUTI_CHANNEL_REMOTE_JOIN:
                    MultiChannelEventResult remoteJoin = new MultiChannelEventResult();
                    remoteJoin.unmarshall(data);
                    JLog.Info("OnEventExCallback MUTI_CHANNEL_REMOTE_JOIN " + remoteJoin.uid);
                    if (ChannelOnUserJoined != null)
                    {
                        ChannelOnUserJoined.Invoke(remoteJoin.channenlId, (UInt64)remoteJoin.uid, 0);
                    }
                    break;
                    case (int)UDPMsgType.MUTI_CHANNEL_REMOTE_LEAVE:
                    MultiChannelEventResult remoteLeave = new MultiChannelEventResult();
                    remoteLeave.unmarshall(data);
                    JLog.Info("OnEventExCallback MUTI_CHANNEL_REMOTE_LEAVE " + remoteLeave.uid);
                    removeForMultiChannelUser(_LJRtcConnection, remoteLeave.uid);
                    if (ChannelOnUserOffLine != null)
                    {
                        ChannelOnUserOffLine.Invoke(remoteLeave.channenlId, (UInt64)remoteLeave.uid);
                    }
                    break;
                default:
                    break;
            }
        }

        internal void OnDecodeVideo(byte[] dcodeBuffer, int width, int height, UInt64 uid, int pixel_fmt)
        {
            bool containsUid = _callbackFirstMap.ContainsKey(uid);
            if (ChannelOnFirstRemoteVideoFrame != null && !containsUid) {
                UInt64 currentTime = (UInt64)TimeHelper.GetCurrenTime();
                ChannelOnFirstRemoteVideoFrame.Invoke(_channelId, uid, width, height, (int)(currentTime - _joinChannelTime));
            }
            if (!containsUid)
            {
                _callbackFirstMap.TryAdd(uid, new ChannelVideoInfo(width, height));
            }
            else {
                if (ChannelOnVideoSizeChanged != null) {
                    if (_callbackFirstMap[uid].width != width || _callbackFirstMap[uid].height != height)
                    {
                        ChannelOnVideoSizeChanged.Invoke(_channelId, uid, width, height);
                        _callbackFirstMap.TryUpdate(uid, new ChannelVideoInfo(width, height), _callbackFirstMap[uid]);
                    }
                }
            }

        }
    }
}

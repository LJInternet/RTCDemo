
using LJ.RTC.Common;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace LJ.RTC
{
    public class MultiStreamManager : BaseRtcEngineModule
    {

        private ConcurrentDictionary<string, RemoteRenderView> _renderViews = new ConcurrentDictionary<string, RemoteRenderView>();
        private ConcurrentDictionary<string, LJChannel> _rtcChannels = new ConcurrentDictionary<string, LJChannel>();
        public MultiStreamManager(IRtcEngineApi rtcEngineApi) : base(rtcEngineApi)
        {
            OnCreate();
        }

        public override void OnCreate()
        {
            if (mRtcEngineApi != null)
            {
                mRtcEngineApi.RegisterDecodeVideoEx(OnDecodeVideoInternel);
                mRtcEngineApi.RegisterEventExListener(OnEventExCallback);
            }
        }

        public override void OnDestroy()
        {
            foreach (var view in _renderViews.Values) {
                if (view != null) {
                    view.OnDestroy();
                }
            }
            _renderViews.Clear();
            _rtcChannels.Clear();
            base.OnDestroy();
        }

        internal int SetForMultiChannelUser(LJRtcConnection connection, RawImage imange, long uid, int fps)
        {
            MainThreadHelper.QueueOnMainThread((object obj) => {
                string key = connection.key + uid;
                RemoteRenderView view;
                _renderViews.TryGetValue(key, out view);
                if (view == null)
                {
                    view = new RemoteRenderView(key);
                    view.SetRemodeRender(imange);
                    view.OnCreate();
                    _renderViews.TryAdd(key, view);
                }
                IRtcEngineEx rtcEngine = IRtcEngineEx.GetEx();
                if (rtcEngine != null)
                {

                    rtcEngine.SubscriberAudioStream(connection, uid);
                    rtcEngine.SubscriberVideoStream(connection, uid);
                }
            }, null);
            return 0; 
        }

        internal int removeForMultiChannelUser(LJRtcConnection connection, long uid)
        {
            MainThreadHelper.QueueOnMainThread((object obj) => {
                string key = connection.key + uid;
                RemoteRenderView view;
                _renderViews.TryRemove(key, out view);
                if (view == null)
                {
                    return;
                }
                view.OnDestroy();
                IRtcEngineEx rtcEngine = IRtcEngineEx.GetEx();
                if (rtcEngine != null)
                {
                    rtcEngine.UnsubscriberAudioStream(connection, uid);
                    rtcEngine.UnsubscriberVideoStream(connection, uid);
                }
            }, null);
            return 0;
        }

        internal void SetChannel(LJChannel channel)
        {
            if (channel._uid != 0)
            {
                _rtcChannels.TryAdd(channel._channelId + channel._uid, channel);
            }
            else {
                _rtcChannels.TryAdd(channel._channelId, channel);
            }
        }

        private byte[] mDecodeBuffer;
        private void OnDecodeVideoInternel(IntPtr buf, Int32 len, Int32 width,Int32 height,
            int pixel_fmt, IntPtr channelId, int channelIdLen, UInt64 uid, UInt64 localUid) {

            string channelName = Marshal.PtrToStringAnsi(channelId, channelIdLen);
            string keyStr = channelName + localUid + uid;
            RemoteRenderView view;
            _renderViews.TryGetValue(keyStr, out view);
            if (mDecodeBuffer == null || mDecodeBuffer.Length != len) {
                mDecodeBuffer = new byte[len];
            }
            Marshal.Copy(buf, mDecodeBuffer, 0, len);
            if (view != null) {
                view.OnDecodedVideoFrame(mDecodeBuffer, width, height, pixel_fmt);
            }
            LJChannel channel = GetChannel(channelName, localUid);
            if (channel != null) {
                channel.OnDecodeVideo(mDecodeBuffer, width, height, uid, pixel_fmt);
            }
        }

        public LJChannel GetChannel(string channelId, UInt64 localUid) {
            LJChannel channel;
            _rtcChannels.TryGetValue(channelId, out channel);
            if (channel == null)
            {
                _rtcChannels.TryGetValue(channelId + localUid, out channel);
            }
            return channel;
        }

        private void OnEventExCallback(int type, IntPtr buf, int len, IntPtr channelId, int channelIdLen, UInt64 localUid) {
            string channelName = Marshal.PtrToStringAnsi(channelId, channelIdLen);
            LJChannel channel = GetChannel(channelName, localUid);
            if (channel != null)
            {
                channel.OnEventExCallback(type, buf, len);
            }
        }

        internal void RemoveChannel(string channelId)
        {
            LJChannel channel;
            _rtcChannels.TryRemove(channelId, out channel);
        }

        internal void RemoveChannel(string channelId, long uid)
        {
            LJChannel channel;
            _rtcChannels.TryRemove(channelId+uid, out channel);
        }
    }
}
